using Ninject.Modules;

namespace Stelmaszewskiw.Space.Main
{
    class ContainerBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ISystemConfiguration>().To<SystemConfiguration>().InSingletonScope();
        }
    }
}
