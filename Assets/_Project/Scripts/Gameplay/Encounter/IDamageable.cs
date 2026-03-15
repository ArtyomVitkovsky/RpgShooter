namespace _Project.Scripts.Gameplay.Encounter
{
    public interface IDamageable
    {
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}