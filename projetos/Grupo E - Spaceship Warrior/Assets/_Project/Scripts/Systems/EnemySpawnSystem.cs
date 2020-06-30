using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpaceshipWarrior
{
    public sealed class EnemySpawnSystem : SystemBase
    {
        public sealed class Settings : IComponentData
        {
            public uint MaxWaveCount;
            public Entity EnemyPrefab;
            public AnimationCurve SpawnTimerCurve;
            public AnimationCurve SpawnCountCurve;
        }

        private float _timeToNextWave;
        private Settings _settings;
        private Random _random;
        private EntityQuery _spawnPointsQuery;

        protected override void OnCreate()
        {
            _random = new Random(585720582);
            _spawnPointsQuery = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<EnemySpawnPointTag>(),
                ComponentType.ReadOnly<LocalToWorld>()
            });

            RequireSingletonForUpdate<Settings>();
            RequireSingletonForUpdate<GameState>();
            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnStartRunning()
        {
            Entity settingsEntity = GetSingletonEntity<Settings>();
            _settings = EntityManager.GetComponentObject<Settings>(settingsEntity);
        }

        protected override void OnUpdate()
        {
            Dependency.Complete();

            _timeToNextWave -= Time.DeltaTime;

            if (_timeToNextWave > 0)
            {
                return;
            }

            var gameState = GetSingleton<GameState>();
            gameState.CurrentWave++;

            SetSingleton(gameState);

            float time = math.unlerp(1, _settings.MaxWaveCount, gameState.CurrentWave);
            var spawnCount = (int)math.round(_settings.SpawnCountCurve.Evaluate(time));
            var spawnPoints = new NativeArray<LocalToWorld>(spawnCount, Allocator.Temp);
            NativeArray<LocalToWorld> potentialSpawnPoints = _spawnPointsQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);

            if (potentialSpawnPoints.Length == 0)
            {
                return;
            }

            for (var i = 0; i < spawnCount; i++)
            {
                spawnPoints[i] = potentialSpawnPoints[_random.NextInt(potentialSpawnPoints.Length)];
            }

            foreach (LocalToWorld spawnPoint in spawnPoints)
            {
                Entity enemyEntity = EntityManager.Instantiate(_settings.EnemyPrefab);
                EntityManager.SetComponentData(enemyEntity, new Translation { Value = spawnPoint.Position });
            }

            _timeToNextWave = _settings.SpawnTimerCurve.Evaluate(time);
        }
    }
}
