using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    public abstract class DebugPhysicsEventsSystemBase<TPhysicsEventBufferElement, TDebugPhysicsEvents> : SystemBase
        where TPhysicsEventBufferElement : struct, IPhysicsEventBufferElement
        where TDebugPhysicsEvents : struct, IDebugPhysicsEvents
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<TDebugPhysicsEvents> DebugPhysicsEventsType;
            [ReadOnly] public ArchetypeChunkBufferType<TPhysicsEventBufferElement> PhysicsEventBufferType;
            [ReadOnly] public FixedString64 MessageFormat;
            [ReadOnly] public NativeArray<FixedString32> PhysicsEventStateNames;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<Entity> entityChunk = chunk.GetNativeArray(EntityType);
                NativeArray<TDebugPhysicsEvents> debugPhysicsEventsChunk = chunk.GetNativeArray(DebugPhysicsEventsType);
                BufferAccessor<TPhysicsEventBufferElement> physicsEventBufferAccessor = chunk.GetBufferAccessor(PhysicsEventBufferType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    DynamicBuffer<TPhysicsEventBufferElement> physicsEventsBuffer = physicsEventBufferAccessor[i];

                    for (int j = 0; j < physicsEventsBuffer.Length; j++)
                    {
                        TPhysicsEventBufferElement physicsEventsBufferElement = physicsEventsBuffer[j];
                        FixedString32 state = PhysicsEventStateNames[(int)physicsEventsBufferElement.State];
                        FixedString64 message = FixedString.Format(MessageFormat, state, entityChunk[i].Index, physicsEventsBufferElement.Entity.Index);

                        switch (physicsEventsBufferElement.State)
                        {
                            case PhysicsEventState.Enter:
                            {
                                if (debugPhysicsEventsChunk[i].DebugEnter)
                                {
                                    Debug.Log(message);
                                }

                                break;
                            }

                            case PhysicsEventState.Exit:
                            {
                                if (debugPhysicsEventsChunk[i].DebugExit)
                                {
                                    Debug.Log(message);
                                }

                                break;
                            }

                            case PhysicsEventState.Stay:
                            {
                                if (debugPhysicsEventsChunk[i].DebugStay)
                                {
                                    Debug.Log(message);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        private EntityQuery _entityQuery;
        private FixedString64 _messageFormat;
        private NativeArray<FixedString32> _physicsEventsStateNames;

        protected abstract FixedString32 PhysicsEventType { get; }

        protected override void OnCreate()
        {
            _entityQuery = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<TDebugPhysicsEvents>(),
                ComponentType.ReadOnly<TPhysicsEventBufferElement>()
            });

            _messageFormat = "{0} between Entity {1} and Entity {2}";
            _physicsEventsStateNames = new NativeArray<FixedString32>(3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory)
            {
                [0] = $"{PhysicsEventType} Enter",
                [1] = $"{PhysicsEventType} Exit",
                [2] = $"{PhysicsEventType} Stay"
            };
        }

        protected override void OnDestroy()
        {
            if (_physicsEventsStateNames.IsCreated)
            {
                _physicsEventsStateNames.Dispose();
            }
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                EntityType = GetArchetypeChunkEntityType(),
                DebugPhysicsEventsType = GetArchetypeChunkComponentType<TDebugPhysicsEvents>(true),
                PhysicsEventBufferType = GetArchetypeChunkBufferType<TPhysicsEventBufferElement>(true),
                MessageFormat = _messageFormat,
                PhysicsEventStateNames = _physicsEventsStateNames
            }.ScheduleParallel(_entityQuery, Dependency);
        }
    }
}
