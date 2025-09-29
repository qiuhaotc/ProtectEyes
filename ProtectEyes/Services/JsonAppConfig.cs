using System.IO;
using Microsoft.Extensions.Configuration;

namespace ProtectEyes.Services;

public class JsonAppConfig : IAppConfig
{
    readonly string _filePath;
    readonly IConfigurationRoot _root;
    readonly IConfigurationSection _section;

    public JsonAppConfig(string? basePath = null)
    {
        basePath ??= Directory.GetCurrentDirectory();
        _filePath = Path.Combine(basePath, "appsettings.json");
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        _root = builder.Build();
        _section = _root.GetSection("ProtectEyes");
        RunBetween = _section.GetValue<int?>("RunBetween") ?? 20;
        DisplayNotifySeconds = _section.GetValue<int?>("DisplayNotifySeconds") ?? 10;
        AutoStart = _section.GetValue<bool?>("AutoStart") ?? false;
    }

    public int RunBetween { get; set; }
    public int DisplayNotifySeconds { get; set; }
    public bool AutoStart { get; set; }

    public void Save()
    {
        // 简单覆盖写入，避免引入额外 JSON 库（.NET 自带 System.Text.Json）
        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            ProtectEyes = new
            {
                RunBetween,
                DisplayNotifySeconds,
                AutoStart
            }
        }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
