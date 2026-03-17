using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Enemy.Health;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using _Project.Scripts.Gameplay.Enemy;
using _Project.Scripts.Gameplay.Player;

namespace _Project.Scripts.Gameplay.Enemy.Movement
{
    public class EnemyMovementComponent : IInitializable, ITickable
    {
        [Inject] private IConfigService _configService;
        [Inject] private EnemyHealthComponent _enemyHealthComponent;

        [Inject(Id = EnemyCharacterInstaller.ENEMY_NAVMESH_AGENT)]
        private NavMeshAgent _agent;

        [Inject(Id = EnemyCharacterInstaller.ENEMY_TRANSFORM)]
        private Transform _enemyTransform;

        [Inject(Id = PlayerCharacterInstaller.PLAYER_TRANSFORM)]
        private Transform _playerTransform;

        [Inject(Id = EnemyCharacterInstaller.ENEMY_DETECTION_RADIUS)]
        private float _detectionRadius;

        [Inject(Id = EnemyCharacterInstaller.ENEMY_STOPPING_DISTANCE)]
        private float _stoppingDistance;

        private float _moveSpeed;

        public void Initialize()
        {
            if (_agent == null)
            {
                return;
            }

            _moveSpeed = _agent.speed;

            if (_configService != null)
            {
                float configuredSpeed = _configService.GetConfig<CharacterStatType, float>(CharacterStatType.Speed);
                if (configuredSpeed > 0f)
                {
                    _moveSpeed = configuredSpeed;
                }
            }

            _agent.stoppingDistance = _stoppingDistance;
            _agent.speed = _moveSpeed;

            if (_enemyHealthComponent != null)
            {
                _enemyHealthComponent.Died += OnDied;
            }
        }

        public void Tick()
        {
            if (_agent == null || _playerTransform == null || _enemyTransform == null)
            {
                return;
            }

            if (_enemyHealthComponent != null && !_enemyHealthComponent.IsAlive)
            {
                if (!_agent.isStopped)
                {
                    _agent.isStopped = true;
                }

                return;
            }

            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);

            if (distanceToPlayer <= _detectionRadius)
            {
                if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                }

                _agent.SetDestination(_playerTransform.position);
            }
            else
            {
                if (!_agent.isStopped)
                {
                    _agent.isStopped = true;
                }
            }
        }

        private void OnDied()
        {
            if (_agent != null)
            {
                _agent.isStopped = true;
            }
        }
    }
}

