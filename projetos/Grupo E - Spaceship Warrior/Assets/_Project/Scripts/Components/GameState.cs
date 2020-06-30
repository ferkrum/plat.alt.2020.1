using Unity.Entities;

namespace SpaceshipWarrior
{
    [GenerateAuthoringComponent]
    public struct GameState : IComponentData
    {
        public uint CurrentWave;
        public uint TotalScore;
    }
}
