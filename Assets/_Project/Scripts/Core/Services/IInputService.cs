using _Project.Scripts.Gameplay.Player;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public interface IInputService
    {
        public PlayerActionsProvider GetActionsProvider();
    }

    public class InputService : IInputService
    {
        private PlayerActionsProvider playerActionsProvider;
        private InputSystem_Actions inputWrapper; 

        
        public PlayerActionsProvider GetActionsProvider()
        {
            if (playerActionsProvider == null)
            {
                playerActionsProvider = new PlayerActionsProvider();
                
                inputWrapper ??= new InputSystem_Actions();
                inputWrapper.Player.SetCallbacks(playerActionsProvider);
                inputWrapper.Player.Enable();
            }
            
            return playerActionsProvider;
        }
    }

    public class InputServiceInstaller : Installer<InputServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle().NonLazy();
        }
    }
}