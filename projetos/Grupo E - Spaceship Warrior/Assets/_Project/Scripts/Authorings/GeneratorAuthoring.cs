using Coimbra.Dots.Physics;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public sealed class GeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private int _maxHealth;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new GeneratorTag());
            destinationManager.AddComponentData(entity, new CollisionEventsListener());
            destinationManager.AddComponentData(entity, new CurrentHealth());
            destinationManager.AddComponentData(entity, new MaxHealth { Value = _maxHealth });
        }
    }
}
