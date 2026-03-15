using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Player.PlayerStats
{
    [CreateAssetMenu(menuName = "Configs/Player Stat Level Config Set", fileName = "PlayerStatLevelConfigSet")]
    public class PlayerStatLevelConfigsSet : ScriptableObject
    {
        [SerializeField] private List<PlayerStatLevelConfig> configs;

        public PlayerStatLevelConfig GetConfig(PlayerStatType type)
        {
            var config = configs.FirstOrDefault(c => c.Type == type);

            if (config == null)
            {
                return null;
            }

            return config;
        }
    }
}