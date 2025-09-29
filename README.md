# Protect Eyes
定时弹窗 保护眼睛  
  
可以自定义弹窗时间 弹窗间隔 开机启动

## Misc

|Status|Value|
|:----|:---:|
|Stars|[![Stars](https://img.shields.io/github/stars/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes)
|Forks|[![Forks](https://img.shields.io/github/forks/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes)
|License|[![License](https://img.shields.io/github/license/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes)
|Issues|[![Issues](https://img.shields.io/github/issues/qiuhaotc/ProtectEyes)](https://github.com/qiuhaotc/ProtectEyes)
|Release Downloads|[![Downloads](https://img.shields.io/github/downloads/qiuhaotc/ProtectEyes/total.svg)](https://github.com/qiuhaotc/ProtectEyes/releases)

## 功能梗概

- 周期计时，到期弹出覆盖窗口提醒休息
- 多显示器支持
- 休息窗口倒计时
- 托盘菜单快速退出
- JSON 配置持久化（`appsettings.json`）

## 配置 (appsettings.json)

```json
{
  "ProtectEyes": {
    "RunBetween": 20,
    "DisplayNotifySeconds": 10,
    "AutoStart": false
  }
}
```

## 构建与运行

```powershell
dotnet build .\ProtectEyes.sln -c Release
dotnet run --project .\ProtectEyes\ProtectEyes.csproj
```

## 发布

```powershell
# 框架依赖
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained false -o .\publish
# 自包含单文件
dotnet publish .\ProtectEyes\ProtectEyes.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o .\publish-self
```

## 测试

```powershell
dotnet test .\ProtectEyes.sln -c Release
```

## 结构

- `ProtectEyes/Services`
  - `IAppConfig` / `JsonAppConfig`：JSON 配置抽象与实现
  - `ITimer` / `DispatcherTimerAdapter`：UI 线程定时器抽象
  - `MockManualTimer`：测试驱动的手动推进计时器
  - `IAutoStartManager` / `AutoStartManager`：注册表(HKCU Run) 自动启动管理
- `ProtectEyesViewModel`：周期休息控制逻辑（已使用注入计时器）
- `NotifyViewModel`：倒计时 + 自动关闭（通过 ITimer 可测试）
- `App.xaml.cs`：Host + DI 启动入口
- `.github/workflows/ci.yml`：构建 / 测试 / 发布工件 CI

## 已实现的改进

- 通用 Host + 依赖注入
- 自动启动改用 HKCU 注册表，无需管理员
- Mock 计时器测试（AutoStart + 倒计时覆盖）
- GitHub Actions CI 工作流
- 移除不再需要的 `ChangeRegistry` 项目

## 后续可选改进

- 提供 UI 中动态修改 `DisplayNotifySeconds` 的双向数据绑定
- 增加日志（`ILogger`）与异常捕获统一处理
- 引入可选托盘右键配置入口
- 添加版本信息与更新检查
- 