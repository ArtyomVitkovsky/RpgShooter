using Zenject;

namespace _Project.Scripts.Core.Services
{
    public class SceneServiceInstaller : Installer<SceneServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ScenesService>().AsSingle().NonLazy();
        }
    }
}