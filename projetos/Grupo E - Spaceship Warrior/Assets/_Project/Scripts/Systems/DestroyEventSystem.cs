using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(DamageEventSystem))]
    public sealed class DestroyEventSystem : SystemBase
    {
        [BurstCompile]
        private struct DestroyJob : IJobChunk
        {
            [ReadOnly] public Entity SfxHolderEntity;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ComponentDataFromEntity<EnemyTag> EnemyTagFromEntity;
            [ReadOnly] public ComponentDataFromEntity<GeneratorTag> GeneratorTagFromEntity;

            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Entity entity = entityArray[i];

                    if (EnemyTagFromEntity.HasComponent(entity))
                    {
                        EntityCommandBuffer.AddComponent<EnemyExplosionSoundEventTag>(SfxHolderEntity);
                    }
                    else if (GeneratorTagFromEntity.HasComponent(entity))
                    {
                        EntityCommandBuffer.AddComponent<GeneratorExplosionSoundEventTag>(SfxHolderEntity);
                    }
                    else
                    {
                        EntityCommandBuffer.AddComponent<ProjectileHitSoundEventTag>(SfxHolderEntity);
                    }

                    EntityCommandBuffer.DestroyEntity(entity);
                }
            }
        }

        private EntityQuery _query;
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(new EntityQueryDesc
            {
                Any = new [] { ComponentType.ReadOnly<DestroyEventTag>(), ComponentType.ReadOnly<SelfDestroyEventTag>() }
            });

            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new DestroyJob
            {
                SfxHolderEntity = GetSingletonEntity<GameIsRunningTag>(),
                EntityType = GetArchetypeChunkEntityType(),
                EnemyTagFromEntity = GetComponentDataFromEntity<EnemyTag>(true),
                GeneratorTagFromEntity = GetComponentDataFromEntity<GeneratorTag>(true),
                EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
            }.ScheduleSingle(_query, Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
