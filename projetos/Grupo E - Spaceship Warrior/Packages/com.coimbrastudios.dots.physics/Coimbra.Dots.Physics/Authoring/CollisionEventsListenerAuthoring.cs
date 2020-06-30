using Unity.Entities;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    internal sealed class CollisionEventsListenerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private bool _calculateDetails = default;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
            destinationManager.AddComponentData(entity, new CollisionEventsListener
            {
                CalculateDetails = _calculateDetails
            });
        }
    }
}
