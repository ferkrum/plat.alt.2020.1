using System.IO.Ports;
using Unity.Entities;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed class InputSystemGroup : ComponentSystemGroup
    {
        private SystemBase _inputSystem;

        protected override void OnStartRunning()
        {
            var arduinoInputSystemSettings = GetSingleton<ArduinoInputSystem.Settings>();
            var arduinoPortName = arduinoInputSystemSettings.PortName.ToString();
            string[] availablePorts = SerialPort.GetPortNames();

            foreach (string availablePort in availablePorts)
            {
                if (!string.Equals(availablePort, arduinoPortName))
                {
                    continue;
                }

                _inputSystem = World.GetOrCreateSystem<ArduinoInputSystem>();
            }

            if (_inputSystem == null)
            {
                _inputSystem = World.GetOrCreateSystem<DesktopInputSystem>();
            }
        }

        protected override void OnUpdate()
        {
            _inputSystem.Update();
        }
    }
}
