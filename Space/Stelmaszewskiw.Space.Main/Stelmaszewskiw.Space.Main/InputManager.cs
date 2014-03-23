using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Stelmaszewskiw.Space.Main
{
    public class InputManager : IDisposable, ICloneable
    {
        private readonly Dictionary<Keys, bool> inputKeys = new Dictionary<Keys, bool>();

        public InputManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                inputKeys[(Keys) key] = false;
            }
        }

        public bool IsKeyDown(Keys key)
        {
            return inputKeys[key];
        }

        public void KeyDown(Keys key)
        {
            inputKeys[key] = true;
        }

        public void KeyUp(Keys key)
        {
            inputKeys[key] = false;
        }

        public void Dispose()
        {
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
