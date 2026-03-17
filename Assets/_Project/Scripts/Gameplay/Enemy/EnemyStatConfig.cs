using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Enemy
{
    [Serializable]
    public struct EnemyStatData
    {
        public CharacterStatType Type;
        public float Value;
    }

    [CreateAssetMenu(menuName = "Configs/Enemy Stat Config", fileName = "EnemyStatConfig")]
    public class EnemyStatConfig : ScriptableObject, IConfig<CharacterStatType, float>
    {
        [SerializeField] private List<EnemyStatData> stats;

        public float GetConfig(CharacterStatType key)
        {
            if (stats == null || stats.Count == 0)
            {
                Debug.LogWarning("EnemyStatConfig is empty, returning 0 for all stats.");
                return 0f;
            }

            foreach (var stat in stats)
            {
                if (stat.Type == key)
                {
                    return stat.Value;
                }
            }

            Debug.LogWarning($"EnemyStatConfig: No value configured for stat type {key}, returning 0.");
            return 0f;
        }
    }
}

