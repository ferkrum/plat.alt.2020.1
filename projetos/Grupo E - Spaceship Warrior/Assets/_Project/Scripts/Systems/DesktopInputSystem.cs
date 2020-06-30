using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceshipWarrior
{
    [DisableAutoCreation]
    public sealed class DesktopInputSystem : SystemBase, InputCallbacks.IGameplayActions
    {
        private bool _fire;
        private float2 _aimScreenPosition;
        private EntityQuery _gameIsRunningQuery;
        private CameraSystem _cameraSystem;
        private InputCallbacks _callbacks;

        protected override void OnCreate()
        {
            _gameIsRunningQuery = GetEntityQuery(ComponentType.ReadOnly<GameIsRunningTag>());
            _cameraSystem = World.GetExistingSystem<CameraSystem>();
            _callbacks = new InputCallbacks();
            _callbacks.Gameplay.SetCallbacks(this);

            RequireSingletonForUpdate<MainCameraTag>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnStartRunning()
        {
            _callbacks.Enable();
        }

        protected override void OnUpdate()
        {
            Entity entity = GetSingletonEntity<PlayerTag>();
            float3 cameraPosition = _cameraSystem.GetCameraPosition();

            var translation = GetComponent<Translation>(entity);
            var movementDirection = GetComponent<MovementDirection>(entity);
            var fixedAimScreenPosition = new float3(_aimScreenPosition, cameraPosition.y - translation.Value.y);

            float3 aimWorldPosition = _cameraSystem.ScreenToWorldPoint(fixedAimScreenPosition);
            movementDirection.Value = math.normalize(aimWorldPosition - translation.Value);

            SetComponent(entity, movementDirection);

            if (_fire)
            {
                var gunReference = GetComponent<GunReference>(entity);

                if (_gameIsRunningQuery.CalculateEntityCount() == 0)
                {
                    EntityManager.AddComponentData(gunReference.Value, new FireEventTag());
                }
                else
                {
                    var isFireEnabled = GetComponent<IsFireEnabled>(gunReference.Value);

                    if (isFireEnabled.Value)
                    {
                        isFireEnabled.Value = false;

                        EntityManager.SetComponentData(gunReference.Value, isFireEnabled);
                        EntityManager.AddComponentData(gunReference.Value, new FireEventTag());
                        EntityManager.AddComponentData(gunReference.Value, new FireSoundEventTag());
                    }
                }
            }

            _fire = false;
        }

        protected override void OnStopRunning()
        {
            _callbacks.Disable();
        }

        void InputCallbacks.IGameplayActions.OnAim(InputAction.CallbackContext context)
        {
            _aimScreenPosition = context.ReadValue<Vector2>();
        }

        void InputCallbacks.IGameplayActions.OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }

            _fire = true;
        }
    }
}
