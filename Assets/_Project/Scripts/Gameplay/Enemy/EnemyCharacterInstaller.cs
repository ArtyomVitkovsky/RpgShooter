using _Project.Scripts.Gameplay.Enemy.Health;
using _Project.Scripts.Gameplay.Enemy.Movement;
using _Project.Scripts.Gameplay.Player;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts.Gameplay.Enemy
{
    public class EnemyCharacterInstaller : MonoInstaller
    {
        public const string ENEMY_TRANSFORM = "ENEMY_TRANSFORM";
        public const string ENEMY_NAVMESH_AGENT = "ENEMY_NAVMESH_AGENT";
        public const string ENEMY_DETECTION_RADIUS = "ENEMY_DETECTION_RADIUS";
        public const string ENEMY_STOPPING_DISTANCE = "ENEMY_STOPPING_DISTANCE";

        [SerializeField] private EnemyCharacter enemyCharacter;
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private NavMeshAgent navMeshAgent;
        

        [Header("Movement Settings")]
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float stoppingDistance = 1.5f;

        public override void InstallBindings()
        {
            Container.BindInstance(enemyTransform).WithId(ENEMY_TRANSFORM);
            Container.BindInstance(navMeshAgent).WithId(ENEMY_NAVMESH_AGENT);

            Container.BindInstance(detectionRadius).WithId(ENEMY_DETECTION_RADIUS);
            Container.BindInstance(stoppingDistance).WithId(ENEMY_STOPPING_DISTANCE);

            Container.BindInstance(enemyCharacter).AsSingle();

            Container.BindInterfacesAndSelfTo<EnemyHealthComponent>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EnemyMovementComponent>().AsSingle().NonLazy();
        }
    }
}

