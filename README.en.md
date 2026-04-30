# Protect Eyes

[中文](README.md) | [English](README.en.md)

Protect Eyes is a Windows desktop reminder app that helps you take regular eye breaks. It runs from the system tray, shows timed rest popups, supports multiple monitors, can start with Windows, and includes Chinese and English UI modes.

## Status

| Status | Value |
|:--|:--:|
| Stars | [![Stars](https://img.shields.io/github/stars/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Forks | [![Forks](https://img.shields.io/github/forks/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| License | [![License](https://img.shields.io/github/license/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Issues | [![Issues](https://img.shields.io/github/issues/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Release Downloads | [![Downloads](https://img.shields.io/github/downloads/qiuhaotc/ProtectEyes/total.svg)](https://github.com/qiuhaotc/ProtectEyes/releases) |

## Features

- Periodic timer that shows rest reminder windows when the interval ends
- Multiple monitor support, with a reminder window on each screen
- Countdown inside the rest reminder window
- Automatic closing after the configured reminder duration
- System tray mode with double-click show/hide behavior
- Quick exit from the tray context menu
- Start with Windows through the current user's HKCU Run registry entry
- Automatic, Chinese, and English language modes
- Manual popup simulation for quickly checking the reminder experience
- JSON-based configuration persistence

## Tech Stack

- .NET 9.0 Windows
- WPF + Windows Forms NotifyIcon
- Material Design in XAML
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Configuration.Json
- NUnit

## Configuration

The configuration file is [ProtectEyes/appsettings.json](ProtectEyes/appsettings.json).

```json
{
  "ProtectEyes": {
    "RunBetween": 20,
    "DisplayNotifySeconds": 10,
    "AutoStart": false,
    "LanguageMode": "auto"
  }
}
```

Configuration keys:

| Key | Description |
|:--|:--|
| `RunBetween` | Interval between rest reminders, in minutes |
| `DisplayNotifySeconds` | How long the reminder window stays visible, in seconds |
| `AutoStart` | Whether the app starts with Windows |
| `LanguageMode` | UI language mode: `auto`, `zh-CN`, or `en-US` |

## Build and Run

```powershell
dotnet build .\ProtectEyes.sln -c Release
dotnet run --project .\ProtectEyes\ProtectEyes.csproj
```

## Test

```powershell
dotnet test .\ProtectEyes.sln -c Release
```

## Publish

```powershell
# Framework-dependent
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained false -o .\publish

# Self-contained single file
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o .\publish-self
```

## Project Structure

- [ProtectEyes/Infrastructure](ProtectEyes/Infrastructure): application infrastructure, currently including the DI container entry point `AppHost`
- [ProtectEyes/Services](ProtectEyes/Services): configuration services and service abstractions
- [ProtectEyes/Services/Interfaces](ProtectEyes/Services/Interfaces): interfaces such as `IAutoStartService`, `INotifyService`, `ITimerService`, and `ILocalizationService`
- [ProtectEyes/Services/Implementations](ProtectEyes/Services/Implementations): implementations for auto-start, notification scheduling, timers, and localization
- [ProtectEyes/ViewModels](ProtectEyes/ViewModels): `MainViewModel` and `NotifyViewModel` based on CommunityToolkit.Mvvm
- [ProtectEyes/Resources](ProtectEyes/Resources): icon and rest reminder background image assets
- [ProtectEyeTest](ProtectEyeTest): NUnit test project covering ViewModels, notification timers, auto-start behavior, and configuration
- [plans/modernization-plan.md](plans/modernization-plan.md): modernization plan notes

## Recent Improvements

- Refreshed the main window and notification window with Material Design in XAML
- Replaced custom `BaseViewModel` and `CommandHandler` code with CommunityToolkit.Mvvm
- Split reminder scheduling, countdown, auto-start, and localization logic into testable services
- Centralized dependency injection registration through `AppHost`
- Added `ILogger` integration
- Added language switching with persisted `LanguageMode` configuration
- Added a simulate-popup command for immediate reminder checks
- Updated tests for the new `MainViewModel`, `NotifyViewModel`, and service interfaces

## Possible Next Steps

- Add configuration validation and friendlier error messages
- Add file logging or configurable log levels
- Add version information and update checks
- Expand the tray menu with pause, resume, and remind-now actions
