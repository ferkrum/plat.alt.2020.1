using Unity.Entities;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    internal sealed class TriggerEventsListenerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddBuffer<TriggerEventBufferElement>(entity);
        }
    }
}
