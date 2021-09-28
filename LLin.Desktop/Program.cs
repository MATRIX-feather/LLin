using osu.Framework;
using osu.Framework.Platform;

namespace LLin.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"LLin"))
            using (osu.Framework.Game game = new LLinGameDesktop())
                host.Run(game);
        }
    }
}
