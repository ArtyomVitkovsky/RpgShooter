using System.Collections.Generic;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public interface IConfigService
    {
        PlayerStatLevelConfig GetPlayerStatLevelConfig(PlayerStatType type);
    }

    public class ConfigServiceInstaller : Installer<ConfigServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ConfigService>().AsSingle().NonLazy();
        }
    }

    public class ConfigService : IConfigService
    {
        [Inject] private PlayerStatLevelConfigsSet playerStatLevelConfigs;

        public PlayerStatLevelConfig GetPlayerStatLevelConfig(PlayerStatType type)
        {
            return playerStatLevelConfigs.GetConfig(type);
        }
    }
}

