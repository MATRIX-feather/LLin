#nullable disable

using System;

namespace osu.Game.Rulesets.IGPlayer.Feature.Player.Misc
{
    public class TypeWrapper
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}
