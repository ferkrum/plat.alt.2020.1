using Unity.Entities;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    internal abstract class DebugPhysicsEventsAuthoringBase<T> : MonoBehaviour, IConvertGameObjectToEntity
        where T : struct, IDebugPhysicsEvents
    {
        [SerializeField] private bool _debugEnter = true;
        [SerializeField] private bool _debugExit = true;
        [SerializeField] private bool _debugStay = default;

        public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddComponentData<T>(entity, new T
            {
                DebugEnter = _debugEnter,
                DebugExit = _debugExit,
                DebugStay = _debugStay
            });
        }
    }
}
