using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(HealthInitializationSystem))]
    public sealed class PlayerHealthDisplaySystem : SystemBase
    {
        private UIManagerReference _uiManager;

        protected override void OnCreate()
        {
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GeneratorTag>(), ComponentType.ReadOnly<CurrentHealth>());
            query.SetChangedVersionFilter(ComponentType.ReadOnly<CurrentHealth>());

            RequireForUpdate(query);
            RequireSingletonForUpdate<UIManagerReference>();
            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnStartRunning()
        {
            Entity entity = GetSingletonEntity<UIManagerReference>();
            _uiManager = EntityManager.GetComponentObject<UIManagerReference>(entity);
        }

        protected override void OnUpdate()
        {
            var totalCurrentHealth = 0;
            var totalMaxHealth = 0;

            Entities
                .WithoutBurst()
                .WithAll<GeneratorTag>()
                .ForEach((in CurrentHealth currentHealth, in MaxHealth maxHealth) =>
                {
                    totalCurrentHealth += currentHealth.Value;
                    totalMaxHealth += maxHealth.Value;
                })
                .Run();

            _uiManager.Value.UpdateHealthBar((float)totalCurrentHealth / totalMaxHealth);
        }
    }
}
