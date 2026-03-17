using System;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player;
using _Project.Scripts.UI.PlayerScreen;
using GameTemplate.UI;
using Zenject;

namespace _Project.Scripts.Gameplay.Services
{
    public class PlayerInputCommandServiceInstaller : Installer<PlayerInputCommandServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerInputCommandService>().AsSingle().NonLazy();
        }
    }

    public class PlayerInputCommandService : IInitializable, IDisposable
    {
        private readonly IInputService _inputService;
        private readonly UIStackNavigator _gameplayScreensNavigator;

        private PlayerActionsProvider _actionsProvider;

        public PlayerInputCommandService(
            IInputService inputService,
            [Inject(Id = NavigatorIds.GamePlayScreensNavigator)] UIStackNavigator gameplayScreensNavigator)
        {
            _inputService = inputService;
            _gameplayScreensNavigator = gameplayScreensNavigator;
        }

        public void Initialize()
        {
            _actionsProvider = _inputService.GetActionsProvider();

            if (_actionsProvider != null)
            {
                _actionsProvider.OnKButtonPressedAction += HandleKButtonPressed;
            }
        }

        private void HandleKButtonPressed()
        {
            _gameplayScreensNavigator.Push<UIPlayerStatsScreen>();
        }

        public void Dispose()
        {
            if (_actionsProvider != null)
            {
                _actionsProvider.OnKButtonPressedAction -= HandleKButtonPressed;
            }
        }
    }
}

