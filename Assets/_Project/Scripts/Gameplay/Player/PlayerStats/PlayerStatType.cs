using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Services;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.PlayerStats
{
    public enum PlayerStatType
    {
        Health = 0,
        Speed = 1,
        Damage = 2,
    }
    
    [Serializable]
    public struct PlayerStatData
    {
        public PlayerStatType Type;
        public int CurrentLevel;
        public float Value;
    }

    public interface IPlayerStatsService
    {
        public void Bootstrap();
        
        public PlayerStatData GetStat(PlayerStatType type);
        
        public float GetStatValue(PlayerStatType type);
        
        public PlayerStatData IncreaseStatLevel(PlayerStatType type);
        
        public PlayerStatData DecreaseStatLevel(PlayerStatType type);
    }

    public class PlayerStatsServiceInstaller : Installer<PlayerStatsServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerStatsService>().AsSingle().NonLazy();
        }
    }
    
    public class PlayerStatsService : IPlayerStatsService
    {
        private const string StatSaveKeyPrefix = "PlayerStat_";
        
        [Inject] private ISavingService savingService;
        [Inject] private IConfigService configService;

        private List<PlayerStatData> playerStatsData;
        private Dictionary<PlayerStatType, float> playerStatsDataValues;
        
        public void Bootstrap()
        {
            playerStatsData = new List<PlayerStatData>();
            playerStatsDataValues = new Dictionary<PlayerStatType, float>();

            foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
            {
                string key = GetSaveKey(type);

                PlayerStatData data;

                if (savingService.HasKey(key))
                {
                    data = savingService.Load<PlayerStatData>(key);
                }
                else
                {
                    data = GetDefaultStatData(type);
                }

                UpdateValueFromConfig(ref data);
                
                playerStatsData.Add(data);
                playerStatsDataValues[type] = data.Value;
            }
        }

        public PlayerStatData GetStat(PlayerStatType type)
        {
            int index = GetStatIndex(type);
            if (index < 0)
            {
                var defaultData = GetDefaultStatData(type);
                UpdateValueFromConfig(ref defaultData);
                return defaultData;
            }

            var data = playerStatsData[index];
            UpdateValueFromConfig(ref data);
            playerStatsData[index] = data;
            playerStatsDataValues[type] = data.Value;

            return data;
        }

        public float GetStatValue(PlayerStatType type)
        {
            if (playerStatsDataValues != null && playerStatsDataValues.TryGetValue(type, out float value))
            {
                return value;
            }

            var data = GetDefaultStatData(type);
            UpdateValueFromConfig(ref data);
            return data.Value;
        }

        public PlayerStatData IncreaseStatLevel(PlayerStatType type)
        {
            int index = GetStatIndex(type);
            if (index < 0)
            {
                var defaultData = GetDefaultStatData(type);
                UpdateValueFromConfig(ref defaultData);
                return defaultData;
            }

            var data = playerStatsData[index];
            data.CurrentLevel++;

            UpdateValueFromConfig(ref data);

            playerStatsData[index] = data;
            playerStatsDataValues[type] = data.Value;

            savingService.Save(GetSaveKey(type), data);

            return data;
        }

        public PlayerStatData DecreaseStatLevel(PlayerStatType type)
        {
            int index = GetStatIndex(type);
            if (index < 0)
            {
                var defaultData = GetDefaultStatData(type);
                UpdateValueFromConfig(ref defaultData);
                return defaultData;
            }

            var data = playerStatsData[index];
            if (data.CurrentLevel > 0)
            {
                data.CurrentLevel--;
            }

            UpdateValueFromConfig(ref data);

            playerStatsData[index] = data;
            playerStatsDataValues[type] = data.Value;

            savingService.Save(GetSaveKey(type), data);

            return data;
        }

        private int GetStatIndex(PlayerStatType type)
        {
            if (playerStatsData == null)
            {
                return -1;
            }
            
            return playerStatsData.FindIndex(s => s.Type == type);
        }

        private PlayerStatData GetDefaultStatData(PlayerStatType type)
        {
            if (configService != null)
            {
                var config = configService.GetPlayerStatLevelConfig(type);
                if (config != null)
                {
                    var levelData = config.GetLevelData(0);

                    return new PlayerStatData()
                    {
                        Type = type,
                        CurrentLevel = 1,
                        Value = levelData.Level
                    };
                }
            }

            return new PlayerStatData
            {
                Type = type,
                CurrentLevel = 1,
                Value = 0f
            };
        }

        private static string GetSaveKey(PlayerStatType type)
        {
            return $"{StatSaveKeyPrefix}{type}";
        }

        private void UpdateValueFromConfig(ref PlayerStatData data)
        {
            if (configService == null)
            {
                return;
            }

            var config = configService.GetPlayerStatLevelConfig(data.Type);
            if (config == null)
            {
                return;
            }

            var levelData = config.GetLevelData(data.CurrentLevel);
            data.Value = levelData.Value;
        }
    }
}