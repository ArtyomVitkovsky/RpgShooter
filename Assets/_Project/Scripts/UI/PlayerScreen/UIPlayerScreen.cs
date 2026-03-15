using _Project.Scripts.Gameplay.Player.Health;
using GameTemplate.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.PlayerScreen
{
    public class UIPlayerScreen : UIStackScreen
    {
        [Inject] private IPlayerHealthService playerHealthService;

        [SerializeField] private Image healthBar;
        
        private void Awake()
        {
            playerHealthService.CurrentHealthRx
                .CombineLatest(playerHealthService.MaxHealthRx, (cur, max) => (cur, max))
                .Subscribe(HandleCurrentHealthChanged);
        }

        private void HandleCurrentHealthChanged((float cur, float max) healthData)
        {
            healthBar.fillAmount = healthData.cur / healthData.max;
        }
    }
}