using System.Reflection;
using Android.App;
using Android.OS;
using osu.Framework.Android;
using osu.Game.Tests;

namespace osu.Game.Rulesets.IGPlayer.Tests.Android;

[Activity(ConfigurationChanges = DEFAULT_CONFIG_CHANGES, LaunchMode = DEFAULT_LAUNCH_MODE, Exported = true, MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    protected override Framework.Game CreateGame() => new OsuTestBrowser();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // See the comment in OsuGameActivity
        Assembly.Load("osu.Game.Rulesets.Osu");
        Assembly.Load("osu.Game.Rulesets.IGPlayer");
    }
}
