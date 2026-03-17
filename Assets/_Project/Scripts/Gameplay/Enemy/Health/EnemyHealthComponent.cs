using System;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Encounter;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Enemy.Health
{
    public class EnemyHealthComponent
    {
        private readonly IConfigService _configService;
        private readonly float _baseMaxHealth;

        private float _currentHealth;

        public bool IsAlive => _currentHealth > 0f;

        public event Action Died;

        [Inject]
        public EnemyHealthComponent(
            IConfigService configService)
        {
            _configService = configService;

            InitializeHealth();
        }

        private void InitializeHealth()
        {
            float configuredMaxHealth = _baseMaxHealth;

            if (_configService != null)
            {
                configuredMaxHealth = _configService.GetConfig<CharacterStatType, float>(CharacterStatType.Health);
            }

            configuredMaxHealth = Mathf.Max(configuredMaxHealth, 0f);

            _currentHealth = Mathf.Clamp(configuredMaxHealth, 0f, configuredMaxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || !IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0f);

            if (!IsAlive)
            {
                Died?.Invoke();
            }
        }
    }
}

