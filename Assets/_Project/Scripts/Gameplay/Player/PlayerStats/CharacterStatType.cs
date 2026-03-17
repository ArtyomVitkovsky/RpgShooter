using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Services;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.PlayerStats
{
    public enum CharacterStatType
    {
        Health = 0,
        Speed = 1,
        Damage = 2,
    }
    
    [Serializable]
    public struct CharacterStatData
    {
        public CharacterStatType Type;
        public int CurrentLevel;
        public float Value;
    }

    public interface IPlayerStatsService
    {
        public void Bootstrap();
        
        public CharacterStatData GetStat(CharacterStatType type);

        public CharacterStatData[] GetAllStats();
        
        public int GetAvailableSkillPoints();
        
        public void AddSkillPoints(int amount);
        
        public float GetStatValue(CharacterStatType type);
        
        public CharacterStatData IncreaseStatLevel(CharacterStatType type);
        
        public CharacterStatData DecreaseStatLevel(CharacterStatType type);

        public void RestoreStats(CharacterStatData[] stats, int availableSkillPoints);
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
        private const string SkillPointsSaveKey = "Player_SkillPoints";
        
        [Inject] private ISavingService savingService;
        [Inject] private IConfigService configService;

        private List<CharacterStatData> playerStatsData;
        private Dictionary<CharacterStatType, float> playerStatsDataValues;
        private int _availableSkillPoints;
        
        public void Bootstrap()
        {
            playerStatsData = new List<CharacterStatData>();
            playerStatsDataValues = new Dictionary<CharacterStatType, float>();

            _availableSkillPoints = 0;
            if (savingService.HasKey(SkillPointsSaveKey))
            {
                _availableSkillPoints = savingService.Load<int>(SkillPointsSaveKey);
            }

            foreach (CharacterStatType type in Enum.GetValues(typeof(CharacterStatType)))
            {
                string key = GetSaveKey(type);

                CharacterStatData data;

                if (savingService.HasKey(key))
                {
                    data = savingService.Load<CharacterStatData>(key);
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

        public int GetAvailableSkillPoints()
        {
            return _availableSkillPoints;
        }

        public CharacterStatData[] GetAllStats()
        {
            if (playerStatsData == null)
            {
                return Array.Empty<CharacterStatData>();
            }

            return playerStatsData.ToArray();
        }

        public void AddSkillPoints(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _availableSkillPoints += amount;

            if (savingService != null)
            {
                savingService.Save(SkillPointsSaveKey, _availableSkillPoints);
            }
        }

        public CharacterStatData GetStat(CharacterStatType type)
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

        public void RestoreStats(CharacterStatData[] stats, int availableSkillPoints)
        {
            if (stats == null)
            {
                return;
            }

            playerStatsData = new List<CharacterStatData>(stats.Length);
            playerStatsDataValues = new Dictionary<CharacterStatType, float>(stats.Length);

            foreach (var stat in stats)
            {
                var data = stat;
                UpdateValueFromConfig(ref data);

                playerStatsData.Add(data);
                playerStatsDataValues[data.Type] = data.Value;

                if (savingService != null)
                {
                    savingService.Save(GetSaveKey(data.Type), data);
                }
            }

            _availableSkillPoints = availableSkillPoints;
            if (savingService != null)
            {
                savingService.Save(SkillPointsSaveKey, _availableSkillPoints);
            }
        }

        public float GetStatValue(CharacterStatType type)
        {
            if (playerStatsDataValues != null && playerStatsDataValues.TryGetValue(type, out float value))
            {
                return value;
            }

            var data = GetDefaultStatData(type);
            UpdateValueFromConfig(ref data);
            return data.Value;
        }

        public CharacterStatData IncreaseStatLevel(CharacterStatType type)
        {
            int index = GetStatIndex(type);
            if (index < 0)
            {
                var defaultData = GetDefaultStatData(type);
                UpdateValueFromConfig(ref defaultData);
                return defaultData;
            }

            if (_availableSkillPoints <= 0)
            {
                return playerStatsData[index];
            }

            var data = playerStatsData[index];
            data.CurrentLevel++;

            UpdateValueFromConfig(ref data);

            playerStatsData[index] = data;
            playerStatsDataValues[type] = data.Value;

            _availableSkillPoints--;
            if (savingService != null)
            {
                savingService.Save(SkillPointsSaveKey, _availableSkillPoints);
            }

            savingService.Save(GetSaveKey(type), data);

            return data;
        }

        public CharacterStatData DecreaseStatLevel(CharacterStatType type)
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

        private int GetStatIndex(CharacterStatType type)
        {
            if (playerStatsData == null)
            {
                return -1;
            }
            
            return playerStatsData.FindIndex(s => s.Type == type);
        }

        private CharacterStatData GetDefaultStatData(CharacterStatType type)
        {
            if (configService != null)
            {
                var config = configService.GetConfig<CharacterStatType, PlayerStatLevelConfig>(type);
                if (config != null)
                {
                    var levelData = config.GetLevelData(0);

                    return new CharacterStatData()
                    {
                        Type = type,
                        CurrentLevel = 1,
                        Value = levelData.Level
                    };
                }
            }

            return new CharacterStatData
            {
                Type = type,
                CurrentLevel = 1,
                Value = 0f
            };
        }

        private static string GetSaveKey(CharacterStatType type)
        {
            return $"{StatSaveKeyPrefix}{type}";
        }

        private void UpdateValueFromConfig(ref CharacterStatData data)
        {
            if (configService == null)
            {
                return;
            }

            var config = configService.GetConfig<CharacterStatType, PlayerStatLevelConfig>(data.Type);
            if (config == null)
            {
                return;
            }

            var levelData = config.GetLevelData(data.CurrentLevel);
            data.Value = levelData.Value;
        }
    }
}