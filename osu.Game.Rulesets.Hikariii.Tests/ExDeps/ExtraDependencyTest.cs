using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using LLin.Extras;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Hikariii.Tests.ExDeps;

public partial class ExtraDependencyTest : OsuTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        TextFlowContainer fld = new TextFlowContainer();
        Add(fld);

        AddStep("check sandbox", () =>
        {
            var loader = new SandboxLoader();
            Exception? ex = loader.Check();

            if (ex != null)
            {
                fld.AddParagraph($"No! sandbox check failed: {ex.Message}");
                Logging.LogError(ex, "Failed loading sandbox");
                return;
            }

            fld.AddParagraph("Ok sandbox check passed with no exceptions");
        });
    }
}
