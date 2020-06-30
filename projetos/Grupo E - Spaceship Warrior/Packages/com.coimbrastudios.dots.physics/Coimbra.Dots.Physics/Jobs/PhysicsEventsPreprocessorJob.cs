using Unity.Burst;
using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    [BurstCompile]
    public struct PhysicsEventsPreprocessorJob<T> : IJobChunk
        where T : struct, IPhysicsEventBufferElement
    {
        public ArchetypeChunkBufferType<T> PhysicsEventBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            BufferAccessor<T> physicsEventBufferAccessor = chunk.GetBufferAccessor(PhysicsEventBufferType);

            for (int i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<T> physicsEventBuffer = physicsEventBufferAccessor[i];

                for (int j = 0; j < physicsEventBuffer.Length; j++)
                {
                    T physicsEventBufferElement = physicsEventBuffer[j];
                    physicsEventBufferElement.IsColliding = false;
                    physicsEventBuffer[j] = physicsEventBufferElement;
                }
            }
        }
    }
}
