using Unity.Entities;

namespace SpaceshipWarrior
{
    [GenerateAuthoringComponent]
    public struct FireCooldownElapsedTime : IComponentData
    {
        public float Value;
    }
}
