using System.Globalization;
using ProtectEyes.Services.Interfaces;

namespace ProtectEyes.Services.Implementations;

public class LocalizationService : ILocalizationService
{
    public const string AutoMode = "auto";
    public const string ChineseMode = "zh-CN";
    public const string EnglishMode = "en-US";

    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new()
    {
        [ChineseMode] = new Dictionary<string, string>
        {
            ["ConfigTab"] = "配置",
            ["AboutTab"] = "关于",
            ["DurationDesc"] = "休息间隔：{0} 分钟",
            ["DisplayNotifySecondsDesc"] = "弹窗显示时间：{0} 秒",
            ["MinutesHint"] = "分钟",
            ["SecondsHint"] = "秒",
            ["AllowExecution"] = "允许执行",
            ["AutoStart"] = "开机自动启动",
            ["Language"] = "语言",
            ["SaveConfig"] = "保存配置",
            ["SimulateNotify"] = "模拟弹窗",
            ["Exit"] = "退出",
            ["StatusRunning"] = "状态：{0}",
            ["Running"] = "运行中",
            ["Paused"] = "已暂停",
            ["GitHubLabel"] = "GitHub：",
            ["LastModified"] = "最后修改日期：2026/04/30",
            ["OpenLinkInvalidTitle"] = "打开链接",
            ["OpenLinkInvalidMessage"] = "链接格式不正确：{0}",
            ["OpenLinkErrorTitle"] = "错误",
            ["OpenLinkErrorMessage"] = "无法打开链接：{0}",
            ["TrayClose"] = "关闭",
            ["NotifyCountdown"] = "请休息一下，剩余时间：{0} 秒"
        },
        [EnglishMode] = new Dictionary<string, string>
        {
            ["ConfigTab"] = "Settings",
            ["AboutTab"] = "About",
            ["DurationDesc"] = "Rest interval: {0} minutes",
            ["DisplayNotifySecondsDesc"] = "Popup display time: {0}s",
            ["MinutesHint"] = "minutes",
            ["SecondsHint"] = "seconds",
            ["AllowExecution"] = "Enable reminders",
            ["AutoStart"] = "Start with Windows",
            ["Language"] = "Language",
            ["SaveConfig"] = "Save",
            ["SimulateNotify"] = "Simulate popup",
            ["Exit"] = "Exit",
            ["StatusRunning"] = "Status: {0}",
            ["Running"] = "Running",
            ["Paused"] = "Paused",
            ["GitHubLabel"] = "GitHub: ",
            ["LastModified"] = "Last modified: 2026/04/30",
            ["OpenLinkInvalidTitle"] = "Open link",
            ["OpenLinkInvalidMessage"] = "Invalid link: {0}",
            ["OpenLinkErrorTitle"] = "Error",
            ["OpenLinkErrorMessage"] = "Unable to open link: {0}",
            ["TrayClose"] = "Close",
            ["NotifyCountdown"] = "Please take a rest. Time remaining: {0}s"
        }
    };

    private readonly IAppConfig _config;
    private string _languageMode;

    public LocalizationService(IAppConfig config)
    {
        _config = config;
        _languageMode = NormalizeMode(config.LanguageMode);
    }

    public IReadOnlyList<LanguageOption> LanguageOptions { get; } = new[]
    {
        new LanguageOption(AutoMode, "自动 / Auto"),
        new LanguageOption(ChineseMode, "中文"),
        new LanguageOption(EnglishMode, "English")
    };

    public string LanguageMode => _languageMode;

    public event EventHandler? LanguageChanged;

    public void SetLanguageMode(string mode)
    {
        var normalized = NormalizeMode(mode);
        if (_languageMode == normalized)
        {
            return;
        }

        _languageMode = normalized;
        _config.LanguageMode = normalized;
        _config.Save();
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public string Translate(string key)
    {
        var language = ResolveLanguage();
        if (Translations[language].TryGetValue(key, out var value))
        {
            return value;
        }

        return Translations[EnglishMode].TryGetValue(key, out var fallback) ? fallback : key;
    }

    public string Translate(string key, params object[] args)
        => string.Format(CultureInfo.CurrentCulture, Translate(key), args);

    private string ResolveLanguage()
    {
        if (_languageMode != AutoMode)
        {
            return _languageMode;
        }

        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase)
            ? ChineseMode
            : EnglishMode;
    }

    private static string NormalizeMode(string? mode)
        => mode switch
        {
            ChineseMode => ChineseMode,
            EnglishMode => EnglishMode,
            _ => AutoMode
        };
}
