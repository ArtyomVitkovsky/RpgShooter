using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.Movement;
using _Project.Scripts.Gameplay.Player.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Player
{
    public class PlayerCharacterInstaller : MonoInstaller
    {
        public const string PLAYER_TRANSFORM = "PLAYER_TRANSFORM";
        public const string PLAYER_RIGIDBODY = "PLAYER_RIGIDBODY";
        public const string CAMERA_HOLDER = "CAMERA_HOLDER";
        
        [SerializeField] private PlayerCharacter playerCharacter;
        [SerializeField] private Transform transform;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Transform cameraHolder;

        public override void InstallBindings()
        {
            Container.BindInstance(transform).WithId(PLAYER_TRANSFORM);
            Container.BindInstance(rigidbody).WithId(PLAYER_RIGIDBODY);
            Container.BindInstance(cameraHolder).WithId(CAMERA_HOLDER);
            
            Container.BindInterfacesAndSelfTo<PlayerMovementComponent>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerDamageableComponent>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerGunComponent>().AsSingle().NonLazy();

            Container.BindInstance(playerCharacter).AsSingle();
        }
    }
}