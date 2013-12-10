using SharpDX;
using SharpDX.Toolkit;

namespace Stelmaszewskiw.Space
{
    public class Space : Game
    {
        private readonly GraphicsDeviceManager _graphicDeviceManager;

        public Space()
        {
            _graphicDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Window.Title = "Space by Wojciech Stelmaszewski (2013)";

            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
