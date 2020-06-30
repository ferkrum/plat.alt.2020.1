using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Coimbra.Dots.Physics
{
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    public sealed class CollisionEventsSystem : SystemBase
    {
        [BurstCompile]
        private struct CollisionEventJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<CollisionEventsListener> CollisionEventsListenerFromEntity;
            [ReadOnly] public PhysicsWorld PhysicsWorld;

            public BufferFromEntity<CollisionEventBufferElement> CollisionEventBufferFromEntity;

            public void Execute(CollisionEvent collisionEvent)
            {
                CollisionEvent.Details collisionEventDetails = default;

                bool calculateDetailsForA = false;
                bool calculateDetailsForB = false;

                if (CollisionEventsListenerFromEntity.Exists(collisionEvent.EntityA))
                {
                    calculateDetailsForA = CollisionEventsListenerFromEntity[collisionEvent.EntityA].CalculateDetails;
                }

                if (CollisionEventsListenerFromEntity.Exists(collisionEvent.EntityB))
                {
                    calculateDetailsForB = CollisionEventsListenerFromEntity[collisionEvent.EntityB].CalculateDetails;
                }

                if (calculateDetailsForA || calculateDetailsForB)
                {
                    collisionEventDetails = collisionEvent.CalculateDetails(ref PhysicsWorld);
                }

                ProcessEntity(collisionEvent.EntityA, collisionEvent.EntityB, collisionEvent.Normal, calculateDetailsForA, collisionEventDetails);
                ProcessEntity(collisionEvent.EntityB, collisionEvent.EntityA, collisionEvent.Normal, calculateDetailsForB, collisionEventDetails);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void ProcessEntity(Entity entity, Entity otherEntity, float3 normal, bool calculateDetails, CollisionEvent.Details collisionEventDetails)
            {
                if (CollisionEventBufferFromEntity.Exists(entity) == false)
                {
                    return;
                }

                DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = CollisionEventBufferFromEntity[entity];

                for (int i = 0; i < collisionEventBuffer.Length; i++)
                {
                    CollisionEventBufferElement collisionEventBufferElement = collisionEventBuffer[i];

                    if (collisionEventBufferElement.Entity != otherEntity)
                    {
                        continue;
                    }

                    collisionEventBufferElement.IsColliding = true;
                    collisionEventBufferElement.State = collisionEventBufferElement.State == PhysicsEventState.Exit ? PhysicsEventState.Enter : PhysicsEventState.Stay;
                    collisionEventBufferElement.Normal = normal;
                    collisionEventBufferElement.HasCollisionDetails = calculateDetails;
                    collisionEventBufferElement.CollisionDetails.AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition;
                    collisionEventBufferElement.CollisionDetails.EstimatedImpulse = collisionEventDetails.EstimatedImpulse;
                    collisionEventBuffer[i] = collisionEventBufferElement;

                    return;
                }

                collisionEventBuffer.Add(new CollisionEventBufferElement
                {
                    IsColliding = true,
                    State = PhysicsEventState.Enter,
                    Normal = normal,
                    Entity = otherEntity,
                    HasCollisionDetails = calculateDetails,
                    CollisionDetails = new CollisionDetails
                    {
                        EstimatedImpulse = collisionEventDetails.EstimatedImpulse,
                        AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition
                    }
                });
            }
        }

        private const int InitialCollisionDetailsCapacity = 256;

        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private EndFramePhysicsSystem _endFramePhysicsSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        private EntityQuery _entityQuery;
        private NativeHashMap<Entity, UnsafeList<CollisionEventBufferElement>> _collisionEvents;
        private NativeHashMap<Entity, UnsafeHashMap<Entity, CollisionEvent.Details>> _collisionDetails;

        public bool TryGetCollisionDetails(Entity entity, NativeHashMap<Entity, CollisionEvent.Details> results)
        {
            if (_collisionDetails.TryGetValue(entity, out UnsafeHashMap<Entity, CollisionEvent.Details> unsafeResults) == false)
            {
                results = default;

                return false;
            }

            results.Clear();
            GetCollisionDetails(unsafeResults, results);

            return true;
        }

        public bool TryGetCollisionDetails(Entity entity, out NativeHashMap<Entity, CollisionEvent.Details> results, Allocator allocator)
        {
            if (_collisionDetails.TryGetValue(entity, out UnsafeHashMap<Entity, CollisionEvent.Details> unsafeResults) == false)
            {
                results = default;

                return false;
            }

            results = new NativeHashMap<Entity, CollisionEvent.Details>(unsafeResults.Count(), allocator);
            GetCollisionDetails(unsafeResults, results);

            return true;
        }

        private void GetCollisionDetails(UnsafeHashMap<Entity, CollisionEvent.Details> unsafeResults, NativeHashMap<Entity, CollisionEvent.Details> results)
        {
            // TODO: Implement.
        }

        protected override void OnCreate()
        {
            _collisionDetails = new NativeHashMap<Entity, UnsafeHashMap<Entity, CollisionEvent.Details>>(InitialCollisionDetailsCapacity, Allocator.Persistent);
            _buildPhysicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            _endFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
            _stepPhysicsWorldSystem = World.GetExistingSystem<StepPhysicsWorld>();
            _entityQuery = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<PhysicsCollider>(),
                ComponentType.ReadOnly<CollisionEventsListener>(),
                ComponentType.ReadWrite<CollisionEventBufferElement>()
            });
        }

        protected override void OnDestroy()
        {
            if (_collisionDetails.IsCreated == false)
            {
                return;
            }

            foreach (UnsafeHashMap<Entity, CollisionEvent.Details> unsafeHashMap in _collisionDetails.GetValueArray(Allocator.Temp))
            {
                if (unsafeHashMap.IsCreated)
                {
                    unsafeHashMap.Dispose();
                }
            }

            _collisionDetails.Dispose();
        }

        protected override void OnUpdate()
        {
            ArchetypeChunkBufferType<CollisionEventBufferElement> collisionEventBufferType = GetArchetypeChunkBufferType<CollisionEventBufferElement>();

            Dependency = new PhysicsEventsPreprocessorJob<CollisionEventBufferElement>
            {
                PhysicsEventBufferType = collisionEventBufferType
            }.ScheduleParallel(_entityQuery, Dependency);

            Dependency = new CollisionEventJob
            {
                CollisionEventsListenerFromEntity = GetComponentDataFromEntity<CollisionEventsListener>(true),
                PhysicsWorld = _buildPhysicsWorldSystem.PhysicsWorld,
                CollisionEventBufferFromEntity = GetBufferFromEntity<CollisionEventBufferElement>(),
            }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, Dependency);

            _endFramePhysicsSystem.AddInputDependency(Dependency);

            Dependency = new PhysicsEventsPostprocessorJob<CollisionEventBufferElement>
            {
                PhysicsEventBufferType = collisionEventBufferType,
            }.ScheduleParallel(_entityQuery, Dependency);
        }
    }
}
