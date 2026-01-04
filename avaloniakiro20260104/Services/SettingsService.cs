using System;
using System.IO;
using System.Text.Json;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

public static class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "鋒兄Next資訊管理",
        "settings.json"
    );

    public static SystemSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<SystemSettings>(json);
                return settings ?? new SystemSettings();
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤，但不影響程式運行
            Console.WriteLine($"載入設定時發生錯誤: {ex.Message}");
        }

        return new SystemSettings();
    }

    public static void SaveSettings(SystemSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            // 記錄錯誤
            Console.WriteLine($"儲存設定時發生錯誤: {ex.Message}");
            throw; // 重新拋出異常，讓 UI 可以處理
        }
    }
}