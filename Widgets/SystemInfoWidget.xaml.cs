using System;
using System.Windows;
using System.Windows.Threading;
using UnitDesk.Utilities;

namespace UnitDesk.Widgets
{
    /// <summary>
    /// Widget that displays system information like CPU, RAM, and GPU usage
    /// </summary>
    public partial class SystemInfoWidget : BaseWidget
    {
        private readonly HardwareMonitor _hardwareMonitor;
        private DispatcherTimer _systemInfoTimer;
        private DispatcherTimer _gpuInfoTimer;

        public SystemInfoWidget(string widgetId) : base(widgetId)
        {
            InitializeComponent();
            _hardwareMonitor = new HardwareMonitor();
            TitleText.Text = widgetId; // Can be updated from configuration later
        }

        protected override void InitializeWidget()
        {
            // Additional initialization if needed
        }

        protected override void StartDataUpdates()
        {
            // Update system information (CPU & RAM)
            _systemInfoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _systemInfoTimer.Tick += (s, e) => UpdateSystemInfo();
            _systemInfoTimer.Start();

            // Update GPU information
            _gpuInfoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _gpuInfoTimer.Tick += (s, e) => UpdateGpuInfo();
            _gpuInfoTimer.Start();
        }

        private void UpdateSystemInfo()
        {
            var cpuUsage = _hardwareMonitor.GetCpuUsage();
            var ramUsage = _hardwareMonitor.GetRamUsage();
            SystemInfoText.Text = $"CPU: {cpuUsage:F1}%  |  RAM: {ramUsage:F1}%";
        }

        private void UpdateGpuInfo()
        {
            GpuInfoText.Text = _hardwareMonitor.GetGpuInfo();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetPosition();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Let the widget manager handle closing
            WidgetManager.Instance.CloseWidget(WidgetId);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            // Clean up resources
            _systemInfoTimer?.Stop();
            _gpuInfoTimer?.Stop();
            _hardwareMonitor?.Dispose();
        }
    }
}
