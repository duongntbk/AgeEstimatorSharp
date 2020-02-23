using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (Dispatcher != null)
            {
                Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            }
        }

        static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;

            var message = $"An error has occured : {Environment.NewLine}{ex.Message}";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                message += $"{Environment.NewLine}{ex.Message}";
            }

            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
