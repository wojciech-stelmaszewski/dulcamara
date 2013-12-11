using System;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stelmaszewskiw.Space.Core.Game;

namespace Stelmaszewskiw.Space.Input
{
    public interface IKeyboardManager : IGameComponent
    {
        bool IsKeyDown(Keys key);
        bool IsKeyUp(Keys key);
    }

    public class KeyboardManager : UpdateableGameComponentBase, IKeyboardManager
    {
        private readonly SharpDX.Toolkit.Input.KeyboardManager _keyboardManager;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public KeyboardManager(IGame game)
            : base(game)
        {
            var sharpDxGame = game as Game;

            if (sharpDxGame == null)
            {
                throw new ApplicationException("Unknown game type!");
            }

            _keyboardManager = new SharpDX.Toolkit.Input.KeyboardManager(sharpDxGame);
        }

        public override void Update(GameTime gameTime)
        {
            _currentKeyboardState = _keyboardManager.GetState();

            //TODO This should be done after all other updates.
            _previousKeyboardState = _currentKeyboardState;
        }

        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }
        
        public bool IsKeyUp(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }
    }
}
