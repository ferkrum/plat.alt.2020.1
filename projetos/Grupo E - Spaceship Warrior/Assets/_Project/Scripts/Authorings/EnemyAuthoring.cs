using Coimbra.Dots.Physics;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    [RequireComponent(typeof(PhysicsShapeAuthoring))]
    [RequireComponent(typeof(PhysicsBodyAuthoring))]
    public sealed class EnemyAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _damage;
        [SerializeField] private uint _score;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new EnemyTag());
            destinationManager.AddComponentData(entity, new CollisionEventsListener());
            destinationManager.AddComponentData(entity, new CurrentHealth());
            destinationManager.AddComponentData(entity, new MaxHealth { Value = _maxHealth });
            destinationManager.AddComponentData(entity, new MovementDirection { Value = MathUtility.Forward });
            destinationManager.AddComponentData(entity, new MovementSpeed { Value = _movementSpeed });
            destinationManager.AddComponentData(entity, new Damage
            {
                AutoDestroy = true,
                Value = _damage
            });
            destinationManager.AddComponentData(entity, new Score { Value = _score });
        }
    }
}
