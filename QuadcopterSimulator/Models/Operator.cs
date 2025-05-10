using System;
using System.Threading.Tasks;

namespace QuadcopterSimulator.Models
{
    public class Operator
    {
        private readonly Quadcopter _quadcopter;
        private bool _isControllerEnabled = false;

        public event EventHandler<OperatorEventArgs>? ControllerStatusChanged;
        public event EventHandler<OperatorEventArgs>? StatusChanged;

        public bool IsControllerEnabled
        {
            get => _isControllerEnabled;
            private set
            {
                if (_isControllerEnabled != value)
                {
                    _isControllerEnabled = value;
                    ControllerStatusChanged?.Invoke(this, new OperatorEventArgs { Message = $"Пульт управления {(value ? "включен" : "выключен")}" });
                }
            }
        }

        public Operator(Quadcopter quadcopter)
        {
            _quadcopter = quadcopter;
            _quadcopter.EmergencyLanding += OnEmergencyLanding;
        }

        public void EnableController()
        {
            IsControllerEnabled = true;
            StatusChanged?.Invoke(this, new OperatorEventArgs { Message = "Оператор готов к управлению" });
        }

        public void DisableController()
        {
            IsControllerEnabled = false;
            StatusChanged?.Invoke(this, new OperatorEventArgs { Message = "Оператор отключил пульт управления" });
        }

        public async Task StartFlight()
        {
            if (!IsControllerEnabled)
            {
                StatusChanged?.Invoke(this, new OperatorEventArgs { Message = "Невозможно начать полет: пульт управления выключен" });
                return;
            }

            StatusChanged?.Invoke(this, new OperatorEventArgs { Message = "Оператор начал полет" });
            await _quadcopter.StartFlying();
        }

        private void OnEmergencyLanding(object? sender, QuadcopterEventArgs e)
        {
            StatusChanged?.Invoke(this, new OperatorEventArgs { Message = "Оператор получил уведомление об аварийной ситуации" });
        }
    }

    public class OperatorEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
    }
} 