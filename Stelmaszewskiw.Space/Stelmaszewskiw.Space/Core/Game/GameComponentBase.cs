namespace Stelmaszewskiw.Space.Core.Game
{
    public class GameComponentBase : IGameComponent
    {
        public IGame Game { get; private set; }

        public GameComponentBase(IGame game)
        {
            Game = game;
        }

        public virtual void Dispose()
        {

        }
    }
}
