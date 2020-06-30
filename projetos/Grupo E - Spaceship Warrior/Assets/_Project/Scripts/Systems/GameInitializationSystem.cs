using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InputSystemGroup))]
    public sealed class GameInitializationSystem : SystemBase
    {
        private EntityQuery _query;
        private UIManagerReference _uiManager;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(ComponentType.ReadOnly<GameIsRunningTag>());
        }

        protected override void OnStartRunning()
        {
            Entity entity = GetSingletonEntity<UIManagerReference>();
            _uiManager = EntityManager.GetComponentObject<UIManagerReference>(entity);
            _uiManager.Value.ShowStartPanel();
        }

        protected override void OnUpdate()
        {
            int entityCount = _query.CalculateEntityCount();

            if (entityCount > 0)
            {
                return;
            }

            var start = false;

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithAll<FireEventTag>()
                .ForEach((Entity entity) =>
                {
                    start = true;
                    EntityManager.RemoveComponent<FireEventTag>(entity);
                })
                .Run();

            if (!start)
            {
                return;
            }

            _uiManager.Value.HideActivePanel();

            Entity gameStateEntity = GetSingletonEntity<GameState>();
            EntityManager.AddComponent<GameIsRunningTag>(gameStateEntity);
        }
    }
}
