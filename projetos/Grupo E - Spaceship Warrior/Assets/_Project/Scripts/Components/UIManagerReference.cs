using Unity.Entities;

namespace SpaceshipWarrior
{
    [GenerateAuthoringComponent]
    public sealed class UIManagerReference : IComponentData
    {
        public UIManager Value;
    }
}