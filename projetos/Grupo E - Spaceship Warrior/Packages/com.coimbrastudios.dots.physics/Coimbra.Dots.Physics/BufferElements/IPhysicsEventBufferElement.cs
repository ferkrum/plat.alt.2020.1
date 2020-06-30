using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    public interface IPhysicsEventBufferElement : IBufferElementData
    {
        bool IsColliding { get; set; }
        PhysicsEventState State { get; set; }
        Entity Entity { get; set; }
    }
}
