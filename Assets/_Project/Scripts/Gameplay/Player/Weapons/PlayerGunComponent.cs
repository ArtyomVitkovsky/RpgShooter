using _Project.Scripts.Core.Services;
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

        [Inject(Id = PlayerCharacterInstaller.CAMERA_HOLDER)]
        private Transform _muzzleTransform;

        private PlayerActionsProvider _actions;
        private float _cooldown;

        private readonly float _fireRate = 10f; // shots per second
        private readonly float _maxDistance = 100f;
        private readonly float _spreadAngle = 1.0f;
        private readonly LayerMask _hitLayers = ~0;

        public void Initialize()
        {
            _actions = _inputService.GetActionsProvider();
        }

        public void Tick()
        {
            if (_actions == null)
            {
                return;
            }

            _cooldown -= Time.deltaTime;

            if (_actions.Attack && _cooldown <= 0f)
            {
                Shoot();
            }
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
                damage = _playerStatsService.GetStatValue(PlayerStatType.Damage);
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
            }
        }
    }
}
