using System;
using System.Windows;
using System.Windows.Threading;
using UnitDesk.Utilities;

namespace UnitDesk.Widgets
{
    /// <summary>
    /// Widget that displays current time and date
    /// </summary>
    public partial class ClockWidget : BaseWidget
    {
        private DispatcherTimer? _clockTimer;

        public ClockWidget(string widgetId) : base(widgetId)
        {
            InitializeComponent();
            TitleText.Text = widgetId; // Can be updated from configuration later
        }

        protected override void InitializeWidget()
        {
            // Additional initialization if needed
        }

        protected override void StartDataUpdates()
        {
            // Initial update
            UpdateClock();
            
            // Set up timer for updates
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
        }

        private void UpdateClock()
        {
            DateTime now = DateTime.Now;
            TimeText.Text = now.ToString("HH:mm:ss");
            DateText.Text = now.ToString("dddd, MMMM d, yyyy");
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
            _clockTimer?.Stop();
        }
    }
}
