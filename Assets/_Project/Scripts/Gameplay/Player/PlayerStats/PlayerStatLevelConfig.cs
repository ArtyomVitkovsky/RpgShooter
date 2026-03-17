using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Player.PlayerStats
{
    [Serializable]
    public struct PlayerStatLevelData
    {
        public int Level;
        public int Value;
    }
    
    [CreateAssetMenu(menuName = "Configs/Player Stat Level Config", fileName = "PlayerStatLevelConfig")]
    public class PlayerStatLevelConfig : ScriptableObject
    {
        [SerializeField] private CharacterStatType type;

        [SerializeField] private List<PlayerStatLevelData> levels;

        public CharacterStatType Type => type;

        public PlayerStatLevelData GetLevelData(int requestedLevel)
        {
            if (levels == null || levels.Count == 0)
            {
                Debug.LogError($"Level config for {type} is empty!");
                return default;
            }

            var sortedLevels = levels.OrderBy(l => l.Level).ToList();

            PlayerStatLevelData bestFit = sortedLevels[0];

            foreach (var data in sortedLevels)
            {
                if (data.Level == requestedLevel)
                {
                    return data;
                }

                if (data.Level < requestedLevel)
                {
                    bestFit = data;
                }
                else
                {
                    break;
                }
            }

            return bestFit;
        }
    }
}

