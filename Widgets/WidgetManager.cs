using System;
using System.Collections.Generic;
using System.Windows;
using UnitDesk.Utilities;

namespace UnitDesk.Widgets
{
    /// <summary>
    /// Manages the creation and lifecycle of all widgets in the application
    /// </summary>
    public class WidgetManager
    {
        private static WidgetManager _instance;
        private readonly Dictionary<string, BaseWidget> _activeWidgets;
        private readonly WidgetConfigurationService _configService;
        private readonly WidgetStateService _stateService = new WidgetStateService();

        public static WidgetManager Instance => _instance ??= new WidgetManager();

        private WidgetManager()
        {
            _activeWidgets = new Dictionary<string, BaseWidget>();
            _configService = new WidgetConfigurationService();
        }

        private void SaveOpenWidgetStates()
        {
            var openIds = new List<string>(_activeWidgets.Keys);
            _stateService.SaveOpenWidgetIds(openIds);
        }

        /// <summary>
        /// Creates and shows a new widget of the specified type
        /// </summary>
        /// <typeparam name="T">Type of widget to create</typeparam>
        /// <param name="widgetId">Unique identifier for the widget</param>
        /// <returns>The created widget instance</returns>
        public T CreateWidget<T>(string widgetId) where T : BaseWidget
        {
            // Check if widget with this ID already exists
            if (_activeWidgets.ContainsKey(widgetId))
            {
                throw new InvalidOperationException($"Widget with ID '{widgetId}' already exists");
            }

            // Create widget instance using reflection
            T widget = (T)Activator.CreateInstance(typeof(T), widgetId);
            
            // Store in active widgets dictionary
            _activeWidgets.Add(widgetId, widget);
            
            // Show the widget
            widget.Show();
            
            SaveOpenWidgetStates();
            
            return widget;
        }

        /// <summary>
        /// Gets an existing widget by its ID
        /// </summary>
        public T? GetWidget<T>(string widgetId) where T : BaseWidget
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget) && widget is T typedWidget)
            {
                return typedWidget;
            }
            return null;
        }

        /// <summary>
        /// Closes and removes a widget
        /// </summary>
        public bool CloseWidget(string widgetId)
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
            {
                widget.Close();
                var removed = _activeWidgets.Remove(widgetId);
                SaveOpenWidgetStates();
                return removed;
            }
            return false;
        }

        /// <summary>
        /// Creates widgets based on saved configuration
        /// </summary>
        public void CreateWidgetsFromConfiguration()
        {
            // Load the last open widgets from state file
            var openWidgetIds = _stateService.LoadOpenWidgetIds();
            if (openWidgetIds.Count > 0)
            {
                foreach (var widgetId in openWidgetIds)
                {
                    try
                    {
                        switch (widgetId)
                        {
                            case "system-info":
                                CreateWidget<SystemInfoWidget>(widgetId);
                                break;
                            case "disk-info":
                                CreateWidget<DiskInfoWidget>(widgetId);
                                break;
                            case "clock":
                                CreateWidget<ClockWidget>(widgetId);
                                break;
                        }
                    }
                    catch { /* ignore errors */ }
                }
                return; // If WidgetState.json has data, skip legacy config
            }
            // If WidgetState.json is missing or empty, fall back to legacy config
            var widgetConfigs = _configService.GetWidgetConfigurations();
            foreach (var config in widgetConfigs)
            {
                try
                {
                    switch (config.WidgetType)
                    {
                        case "SystemInfoWidget":
                            if (GetWidget<SystemInfoWidget>(config.WidgetId) == null)
                                CreateWidget<SystemInfoWidget>(config.WidgetId);
                            break;
                        case "DiskInfoWidget":
                            if (GetWidget<DiskInfoWidget>(config.WidgetId) == null)
                                CreateWidget<DiskInfoWidget>(config.WidgetId);
                            break;
                        case "ClockWidget":
                            if (GetWidget<ClockWidget>(config.WidgetId) == null)
                                CreateWidget<ClockWidget>(config.WidgetId);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error creating widget {config.WidgetId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Closes all widgets
        /// </summary>
        public void CloseAllWidgets()
        {
            foreach (var widget in _activeWidgets.Values)
            {
                widget.Close();
            }
            _activeWidgets.Clear();
        }
    }
}
