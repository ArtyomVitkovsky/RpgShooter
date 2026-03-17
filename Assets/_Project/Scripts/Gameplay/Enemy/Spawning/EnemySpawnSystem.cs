using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Gameplay.Enemy;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Gameplay.Enemy.Spawning
{
    public class EnemySpawnSystem : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int enemiesPerWave = 5;
        [SerializeField] private float spawnInterval = 0.5f;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int poolSize = 20;

        private readonly Queue<GameObject> _enemyPool = new Queue<GameObject>();
        private readonly List<EnemyCharacter> _aliveEnemies = new List<EnemyCharacter>();
        private readonly Dictionary<EnemyCharacter, Action> _deathHandlers =
            new Dictionary<EnemyCharacter, Action>();

        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            CreatePool();
            RunEndlessSpawnLoop().Forget();
        }

        private void CreatePool()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("EnemySpawnSystem: enemyPrefab is not assigned.");
                return;
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemyInstance;

                if (_container != null)
                {
                    enemyInstance = _container.InstantiatePrefab(enemyPrefab);
                }
                else
                {
                    enemyInstance = Instantiate(enemyPrefab);
                }

                enemyInstance.SetActive(false);
                _enemyPool.Enqueue(enemyInstance);
            }
        }

        private async UniTaskVoid RunEndlessSpawnLoop()
        {
            var token = this.GetCancellationTokenOnDestroy();

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    SpawnEnemy();
                    await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval), cancellationToken: token);

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }

                while (_aliveEnemies.Count > 0 && !token.IsCancellationRequested)
                {
                    await UniTask.Yield(token);
                }

                if (timeBetweenWaves > 0f && !token.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenWaves), cancellationToken: token);
                }
            }
        }

        private void SpawnEnemy()
        {
            if (_enemyPool.Count == 0)
            {
                Debug.LogWarning("EnemySpawnSystem: Pool exhausted, consider increasing poolSize.");
                return;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("EnemySpawnSystem: No spawn points assigned.");
                return;
            }

            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            var enemy = _enemyPool.Dequeue();
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            enemy.SetActive(true);

            var character = enemy.GetComponentInChildren<EnemyCharacter>();
            if (character != null)
            {
                _aliveEnemies.Add(character);

                if (!_deathHandlers.ContainsKey(character))
                {
                    Action handler = () => OnEnemyDied(character);
                    _deathHandlers[character] = handler;
                    character.Died += handler;
                }
            }
        }

        private void OnEnemyDied(EnemyCharacter character)
        {
            _aliveEnemies.Remove(character);

            if (_deathHandlers.TryGetValue(character, out var handler))
            {
                character.Died -= handler;
                _deathHandlers.Remove(character);
            }

            var enemy = character.gameObject;
            enemy.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }
    }
}

