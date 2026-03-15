using System;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UniRx;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.Health
{
    public interface IPlayerHealthService
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }

        IReadOnlyReactiveProperty<float> CurrentHealthRx { get; }
        IReadOnlyReactiveProperty<float> MaxHealthRx { get; }

        IObservable<Unit> OnDeath { get; }

        void Bootstrap();

        void SetMaxFromStats();
        void Heal(float amount);
        void TakeDamage(float amount);
        void SetHealth(float value);
    }

    public class PlayerHealthServiceInstaller : Installer<PlayerHealthServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerHealthService>().AsSingle().NonLazy();
        }
    }

    public class PlayerHealthService : IPlayerHealthService
    {
        [Inject] private IPlayerStatsService _playerStatsService;

        private readonly FloatReactiveProperty _currentHealth = new FloatReactiveProperty();
        private readonly FloatReactiveProperty _maxHealth = new FloatReactiveProperty();
        private readonly Subject<Unit> _onDeath = new Subject<Unit>();

        public float CurrentHealth => _currentHealth.Value;
        public float MaxHealth => _maxHealth.Value;

        public IReadOnlyReactiveProperty<float> CurrentHealthRx => _currentHealth;
        public IReadOnlyReactiveProperty<float> MaxHealthRx => _maxHealth;

        public IObservable<Unit> OnDeath => _onDeath;

        public void Bootstrap()
        {
            SetMaxFromStats();
            _currentHealth.Value = MaxHealth;
        }

        public void SetMaxFromStats()
        {
            if (_playerStatsService == null)
            {
                _maxHealth.Value = 0f;
                return;
            }

            var maxFromStats = _playerStatsService.GetStatValue(PlayerStatType.Health);
            _maxHealth.Value = maxFromStats > 0 ? maxFromStats : 0f;

            if (CurrentHealth > MaxHealth)
            {
                _currentHealth.Value = MaxHealth;
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || MaxHealth <= 0f)
            {
                return;
            }

            _currentHealth.Value = Math.Min(CurrentHealth + amount, MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || MaxHealth <= 0f)
            {
                return;
            }

            if (CurrentHealth <= 0f)
            {
                return;
            }

            _currentHealth.Value = Math.Max(CurrentHealth - amount, 0f);

            if (CurrentHealth <= 0f)
            {
                _onDeath.OnNext(Unit.Default);
            }
        }

        public void SetHealth(float value)
        {
            var clamped = Math.Clamp(value, 0f, MaxHealth);
            _currentHealth.Value = clamped;

            if (CurrentHealth <= 0f)
            {
                _onDeath.OnNext(Unit.Default);
            }
        }
    }
}

