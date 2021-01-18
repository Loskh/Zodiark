namespace Zodiark.Namazu
{
    /// <summary>
    /// Preset Model for use in the application.
    /// </summary>
    public class Preset
    {
        public Preset()
        {
        }

        public static Preset Reset;

        static Preset()
        {
            Reset = new Preset
            {
                A = new Waymark(0, 0, 0, 0, false),
                B = new Waymark(0, 0, 0, 0, false),
                C = new Waymark(0, 0, 0, 0, false),
                D = new Waymark(0, 0, 0, 0, false),
                One = new Waymark(0, 0, 0, 0, false),
                Two = new Waymark(0, 0, 0, 0, false),
                Three = new Waymark(0, 0, 0, 0, false),
                Four = new Waymark(0, 0, 0, 0, false)
            };
        }

        /// <summary>
        /// Name of this preset.
        /// </summary>
        //public string Name { get; set; }

        /// <summary>
        /// Map ID of where this preset belongs.
        /// </summary>
        //public uint MapID { get; set; }

        /// <summary>
        /// Waymark values for all of every waymark in the game.
        /// </summary>
        public Waymark A { get; set; }

        public Waymark B { get; set; }
        public Waymark C { get; set; }
        public Waymark D { get; set; }
        public Waymark One { get; set; }
        public Waymark Two { get; set; }
        public Waymark Three { get; set; }
        public Waymark Four { get; set; }

        /// <summary>
        /// Property Changed event handler for this model.
        /// </summary>
#pragma warning disable 67
#pragma warning restore 67
    }
}