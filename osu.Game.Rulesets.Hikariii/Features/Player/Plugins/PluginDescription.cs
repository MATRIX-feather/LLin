using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

public record PluginDescription(LocalisableString Name, LocalisableString Description, string[] Authors)
{
    public string AuthorString() => string.Join(", ", Authors);
}
