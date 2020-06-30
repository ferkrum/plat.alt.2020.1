using Unity.Entities;

namespace SpaceshipWarrior
{
    [GenerateAuthoringComponent]
    public struct FireCooldown : IComponentData
    {
        public float Value;
    }
}
