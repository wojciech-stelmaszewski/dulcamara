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

        /// <summary>
        /// Full screen mode indicator.
        /// </summary>
        public bool FullScreen { get; set; }

        /// <summary>
        /// Screen refresh sync with buffer swaping rate indicator.
        /// </summary>
        public bool VerticalSyncEnabled { get; set; }

        /// <summary>
        /// Far plane distance.
        /// </summary>
        public float ScreenDepth { get; set; }

        /// <summary>
        /// Near plane distance.
        /// </summary>
        public float ScreenNear { get; set; }
    }
}
