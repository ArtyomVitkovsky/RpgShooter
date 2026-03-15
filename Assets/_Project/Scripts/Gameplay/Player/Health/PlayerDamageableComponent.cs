using _Project.Scripts.Gameplay.Encounter;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.Health
{
    public class PlayerDamageableComponent : IDamageable
    {
        [Inject] private IPlayerHealthService _playerHealthService;

        public bool IsAlive => _playerHealthService != null && _playerHealthService.CurrentHealth > 0f;

        public void TakeDamage(float amount)
        {
            if (_playerHealthService == null || amount <= 0f)
            {
                return;
            }

            _playerHealthService.TakeDamage(amount);
        }
    }
}

