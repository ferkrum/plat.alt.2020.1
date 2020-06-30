using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public sealed class DamageEventSystem : SystemBase
    {
        [BurstCompile]
        private struct DamageJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<DamageEvent> DamageEventType;

            public ArchetypeChunkComponentType<CurrentHealth> CurrentHealthType;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
                NativeArray<DamageEvent> damageEventArray = chunk.GetNativeArray(DamageEventType);
                NativeArray<CurrentHealth> currentHealthArray = chunk.GetNativeArray(CurrentHealthType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Entity entity = entityArray[i];
                    DamageEvent damageEvent = damageEventArray[i];
                    CurrentHealth currentHealth = currentHealthArray[i];

                    EntityCommandBuffer.RemoveComponent<DamageEvent>(entity);

                    int result = currentHealth.Value - damageEvent.Value;
                    currentHealth.Value = result < 0 ? 0 : result;

                    if (currentHealth.Value == 0)
                    {
                        EntityCommandBuffer.AddComponent<DestroyEventTag>(entity);
                    }

                    currentHealthArray[i] = currentHealth;
                }
            }
        }

        private static readonly ComponentType[] ComponentTypes =
        {
            ComponentType.ReadOnly<DamageEvent>(),
            ComponentType.ReadWrite<CurrentHealth>()
        };

        private EntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(ComponentTypes);
            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new DamageJob
            {
                EntityType = GetArchetypeChunkEntityType(),
                DamageEventType = GetArchetypeChunkComponentType<DamageEvent>(true),
                CurrentHealthType = GetArchetypeChunkComponentType<CurrentHealth>(),
                EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
            }.ScheduleSingle(_query, Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
