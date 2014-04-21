using System;

namespace Stelmaszewskiw.Space.Main
{
    public static class ComponentsGetter
    {
        ///////////////// D3D11DEVICE COMPONENT //////////////////////////////////////////////////////////

        public const string D3D11DeviceComponentName = "D3D11Device";

        private static Func<SharpDX.Direct3D11.Device> _d3D11DeviceGetter;

        public static SharpDX.Direct3D11.Device D3D11Device
        {
            get
            {
                if(_d3D11DeviceGetter == null)
                {
                    ThrowNotInitializedComponentApplicationException(D3D11DeviceComponentName);
                }
// ReSharper disable PossibleNullReferenceException
                return _d3D11DeviceGetter();
// ReSharper restore PossibleNullReferenceException
            }
        }

        public static void InitializeD3D11DeviceComponent(Func<SharpDX.Direct3D11.Device> d3D11DeviceGetter)
        {
            _d3D11DeviceGetter = d3D11DeviceGetter;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        /// ///////////////// D3D11DEVICECONTEXT COMPONENT //////////////////////////////////////////////////////////

        public const string D3D11DeviceContextComponentName = "D3D11DeviceContext";

        private static Func<SharpDX.Direct3D11.DeviceContext> _d3D11DeviceContextGetter;

        public static SharpDX.Direct3D11.DeviceContext D3D11DeviceContext
        {
            get
            {
                if (_d3D11DeviceGetter == null)
                {
                    ThrowNotInitializedComponentApplicationException(D3D11DeviceContextComponentName);
                }
                // ReSharper disable PossibleNullReferenceException
                return _d3D11DeviceContextGetter();
                // ReSharper restore PossibleNullReferenceException
            }
        }

        public static void InitializeD3D11DeviceContextComponent(Func<SharpDX.Direct3D11.DeviceContext> d3D11DeviceContextGetter)
        {
            _d3D11DeviceContextGetter = d3D11DeviceContextGetter;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        private static void ThrowNotInitializedComponentApplicationException(string componentName)
        {
            throw new ApplicationException(String.Format("Component {0} was not initialized! You should use proper " +
                                                         "ComponetGetter initialization method.", componentName));
        }
    }
}
