using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public sealed class MovementSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<MovementDirection> MovementDirectionType;
            [ReadOnly] public ArchetypeChunkComponentType<MovementSpeed> MovementSpeedType;

            public ArchetypeChunkComponentType<PhysicsVelocity> PhysicsVelocityType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<MovementDirection> movementDirectionArray = chunk.GetNativeArray(MovementDirectionType);
                NativeArray<MovementSpeed> movementSpeedArray = chunk.GetNativeArray(MovementSpeedType);
                NativeArray<PhysicsVelocity> physicsVelocityArray = chunk.GetNativeArray(PhysicsVelocityType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    PhysicsVelocity physicsVelocity = physicsVelocityArray[i];
                    physicsVelocity.Linear = movementDirectionArray[i].Value * movementSpeedArray[i].Value;
                    physicsVelocityArray[i] = physicsVelocity;
                }
            }
        }

        private static readonly ComponentType[] QueryComponents =
        {
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadOnly<MovementDirection>(),
            ComponentType.ReadOnly<MovementSpeed>()
        };
        private static readonly ComponentType[] ChangeFilterComponents =
        {
            ComponentType.ReadOnly<MovementDirection>(),
            ComponentType.ReadOnly<MovementSpeed>()
        };

        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(QueryComponents);
            _query.SetChangedVersionFilter(ChangeFilterComponents);

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                MovementDirectionType = GetArchetypeChunkComponentType<MovementDirection>(true),
                MovementSpeedType = GetArchetypeChunkComponentType<MovementSpeed>(true),
                PhysicsVelocityType = GetArchetypeChunkComponentType<PhysicsVelocity>(),
            }.ScheduleSingle(_query, Dependency);
        }
    }
}
