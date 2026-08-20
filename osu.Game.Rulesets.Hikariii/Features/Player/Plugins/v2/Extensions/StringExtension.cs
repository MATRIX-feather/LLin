using System.Text.RegularExpressions;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins.v2.Extensions;

public static partial class StringExtension
{
    [GeneratedRegex(@"[a-zA-Z-]")]
    private static partial Regex hikariiiPluginNameRegex();

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex hikariiiPluginNonSuggestedNameRegex();

    public static bool TestHikariiiPluginName(this string name)
    {
        return name.Length <= 24
               && string.IsNullOrEmpty(hikariiiPluginNameRegex().Replace(name, string.Empty));
    }

    // haven't tested yet...
    public static bool TestNonSuggestedHikariiiNameCharacters(this string name)
    {
        return hikariiiPluginNonSuggestedNameRegex().Match(name).Success;
    }
}
