using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Coimbra.Dots.Physics
{
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    public sealed class TriggerEventsSystem : SystemBase
    {
        [BurstCompile]
        private struct TriggerEventJob : ITriggerEventsJob
        {
            public BufferFromEntity<TriggerEventBufferElement> TriggerEventBufferFromEntity;

            public void Execute(TriggerEvent triggerEvent)
            {
                ProcessForEntity(triggerEvent.EntityA, triggerEvent.EntityB);
                ProcessForEntity(triggerEvent.EntityB, triggerEvent.EntityA);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void ProcessForEntity(Entity entity, Entity otherEntity)
            {
                if (TriggerEventBufferFromEntity.Exists(entity) == false)
                {
                    return;
                }

                DynamicBuffer<TriggerEventBufferElement> triggerEventBuffer = TriggerEventBufferFromEntity[entity];

                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    TriggerEventBufferElement triggerEventBufferElement = triggerEventBuffer[i];

                    if (triggerEventBufferElement.Entity != otherEntity)
                    {
                        continue;
                    }

                    triggerEventBufferElement.State = triggerEventBufferElement.State == PhysicsEventState.Exit ? PhysicsEventState.Enter : PhysicsEventState.Stay;
                    triggerEventBufferElement.IsColliding = true;
                    triggerEventBuffer[i] = triggerEventBufferElement;

                    return;
                }

                triggerEventBuffer.Add(new TriggerEventBufferElement
                {
                    IsColliding = true,
                    Entity = otherEntity,
                    State = PhysicsEventState.Enter
                });
            }
        }

        private BuildPhysicsWorld _buildPhysicsWorldSystem;
        private EndFramePhysicsSystem _endFramePhysicsSystem;
        private StepPhysicsWorld _stepPhysicsWorldSystem;
        private EntityQuery _entityQuery;

        protected override void OnCreate()
        {
            _buildPhysicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            _endFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
            _stepPhysicsWorldSystem = World.GetExistingSystem<StepPhysicsWorld>();
            _entityQuery = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<PhysicsCollider>(),
                ComponentType.ReadWrite<TriggerEventBufferElement>()
            });
        }

        protected override void OnUpdate()
        {
            ArchetypeChunkBufferType<TriggerEventBufferElement> triggerEventBufferType = GetArchetypeChunkBufferType<TriggerEventBufferElement>();

            Dependency = new PhysicsEventsPreprocessorJob<TriggerEventBufferElement>
            {
                PhysicsEventBufferType = triggerEventBufferType,
            }.ScheduleParallel(_entityQuery, Dependency);

            Dependency = new TriggerEventJob
            {
                TriggerEventBufferFromEntity = GetBufferFromEntity<TriggerEventBufferElement>(),
            }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, Dependency);

            _endFramePhysicsSystem.AddInputDependency(Dependency);

            Dependency = new PhysicsEventsPostprocessorJob<TriggerEventBufferElement>
            {
                PhysicsEventBufferType = triggerEventBufferType,
            }.ScheduleParallel(_entityQuery, Dependency);
        }
    }
}
