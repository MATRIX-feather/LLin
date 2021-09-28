using System;

namespace LLin.Game.Screens.Mvis.Plugins
{
    public static class PluginExtensions
    {
        public static string ToPluginPath(this Object pl)
        {
            return pl.GetType().Name + "@" + pl.GetType().Namespace;
        }
    }
}
