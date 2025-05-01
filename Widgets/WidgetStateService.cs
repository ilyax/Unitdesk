using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UnitDesk.Widgets
{
    public class WidgetStateService
    {
        private readonly string _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WidgetState.json");

        public List<string> LoadOpenWidgetIds()
        {
            if (!File.Exists(_stateFilePath))
                return new List<string>();
            try
            {
                var json = File.ReadAllText(_stateFilePath);
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public void SaveOpenWidgetIds(List<string> widgetIds)
        {
            try
            {
                var json = JsonSerializer.Serialize(widgetIds);
                File.WriteAllText(_stateFilePath, json);
            }
            catch { /* ignore errors */ }
        }
    }
}
