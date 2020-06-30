using Unity.Entities;

namespace SpaceshipWarrior
{
    [GenerateAuthoringComponent]
    public struct IsFireEnabled : IComponentData
    {
        public bool Value;
    }
}
