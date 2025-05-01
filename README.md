# UnitDesk - Desktop Monitoring Widgets

UnitDesk is a modular, customizable desktop widget framework for Windows that provides real-time system monitoring. It uses Windows Presentation Foundation (WPF) to create lightweight, transparent widgets that sit directly on your desktop.

![hippo](https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExZXJnbmRwNHh5dmtiemd2amtrZG1jN295bzRiZjltbG9hdGVneHU4eCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/bIt5sBF9V46rmwaOKW/giphy.gif)

## Features

- **Multiple Widget Types**:
  - System Info Widget (CPU, RAM, GPU usage)
  - Disk Space Monitoring
  - Clock and Date Display
  - Easily extensible for custom widgets

- **System Tray Integration**:
  - Manage widgets directly from the Windows system tray icon
  - Open, close, or exit the application from the tray menu
  - Tray menu always reflects the current widget state in real time

- **Widget State Persistence**:
  - Remembers which widgets you had open or closed at your last session
  - On restart, only your previously open widgets are restored
  - Works automatically, no setup required

- **Desktop Integration**:
  - Widgets sit directly on the desktop background
  - Semi-transparent, non-intrusive design
  - Stay-on-top capability

- **User-Friendly**:
  - Drag and drop positioning
  - Position memory between sessions
  - Reset to default position option
  - Simple close button for each widget

- **Performance Monitoring**:
  - Real-time CPU usage tracking
  - RAM usage monitoring
  - GPU load and memory usage for NVIDIA and AMD cards
  - Disk space monitoring for all drives

## Technology Stack

- **Framework**: .NET 8.0 with WPF (Windows Presentation Foundation)
- **Language**: C#
- **Hardware Monitoring**: LibreHardwareMonitor
- **Serialization**: System.Text.Json
- **Windows Integration**: Win32 API via P/Invoke

## Architecture

UnitDesk is built with a modular, extensible architecture following SOLID principles:

### Core Components

1. **Widget Framework**:
   - `BaseWidget`: Abstract base class for all widgets
   - `WidgetManager`: Singleton service that handles widget creation and lifecycle
   - `WidgetConfiguration`: Configuration system for widget settings

2. **Utility Services**:
   - `HardwareMonitor`: System hardware monitoring (CPU, RAM, GPU)
   - `DiskMonitor`: Disk space monitoring
   - `WidgetStorageService`: Persistence of widget settings
   - `DesktopIntegration`: Desktop background integration via Win32 API

### Design Approach

- **Modular Design**: Each component has a single responsibility
- **Extensibility**: Easy to add new widget types
- **Dependency Injection**: Services are injected into widgets
- **Error Handling**: Robust exception handling throughout
- **Resource Management**: Proper cleanup with IDisposable implementation

## Installation

### Prerequisites

- Windows 10/11
- .NET 8.0 Runtime
- Visual Studio 2022 (for development)

### Setup

1. Clone the repository or download the release
2. Build the solution using Visual Studio or .NET CLI
3. Run the application (UnitDesk.exe)

### Automated Windows Installer

- Each push to the repository triggers a GitHub Actions workflow that:
  - Builds the app for Windows (self-contained, no .NET runtime needed)
  - Packages the app into a Windows installer (`UnitDeskSetup.exe`) using Inno Setup
  - Uploads the installer as a downloadable artifact on the Actions page
- You can always get the latest installer from the [GitHub Actions](../../actions) tab.

## Usage

### Running the Application

When you start UnitDesk, three default widgets will appear on your desktop:
- System Info Widget (CPU, RAM, GPU stats)
- Disk Info Widget (Drive space information)
- Clock Widget (Time and date)

### Widget Controls

Each widget has the following controls:
- **Drag Area**: Click and drag anywhere on the widget to reposition
- **Reset Button (⟲)**: Click to reset widget to its default position
- **Close Button (✕)**: Click to close the widget

### Configuration

Widget settings are stored in:
`%APPDATA%\UnitDesk\`

- Individual widget positions: `widget-id.json`
- Widget configurations: `widgets.json`

## Extending with Custom Widgets

To create a new widget type:

1. Create a new XAML file and code-behind class in the Widgets folder
2. Inherit from BaseWidget
3. Implement the required abstract methods:
   - InitializeWidget()
   - StartDataUpdates()
4. Add your widget to App.xaml.cs or the widget configuration

Example:
```csharp
// In App.xaml.cs
WidgetManager.Instance.CreateWidget<YourCustomWidget>("your-widget-id");
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for hardware monitoring capabilities
