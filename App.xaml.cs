using System;
using System.Windows;
using UnitDesk.Widgets;

namespace UnitDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Create and show widgets using the widget manager
                //WidgetManager.Instance.CreateWidget<SystemInfoWidget>("system-info");
                //WidgetManager.Instance.CreateWidget<DiskInfoWidget>("disk-info");
                //WidgetManager.Instance.CreateWidget<ClockWidget>("clock");
                
                // Alternative approach to create widgets from saved configuration
                 WidgetManager.Instance.CreateWidgetsFromConfiguration();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error starting application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Clean up by closing all widgets
            WidgetManager.Instance.CloseAllWidgets();
            base.OnExit(e);
        }
    }
}
