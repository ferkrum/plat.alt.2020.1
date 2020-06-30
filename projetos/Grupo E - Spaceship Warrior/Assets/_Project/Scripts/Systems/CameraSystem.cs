using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SpaceshipWarrior
{
    public sealed class CameraSystem : SystemBase
    {
        private Camera m_Camera;

        private Camera Camera
        {
            get
            {
                if (m_Camera == null)
                {
                    Entity entity = GetSingletonEntity<MainCameraTag>();
                    m_Camera = EntityManager.GetComponentObject<Camera>(entity);
                }

                return m_Camera;
            }
        }

        public float3 GetCameraPosition()
        {
            Entity entity = GetSingletonEntity<MainCameraTag>();
            var translation = GetComponent<Translation>(entity);

            return translation.Value;
        }

        public float3 ScreenToWorldPoint(float3 value)
        {
            return Camera.ScreenToWorldPoint(value);
        }

        protected override void OnUpdate()
        {
        }
    }
}
