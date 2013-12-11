using Ninject;
using SharpDX.Toolkit.Graphics;

namespace Stelmaszewskiw.Space.Core.Game
{
    public interface IGame
    {
        IKernel Kernel { get; }

        GraphicsDevice GraphicsDevice { get; }

        void RegisterGameComponents(IGameComponent gameComponent);
    }
}
