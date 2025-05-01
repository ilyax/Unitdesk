using System;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using UnitDesk.Utilities;

namespace UnitDesk.Widgets
{
    /// <summary>
    /// Widget that displays disk usage information
    /// </summary>
    public partial class DiskInfoWidget : BaseWidget
    {
        private readonly DiskMonitor _diskMonitor;
        private DispatcherTimer _diskInfoTimer;
        private ObservableCollection<string> _diskItems;

        public DiskInfoWidget(string widgetId) : base(widgetId)
        {
            InitializeComponent();
            _diskMonitor = new DiskMonitor();
            _diskItems = new ObservableCollection<string>();
            DiskList.ItemsSource = _diskItems;
            TitleText.Text = widgetId; // Can be updated from configuration later
        }

        protected override void InitializeWidget()
        {
            // Additional initialization if needed
        }

        protected override void StartDataUpdates()
        {
            // Initial update
            UpdateDiskInfo();
            
            // Set up timer for updates
            _diskInfoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _diskInfoTimer.Tick += (s, e) => UpdateDiskInfo();
            _diskInfoTimer.Start();
        }

        private void UpdateDiskInfo()
        {
            try
            {
                _diskItems.Clear();
                var diskInfoList = _diskMonitor.GetDiskInfo();
                
                foreach (var diskInfo in diskInfoList)
                {
                    _diskItems.Add(diskInfo);
                }
            }
            catch (Exception ex)
            {
                _diskItems.Clear();
                _diskItems.Add($"Error: {ex.Message}");
            }
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
            _diskInfoTimer?.Stop();
        }
    }
}
