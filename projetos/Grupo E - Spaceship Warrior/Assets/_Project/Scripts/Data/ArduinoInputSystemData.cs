using Coimbra;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [CreateAssetMenu(menuName = nameof(SpaceshipWarrior) + "/" + nameof(ArduinoInputSystemData))]
    public sealed class ArduinoInputSystemData : ScriptableSingleton<ArduinoInputSystemData>
    {
        [SerializeField] private int _baudRate = 9600;
        [SerializeField] private float _verticalAxisMultiplier = 70f;
        [SerializeField] private string _portName = "COM4";

        protected override void OnInitialize()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            Entity entity = world.EntityManager.CreateEntity();

            world.EntityManager.AddComponentData(entity, new ArduinoInputSystem.Settings
            {
                BaudRate = _baudRate,
                VerticalAxisMultiplier = _verticalAxisMultiplier,
                PortName = _portName
            });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            GetInstance(true);
        }
    }
}
