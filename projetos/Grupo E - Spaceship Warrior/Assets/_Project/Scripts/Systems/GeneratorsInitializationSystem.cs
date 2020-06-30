using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed class GeneratorsInitializationSystem : SystemBase
    {
        public struct Settings : IComponentData
        {
            public Entity GeneratorPrefab;
        }

        private Settings _settings;
        private EntityQuery _generatorsInitializedQuery;
        private EntityQuery _spawnPointsQuery;

        protected override void OnCreate()
        {
            _generatorsInitializedQuery = GetEntityQuery(ComponentType.ReadOnly<GeneratorsInitializedTag>());
            _spawnPointsQuery = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<GeneratorSpawnPointTag>(),
                ComponentType.ReadOnly<LocalToWorld>()
            });

            RequireSingletonForUpdate<Settings>();
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnStartRunning()
        {
            _settings = GetSingleton<Settings>();
        }

        protected override void OnUpdate()
        {
            int entityCount = _generatorsInitializedQuery.CalculateEntityCount();

            if (entityCount > 0)
            {
                return;
            }

            NativeArray<LocalToWorld> spawnPoints = _spawnPointsQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);

            foreach (LocalToWorld spawnPoint in spawnPoints)
            {
                Entity generatorEntity = EntityManager.Instantiate(_settings.GeneratorPrefab);
                EntityManager.SetComponentData(generatorEntity, new Translation { Value = spawnPoint.Position });
            }

            Entity gameStateEntity = GetSingletonEntity<GameState>();
            EntityManager.AddComponent<GeneratorsInitializedTag>(gameStateEntity);
        }
    }
}
