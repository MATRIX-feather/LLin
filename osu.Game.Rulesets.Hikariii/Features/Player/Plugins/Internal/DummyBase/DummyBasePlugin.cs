using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hikariii.Features.Configuration;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal.DummyBase
{
    internal partial class DummyBasePlugin : LLinPlugin
    {
        internal DummyBasePlugin(MConfigManager config, LLinPluginManager plmgr, LLinPluginProvider provider)
            : base(provider)
        {
            this.config = config;
            this.PluginManager = plmgr;

            Name = "基本设置";
        }

        private readonly MConfigManager config;

        protected override Drawable CreateContent()
        {
            return new Box
            {
                Alpha = 0
            };
        }

        protected override bool OnContentLoaded(Drawable content)
        {
            return true;
        }

        protected override bool PostInit()
        {
            return true;
        }
    }
}
