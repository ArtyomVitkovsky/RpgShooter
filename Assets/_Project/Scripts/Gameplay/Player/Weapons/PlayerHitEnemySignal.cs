using UnityEngine;

namespace _Project.Scripts.Gameplay.Player.Weapons
{
    public readonly struct PlayerHitEnemySignal
    {
        public readonly Vector3 Point;
        public readonly float Damage;

        public PlayerHitEnemySignal(Vector3 point, float damage)
        {
            Point = point;
            Damage = damage;
        }
    }
}

