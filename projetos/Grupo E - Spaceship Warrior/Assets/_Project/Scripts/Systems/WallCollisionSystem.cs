using Coimbra.Dots.Physics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [DisableAutoCreation]
    public sealed class WallCollisionSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<BounceBackImpulse> BounceBackImpulseType;
            [ReadOnly] public ArchetypeChunkBufferType<CollisionEventBufferElement> CollisionEventBufferType;
            [ReadOnly] public ComponentDataFromEntity<PlayerTag> PlayerTagFromEntity;
            [ReadOnly] public ComponentDataFromEntity<PhysicsMass> PhysicsMassFromEntity;
            [ReadOnly] public ComponentDataFromEntity<LocalToWorld> LocalToWorldFromEntity;

            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityFromEntity;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
                NativeArray<BounceBackImpulse> bounceBackImpulseArray = chunk.GetNativeArray(BounceBackImpulseType);
                BufferAccessor<CollisionEventBufferElement> collisionEventBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Entity entityA = entityArray[i];
                    BounceBackImpulse bounceBackImpulse = bounceBackImpulseArray[i];
                    DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = collisionEventBufferAccessor[i];

                    for (var j = 0; j < collisionEventBuffer.Length; j++)
                    {
                        CollisionEventBufferElement collisionEvent = collisionEventBuffer[j];

                        if (collisionEvent.State != PhysicsEventState.Enter)
                        {
                            continue;
                        }

                        Entity entityB = collisionEvent.Entity;

                        if (PlayerTagFromEntity.HasComponent(entityA))
                        {
                            ApplyImpulse(entityA, bounceBackImpulse.Value);
                        }

                        if (PlayerTagFromEntity.HasComponent(entityB))
                        {
                            ApplyImpulse(entityB, bounceBackImpulse.Value);
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void ApplyImpulse(Entity entity, float value)
            {
                if (!PhysicsVelocityFromEntity.HasComponent(entity))
                {
                    return;
                }

                if (!PhysicsMassFromEntity.HasComponent(entity))
                {
                    return;
                }

                if (!LocalToWorldFromEntity.HasComponent(entity))
                {
                    return;
                }

                PhysicsVelocity physicsVelocity = PhysicsVelocityFromEntity[entity];
                PhysicsMass physicsMass = PhysicsMassFromEntity[entity];
                LocalToWorld localToWorld = LocalToWorldFromEntity[entity];

                physicsVelocity.ApplyLinearImpulse(physicsMass, -localToWorld.Forward * value);
                PhysicsVelocityFromEntity[entity] = physicsVelocity;
            }
        }

        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(new []
            {
                ComponentType.ReadOnly<WallTag>(),
                ComponentType.ReadOnly<BounceBackImpulse>(),
                ComponentType.ReadOnly<CollisionEventBufferElement>()
            });
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                EntityType = GetArchetypeChunkEntityType(),
                BounceBackImpulseType = GetArchetypeChunkComponentType<BounceBackImpulse>(true),
                CollisionEventBufferType = GetArchetypeChunkBufferType<CollisionEventBufferElement>(true),
                PlayerTagFromEntity = GetComponentDataFromEntity<PlayerTag>(true),
                PhysicsMassFromEntity = GetComponentDataFromEntity<PhysicsMass>(true),
                LocalToWorldFromEntity = GetComponentDataFromEntity<LocalToWorld>(true),
                PhysicsVelocityFromEntity = GetComponentDataFromEntity<PhysicsVelocity>()
            }.ScheduleSingle(_query, Dependency);
        }
    }
}
