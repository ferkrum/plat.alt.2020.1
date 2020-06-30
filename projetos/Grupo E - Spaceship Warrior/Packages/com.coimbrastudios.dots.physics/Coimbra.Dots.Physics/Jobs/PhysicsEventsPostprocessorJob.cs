using Unity.Burst;
using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    [BurstCompile]
    public struct PhysicsEventsPostprocessorJob<T> : IJobChunk
        where T : struct, IPhysicsEventBufferElement
    {
        public ArchetypeChunkBufferType<T> PhysicsEventBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            BufferAccessor<T> physicsEventBufferAccessor = chunk.GetBufferAccessor(PhysicsEventBufferType);

            for (int i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<T> physicsEventBuffer = physicsEventBufferAccessor[i];

                for (int j = physicsEventBuffer.Length - 1; j >= 0; j--)
                {
                    T physicsEventBufferElement = physicsEventBuffer[j];

                    if (physicsEventBufferElement.IsColliding)
                    {
                        continue;
                    }

                    if (physicsEventBufferElement.State == PhysicsEventState.Exit)
                    {
                        physicsEventBuffer.RemoveAt(j);
                    }
                    else
                    {
                        physicsEventBufferElement.State = PhysicsEventState.Exit;
                        physicsEventBuffer[j] = physicsEventBufferElement;
                    }
                }
            }
        }
    }
}
