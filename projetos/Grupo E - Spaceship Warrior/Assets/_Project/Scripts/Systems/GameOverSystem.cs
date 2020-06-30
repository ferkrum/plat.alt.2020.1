using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DestroyEventSystem))]
    public sealed class GameOverSystem : SystemBase
    {
        private UIManagerReference _uiManager;

        protected override void OnCreate()
        {
            EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<GeneratorTag>());

            RequireForUpdate(query);
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<GameIsRunningTag>();
            RequireSingletonForUpdate<GeneratorsInitializedTag>();
        }

        protected override void OnStartRunning()
        {
            Entity entity = GetSingletonEntity<UIManagerReference>();
            _uiManager = EntityManager.GetComponentObject<UIManagerReference>(entity);
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnStopRunning()
        {
            _uiManager.Value.ShowGameOverPanel();

            Entity gameStateEntity = GetSingletonEntity<GameState>();
            Entity gameIsRunningEntity = GetSingletonEntity<GameIsRunningTag>();
            Entity generatorsInitializedEntity = GetSingletonEntity<GeneratorsInitializedTag>();

            EntityManager.RemoveComponent<GameIsRunningTag>(gameIsRunningEntity);
            EntityManager.RemoveComponent<GeneratorsInitializedTag>(generatorsInitializedEntity);

            var gameState = GetSingleton<GameState>();
            gameState.CurrentWave = 0;
            gameState.TotalScore = 0;

            EntityManager.SetComponentData(gameStateEntity, gameState);
        }
    }
}
