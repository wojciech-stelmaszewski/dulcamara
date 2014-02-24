using System;
using SharpDX.DXGI;

namespace Stelmaszewskiw.Space.Main
{
    public class DX3D11
    {
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; set; }

        private void Initialize(SystemConfiguration systemConfiguration, IntPtr windowHandle)
        {
            try
            {
                //Store the vsync setting.
                var verticalSyncEnabled = systemConfiguration.VerticalSyncEnabled;

                var rational = new Rational(0, 1);
       
                //Create a DirectX graphics interface factory.
                using (var factory = new Factory())
                {
                    //Use the factory to create an adapter for the primary graphics interface (video card).
                    using (var adapter = factory.GetAdapter(0))
                    {
                        //Get primary adapter output (monitor).
                        using (var monitor = adapter.Outputs[0])
                        {
                            //Get modes that fit the DXGI_FORMAT_R8G8B8A8_UNORM display format for the adapter output (monitor).
                            var modes = monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm,
                                                                   DisplayModeEnumerationFlags.Interlaced);

                            //Now go through all the display modes and find the one that matches the screen width and height.
                            //When a match is found store the refresh rate for that monitor, if vertical sync is enabled.
                            //Otherwise we use default maximum refresh rate.


                            if (verticalSyncEnabled)
                            {
                                foreach (var mode in modes)
                                {
                                    if (mode.Width == systemConfiguration.Width &&
                                        mode.Height == systemConfiguration.Height)
                                    {
                                        rational = new Rational(mode.RefreshRate.Numerator, mode.RefreshRate.Denominator);
                                        break;
                                    }
                                }
                            }

                            //Get the adapter (video card) description.
                            var adapterDescription = adapter.Description;

                            //Store the dedicated video card memory in the megabytes.
                            VideoCardMemory = adapterDescription.DedicatedVideoMemory >> 10 >> 10;
                                //We divide two times by 1024 = 2^10.

                            //Convert the name of the video card to a character array and store it.
                            VideoCardDescription = adapterDescription.Description;
                        }
                    }
                }

                //Turn multisampling off.
                var sampleDescription = new SampleDescription()
                                           {
                                               Count = 1,
                                               Quality = 0
                                           };

                //Initialize the swap chain description.
                var swapChainDescription = new SwapChainDescription()
                                               {
                                                   //Set to a single back buffer.
                                                   BufferCount = 1,
                                                   //Set the width and height of the back buffer.
                                                   ModeDescription =
                                                       new ModeDescription(systemConfiguration.Width,
                                                                           systemConfiguration.Height, rational,
                                                                           Format.R8G8B8A8_UNorm),
                                                   //Set the usage of the back buffer.
                                                   Usage = Usage.RenderTargetOutput,
                                                   //Set the handle for the window to render to.
                                                   OutputHandle = windowHandle,
                                                   //Set multisampling as defined above.
                                                   SampleDescription = sampleDescription,
                                                   //Set the full screen mode according to system configuration.
                                                   IsWindowed = !systemConfiguration.FullScreen,
                                                   //Don't sent the advanced flags.
                                                   Flags = SwapChainFlags.None,
                                                   //Discard the back buffer content after processing.
                                                   SwapEffect = SwapEffect.Discard
                                               };
            }
            catch (Exception exception)
            {
                
                throw;
            }
        }
    }
}
