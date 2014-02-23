namespace Stelmaszewskiw.Space.Main
{
    public class SystemConfiguration
    {
        /// <summary>
        /// Window title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the window.
        /// </summary>
        public int Height { get; set; }

        public bool WaitVerticalBlanking { get; set; }
    }
}
