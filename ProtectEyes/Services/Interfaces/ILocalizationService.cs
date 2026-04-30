namespace ProtectEyes.Services.Interfaces;

public sealed record LanguageOption(string Mode, string DisplayName);

public interface ILocalizationService
{
    IReadOnlyList<LanguageOption> LanguageOptions { get; }
    string LanguageMode { get; }
    event EventHandler? LanguageChanged;
    void SetLanguageMode(string mode);
    string Translate(string key);
    string Translate(string key, params object[] args);
}
