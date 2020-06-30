using Coimbra.Dots.Physics;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public sealed class WallAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float _bounceBackImpulse = 20f;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new WallTag());
            destinationManager.AddComponentData(entity, new CollisionEventsListener());
            destinationManager.AddComponentData(entity, new BounceBackImpulse { Value = _bounceBackImpulse });
        }
    }
}
