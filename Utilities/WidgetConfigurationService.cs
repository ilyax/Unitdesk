using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UnitDesk.Utilities
{
    /// <summary>
    /// Manages widget configurations, including types and settings
    /// </summary>
    public class WidgetConfigurationService
    {
        private readonly string _configFile;

        public WidgetConfigurationService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configDir = Path.Combine(appData, "UnitDesk");
            _configFile = Path.Combine(configDir, "widgets.json");
            
            // Ensure directory exists
            Directory.CreateDirectory(configDir);
            
            // Create default config if it doesn't exist
            if (!File.Exists(_configFile))
            {
                CreateDefaultConfiguration();
            }
        }

        /// <summary>
        /// Gets all widget configurations
        /// </summary>
        public List<WidgetConfiguration> GetWidgetConfigurations()
        {
            try
            {
                if (File.Exists(_configFile))
                {
                    string json = File.ReadAllText(_configFile);
                    return JsonSerializer.Deserialize<List<WidgetConfiguration>>(json) ?? new List<WidgetConfiguration>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading widget configurations: {ex.Message}");
            }
            
            return new List<WidgetConfiguration>();
        }

        /// <summary>
        /// Saves widget configurations
        /// </summary>
        public void SaveWidgetConfigurations(List<WidgetConfiguration> configurations)
        {
            try
            {
                string json = JsonSerializer.Serialize(configurations, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_configFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving widget configurations: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new widget configuration
        /// </summary>
        public void AddWidgetConfiguration(WidgetConfiguration configuration)
        {
            var configurations = GetWidgetConfigurations();
            
            // Check for duplicate IDs
            if (configurations.Exists(c => c.WidgetId == configuration.WidgetId))
            {
                throw new InvalidOperationException($"Widget with ID '{configuration.WidgetId}' already exists");
            }
            
            configurations.Add(configuration);
            SaveWidgetConfigurations(configurations);
        }

        /// <summary>
        /// Removes a widget configuration
        /// </summary>
        public bool RemoveWidgetConfiguration(string widgetId)
        {
            var configurations = GetWidgetConfigurations();
            bool removed = configurations.RemoveAll(c => c.WidgetId == widgetId) > 0;
            
            if (removed)
            {
                SaveWidgetConfigurations(configurations);
            }
            
            return removed;
        }

        /// <summary>
        /// Creates default widget configuration
        /// </summary>
        private void CreateDefaultConfiguration()
        {
            var defaultConfigs = new List<WidgetConfiguration>
            {
                new WidgetConfiguration
                {
                    WidgetId = "system-info",
                    WidgetType = "SystemInfoWidget",
                    Title = "System Information"
                },
                new WidgetConfiguration
                {
                    WidgetId = "disk-info",
                    WidgetType = "DiskInfoWidget",
                    Title = "Disk Information"
                },
                new WidgetConfiguration
                {
                    WidgetId = "clock",
                    WidgetType = "ClockWidget",
                    Title = "Clock"
                }
            };
            
            SaveWidgetConfigurations(defaultConfigs);
        }
    }

    /// <summary>
    /// Represents configuration for a single widget
    /// </summary>
    public class WidgetConfiguration
    {
        /// <summary>
        /// Unique identifier for the widget
        /// </summary>
        public string WidgetId { get; set; }
        
        /// <summary>
        /// Type name of the widget
        /// </summary>
        public string WidgetType { get; set; }
        
        /// <summary>
        /// Display title for the widget
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Additional configuration settings specific to this widget type
        /// </summary>
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}
