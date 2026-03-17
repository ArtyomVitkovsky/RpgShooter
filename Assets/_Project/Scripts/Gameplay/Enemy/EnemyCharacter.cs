using System;
using _Project.Scripts.Gameplay.Enemy.Health;
using _Project.Scripts.Gameplay.Encounter;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Enemy
{
    public class EnemyCharacter : MonoBehaviour, IDamageable
    {
        [Inject] private EnemyHealthComponent _enemyHealthComponent;
        [Inject] private IPlayerStatsService _playerStatsService;

        public bool IsAlive => _enemyHealthComponent != null && _enemyHealthComponent.IsAlive;

        public event Action Died
        {
            add
            {
                if (_enemyHealthComponent != null)
                {
                    _enemyHealthComponent.Died += value;
                }
            }
            remove
            {
                if (_enemyHealthComponent != null)
                {
                    _enemyHealthComponent.Died -= value;
                }
            }
        }

        public void TakeDamage(float amount)
        {
            if (_enemyHealthComponent == null)
            {
                return;
            }

            bool wasAlive = _enemyHealthComponent.IsAlive;
            _enemyHealthComponent.TakeDamage(amount);

            if (wasAlive && !_enemyHealthComponent.IsAlive && _playerStatsService != null)
            {
                _playerStatsService.AddSkillPoints(1);
            }
        }
    }
}

