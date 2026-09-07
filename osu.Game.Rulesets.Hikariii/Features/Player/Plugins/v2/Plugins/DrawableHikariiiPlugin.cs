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

        /// <summary>
        /// true if this plugin has an exit animation, otherwise false.
        /// </summary>
        public virtual bool HasExitAnimation => false;

        /// <summary>
        /// Play exit animation within 3 seconds.
        /// If <see cref="HasExitAnimation"/> is set to false, this will be ignored.
        /// </summary>
        public virtual void PlayExit()
        {
        }
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
