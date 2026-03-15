using System;
using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.Movement;
using _Project.Scripts.UI.PlayerScreen;
using GameTemplate.UI;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        [Inject] private PlayerMovementComponent playerMovementComponent;
        [Inject] private PlayerDamageableComponent playerDamageableComponent;

        [Inject(Id = NavigatorIds.GamePlayScreensNavigator)] 
        private UIStackNavigator gameplayScreens;

        private void Start()
        {
            gameplayScreens.Push<UIPlayerScreen>();
        }
    }
}