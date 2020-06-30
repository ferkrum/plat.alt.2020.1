using Unity.Entities;

namespace Coimbra.Dots.Physics
{
    public interface IDebugPhysicsEvents : IComponentData
    {
        bool DebugEnter { get; set; }
        bool DebugExit { get; set; }
        bool DebugStay { get; set; }
    }
}
