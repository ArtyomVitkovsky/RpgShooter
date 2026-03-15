using Zenject;

namespace _Project.Scripts.Gameplay.Services
{
    public class GameplayBootstrapServiceInstaller : Installer<GameplayBootstrapServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BootstrapService>().AsSingle().NonLazy();
        }
    }
}
