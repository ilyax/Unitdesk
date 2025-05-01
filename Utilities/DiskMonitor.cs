using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitDesk.Utilities
{
    /// <summary>
    /// Provides methods for monitoring disk usage and information
    /// </summary>
    public class DiskMonitor
    {
        /// <summary>
        /// Gets information about all fixed drives in the system
        /// </summary>
        /// <returns>List of formatted disk information strings</returns>
        public List<string> GetDiskInfo()
        {
            try
            {
                var diskInfoList = new List<string>();
                
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                    .ToList();

                foreach (var drive in drives)
                {
                    string diskName = string.IsNullOrEmpty(drive.VolumeLabel) 
                        ? drive.Name 
                        : $"{drive.Name} ({drive.VolumeLabel})";
                        
                    double freeGb = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    double totalGb = drive.TotalSize / (1024.0 * 1024 * 1024);
                    double usedPercentage = 100 - ((double)drive.AvailableFreeSpace / drive.TotalSize * 100);

                    diskInfoList.Add($"{diskName}: {freeGb:F1} GB free / {totalGb:F1} GB total ({usedPercentage:F1}% used)");
                }

                return diskInfoList;
            }
            catch (Exception ex)
            {
                // Log exception and handle gracefully
                Console.WriteLine($"Error getting disk info: {ex.Message}");
                return new List<string> { "Error retrieving disk information" };
            }
        }

        /// <summary>
        /// Gets detailed information about a specific drive
        /// </summary>
        /// <param name="driveName">Drive letter or path (e.g. "C:" or "C:\\")</param>
        public DriveDetails? GetDriveDetails(string driveName)
        {
            try
            {
                var drive = new DriveInfo(driveName);
                if (!drive.IsReady)
                    return null;

                return new DriveDetails
                {
                    Name = drive.Name,
                    Label = drive.VolumeLabel,
                    DriveType = drive.DriveType,
                    TotalSizeBytes = drive.TotalSize,
                    FreeSpaceBytes = drive.AvailableFreeSpace,
                    UsedSpaceBytes = drive.TotalSize - drive.AvailableFreeSpace,
                    UsedPercentage = 100 - ((double)drive.AvailableFreeSpace / drive.TotalSize * 100),
                    DriveFormat = drive.DriveFormat
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting drive details for {driveName}: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Class representing detailed information about a drive
    /// </summary>
    public class DriveDetails
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public DriveType DriveType { get; set; }
        public long TotalSizeBytes { get; set; }
        public long FreeSpaceBytes { get; set; }
        public long UsedSpaceBytes { get; set; }
        public double UsedPercentage { get; set; }
        public string DriveFormat { get; set; }

        public double TotalSizeGB => TotalSizeBytes / (1024.0 * 1024 * 1024);
        public double FreeSpaceGB => FreeSpaceBytes / (1024.0 * 1024 * 1024);
        public double UsedSpaceGB => UsedSpaceBytes / (1024.0 * 1024 * 1024);
    }
}
