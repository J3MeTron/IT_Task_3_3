using System;
using System.Threading.Tasks;
using System.Threading;

namespace QuadcopterSimulator.Models
{
    public class Quadcopter
    {
        private bool _isGpsEnabled = true;
        private bool _isFlying = false;
        private Random _random = new Random();

        public event EventHandler<QuadcopterEventArgs>? GpsStatusChanged;
        public event EventHandler<QuadcopterEventArgs>? EmergencyLanding;
        public event EventHandler<QuadcopterEventArgs>? StatusChanged;

        public bool IsGpsEnabled
        {
            get => _isGpsEnabled;
            private set
            {
                if (_isGpsEnabled != value)
                {
                    _isGpsEnabled = value;
                    GpsStatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = $"GPS {(value ? "включен" : "выключен")}" });
                }
            }
        }

        public bool IsFlying
        {
            get => _isFlying;
            private set
            {
                if (_isFlying != value)
                {
                    _isFlying = value;
                    StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = $"Квадрокоптер {(value ? "взлетел" : "приземлился")}" });
                }
            }
        }

        public async Task StartFlying()
        {
            if (!IsFlying)
            {
                StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Подготовка к взлету..." });
                await Task.Delay(1000); // Имитация подготовки
                IsFlying = true;
                StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Квадрокоптер успешно взлетел" });
                await SimulateFlight();
            }
            else
            {
                StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Квадрокоптер уже в полете" });
            }
        }

        private async Task SimulateFlight()
        {
            while (IsFlying)
            {
                // 10% шанс отключения GPS
                if (_random.Next(100) < 10)
                {
                    StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Обнаружена проблема с GPS!" });
                    IsGpsEnabled = false;
                    EmergencyLanding?.Invoke(this, new QuadcopterEventArgs { Message = "Аварийная ситуация! GPS отключен! Начинаю аварийную посадку!" });
                    await EmergencyLandingSequence();
                }
                await Task.Delay(1000);
            }
        }

        private async Task EmergencyLandingSequence()
        {
            StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Начинаю аварийную посадку..." });
            await Task.Delay(2000); // Имитация времени посадки
            IsFlying = false;
            StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Аварийная посадка завершена. Требуется ремонт GPS" });
        }

        public void Repair()
        {
            StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Начало ремонта квадрокоптера..." });
            IsGpsEnabled = true;
            StatusChanged?.Invoke(this, new QuadcopterEventArgs { Message = "Квадрокоптер успешно отремонтирован" });
        }
    }

    public class QuadcopterEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
    }
} 