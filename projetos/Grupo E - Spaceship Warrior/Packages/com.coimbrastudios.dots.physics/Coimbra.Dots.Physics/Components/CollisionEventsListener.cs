using System;
using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    [Serializable]
    public struct CollisionEventsListener : IComponentData
    {
        public bool CalculateDetails;
    }
}
