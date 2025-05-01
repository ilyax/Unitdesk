using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace UnitDesk.Utilities
{
    /// <summary>
    /// Handles persistence of widget settings and state
    /// </summary>
    public class WidgetStorageService
    {
        private readonly string _settingsDirectory;

        public WidgetStorageService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _settingsDirectory = Path.Combine(appData, "UnitDesk");
            
            // Ensure directory exists
            Directory.CreateDirectory(_settingsDirectory);
        }

        /// <summary>
        /// Saves the widget position to storage
        /// </summary>
        public void SavePosition(string widgetId, double left, double top)
        {
            try
            {
                var position = new WidgetPosition { Left = left, Top = top };
                var json = JsonSerializer.Serialize(position);
                string filePath = GetSettingsFilePath(widgetId);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Log exception and handle gracefully
                Console.WriteLine($"Error saving position for widget {widgetId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the widget position from storage
        /// </summary>
        public WidgetPosition LoadPosition(string widgetId)
        {
            try
            {
                string filePath = GetSettingsFilePath(widgetId);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<WidgetPosition>(json);
                }
            }
            catch (Exception ex)
            {
                // Log exception and handle gracefully
                Console.WriteLine($"Error loading position for widget {widgetId}: {ex.Message}");
            }

            // Return default position if loading fails
            return new WidgetPosition
            {
                Left = 50,
                Top = 50
            };
        }

        /// <summary>
        /// Gets the settings file path for a specific widget
        /// </summary>
        private string GetSettingsFilePath(string widgetId)
        {
            return Path.Combine(_settingsDirectory, $"{widgetId}.json");
        }

        /// <summary>
        /// Gets all saved widget IDs from the settings directory
        /// </summary>
        public List<string> GetSavedWidgetIds()
        {
            var widgetIds = new List<string>();
            
            try
            {
                if (Directory.Exists(_settingsDirectory))
                {
                    string[] files = Directory.GetFiles(_settingsDirectory, "*.json");
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        widgetIds.Add(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting saved widget IDs: {ex.Message}");
            }
            
            return widgetIds;
        }
    }

    /// <summary>
    /// Represents the position data for the widget
    /// </summary>
    public class WidgetPosition
    {
        public double Left { get; set; }
        public double Top { get; set; }
    }
}
