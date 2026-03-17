using _Project.Scripts.Gameplay.Player.PlayerStats;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public interface IConfig<in K, out V>
    {
        V GetConfig(K key);
    }

    public interface IConfigService
    {
        public V GetConfig<K, V>(K key);
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
        private readonly IConfig<CharacterStatType, PlayerStatLevelConfig> _playerStatLevelConfigs;
        private readonly IConfig<CharacterStatType, float> _enemyStatConfigs;

        [Inject]
        public ConfigService(
            IConfig<CharacterStatType, PlayerStatLevelConfig> playerStatLevelConfigs,
            IConfig<CharacterStatType, float> enemyStatConfigs)
        {
            _playerStatLevelConfigs = playerStatLevelConfigs;
            _enemyStatConfigs = enemyStatConfigs;
        }

        public V GetConfig<K, V>(K key)
        {
            if (typeof(K) == typeof(CharacterStatType) && typeof(V) == typeof(PlayerStatLevelConfig))
            {
                var typedKey = (CharacterStatType)(object)key;
                var result = _playerStatLevelConfigs.GetConfig(typedKey);
                return (V)(object)result;
            }

            if (typeof(K) == typeof(CharacterStatType) && typeof(V) == typeof(float))
            {
                var typedKey = (CharacterStatType)(object)key;
                var result = _enemyStatConfigs.GetConfig(typedKey);
                return (V)(object)result;
            }

            throw new System.NotSupportedException(
                $"No config mapping registered for key type {typeof(K)} and value type {typeof(V)}");
        }
    }
}