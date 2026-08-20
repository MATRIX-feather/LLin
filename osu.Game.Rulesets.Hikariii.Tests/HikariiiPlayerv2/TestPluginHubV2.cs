using NUnit.Framework;
using NUnit.Framework.Constraints;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Platform;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Loader;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Hikariii.Tests;

public partial class TestPluginHubV2 : OsuTestScene
{
    private HikariiiPluginHub pluginHub;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        Dependencies.Cache(storage);

        Add(pluginHub = new HikariiiPluginHub());
        Logging.Log("OK Added plugin Hub");

        AddStep("Load builtin plugin providers", loadBuiltinProviders);
    }

    [Test]
    public void TestPluginName()
    {
        test("hikariii-core", Is.True);
        test("StoryboardIntegration", Is.True);
        test("Storyboard-Integration", Is.True);

        test("Storyboard-Integrationnnnnnnnnnn", Is.False); // too many characters
        test("Storyboard_Integration", Is.False); // '_'
        test("osu人能飞", Is.False); // illegal characters
        test("Osu!", Is.False); // '!'
        test("O s u", Is.False); // ' '
        return;

        void test(string id, IResolveConstraint condition)
        {
            Logging.Log($"TestPluginName: {id} expected: {condition}");
            Assert.That(id.TestHikariiiPluginName(), condition, $"failed testing {id} as {condition}");
        }
    }

    private void loadBuiltinProviders()
    {
        IHikariiiPluginProvider[] rejectedProviders;
        pluginHub.LoadFrom(new HikariiiBundledPluginLoader(), out rejectedProviders);

        Logging.Log("Rejected plugins: ");
        rejectedProviders.ForEach(rj => Logging.Log(rj.ToString()));
        Logging.Log("Done listing Rejected plugins");
    }
}
