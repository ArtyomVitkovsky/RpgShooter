using Zenject;

namespace GameTemplate.UI
{
    public class UIScreenInstanceManagerInstaller : Installer<UIScreenInstanceManagerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIScreenInstanceManager>().AsSingle().NonLazy();
        }
    }
}