using Zenject;

namespace GameTemplate.Core.Bootstrap
{
    public class BootstrapServiceInstaller : Installer<BootstrapServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BootstrapService>().AsSingle().NonLazy();
        }
    }
}