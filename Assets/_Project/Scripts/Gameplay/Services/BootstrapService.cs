using Cysharp.Threading.Tasks;
using GameTemplate.Core.Bootstrap;
using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.PlayerStats;

namespace _Project.Scripts.Gameplay.Services
{
    public class BootstrapService : IBootstrapService
    {
        private readonly IPlayerStatsService _playerStatsService;
        private readonly IPlayerHealthService _playerHealthService;

        public BootstrapService(
            IPlayerStatsService playerStatsService,
            IPlayerHealthService playerHealthService)
        {
            _playerStatsService = playerStatsService;
            _playerHealthService = playerHealthService;

            Bootstrap();
        }

        public UniTask Bootstrap()
        {
            _playerStatsService.Bootstrap();
            _playerHealthService.Bootstrap();

            return UniTask.CompletedTask;
        }
    }
}