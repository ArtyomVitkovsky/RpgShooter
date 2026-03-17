using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Config.PlayerConfig
{
    [CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Configs/ConfigsInstaller")]
    public class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
    {
        [SerializeField] private PlayerMovementConfig playerMovementConfig;
        [SerializeField] private PlayerStatLevelConfigsSet playerStatLevelConfigsSet;
        [SerializeField] private Gameplay.Enemy.EnemyStatConfig enemyStatConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(playerMovementConfig);
            Container
                .Bind<IConfig<CharacterStatType, PlayerStatLevelConfig>>()
                .FromInstance(playerStatLevelConfigsSet);
            Container
                .Bind<IConfig<CharacterStatType, float>>()
                .FromInstance(enemyStatConfig);
        }
    }
}