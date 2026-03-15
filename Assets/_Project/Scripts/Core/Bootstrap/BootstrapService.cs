using _Project.Scripts.Constants;
using _Project.Scripts.Core.Services;
using Cysharp.Threading.Tasks;
using GameTemplate.UI;
using UnityEngine;
using Zenject;

namespace GameTemplate.Core.Bootstrap
{
    public interface IBootstrapService
    {
        UniTask Bootstrap();
    }

    public class BootstrapService : IBootstrapService
    {
        private readonly IScenesService _scenesService;
        private readonly UIStackNavigator _rootNavigator;
        private readonly UIStackNavigator _overlayNavigator;
        
        public BootstrapService(
            IScenesService scenesService,
            [Inject(Id = NavigatorIds.RootScreensNavigator)] UIStackNavigator rootNavigator,
            [Inject(Id = NavigatorIds.OverlayScreensNavigator)] UIStackNavigator overlayNavigator
            )
        {
            _scenesService = scenesService;
            _rootNavigator = rootNavigator;
            _overlayNavigator = overlayNavigator;
            
            Bootstrap();
        }

        public async UniTask Bootstrap()
        {
            await _scenesService.LoadScene(SceneNames.GAMEPLAY_SCENE);

            await UniTask.WaitForSeconds(1);
        }
    }
}


