using Coimbra.Dots.Physics;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public class ProjectileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private int _damage;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new CollisionEventsListener());
            destinationManager.AddComponentData(entity, new MovementDirection { Value = MathUtility.Forward });
            destinationManager.AddComponentData(entity, new MovementSpeed { Value = _movementSpeed });
            destinationManager.AddComponentData(entity, new Damage
            {
                AutoDestroy = true,
                Value = _damage
            });
        }
    }
}
