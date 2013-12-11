using System;

namespace Stelmaszewskiw.Space.Core.Game
{
    public interface IGameComponent : IDisposable
    {
        IGame Game { get; }
    }
}
