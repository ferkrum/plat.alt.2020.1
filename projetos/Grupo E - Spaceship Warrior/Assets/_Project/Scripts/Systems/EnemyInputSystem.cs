using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed class EnemyInputSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [DeallocateOnJobCompletion] public NativeArray<LocalToWorld> Targets;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;

            public ArchetypeChunkComponentType<MovementDirection> MovementDirectionType;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
                NativeArray<Translation> translationArray = chunk.GetNativeArray(TranslationType);
                NativeArray<MovementDirection> movementDirectionArray = chunk.GetNativeArray(MovementDirectionType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Translation currentTranslation = translationArray[i];
                    Translation targetTranslation = GetClosestTranslationFromArray(currentTranslation, Targets);
                    float3 direction = math.normalize(targetTranslation.Value - currentTranslation.Value);

                    direction.y = 0f;
                    movementDirectionArray[i] = new MovementDirection { Value = direction };

                    EntityCommandBuffer.AddComponent<HasTargetTag>(entityArray[i]);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Translation GetClosestTranslationFromArray(Translation current, NativeArray<LocalToWorld> potentialTargets)
            {
                var closestDistance = float.MaxValue;
                var closest = new Translation { Value = float3.zero };

                if (potentialTargets.Length == 0)
                {
                    return closest;
                }

                for (var i = 0; i < potentialTargets.Length; i++)
                {
                    float distance = math.distance(current.Value, potentialTargets[i].Position);

                    if (!(distance < closestDistance))
                    {
                        continue;
                    }

                    closestDistance = distance;
                    closest.Value = potentialTargets[i].Position;
                }

                return closest;
            }
        }

        private EntityQuery _updateQuery;
        private EntityQuery _targetEntitiesQuery;
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _updateQuery = GetEntityQuery(new []
            {
                ComponentType.ReadOnly<EnemyTag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadWrite<MovementDirection>()
            });

            _targetEntitiesQuery = GetEntityQuery(new []
            {
                ComponentType.ReadOnly<GeneratorTag>(),
                ComponentType.ReadOnly<LocalToWorld>()
            });

            _entityCommandBufferSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                Targets = _targetEntitiesQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                EntityType = GetArchetypeChunkEntityType(),
                TranslationType = GetArchetypeChunkComponentType<Translation>(true),
                MovementDirectionType = GetArchetypeChunkComponentType<MovementDirection>(),
                EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
            }.ScheduleSingle(_updateQuery, Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
