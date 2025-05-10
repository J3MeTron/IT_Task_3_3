using System;
using System.Threading.Tasks;

namespace QuadcopterSimulator.Models
{
    public class Mechanic
    {
        private readonly Quadcopter _quadcopter;

        public event EventHandler<MechanicEventArgs>? StatusChanged;

        public Mechanic(Quadcopter quadcopter)
        {
            _quadcopter = quadcopter;
            _quadcopter.EmergencyLanding += OnEmergencyLanding;
        }

        private void OnEmergencyLanding(object? sender, QuadcopterEventArgs e)
        {
            StatusChanged?.Invoke(this, new MechanicEventArgs { Message = "Механик получил уведомление о необходимости ремонта" });
        }

        public async Task RepairQuadcopter()
        {
            StatusChanged?.Invoke(this, new MechanicEventArgs { Message = "Механик начал ремонт квадрокоптера" });
            await Task.Delay(3000); // Имитация времени ремонта
            _quadcopter.Repair();
            StatusChanged?.Invoke(this, new MechanicEventArgs { Message = "Механик завершил ремонт квадрокоптера" });
        }
    }

    public class MechanicEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
    }
} 