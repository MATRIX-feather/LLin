using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Plugins
{
    public abstract partial class DrawableHikariiiPlugin : Container
    {
        /// <summary>
        ///
        /// </summary>
        public virtual ContentLayerType ContentLayer => ContentLayerType.Foreground;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual DrawablePluginPage? CreateDrawablePluginPage() => null;

        [Resolved]
        private IImplementLLin llin { get; set; }

        [Obsolete("Only for migrating old plugins to the new form")]
        protected IImplementLLin LLin => llin;

        protected IImplementLLin Player => llin;
    }

    public enum ContentLayerType
    {
        Background,
        Foreground,

        /// <summary>
        /// Currently not supported
        /// </summary>
        Overlay
    }
}
