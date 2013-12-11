using System.Collections.Generic;
using System.Linq;
using Ninject;
using SharpDX.Toolkit;

namespace Stelmaszewskiw.Space.Core.Game
{
    public abstract class GameBase : SharpDX.Toolkit.Game, IGame
    {
        protected IList<IGameComponent> _gameComponents;

        public IKernel Kernel { get; private set; }

        protected GameBase()
        {
            Kernel = new StandardKernel();
            _gameComponents = new List<IGameComponent>();

            
        }
        
        public virtual void RegisterGameComponents(IGameComponent gameComponent)
        {
            _gameComponents.Add(gameComponent);
        }

        protected override void Update(GameTime gameTime)
        {
            //TODO Ordering.
            foreach (var gameComponent in _gameComponents.OfType<IUpdateable>())
            {
                if (gameComponent.Enabled)
                {
                    gameComponent.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //TODO Ordering.
            foreach (var gameComponent in _gameComponents.OfType<IDrawable>())
            {
                if (gameComponent.Visible)
                {
                    gameComponent.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Kernel.Dispose();
            base.Dispose(disposeManagedResources);
        }
    }
}
