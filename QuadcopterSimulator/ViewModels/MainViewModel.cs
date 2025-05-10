using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using QuadcopterSimulator.Models;

namespace QuadcopterSimulator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<QuadcopterModel> _quadcopters = new();
        private string _statusMessage = string.Empty;

        public ObservableCollection<QuadcopterModel> Quadcopters
        {
            get => _quadcopters;
            set
            {
                _quadcopters = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddQuadcopterCommand { get; } = null!;
        public ICommand StartFlightCommand { get; } = null!;
        public ICommand RepairCommand { get; } = null!;

        public MainViewModel()
        {
            try
            {
                Quadcopters = new ObservableCollection<QuadcopterModel>();
                AddQuadcopterCommand = new RelayCommand(AddQuadcopter);
                StartFlightCommand = new RelayCommand<QuadcopterModel>(StartFlight);
                RepairCommand = new RelayCommand<QuadcopterModel>(Repair);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка инициализации: {ex.Message}");
            }
        }

        private void AddQuadcopter()
        {
            try
            {
                var quadcopter = new Quadcopter();
                var operator_ = new Operator(quadcopter);
                var mechanic = new Mechanic(quadcopter);

                var model = new QuadcopterModel(quadcopter, operator_, mechanic);

                quadcopter.StatusChanged += (s, e) => 
                {
                    UpdateStatus(e.Message);
                    model.UpdateStatus();
                };
                operator_.StatusChanged += (s, e) => 
                {
                    UpdateStatus(e.Message);
                    model.UpdateStatus();
                };
                mechanic.StatusChanged += (s, e) => 
                {
                    UpdateStatus(e.Message);
                    model.UpdateStatus();
                };

                Quadcopters.Add(model);
                UpdateStatus("Добавлен новый квадрокоптер");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка при добавлении квадрокоптера: {ex.Message}");
            }
        }

        private async void StartFlight(QuadcopterModel model)
        {
            if (model == null) return;

            try
            {
                model.Operator.EnableController();
                await model.Operator.StartFlight();
                model.UpdateStatus();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка при запуске полета: {ex.Message}");
            }
        }

        private async void Repair(QuadcopterModel model)
        {
            if (model == null) return;

            try
            {
                await model.Mechanic.RepairQuadcopter();
                model.UpdateStatus();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка при ремонте: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            try
            {
                StatusMessage = $"{DateTime.Now:HH:mm:ss} - {message}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления статуса: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка уведомления об изменении свойства: {ex.Message}");
            }
        }
    }

    public class QuadcopterModel : INotifyPropertyChanged
    {
        private readonly Quadcopter _quadcopter;
        private readonly Operator _operator;
        private readonly Mechanic _mechanic;

        public Quadcopter Quadcopter => _quadcopter;
        public Operator Operator => _operator;
        public Mechanic Mechanic => _mechanic;

        public bool IsGpsEnabled => _quadcopter.IsGpsEnabled;
        public bool IsFlying => _quadcopter.IsFlying;

        public QuadcopterModel(Quadcopter quadcopter, Operator operator_, Mechanic mechanic)
        {
            _quadcopter = quadcopter;
            _operator = operator_;
            _mechanic = mechanic;
        }

        public void UpdateStatus()
        {
            OnPropertyChanged(nameof(IsGpsEnabled));
            OnPropertyChanged(nameof(IsFlying));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter!) ?? true;

        public void Execute(object? parameter) => _execute((T)parameter!);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
} 