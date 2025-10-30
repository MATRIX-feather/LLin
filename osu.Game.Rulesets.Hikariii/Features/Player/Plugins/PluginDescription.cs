namespace osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

public record PluginDescription(string Name, string Description, string[] Authors)
{
    public string AuthorString() => string.Join(", ", Authors);
}
