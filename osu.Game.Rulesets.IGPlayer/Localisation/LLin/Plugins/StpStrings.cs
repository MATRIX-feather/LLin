using osu.Framework.Localisation;

namespace osu.Game.Rulesets.IGPlayer.Localisation.LLin.Plugins
{
    //Sandbox to panel Strings
    public class StpStrings
    {
        private static string prefix = @"M.Resources.Localisation.LLin.Plugins.StpStrings";

        public static LocalisableString AlphaOnIdle => new TranslatableString(getKey(@"alpha_on_idle"), @"Idle Alpha");

        public static LocalisableString ShowParticles => new TranslatableString(getKey(@"show_particles"), @"Show Particles");

        public static LocalisableString ParticleCount => new TranslatableString(getKey(@"particle_count"), @"Particle Count");

        public static LocalisableString ShowBeatmapInfo => new TranslatableString(getKey(@"show_beatmap_info"), @"Show Beatmap Info");

        public static LocalisableString VisualizerLayoutType => new TranslatableString(getKey(@"layout_type"), @"Layout Type");

        public static LocalisableString Radius => new TranslatableString(getKey(@"radius"), @"Radius");

        public static LocalisableString BarType => new TranslatableString(getKey(@"bar_type"), @"Bar Type");

        public static LocalisableString BarWidth => new TranslatableString(getKey(@"bar_width"), @"Bar Width");

        public static LocalisableString BarCount => new TranslatableString(getKey(@"bar_count"), @"Bar Count");

        public static LocalisableString Rotation => new TranslatableString(getKey(@"rotation"), @"Rotation");

        public static LocalisableString DecayTime => new TranslatableString(getKey(@"decay"), @"Decay Time");

        public static LocalisableString HeightMultiplier => new TranslatableString(getKey(@"height_multiplier"), @"Height Multiplier");

        public static LocalisableString Symmetry => new TranslatableString(getKey(@"symmetry"), @"Symmetry");

        public static LocalisableString Smoothness => new TranslatableString(getKey(@"smoothness"), @"Smoothness");

        public static LocalisableString VisualizerAmount => new TranslatableString(getKey(@"visualizer_amount"), @"Visualizer Amount");

        public static LocalisableString BarsPerVisual => new TranslatableString(getKey(@"bars_per_visual"), @"Bars Per Visual");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
