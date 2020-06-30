using Unity.Collections;
using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    [UpdateAfter(typeof(CollisionEventsSystem))]
    public sealed class DebugCollisionEventsSystem : DebugPhysicsEventsSystemBase<CollisionEventBufferElement, DebugCollisionEvents>
    {
        protected override FixedString32 PhysicsEventType => "Collision";
    }
}
