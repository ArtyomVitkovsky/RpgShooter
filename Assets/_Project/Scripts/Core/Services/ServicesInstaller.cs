using GameTemplate.Core.Bootstrap;
using GameTemplate.UI;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public class ServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SavingServiceInstaller.Install(Container);
            
            UIScreenInstanceManagerInstaller.Install(Container);
            
            InputServiceInstaller.Install(Container);
            
            SceneServiceInstaller.Install(Container);
            
            BootstrapServiceInstaller.Install(Container);
        }
    }
}