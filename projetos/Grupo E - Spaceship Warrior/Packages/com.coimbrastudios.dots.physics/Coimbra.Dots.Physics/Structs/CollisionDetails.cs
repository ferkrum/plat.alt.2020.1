using System;
using Unity.Mathematics;

namespace Coimbra.Dots.Physics
{
    [Serializable]
    public struct CollisionDetails
    {
        public float EstimatedImpulse;
        public float3 AverageContactPointPosition;
    }
}
