using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Windows.Input;
using System.Text.Json;
using LibreHardwareMonitor.Hardware;
using UnitDesk.Utilities;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;
using UnitDesk.Widgets;

namespace UnitDesk
{
    public partial class MainWindow : Window
    {
        // Service components
        private readonly HardwareMonitor _hardwareMonitor;
        private readonly WidgetStorageService _storageService;
        private readonly DiskMonitor _diskMonitor;
        private const string WIDGET_ID = "main-window";
        
        // UI state
        private bool _isDragging;
        private System.Windows.Point _dragStartPoint;

        // Tray icon
        private Forms.NotifyIcon? _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize services
            _hardwareMonitor = new HardwareMonitor();
            _storageService = new WidgetStorageService();
            _diskMonitor = new DiskMonitor();
            
            // Setup timers and initial state
            StartClock();
            InitializeSystemMonitoring();
            
            // Load saved position
            LoadPosition();

            // Ensure MainWindow does NOT stay on top
            this.Topmost = false;

            // Start tray icon
            InitializeTrayIcon();
        }

        #region System Monitoring

        private void InitializeSystemMonitoring()
        {
            // Initialize system information timers
            var systemInfoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            systemInfoTimer.Tick += (s, e) => UpdateSystemInfo();
            systemInfoTimer.Start();

            // Initialize disk information timer
            var diskInfoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            diskInfoTimer.Tick += (s, e) => UpdateDiskInfo();
            diskInfoTimer.Start();

            // Initialize GPU information timer
            var gpuTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            gpuTimer.Tick += (s, e) => UpdateGpuInfo();
            gpuTimer.Start();
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

        private void UpdateDiskInfo()
        {
            DiskList.Items.Clear();
            var diskInfoList = _diskMonitor.GetDiskInfo();
            
            foreach (var diskInfo in diskInfoList)
            {
                DiskList.Items.Add(diskInfo);
            }
        }

        #endregion

        #region Clock Management

        private void StartClock()
        {
            //var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            //timer.Tick += (s, e) => ClockText.Text = DateTime.Now.ToLongTimeString();
            //timer.Start();
        }

        #endregion

        #region Drag & Drop

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this);
            CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                SavePosition();
            }
            _isDragging = false;
            ReleaseMouseCapture();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDragging)
            {
                System.Windows.Point currentPoint = e.GetPosition(null);
                Left += currentPoint.X - _dragStartPoint.X;
                Top += currentPoint.Y - _dragStartPoint.Y;
            }
        }

        #endregion

        #region Position Management

        private void SavePosition()
        {
            _storageService.SavePosition(WIDGET_ID, this.Left, this.Top);
        }

        private void LoadPosition()
        {
            var position = _storageService.LoadPosition(WIDGET_ID);
            this.Left = position.Left;
            this.Top = position.Top;
        }

        private void ResetPosition()
        {
            this.Left = 50;
            this.Top = 50;
            SavePosition();
        }

        #endregion

        #region Event Handlers

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetPosition();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Removed SetToDesktop() call to make the window behave normally
        }

        #endregion

        private void SetToDesktop()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            DesktopIntegration.SetWindowToDesktop(hwnd);
        }

        // Initialize tray icon
        private void InitializeTrayIcon()
        {
            _trayIcon = new Forms.NotifyIcon();
            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unitdesk.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    _trayIcon.Icon = new System.Drawing.Icon(iconPath);
                }
                else
                {
                    _trayIcon.Icon = Drawing.SystemIcons.Application;
                }
            }
            catch
            {
                _trayIcon.Icon = Drawing.SystemIcons.Application;
            }
            _trayIcon.Visible = true;
            _trayIcon.Text = "UnitDesk";

            UpdateTrayMenu();
        }

        // Update tray menu and add widget controls
        private void UpdateTrayMenu()
        {
            var contextMenu = new Forms.ContextMenuStrip();

            // Widget open/close menus
            contextMenu.Items.Add(new Forms.ToolStripLabel("Widgets"));
            contextMenu.Items.Add(new Forms.ToolStripSeparator());

            // Widget types and their IDs
            var widgetTypes = new[]
            {
                new { Name = "System Information", Type = typeof(SystemInfoWidget), Id = "system-info" },
                new { Name = "Disk Information", Type = typeof(DiskInfoWidget), Id = "disk-info" },
                new { Name = "Clock", Type = typeof(ClockWidget), Id = "clock" },
            };

            foreach (var widget in widgetTypes)
            {
                bool isOpen = WidgetManager.Instance.GetWidget<BaseWidget>(widget.Id) != null;
                if (isOpen)
                {
                    var closeItem = new Forms.ToolStripMenuItem($"{widget.Name} - Close");
                    closeItem.Click += (s, e) => {
                        WidgetManager.Instance.CloseWidget(widget.Id);
                        UpdateTrayMenu();
                    };
                    contextMenu.Items.Add(closeItem);
                }
                else
                {
                    var openItem = new Forms.ToolStripMenuItem($"{widget.Name} - Open");
                    openItem.Click += (s, e) => {
                        // Open the widget based on its type
                        if (widget.Type == typeof(SystemInfoWidget))
                            WidgetManager.Instance.CreateWidget<SystemInfoWidget>(widget.Id);
                        else if (widget.Type == typeof(DiskInfoWidget))
                            WidgetManager.Instance.CreateWidget<DiskInfoWidget>(widget.Id);
                        else if (widget.Type == typeof(ClockWidget))
                            WidgetManager.Instance.CreateWidget<ClockWidget>(widget.Id);
                        UpdateTrayMenu();
                    };
                    contextMenu.Items.Add(openItem);
                }
            }

            contextMenu.Items.Add(new Forms.ToolStripSeparator());
            var exitMenuItem = new Forms.ToolStripMenuItem("Exit");
            exitMenuItem.Click += (s, e) => {
                if (_trayIcon != null)
                    _trayIcon.Visible = false;
                System.Windows.Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitMenuItem);
            _trayIcon.ContextMenuStrip = contextMenu;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Clean up resources
            _hardwareMonitor?.Dispose();
            // Clean up tray icon
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }
    }
}
