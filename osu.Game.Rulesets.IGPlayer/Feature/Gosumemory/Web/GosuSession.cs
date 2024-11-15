using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

namespace osu.Game.Rulesets.IGPlayer.Feature.Gosumemory.Web;

public class GosuSession : WsSession
{
    private readonly GosuServer gosuServer;

    public GosuSession(GosuServer server)
        : base(server)
    {
        this.gosuServer = server;
    }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        string path = request.Url ?? "/";

        // 跳过favicon.ico
        if (path == "/favicon.ico")
        {
            var response = new HttpResponse();
            response.SetBegin(404);

            this.SendResponse(response);
            return;
        }

        if (path.EndsWith("/ws", StringComparison.Ordinal)
            || path.EndsWith("/json", StringComparison.Ordinal))
        {
            base.OnReceivedRequest(request);
            return;
        }

        this.onFileRequest(path);
    }

    private HttpResponse createResponse(int code)
    {
        var response = new HttpResponse();

        response.SetBegin(code);

        response.SetHeader("Access-Control-Allow-Origin", "*")
                .SetHeader("Cache-Control", "public, max-age=0");

        return response;
    }

    private string getLinkUrl(string link, string name) => $"<a href=\"{link}\">{name}</a>";

    /// <summary>
    /// 包装给定的HTML代码
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private string wrapHtml(string content)
    {
        // 因为不知道怎么让他显示目录所以只好自己搓了 UwU
        string htmlCode = "<html>";

        htmlCode += "<head>"
                    + "<meta charset=\"utf-8\">"
                    + "<meta name=\"color-scheme\" content=\"light dark\">"
                    + "<meta name=\"google\" value=\"notranslate\">"
                    + "</head>";

        htmlCode += content;

        htmlCode += "</html>";
        return htmlCode;
    }

    private void onFileRequest(string requestPath)
    {
        //response.SetHeader("Cache-Control", cache_control_str)
        //        .SetHeader("Access-Control-Allow-Origin", "*");

        var storage = gosuServer.GetStorage();

        if (storage == null)
        {
            var notReadyResponse = createResponse(200);

            string html = "<h1>Gosu文件服务尚未初始化完毕，请稍后再来</h1>"
                          + "<h1>Gosu file service is not initialized yet, please come back later.</h1>";

            notReadyResponse.SetBody(wrapHtml(html));

            this.SendResponse(notReadyResponse);

            return;
        }

        string urlPath = requestPath.StartsWith('/')
            ? requestPath.Remove(0, 1) // 将其变为相对目录
            : requestPath;

        // storagePath始终不为空
        if (string.IsNullOrEmpty(urlPath)) urlPath = ".";

        //Logging.Log("URLPath is " + urlPath);

        // 处理Songs
        if (urlPath.StartsWith("Songs", StringComparison.Ordinal))
        {
            string[] split = urlPath.Split("/", 2);

            if (split.Length < 2)
            {
                Logging.Log($"Illegal argument '{split}', not processing...");

                var illegalArgumentResponse = createResponse(400);
                this.SendResponse(illegalArgumentResponse);

                return;
            }

            string fileName = split[1].Split("?", 2)[0];

            byte[] content = gosuServer.FindStaticOrAsset(fileName) ?? [];

            if (content.Length == 0)
            {
                var nullFileResponse = createResponse(404);
                nullFileResponse.SetBegin(404);
                this.SendResponse(nullFileResponse);

                return;
            }

            HttpResponse fileResponse = createResponse(200);
            fileResponse.SetBody(content);

            this.SendResponse(fileResponse);
            return;
        }

        // 将gosu_statics和urlPath混合，得到我们要的相对存储路径
        string storagePath = Path.Combine("gosu_statics", urlPath);

        // 目标存储的绝对位置
        string targetFilePath = storage.GetFullPath(storagePath);

        // Direct access to file
        // 如果要访问文件, 那么不要进行处理
        if (File.Exists(targetFilePath))
        {
            byte[] content = gosuServer.FindStaticOrAsset(targetFilePath) ?? new byte[] { };

            if (content.Length == 0)
            {
                var nullFileResponse = createResponse(404);
                this.SendResponse(nullFileResponse);

                return;
            }

            var fileResponse = new HttpResponse(200);
            fileResponse.SetHeader("Content-Type", MimeTypeMap.GetMimeType(targetFilePath))
                        .SetBody(File.ReadAllBytes(targetFilePath));

            this.SendResponse(fileResponse);
            return;
        }

        // 文件不存在，是否目录存在？
        if (Path.Exists(targetFilePath))
        {
            try
            {
                string html = "";

                // 反之，添加所有文件和目录的超链接
                var localStorage = storage.GetStorageForDirectory(storagePath);

                foreach (string directory in localStorage.GetDirectories("."))
                    html += getLinkUrl($"/{urlPath}/{directory}", directory) + "<br>";

                foreach (string file in localStorage.GetFiles("."))
                    html += getLinkUrl($"/{urlPath}/{file}", file) + "<br>";

                var response = createResponse(200);
                response.SetBody(wrapHtml(html));

                this.SendResponse(response);
                return;
            }
            catch (Exception e)
            {
                Logging.Log($"Error occurred presenting directory information! {e.Message}");
                Logging.Log($"{e.StackTrace ?? "<No stacktrace>"}");
            }
        }

        string notFoundHtml = getLinkUrl("/", "[根目录]")
                              + "<br><br>"
                              + "404 Not Found UwU";

        var notFoundResponse = new HttpResponse();
        notFoundResponse.SetBegin(200)
                        .SetBody(wrapHtml(notFoundHtml));

        this.SendResponse(notFoundResponse);
    }

    public override void OnWsConnected(HttpRequest request)
    {
        Logging.Log($"WebSocket session with Id {Id} connected!");
    }

    public override void OnWsDisconnected()
    {
        Logging.Log($"WebSocket session with Id {Id} disconnected!");
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Logging.Log("WebSocket Incoming: " + message);
    }

    public override void OnWsError(string error)
    {
        Logging.Log("WS ERRORED: " + error);
    }

    public override void OnWsError(SocketError error)
    {
        Logging.Log("WS ERRORED: " + error);
    }

    protected override void OnError(SocketError error)
    {
        Logging.Log($"Chat WebSocket session caught an error with code {error}");
    }
}
