using System;
using Unity.Entities;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    [Serializable]
    public struct TriggerEventBufferElement : IPhysicsEventBufferElement, IEquatable<TriggerEventBufferElement>
    {
        [SerializeField] private bool m_IsColliding;
        [SerializeField] private PhysicsEventState m_State;
        [SerializeField] private Entity m_Entity;

        public bool IsColliding
        {
            get => m_IsColliding;
            set => m_IsColliding = value;
        }
        public PhysicsEventState State
        {
            get => m_State;
            set => m_State = value;
        }
        public Entity Entity
        {
            get => m_Entity;
            set => m_Entity = value;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            hash ^= IsColliding.GetHashCode();
            hash ^= ((int)State).GetHashCode();
            hash ^= Entity.GetHashCode();

            return hash;
        }

        public bool Equals(TriggerEventBufferElement other)
        {
            return IsColliding == other.IsColliding
                && State == other.State
                && Entity == other.Entity;
        }
    }
}
