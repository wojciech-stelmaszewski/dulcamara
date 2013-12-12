using System;
using SharpDX.Toolkit;

namespace Stelmaszewskiw.Space.Core.Game
{
    public abstract class UpdateableGameComponentBase : GameComponentBase, SharpDX.Toolkit.IUpdateable
    {
        private bool _enabled;
        private int _updateOrder;

        protected UpdateableGameComponentBase(IGame game) : base(game)
        {
            Enabled = true;
        }

        public virtual bool Enabled
        {
            get { return _enabled; }
            private set
            {
                _enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged.Invoke(this, new EventArgs());
                }
            }
        }

        public virtual int UpdateOrder
        {
            get { return _updateOrder; }
            private set
            {
                _updateOrder = value;
                if (UpdateOrderChanged != null)
                {
                    UpdateOrderChanged.Invoke(null, new EventArgs());
                }
            }
        }

        public virtual event EventHandler<EventArgs> EnabledChanged;

        public virtual event EventHandler<EventArgs> UpdateOrderChanged;
        
        public abstract void Update(GameTime gameTime);
    }
}
