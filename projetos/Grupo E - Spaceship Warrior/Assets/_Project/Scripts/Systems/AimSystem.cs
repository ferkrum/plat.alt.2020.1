using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(MovementSystem))]
    public sealed class AimSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<MovementDirection> MovementDirectionType;

            public ArchetypeChunkComponentType<Rotation> RotationType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<MovementDirection> movementDirectionArray = chunk.GetNativeArray(MovementDirectionType);
                NativeArray<Rotation> rotationArray = chunk.GetNativeArray(RotationType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    Rotation rotation = rotationArray[i];
                    rotation.Value = quaternion.LookRotation(movementDirectionArray[i].Value, MathUtility.Up);
                    rotationArray[i] = rotation;
                }
            }
        }

        private static readonly ComponentType[] QueryComponents = { ComponentType.ReadWrite<Rotation>(), ComponentType.ReadOnly<MovementDirection>() };
        private static readonly ComponentType[] ChangeFilterComponents = { ComponentType.ReadOnly<MovementDirection>() };

        private EntityQuery _entityQuery;

        protected override void OnCreate()
        {
            _entityQuery = GetEntityQuery(QueryComponents);
            _entityQuery.SetChangedVersionFilter(ChangeFilterComponents);

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                MovementDirectionType = GetArchetypeChunkComponentType<MovementDirection>(true),
                RotationType = GetArchetypeChunkComponentType<Rotation>()
            }.ScheduleSingle(_entityQuery, Dependency);
        }
    }
}
