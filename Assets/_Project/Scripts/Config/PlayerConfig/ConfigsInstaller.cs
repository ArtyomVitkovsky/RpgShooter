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
        
        public override void InstallBindings()
        {
            Container.BindInstance(playerMovementConfig);
            Container.BindInstance(playerStatLevelConfigsSet);
        }
    }
}