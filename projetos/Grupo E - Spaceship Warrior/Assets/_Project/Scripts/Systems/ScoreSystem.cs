using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DamageEventSystem))]
    [UpdateBefore(typeof(DestroyEventSystem))]
    public sealed class ScoreSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<Score> ScoreValueType;

            public Entity GameStateEntity;
            public GameState CurrentGameState;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                uint scoreDelta = 0;
                NativeArray<Score> scoreValueArray = chunk.GetNativeArray(ScoreValueType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    scoreDelta += scoreValueArray[i].Value;
                }

                CurrentGameState.TotalScore += scoreDelta;
                EntityCommandBuffer.SetComponent(GameStateEntity, CurrentGameState);
            }
        }

        private EntityQuery _query;
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<EnemyTag>(),
                ComponentType.ReadOnly<DestroyEventTag>(),
                ComponentType.ReadOnly<Score>()
            });

            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                ScoreValueType = GetArchetypeChunkComponentType<Score>(true),
                GameStateEntity = GetSingletonEntity<GameState>(),
                CurrentGameState = GetSingleton<GameState>(),
                EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
            }.ScheduleSingle(_query, Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
