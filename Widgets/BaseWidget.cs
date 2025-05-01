using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnitDesk.Utilities;

namespace UnitDesk.Widgets
{
    /// <summary>
    /// Base class for all widgets in the application
    /// </summary>
    public abstract class BaseWidget : Window
    {
        // Common properties for all widgets
        protected WidgetStorageService StorageService { get; private set; }
        protected string WidgetId { get; private set; }
        
        // UI state
        private bool _isDragging;
        private System.Windows.Point _dragStartPoint;

        protected BaseWidget(string widgetId)
        {
            WidgetId = widgetId;
            StorageService = new WidgetStorageService();
            
            // Common widget setup
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            // Topmost = true; // REMOVED
            
            // Set event handlers
            MouseLeftButtonDown += Widget_MouseLeftButtonDown;
            MouseLeftButtonUp += Widget_MouseLeftButtonUp;
            MouseMove += Widget_MouseMove;
            Loaded += Widget_Loaded;
            
            // Load position
            LoadPosition();
        }
        
        #region Abstract Methods
        
        /// <summary>
        /// Initialize the widget-specific functionality
        /// </summary>
        protected abstract void InitializeWidget();
        
        /// <summary>
        /// Start timers and data updates for the widget
        /// </summary>
        protected abstract void StartDataUpdates();
        
        #endregion
        
        #region Common Widget Methods
        
        /// <summary>
        /// Save the current widget position to storage
        /// </summary>
        protected virtual void SavePosition()
        {
            StorageService.SavePosition(WidgetId, Left, Top);
        }
        
        /// <summary>
        /// Load the widget position from storage
        /// </summary>
        protected virtual void LoadPosition()
        {
            var position = StorageService.LoadPosition(WidgetId);
            Left = position.Left;
            Top = position.Top;
        }
        
        /// <summary>
        /// Reset the widget position to default
        /// </summary>
        protected virtual void ResetPosition()
        {
            Left = 50;
            Top = 50;
            SavePosition();
        }
        
        /// <summary>
        /// Set the widget to appear on the desktop
        /// </summary>
        protected virtual void SetToDesktop()
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            DesktopIntegration.SetWindowToDesktop(hwnd);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void Widget_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this);
            CaptureMouse();
        }
        
        private void Widget_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                SavePosition();
            }
            _isDragging = false;
            ReleaseMouseCapture();
        }
        
        private void Widget_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDragging)
            {
                System.Windows.Point currentPoint = e.GetPosition(null);
                Left += currentPoint.X - _dragStartPoint.X;
                Top += currentPoint.Y - _dragStartPoint.Y;
            }
        }
        
        private void Widget_Loaded(object sender, RoutedEventArgs e)
        {
            // The widget is integrated into the desktop
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            UnitDesk.Utilities.DesktopIntegration.SetWindowToDesktop(hwnd);
            InitializeWidget();
            StartDataUpdates();
        }
        
        #endregion
    }
}
