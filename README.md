# Protect Eyes

[中文](README.md) | [English](README.en.md)

Protect Eyes 是一个 Windows 桌面护眼提醒工具。它会按照设定的间隔弹出休息提醒窗口，支持多显示器、倒计时、托盘运行、开机启动和中英文界面切换。

## 状态

| Status | Value |
|:--|:--:|
| Stars | [![Stars](https://img.shields.io/github/stars/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Forks | [![Forks](https://img.shields.io/github/forks/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| License | [![License](https://img.shields.io/github/license/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Issues | [![Issues](https://img.shields.io/github/issues/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes) |
| Release Downloads | [![Downloads](https://img.shields.io/github/downloads/qiuhaotc/ProtectEyes/total.svg)](https://github.com/qiuhaotc/ProtectEyes/releases) |

## 功能

- 周期计时，到期后弹出休息提醒窗口
- 多显示器支持，每个屏幕显示独立提醒窗口
- 休息提醒倒计时，并在倒计时结束后自动关闭
- 托盘运行，双击托盘图标可显示或隐藏主窗口
- 托盘菜单支持快速退出
- 支持开机自动启动，使用当前用户的 HKCU Run 注册表项
- 支持自动、中文、英文三种语言模式
- 支持手动模拟弹窗，方便测试提醒效果
- 使用 JSON 配置持久化应用设置

## 技术栈

- .NET 9.0 Windows
- WPF + Windows Forms NotifyIcon
- Material Design in XAML
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Configuration.Json
- NUnit

## 配置

配置文件位于 [ProtectEyes/appsettings.json](ProtectEyes/appsettings.json)。

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

配置项说明：

| Key | Description |
|:--|:--|
| `RunBetween` | 两次休息提醒之间的间隔，单位为分钟 |
| `DisplayNotifySeconds` | 休息提醒窗口显示时长，单位为秒 |
| `AutoStart` | 是否随 Windows 开机启动 |
| `LanguageMode` | 语言模式，支持 `auto`、`zh-CN`、`en-US` |

## 构建与运行

```powershell
dotnet build .\ProtectEyes.sln -c Release
dotnet run --project .\ProtectEyes\ProtectEyes.csproj
```

## 测试

```powershell
dotnet test .\ProtectEyes.sln -c Release
```

## 发布

```powershell
# 框架依赖
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained false -o .\publish

# 自包含单文件
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o .\publish-self
```

## 项目结构

- [ProtectEyes/Infrastructure](ProtectEyes/Infrastructure)：应用级基础设施，目前包含 DI 容器配置入口 `AppHost`
- [ProtectEyes/Services](ProtectEyes/Services)：配置服务和服务抽象
- [ProtectEyes/Services/Interfaces](ProtectEyes/Services/Interfaces)：`IAutoStartService`、`INotifyService`、`ITimerService`、`ILocalizationService` 等接口
- [ProtectEyes/Services/Implementations](ProtectEyes/Services/Implementations)：自动启动、通知调度、计时器、本地化等服务实现
- [ProtectEyes/ViewModels](ProtectEyes/ViewModels)：基于 CommunityToolkit.Mvvm 的主窗口和通知窗口 ViewModel
- [ProtectEyes/Resources](ProtectEyes/Resources)：图标和休息提醒背景图资源
- [ProtectEyeTest](ProtectEyeTest)：NUnit 测试项目，包含 ViewModel、通知计时器、自动启动和配置相关测试
- [plans/modernization-plan.md](plans/modernization-plan.md)：现代化改造方案记录

## 近期改进

- 使用 Material Design in XAML 重新整理主窗口和通知窗口界面
- 使用 CommunityToolkit.Mvvm 替换自定义 `BaseViewModel` 和 `CommandHandler`
- 将提醒调度、倒计时、自动启动和本地化逻辑拆分为可测试服务
- 使用 `AppHost` 集中注册依赖注入服务
- 增加 `ILogger` 日志接入
- 新增语言切换能力，配置可持久化到 `LanguageMode`
- 新增“模拟弹窗”命令，便于即时验证提醒窗口
- 更新测试以覆盖新的 `MainViewModel`、`NotifyViewModel` 和服务接口

## 后续可选改进

- 增加配置校验和更友好的错误提示
- 为日志增加文件输出或可配置日志级别
- 增加版本信息与更新检查
- 提供更丰富的托盘菜单，例如暂停、恢复和立即提醒
