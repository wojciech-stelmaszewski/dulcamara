using System;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace Stelmaszewskiw.Space.Main
{
    public class DX3D11 : IDisposable
    {
        public Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }

        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; set; }

        private SwapChain swapChain;
        private RenderTargetView renderTargetView;
        private Texture2D depthStencilBuffer;
        private DepthStencilView depthStencilView;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;
        private bool verticalSyncEnabled;

        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix OrthographicProjectionMatrix { get; private set; }


        public bool Initialize(SystemConfiguration systemConfiguration, IntPtr windowHandle)
        {
            try
            {
                //Store the vsync setting.
                verticalSyncEnabled = systemConfiguration.VerticalSyncEnabled;

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

                //Create the swap chain, Direct3D device, and Direct3D device context.
                Device device;
                Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None,
                                                              swapChainDescription, out device, out swapChain);


                Device = device;
                DeviceContext = device.ImmediateContext;

                //Get the pointer to the back buffer.
                using (var backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                {
                    //Create the render target view with the back buffer pointer.
                    renderTargetView = new RenderTargetView(device, backBuffer);
                }
                
                //Initialize and set up the description of the depth buffer.
                var depthBufferDescription = new Texture2DDescription
                                                 {
                                                     Width = systemConfiguration.Width,
                                                     Height = systemConfiguration.Height,
                                                     MipLevels = 1,
                                                     ArraySize = 1,
                                                     Format = Format.D24_UNorm_S8_UInt,
                                                     SampleDescription = sampleDescription,
                                                     Usage = ResourceUsage.Default,
                                                     BindFlags = BindFlags.DepthStencil,
                                                     CpuAccessFlags = CpuAccessFlags.None,
                                                     OptionFlags = ResourceOptionFlags.None
                                                 };

                //Create the texture for the depth buffer using the filled out description.
                depthStencilBuffer = new Texture2D(device, depthBufferDescription);

                //Stencil operation if pixel is front-facing.
                var frontFaceDepthStencilOperationDescription = new DepthStencilOperationDescription
                                                           {
                                                               FailOperation = StencilOperation.Keep,
                                                               DepthFailOperation = StencilOperation.Increment,
                                                               PassOperation = StencilOperation.Keep,
                                                               Comparison = Comparison.Always
                                                           };

                //Stencil operation if pixel is back-facing.
                var backFaceDepthStencilOperationDescription = new DepthStencilOperationDescription
                                                                   {
                                                                       FailOperation = StencilOperation.Keep,
                                                                       DepthFailOperation = StencilOperation.Decrement,
                                                                       PassOperation = StencilOperation.Keep,
                                                                       Comparison = Comparison.Always
                                                                   };

                //Initialize and set up the description of the stencil state.
                var depthStencilDescription = new DepthStencilStateDescription
                                                  {
                                                      IsDepthEnabled = true,
                                                      DepthWriteMask = DepthWriteMask.All,
                                                      DepthComparison = Comparison.Less,
                                                      IsStencilEnabled = true,
                                                      StencilReadMask = 0xFF,
                                                      StencilWriteMask = 0xFF,
                                                      FrontFace = frontFaceDepthStencilOperationDescription,
                                                      BackFace = backFaceDepthStencilOperationDescription
                                                  };

                //Create the depth stencil state.
                depthStencilState = new DepthStencilState(Device, depthStencilDescription);

                //Set the depth stencil state.
                DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState);

                //Initialize and set up the depth stencil view.
                var depthStencilViewDescription = new DepthStencilViewDescription
                                                      {
                                                          Format = Format.D24_UNorm_S8_UInt,
                                                          Dimension = DepthStencilViewDimension.Texture2D,
                                                          Texture2D =
                                                              new DepthStencilViewDescription.Texture2DResource
                                                                  {
                                                                      MipSlice = 0
                                                                  }
                                                      };

                //Create the depth stencil view.
                depthStencilView = new DepthStencilView(Device, depthStencilBuffer, depthStencilViewDescription);

                //Bind the render target view and depth stencil buffer to the output render pipeline.
                DeviceContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);

                //Setup the rasterizer state description which will deterimine how and what polygon will be drawn.
                var rasterizerStateDescription = new RasterizerStateDescription()
                                                {
                                                    IsAntialiasedLineEnabled = false,
                                                    CullMode = CullMode.Back,
                                                    DepthBias = 0,
                                                    DepthBiasClamp = 0.0f,
                                                    IsDepthClipEnabled = true,
                                                    FillMode = FillMode.Solid,
                                                    IsFrontCounterClockwise = false,
                                                    IsMultisampleEnabled = false,
                                                    IsScissorEnabled = false,
                                                    SlopeScaledDepthBias = 0.0f
                                                };

                //Create the rasterizer state from description we just filled out.
                rasterizerState = new RasterizerState(Device, rasterizerStateDescription);

                //Now set the rasterizer state.
                DeviceContext.Rasterizer.State = rasterizerState;

                //Setup and create the viewport for rendering.
                DeviceContext.Rasterizer.SetViewport(0, 0, systemConfiguration.Width, systemConfiguration.Height, 0, 1);

                //Setup and create projection matrix.
                ProjectionMatrix = Matrix.PerspectiveFovLH(MathConstsFloat.PiOverFour,
                                                           (float)systemConfiguration.Width/systemConfiguration.Height,
                                                           SystemConfigurationDefaults.ScreenNear,
                                                           SystemConfigurationDefaults.ScreenDepth);

                //Initialize the world matrix to the identity matrix.
                WorldMatrix = Matrix.Identity;

                //Create an ortographic projection matrix for 2D rendering.
                OrthographicProjectionMatrix = Matrix.OrthoLH(systemConfiguration.Width, systemConfiguration.Height,
                                                              SystemConfigurationDefaults.ScreenNear,
                                                              SystemConfigurationDefaults.ScreenDepth);
                
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            //Before shutting down set to windowed mode or when you release the swao chain it will throw an exception.
            if(swapChain != null)
            {
                swapChain.SetFullscreenState(false, null);
            }

            if(rasterizerState != null)
            {
                rasterizerState.Dispose();
                rasterizerState = null;
            }

            if(depthStencilView != null)
            {
                depthStencilView.Dispose();
                depthStencilView = null;
            }

            if(depthStencilState != null)
            {
                depthStencilState.Dispose();
                depthStencilState = null;
            }

            if(depthStencilBuffer != null)
            {
                depthStencilBuffer.Dispose();
                depthStencilBuffer = null;
            }

            if(renderTargetView != null)
            {
                renderTargetView.Dispose();
                renderTargetView = null;
            }

            if(Device != null)
            {
                Device.Dispose();
                Device = null;
            }

            if(swapChain != null)
            {
                swapChain.Dispose();
                swapChain = null;
            }
        }

        public void BeginScene()
        {
            //Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);

            //Clear the back buffer.
            //TODO Standarize colors usage.
            var color = new Color4(100.0f/255, 149.0f/255, 237.0f/255, 0.0f); //Temporary CornflowerBlue.
            DeviceContext.ClearRenderTargetView(renderTargetView, color);
        }

        public void EndScene()
        {
            //Present the back buffer to the screen since rendering is complete.
            if(verticalSyncEnabled)
            {
                //Lock to screen refresh rate.
                swapChain.Present(1, 0);
            }
            else
            {
                //Present as fast as possible.
                swapChain.Present(0, 0);
            }
        }

    }
}
