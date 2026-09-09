using osu.Framework;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Game.Rulesets.Sandbox;

namespace LLin.Extras;

public class SandboxLoader
{
    public Exception? Check()
    {
        try
        {
            const string ruleset_name = "osu.Game.Rulesets.Sandbox.SandboxRuleset, osu.Game.Rulesets.Sandbox";
            var type = Type.GetType(ruleset_name);
            if (type == null)
                return new Exception($"Sandbox Ruleset ({ruleset_name}) not found");

            Logger.Log($"[Hikariii] OK Got sandbox type {type}");
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }

    public void LoadResources(Game game)
    {
        // Add Resource store
        game.Resources.AddStore(new DllResourceStore(typeof(SandboxRuleset).Assembly));
    }
}
