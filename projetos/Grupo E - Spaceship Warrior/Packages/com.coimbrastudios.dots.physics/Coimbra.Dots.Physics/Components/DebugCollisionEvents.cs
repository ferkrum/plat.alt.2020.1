using System;
using UnityEngine;

namespace Coimbra.Dots.Physics
{
    [Serializable]
    public struct DebugCollisionEvents : IDebugPhysicsEvents
    {
        [SerializeField] private bool m_DebugEnter;
        [SerializeField] private bool m_DebugExit;
        [SerializeField] private bool m_DebugStay;

        public bool DebugEnter
        {
            get => m_DebugEnter;
            set => m_DebugEnter = value;
        }
        public bool DebugExit
        {
            get => m_DebugExit;
            set => m_DebugExit = value;
        }
        public bool DebugStay
        {
            get => m_DebugStay;
            set => m_DebugStay = value;
        }
    }
}
