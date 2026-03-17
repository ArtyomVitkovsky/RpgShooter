using System;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using GameTemplate.UI;
using UniRx;
using UnityEngine;
using TMPro;
using Zenject;

namespace _Project.Scripts.UI.PlayerScreen
{
    public class UIPlayerStatsScreen : UIStackScreen
    {
        [Inject] private DiContainer _container;
        [Inject(Optional = true)] private SignalBus _signalBus;
        [Inject(Optional = true)] private IPlayerStatsService _playerStatsService;
        
        [Inject(Id = NavigatorIds.GamePlayScreensNavigator)] 
        private UIStackNavigator gameplayScreensNavigator;

        [SerializeField] private UIPlayerStatView statViewPrefab;
        [SerializeField] private Transform statsContainer;
        [SerializeField] private AnimatedButton backButton;
        [SerializeField] private AnimatedButton applyButton;
        [SerializeField] private TMP_Text skillPointsText;

        private CharacterStatData[] _snapshotStats;
        private int _snapshotSkillPoints;

        private void Awake()
        {
            if (statViewPrefab == null || statsContainer == null)
            {
                return;
            }

            foreach (CharacterStatType type in Enum.GetValues(typeof(CharacterStatType)))
            {
                var view = _container.InstantiatePrefabForComponent<UIPlayerStatView>(
                    statViewPrefab,
                    statsContainer);

                view.Initialize(type);

                view.IncreaseLevelButtonClickAsObservable
                    .Subscribe(_ => UpdateSkillPointsText())
                    .AddTo(this);
            }

            backButton.OnButtonClick.Subscribe(HandleBackButtonClick).AddTo(this);
            applyButton.OnButtonClick.Subscribe(HandleApplyButtonClick).AddTo(this);
        }

        private void OnEnable()
        {
            _signalBus?.Fire<PlayerStatsScreenOpenedSignal>();
            SetCursorEnabled(true);

            if (_playerStatsService != null)
            {
                _snapshotStats = _playerStatsService.GetAllStats();
                _snapshotSkillPoints = _playerStatsService.GetAvailableSkillPoints();
            }

            UpdateSkillPointsText();
        }

        private void OnDisable()
        {
            _signalBus?.Fire<PlayerStatsScreenClosedSignal>();
            SetCursorEnabled(false);
        }

        public void SetCursorEnabled(bool isEnabled)
        {
#if !UNITY_ANDROID && !UNITY_IPHONE
            if (isEnabled)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
#endif
        }

        private void HandleBackButtonClick(AnimatedButton button)
        {
            if (_playerStatsService != null && _snapshotStats != null)
            {
                _playerStatsService.RestoreStats(_snapshotStats, _snapshotSkillPoints);
            }

            gameplayScreensNavigator.Pop();
        }

        private void HandleApplyButtonClick(AnimatedButton button)
        {
            _snapshotStats = null;
            gameplayScreensNavigator.Pop();
        }

        private void UpdateSkillPointsText()
        {
            if (skillPointsText == null || _playerStatsService == null)
            {
                return;
            }

            skillPointsText.text = _playerStatsService.GetAvailableSkillPoints().ToString();
        }
    }
}

