using Coimbra.Dots.Physics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(DamageEventSystem))]
    public sealed class DamagerCollisionSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkBufferType<CollisionEventBufferElement> CollisionEventBufferType;
            [ReadOnly] public ComponentDataFromEntity<CurrentHealth> CurrentHealthGroup;
            [ReadOnly] public ComponentDataFromEntity<Damage> DamageGroup;

            public ComponentDataFromEntity<DamageEvent> DamageEventGroup;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
                BufferAccessor<CollisionEventBufferElement> collisionEventBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Entity entityA = entityArray[i];
                    DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = collisionEventBufferAccessor[i];

                    for (var j = 0; j < collisionEventBuffer.Length; j++)
                    {
                        CollisionEventBufferElement collisionEvent = collisionEventBuffer[j];

                        if (collisionEvent.State != PhysicsEventState.Enter)
                        {
                            continue;
                        }

                        Entity entityB = collisionEvent.Entity;

                        bool currentHealthExistsInA = CurrentHealthGroup.HasComponent(entityA);
                        bool currentHealthExistsInB = CurrentHealthGroup.HasComponent(entityB);
                        bool damageExistsInA = DamageGroup.HasComponent(entityA);
                        bool damageExistsInB = DamageGroup.HasComponent(entityB);

                        if (damageExistsInA && currentHealthExistsInB)
                        {
                            AddOrUpdateDamageEvent(entityB, entityA, DamageGroup[entityA]);
                        }

                        if (damageExistsInB && currentHealthExistsInA)
                        {
                            AddOrUpdateDamageEvent(entityA, entityB, DamageGroup[entityB]);
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void AddOrUpdateDamageEvent(Entity damagedEntity, Entity damagerEntity, Damage damage)
            {
                if (damage.AutoDestroy)
                {
                    EntityCommandBuffer.AddComponent<SelfDestroyEventTag>(damagerEntity);
                }

                if (DamageEventGroup.HasComponent(damagedEntity))
                {
                    DamageEvent damageEvent = DamageEventGroup[damagedEntity];
                    damageEvent.Value += damage.Value;
                }
                else
                {
                    EntityCommandBuffer.AddComponent(damagedEntity, new DamageEvent
                    {
                        Value = damage.Value
                    });
                }
            }
        }

        private EntityQuery _query;
        private EndFramePhysicsSystem _endFramePhysicsSystem;
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(new EntityQueryDesc
            {
                All = new [] { ComponentType.ReadOnly<CollisionEventBufferElement>() },
                Any = new[]
                {
                    ComponentType.ReadOnly<CurrentHealth>(),
                    ComponentType.ReadOnly<Damage>()
                }
            });

            _endFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
            _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                EntityType = GetArchetypeChunkEntityType(),
                CollisionEventBufferType = GetArchetypeChunkBufferType<CollisionEventBufferElement>(true),
                CurrentHealthGroup = GetComponentDataFromEntity<CurrentHealth>(true),
                DamageGroup = GetComponentDataFromEntity<Damage>(true),
                DamageEventGroup = GetComponentDataFromEntity<DamageEvent>(),
                EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
            }.ScheduleSingle(_query, Dependency);

            _endFramePhysicsSystem.AddInputDependency(Dependency);
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
