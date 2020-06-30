using System.IO.Ports;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [DisableAutoCreation]
    public sealed class ArduinoInputSystem : SystemBase
    {
        public struct Settings : IComponentData
        {
            public int BaudRate;
            public float VerticalAxisMultiplier;
            public NativeString32 PortName;
        }

        private const float NormalizerFactor = 1.0f / 32768.0f;

        private EntityQuery _gameIsRunningQuery;
        private Settings _settings;
        private SerialPort _serialPort;

        protected override void OnCreate()
        {
            _gameIsRunningQuery = GetEntityQuery(ComponentType.ReadOnly<GameIsRunningTag>());

            RequireSingletonForUpdate<Settings>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnStartRunning()
        {
            _settings = GetSingleton<Settings>();
            _serialPort = new SerialPort(_settings.PortName.ToString(), _settings.BaudRate);
            _serialPort.Open();
        }

        protected override void OnUpdate()
        {
            Entity playerEntity = GetSingletonEntity<PlayerTag>();

            bool fire = GetFire();

            if (fire)
            {
                var gunReference = GetComponent<GunReference>(playerEntity);

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

            var rotation = GetComponent<Rotation>(playerEntity);
            var movementDirection = GetComponent<MovementDirection>(playerEntity);

            float angleDeltaInDegrees = GetVerticalAxisAngleDelta();
            float angleDeltaInRadians = math.radians(angleDeltaInDegrees);

            quaternion rotationDelta = quaternion.AxisAngle(MathUtility.Up, angleDeltaInRadians);
            float3 directionDelta = math.mul(math.normalize(rotationDelta), MathUtility.Forward);
            movementDirection.Value = math.mul(math.normalize(rotation.Value), directionDelta);

            SetComponent(playerEntity, movementDirection);
        }

        protected override void OnStopRunning()
        {
            _serialPort.Close();
        }

        private bool GetFire()
        {
            if (_serialPort.IsOpen == false)
            {
                return false;
            }

            string data = _serialPort.ReadLine();

            if (string.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            if (int.TryParse(data, out int fire))
            {
                return fire == 1;
            }

            return false;
        }

        private float GetVerticalAxisAngleDelta()
        {
            if (_serialPort.IsOpen == false)
            {
                return 0f;
            }

            string data = _serialPort.ReadLine();

            if (string.IsNullOrWhiteSpace(data))
            {
                return 0f;
            }

            const float tolerance = 0.025f;
            const char splitter = ';';

            string[] rawData = data.Split(splitter);
            float gyroscopeVerticalAngle = int.Parse(rawData[4]) * NormalizerFactor;

            if (math.abs(gyroscopeVerticalAngle) < tolerance)
            {
                gyroscopeVerticalAngle = 0f;
            }

            return -gyroscopeVerticalAngle * _settings.VerticalAxisMultiplier;
        }
    }
}
