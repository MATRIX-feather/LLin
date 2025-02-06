using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hikariii.Features.Player.Interfaces.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.Internal
{
    public partial class DummyFunctionBar : LLinPlugin, IFunctionBarProvider
    {
        public DummyFunctionBar()
        {
            Name = "无";
            Description = "默认底栏";
            Author = "mf-osu";
        }

        protected override Drawable CreateContent()
        {
            throw new NotImplementedException();
        }

        protected override bool OnContentLoaded(Drawable content)
        {
            throw new NotImplementedException();
        }

        protected override bool PostInit()
        {
            throw new NotImplementedException();
        }

        public override int Version => 6;

        public bool OkForHide()
        {
            throw new NotImplementedException();
        }

        public bool AddFunctionControl(IFunctionProvider provider)
        {
            throw new NotImplementedException();
        }

        public bool AddFunctionControls(List<IFunctionProvider> providers)
        {
            throw new NotImplementedException();
        }

        public bool SetFunctionControls(List<IFunctionProvider> providers)
        {
            throw new NotImplementedException();
        }

        public void Remove(IFunctionProvider provider)
        {
            throw new NotImplementedException();
        }

        public void ShowFunctionControl()
        {
        }

        public void HideFunctionControl()
        {
        }

        public List<IPluginFunctionProvider> GetAllPluginFunctionButton()
        {
            throw new NotImplementedException();
        }

        public Action? OnDisable { get; set; }
    }
}
