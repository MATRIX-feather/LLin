using osu.Framework.Platform;
using osu.Framework;
using LLin.Game;

namespace LLin.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"LLin"))
            using (osu.Framework.Game game = new LLinGame())
                host.Run(game);
        }
    }
}
