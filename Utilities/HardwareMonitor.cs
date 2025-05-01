using System;
using System.Diagnostics;
using LibreHardwareMonitor.Hardware;

namespace UnitDesk.Utilities
{
    /// <summary>
    /// Provides methods for monitoring system hardware including CPU, RAM, and GPU
    /// </summary>
    public class HardwareMonitor : IDisposable
    {
        private Computer _computer;
        private PerformanceCounter _cpuCounter;
        
        public HardwareMonitor()
        {
            InitializeCpuCounter();
            InitializeGpuMonitor();
        }

        private void InitializeCpuCounter()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // First call will always return 0
            }
            catch (Exception ex)
            {
                // Log exception and handle gracefully
                Console.WriteLine($"Failed to initialize CPU counter: {ex.Message}");
            }
        }

        private void InitializeGpuMonitor()
        {
            try
            {
                _computer = new Computer
                {
                    IsGpuEnabled = true
                };
                _computer.Open();
            }
            catch (Exception ex)
            {
                // Log exception and handle gracefully
                Console.WriteLine($"Failed to initialize GPU monitor: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current CPU usage as a percentage
        /// </summary>
        public float GetCpuUsage()
        {
            try
            {
                return _cpuCounter?.NextValue() ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the current RAM usage as a percentage
        /// </summary>
        public float GetRamUsage()
        {
            try
            {
                var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                return ramCounter.NextValue();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets detailed information about GPU usage
        /// </summary>
        public string GetGpuInfo()
        {
            try
            {
                string info = "";
                
                if (_computer == null)
                    return "GPU monitoring not initialized.";
                    
                foreach (IHardware hardware in _computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.GpuNvidia ||
                        hardware.HardwareType == HardwareType.GpuAmd)
                    {
                        hardware.Update();
                        // Get Load and Memory sensors
                        var loadSensor = hardware.Sensors
                            .FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Core"));
                        var memSensor = hardware.Sensors
                            .FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("Memory"));

                        if (loadSensor != null)
                            info += $"{hardware.Name} Core: {loadSensor.Value:F1}%\n";
                        if (memSensor != null)
                            info += $"{hardware.Name} Memory: {memSensor.Value:F1}%\n";
                    }
                }

                return string.IsNullOrEmpty(info)
                    ? "GPU information not available."
                    : info.TrimEnd('\n');
            }
            catch (Exception ex)
            {
                return $"Error retrieving GPU info: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _computer?.Close();
            _cpuCounter?.Dispose();
        }
    }
}
