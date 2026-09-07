using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

public record PluginDescription(
    LocalisableString Name,
    LocalisableString Description,
    LocalisableString[] Authors,
    LocalisableString[] Credits)
{
    public PluginDescription(LocalisableString Name, LocalisableString Description, LocalisableString[] Authors)
        : this(Name, Description, Authors, [])
    {
    }

    public string AuthorString() => string.Join(", ", Authors);
}
