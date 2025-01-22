using System;
using System.Reflection;
using Android.App;
using Android.OS;
using osu.Framework.Android;
using osu.Framework.Logging;
using osu.Game.Tests;

namespace osu.Game.Rulesets.Hikariii.Tests;

[Activity(ConfigurationChanges = DEFAULT_CONFIG_CHANGES, LaunchMode = DEFAULT_LAUNCH_MODE, Exported = true, MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    protected override Framework.Game CreateGame() => new OsuTestBrowser();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // See the comment in OsuGameActivity
        try
        {
            Assembly.Load("osu.Game.Rulesets.Osu");
        }
        catch (Exception e)
        {
            Logger.Error(e, "[HikariiiTests] 未能加载 osu.Game.Rulesets.Osu !");
        }

        Assembly.Load("osu.Game.Rulesets.Hikariii");
    }
}
