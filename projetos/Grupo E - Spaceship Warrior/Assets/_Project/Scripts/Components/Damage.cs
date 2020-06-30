using Unity.Entities;

[GenerateAuthoringComponent]
public struct Damage : IComponentData
{
    public bool AutoDestroy;
    public int Value;
}
