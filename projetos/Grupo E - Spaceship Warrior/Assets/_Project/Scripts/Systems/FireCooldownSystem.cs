using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SpaceshipWarrior
{
    public sealed class FireCooldownSystem : SystemBase
    {
        [BurstCompile]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public ArchetypeChunkComponentType<FireCooldown> FireCooldownType;

            public ArchetypeChunkComponentType<IsFireEnabled> IsFireEnabledType;
            public ArchetypeChunkComponentType<FireCooldownElapsedTime> FireCooldownElapsedTimeType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<IsFireEnabled> isFireEnabledArray = chunk.GetNativeArray(IsFireEnabledType);
                NativeArray<FireCooldown> fireCooldownArray = chunk.GetNativeArray(FireCooldownType);
                NativeArray<FireCooldownElapsedTime> fireCooldownElapsedTimeArray = chunk.GetNativeArray(FireCooldownElapsedTimeType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    IsFireEnabled isFireEnabled = isFireEnabledArray[i];

                    if (isFireEnabled.Value)
                    {
                        continue;
                    }

                    FireCooldownElapsedTime fireCooldownElapsedTime = fireCooldownElapsedTimeArray[i];
                    FireCooldown fireCooldown = fireCooldownArray[i];

                    if (fireCooldownElapsedTime.Value > fireCooldown.Value)
                    {
                        isFireEnabled.Value = true;
                        fireCooldownElapsedTime.Value = 0f;

                        isFireEnabledArray[i] = isFireEnabled;
                        fireCooldownElapsedTimeArray[i] = fireCooldownElapsedTime;

                        continue;
                    }

                    fireCooldownElapsedTime.Value += DeltaTime;
                    fireCooldownElapsedTimeArray[i] = fireCooldownElapsedTime;
                }
            }
        }

        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<FireCooldown>(),
                ComponentType.ReadWrite<IsFireEnabled>(),
                ComponentType.ReadWrite<FireCooldownElapsedTime>()
            });

            RequireSingletonForUpdate<GameIsRunningTag>();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                DeltaTime = Time.DeltaTime,
                FireCooldownType = GetArchetypeChunkComponentType<FireCooldown>(true),
                IsFireEnabledType = GetArchetypeChunkComponentType<IsFireEnabled>(),
                FireCooldownElapsedTimeType = GetArchetypeChunkComponentType<FireCooldownElapsedTime>(),
            }.ScheduleSingle(_query, Dependency);
        }
    }
}
