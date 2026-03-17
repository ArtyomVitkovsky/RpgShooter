using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Gameplay.Player.Weapons
{
    public class PlayerHitEnemyVfxPool : MonoBehaviour
    {
        [Inject(Optional = true)] private SignalBus _signalBus;

        [SerializeField] private ParticleSystem hitVfxPrefab;
        [SerializeField] private int poolSize = 16;
        [SerializeField] private int emitAmount = 16;

        private readonly List<ParticleSystem> _pool = new List<ParticleSystem>();
        private int _nextIndex;

        private void Awake()
        {
            if (hitVfxPrefab == null)
            {
                return;
            }

            for (int i = 0; i < poolSize; i++)
            {
                var instance = Instantiate(hitVfxPrefab, transform);
                instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _pool.Add(instance);
            }
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

        private void HandlePlayerHitEnemy(PlayerHitEnemySignal signal)
        {
            if (_pool.Count == 0)
            {
                return;
            }

            var vfx = _pool[_nextIndex];
            _nextIndex = (_nextIndex + 1) % _pool.Count;

            vfx.transform.position = signal.Point;
            vfx.transform.rotation = Quaternion.identity;
            vfx.gameObject.SetActive(true);
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            vfx.Emit(emitAmount);
        }
    }
}

