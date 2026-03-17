using System;
using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.Weapons;
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
        [Inject(Optional = true)] private SignalBus _signalBus;
        
        [Inject(Id = NavigatorIds.GamePlayScreensNavigator)] 
        private UIStackNavigator gameplayScreensNavigator;

        [SerializeField] private Image healthBar;
        [SerializeField] private CanvasGroup hitMarkerCanvasGroup;
        [SerializeField] private float hitMarkerDuration = 0.08f;
        [SerializeField] private AnimatedButton playerStatsButton;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private IDisposable _hitMarkerRestore;
        
        private void Awake()
        {
            playerHealthService.CurrentHealthRx
                .CombineLatest(playerHealthService.MaxHealthRx, (cur, max) => (cur, max))
                .Subscribe(HandleCurrentHealthChanged)
                .AddTo(_disposables);

            hitMarkerCanvasGroup.alpha = 0f;

            playerStatsButton.OnButtonClick.Subscribe(HandlePlayerStatsButtonClick).AddTo(this);
        }

        private void HandlePlayerStatsButtonClick(AnimatedButton button)
        {
            gameplayScreensNavigator.Push<UIPlayerStatsScreen>();
        }

        private void OnEnable()
        {
            if (_signalBus != null)
            {
                _signalBus.Subscribe<PlayerHitEnemySignal>(HandlePlayerHitEnemy);
            }
        }

        private void OnDisable()
        {
            if (_signalBus != null)
            {
                _signalBus.TryUnsubscribe<PlayerHitEnemySignal>(HandlePlayerHitEnemy);
            }
        }

        private void HandleCurrentHealthChanged((float cur, float max) healthData)
        {
            healthBar.fillAmount = healthData.cur / healthData.max;
        }

        private void HandlePlayerHitEnemy(PlayerHitEnemySignal signal)
        {
            ShowHitMarker();
        }

        private void ShowHitMarker()
        {
            if (hitMarkerCanvasGroup == null)
            {
                return;
            }

            _hitMarkerRestore?.Dispose();
            hitMarkerCanvasGroup.alpha = 1f;

            if (hitMarkerDuration <= 0f)
            {
                hitMarkerCanvasGroup.alpha = 0f;
                return;
            }

            _hitMarkerRestore = Observable
                .Timer(TimeSpan.FromSeconds(hitMarkerDuration))
                .ObserveOnMainThread()
                .Subscribe(_ => hitMarkerCanvasGroup.alpha = 0f);
        }

        private void OnDestroy()
        {
            _hitMarkerRestore?.Dispose();
            _disposables.Dispose();
        }
    }
}