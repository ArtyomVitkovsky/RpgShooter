using System;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player.PlayerStats;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;

namespace _Project.Scripts.UI.PlayerScreen
{
    public class UIPlayerStatView : MonoBehaviour
    {
        [Inject] private IPlayerStatsService _playerStatsService;
        [Inject] private IConfigService _configService;

        [SerializeField] private TMP_Text statNameText;
        [SerializeField] private TMP_Text statLevelText;
        [SerializeField] private TMP_Text statValueText;
        [SerializeField] private AnimatedButton increaseLevelButton;

        private Subject<Unit> _onIncreaseLevelButtonClick = new Subject<Unit>();
        
        private CharacterStatType _type;
        
        public IObservable<Unit> IncreaseLevelButtonClickAsObservable => _onIncreaseLevelButtonClick;

        private void Awake()
        {
            if (increaseLevelButton != null)
            {
                increaseLevelButton.OnButtonClick.Subscribe(HandleIncreaseClicked).AddTo(this);
            }
        }

        public void Initialize(CharacterStatType type)
        {
            _type = type;
            UpdateView();
        }

        private void HandleIncreaseClicked(AnimatedButton animatedButton)
        {
            if (_playerStatsService == null)
            {
                return;
            }

            if (_playerStatsService.GetAvailableSkillPoints() <= 0)
            {
                return;
            }

            _playerStatsService.IncreaseStatLevel(_type);
            UpdateView();

            _onIncreaseLevelButtonClick.OnNext(Unit.Default);
        }

        private void UpdateView()
        {
            if (statNameText != null)
            {
                statNameText.text = _type.ToString();
            }

            if (_playerStatsService == null)
            {
                return;
            }

            var data = _playerStatsService.GetStat(_type);

            if (statLevelText != null)
            {
                statLevelText.text = $"Lv. {data.CurrentLevel}";
            }

            if (statValueText != null)
            {
                statValueText.text = data.Value.ToString();
            }

            if (_configService != null)
            {
                _configService.GetConfig<CharacterStatType, PlayerStatLevelConfig>(_type);
            }
        }
    }
}