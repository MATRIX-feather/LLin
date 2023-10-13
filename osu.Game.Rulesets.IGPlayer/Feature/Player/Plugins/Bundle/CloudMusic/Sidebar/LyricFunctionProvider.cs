// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using M.Resources.Localisation.LLin.Plugins;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Graphics;
using osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Types;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Plugins.Bundle.CloudMusic.Sidebar
{
    public class LyricFunctionProvider : ButtonWrapper, IPluginFunctionProvider
    {
        public PluginSidebarPage SourcePage { get; set; }

        public LyricFunctionProvider(PluginSidebarPage page)
        {
            SourcePage = page;

            Icon = FontAwesome.Solid.Music;
            Description = CloudMusicStrings.EntryTooltip;
            Type = FunctionType.Plugin;
        }
    }
}
