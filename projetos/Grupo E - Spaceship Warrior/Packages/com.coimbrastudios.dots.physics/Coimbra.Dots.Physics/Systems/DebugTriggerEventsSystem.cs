using Unity.Collections;
using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    [UpdateAfter(typeof(TriggerEventsSystem))]
    public sealed class DebugTriggerEventsSystem : DebugPhysicsEventsSystemBase<TriggerEventBufferElement, DebugTriggerEvents>
    {
        protected override FixedString32 PhysicsEventType => "Trigger";
    }
}
