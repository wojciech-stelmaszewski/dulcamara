using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;

namespace Stelmaszewskiw.Space.Main
{
    public class System : ICloneable
    {
        private bool isFormResizing;
        private bool isFormClosed;

        private Form MainForm { get; set; }
        private FormWindowState CurrentFormWindowState { get; set; }

        private SystemConfiguration SystemConfiguration { get; set; }
        private InputManager InputManager { get; set; }
        private GraphicsManager GraphicsManager { get; set; }

        public bool Initialize()
        {
            if(SystemConfiguration == null)
            {
                SystemConfiguration = new SystemConfiguration
                                          {
                                              Title = SystemConfigurationDefaults.Title,
                                              Width = SystemConfigurationDefaults.Width,
                                              Height = SystemConfigurationDefaults.Height,
                                              WaitVerticalBlanking = false,
                                          };
            }

            InitializeWindows();

            if(InputManager == null)
            {
                InputManager = new InputManager();
            }

            if(GraphicsManager == null)
            {
                GraphicsManager = new GraphicsManager(SystemConfiguration, MainForm.Handle);
            }

            return true;
        }

        private void InitializeWindows()
        {
            if(MainForm != null)
            {
                return;
            }

            MainForm = new RenderForm(SystemConfiguration.Title)
                           {
                               ClientSize = new Size(SystemConfiguration.Width, SystemConfiguration.Height)
                           };

            MainForm.Show();
        }

        public void Run()
        {
            Initialize();

            MainForm.KeyDown += MainFormKeyDown;
            MainForm.KeyUp += MainFormKeyUp;

            MainForm.Closed += MainFormClosed;
            MainForm.MouseEnter += MainFormMouseEnter;
            MainForm.MouseLeave += MainFormMouseLeave;

            MainForm.Resize += MainFormResize;
            MainForm.ResizeBegin += MainFormResizeBegin;
            MainForm.ResizeEnd += MainFormResizeEnd;

            RenderLoop.Run(MainForm, RenderCallback);
        }

        private void MainFormClosed(object sender, EventArgs e)
        {
            isFormClosed = true;
        }

        private void MainFormMouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void MainFormMouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        private void MainFormResize(object sender, EventArgs e)
        {
            if(MainForm.WindowState != CurrentFormWindowState)
            {
                HandleResize(sender, e);
            }

            CurrentFormWindowState = MainForm.WindowState;
        }

        private void MainFormResizeBegin(object sender, EventArgs e)
        {
            isFormResizing = true;
        }

        private void MainFormResizeEnd(object sender, EventArgs e)
        {
            isFormResizing = false;
            HandleResize(sender, e);
        }

        public void Shutdown()
        {
            if(GraphicsManager != null)
            {
                GraphicsManager.Shutdown();
                GraphicsManager = null;
            }

            if(InputManager != null)
            {
                InputManager = null;
            }

            ShotdownWindow();
        }

        private void RenderCallback()
        {
            if(isFormClosed)
            {
                return;
            }

            var result = Frame();

            if(!result)
            {
                Exit();
            }
        }

        private void MainFormKeyUp(object sender, KeyEventArgs e)
        {
            InputManager.KeyUp(e.KeyCode);
        }

        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            InputManager.KeyDown(e.KeyCode);
        }

        private void HandleResize(object sender, EventArgs e)
        {
            if(MainForm.WindowState == FormWindowState.Minimized)
            {
                return;
            }
        }

        private void Exit()
        {
            MainForm.Close();
        }

        private bool Frame()
        {
            if(InputManager.IsKeyDown(Keys.Escape))
            {
                return false;
            }

            return GraphicsManager.Frame();
        }

        private void ShotdownWindow()
        {
            if(MainForm != null)
            {
                MainForm.Dispose();
            }

            MainForm = null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
