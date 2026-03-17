using _Project.Scripts.Gameplay.Player.PlayerStats;
using _Project.Scripts.Core.Services;
using _Project.Scripts.Gameplay.Player.Health;
using _Project.Scripts.Gameplay.Player.Weapons;
using _Project.Scripts.Gameplay.Services;
using Zenject;

namespace _Project.Scripts.Gameplay
{
    public class GameplayServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            PlayerStatsServiceInstaller.Install(Container);
            ConfigServiceInstaller.Install(Container);
            PlayerHealthServiceInstaller.Install(Container);
            PlayerInputCommandServiceInstaller.Install(Container);
            GameplayBootstrapServiceInstaller.Install(Container);

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<PlayerHitEnemySignal>();
            Container.DeclareSignal<PlayerStatsScreenOpenedSignal>();
            Container.DeclareSignal<PlayerStatsScreenClosedSignal>();
        }
    }
}