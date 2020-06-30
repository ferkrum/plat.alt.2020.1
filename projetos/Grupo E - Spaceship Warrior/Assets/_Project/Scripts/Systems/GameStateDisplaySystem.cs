using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed class GameStateDisplaySystem : SystemBase
    {
        private UIManagerReference _uiManager;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameState>();
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
            var gameState = GetSingleton<GameState>();

            _uiManager.Value.UpdateCurrentWave(gameState.CurrentWave);
            _uiManager.Value.UpdateScore(gameState.TotalScore);
        }
    }
}
