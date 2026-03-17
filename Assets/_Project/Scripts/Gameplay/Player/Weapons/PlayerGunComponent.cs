using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Enemy;
using _Project.Scripts.Gameplay.Encounter;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.Weapons
{
    public class PlayerGunComponent : IInitializable, ITickable
    {
        [Inject] private IInputService _inputService;
        [Inject] private IPlayerStatsService _playerStatsService;
        [Inject(Optional = true)] private SignalBus _signalBus;

        [Inject(Id = PlayerCharacterInstaller.MUZZLE_TRANSFORM)]
        private Transform _muzzleTransform;

        private PlayerActionsProvider _actions;
        private float _cooldown;
        private bool _attackRequested;

        private readonly float _fireRate = 10f;
        private readonly float _maxDistance = 100f;
        private readonly float _spreadAngle = 1.0f;
        private readonly LayerMask _hitLayers = ~0;

        public void Initialize()
        {
            _actions = _inputService.GetActionsProvider();
            if (_actions != null)
            {
                _actions.OnAttackAction += HandleAttackRequested;
            }
        }

        public void Tick()
        {
            if (_actions == null)
            {
                return;
            }

            _cooldown -= Time.deltaTime;

            if (_attackRequested && _cooldown <= 0f)
            {
                Shoot();
                _attackRequested = false;
            }
        }

        private void HandleAttackRequested()
        {
            _attackRequested = true;
        }

        private void Shoot()
        {
            if (_muzzleTransform == null)
            {
                return;
            }

            _cooldown = 1f / Mathf.Max(_fireRate, 0.01f);

            var damage = 10f;
            if (_playerStatsService != null)
            {
                damage = _playerStatsService.GetStatValue(CharacterStatType.Damage);
            }

            var direction = _muzzleTransform.forward;

            if (_spreadAngle > 0f)
            {
                var spread = Random.insideUnitCircle * _spreadAngle;
                var spreadRotation = Quaternion.Euler(spread.y, spread.x, 0f);
                direction = spreadRotation * direction;
            }

            var ray = new Ray(_muzzleTransform.position, direction);

            if (!Physics.Raycast(ray, out var hitInfo, _maxDistance, _hitLayers, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            var damageable = hitInfo.collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(damage);

                // Only show hit marker for enemies.
                if (_signalBus != null && hitInfo.collider.GetComponentInParent<EnemyCharacter>() != null)
                {
                    _signalBus.TryFire(new PlayerHitEnemySignal(hitInfo.point, damage));
                }
            }
        }
    }
}
