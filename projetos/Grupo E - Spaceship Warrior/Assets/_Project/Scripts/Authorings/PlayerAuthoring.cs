using Coimbra.Dots.Physics;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private int _maxHealth;
        [SerializeField] private GameObject _gunGameObject;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new PlayerTag());
            destinationManager.AddComponentData(entity, new CollisionEventsListener());
            destinationManager.AddComponentData(entity, new CurrentHealth());
            destinationManager.AddComponentData(entity, new MaxHealth { Value = _maxHealth });
            destinationManager.AddComponentData(entity, new MovementDirection { Value = MathUtility.Forward });
            destinationManager.AddComponentData(entity, new MovementSpeed { Value = _movementSpeed });
            destinationManager.AddComponentData(entity, new GunReference { Value = conversionSystem.GetPrimaryEntity(_gunGameObject) });
        }
    }
}
