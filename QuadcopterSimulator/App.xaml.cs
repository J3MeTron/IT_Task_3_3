using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace QuadcopterSimulator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        HandleException(e.Exception);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            HandleException(exception);
        }
    }

    private void HandleException(Exception ex)
    {
        MessageBox.Show($"Произошла ошибка: {ex.Message}\n\n{ex.StackTrace}", 
                      "Ошибка", 
                      MessageBoxButton.OK, 
                      MessageBoxImage.Error);
        
        System.Diagnostics.Debug.WriteLine($"Необработанное исключение: {ex}");
    }
}

