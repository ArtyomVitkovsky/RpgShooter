using System;
using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.Movement;
using _Project.Scripts.UI.PlayerInputScreen;
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
        
        [Inject(Id = NavigatorIds.GamePlayAdditionalScreensNavigator)]
        private UIStackNavigator additionalScreens;

        private void Start()
        {
            gameplayScreens.Push<UIPlayerScreen>();

#if UNITY_ANDROID || UNITY_IPHONE
            additionalScreens.Push<UIPlayerInputScreen>();
#endif
        }
    }
}