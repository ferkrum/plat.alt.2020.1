using Unity.Entities;

namespace SpaceshipWarrior
{
    public sealed class SfxEventsSystem : SystemBase
    {
        public delegate void SfxHandler();

        public event SfxHandler OnFire;
        public event SfxHandler OnProjectileHit;
        public event SfxHandler OnEnemyExplosion;
        public event SfxHandler OnGeneratorExplosion;

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<FireSoundEventTag>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<FireSoundEventTag>(entity);
                    OnFire?.Invoke();
                })
                .Run();

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<ProjectileHitSoundEventTag>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<ProjectileHitSoundEventTag>(entity);
                    OnProjectileHit?.Invoke();
                })
                .Run();

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<EnemyExplosionSoundEventTag>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<EnemyExplosionSoundEventTag>(entity);
                    OnEnemyExplosion?.Invoke();
                })
                .Run();

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<GeneratorExplosionSoundEventTag>()
                .ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<GeneratorExplosionSoundEventTag>(entity);
                    OnGeneratorExplosion?.Invoke();
                })
                .Run();
        }
    }
}
