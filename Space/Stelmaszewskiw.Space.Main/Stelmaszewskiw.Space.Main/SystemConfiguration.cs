namespace Stelmaszewskiw.Space.Main
{
    public interface ISystemConfiguration
    {
        /// <summary>
        /// Window title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Height of the window.
        /// </summary>
        int Height { get; set; }

        bool WaitVerticalBlanking { get; set; }

        /// <summary>
        /// Full screen mode indicator.
        /// </summary>
        bool FullScreen { get; set; }

        /// <summary>
        /// Screen refresh sync with buffer swaping rate indicator.
        /// </summary>
        bool VerticalSyncEnabled { get; set; }

        /// <summary>
        /// Far plane distance.
        /// </summary>
        float ScreenDepth { get; set; }

        /// <summary>
        /// Near plane distance.
        /// </summary>
        float ScreenNear { get; set; }
    }

    public class SystemConfiguration : ISystemConfiguration
    {
        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool WaitVerticalBlanking { get; set; }

        public bool FullScreen { get; set; }

        public bool VerticalSyncEnabled { get; set; }

        public float ScreenDepth { get; set; }

        public float ScreenNear { get; set; }
    }
}
