using CommunityToolkit.Mvvm.ComponentModel;
using avaloniakiro20260104.Services;

namespace avaloniakiro20260104.Models;

public partial class SystemSettings : ObservableObject
{
    [ObservableProperty]
    private string _nhostSubdomain = "uxgwdiuehabbzenwtcqo";

    [ObservableProperty]
    private string _nhostAdminSecret = "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr";

    [ObservableProperty]
    private string _nhostRegion = "eu-central-1";

    [ObservableProperty]
    private bool _enableNotifications = true;

    [ObservableProperty]
    private string _theme = "Default";

    [ObservableProperty]
    private string _language = "繁體中文";

    [ObservableProperty]
    private bool _autoSave = true;

    [ObservableProperty]
    private int _autoSaveInterval = 5; // 分鐘

    [ObservableProperty]
    private bool _useGraphQL = true; // 預設使用 GraphQL

    [ObservableProperty]
    private string _currentApiType = "GraphQL";

    public SystemSettings()
    {
        // 預設值已在屬性中設定
    }

    public void SaveSettings()
    {
        SettingsService.SaveSettings(this);
    }

    public void LoadSettings()
    {
        var loadedSettings = SettingsService.LoadSettings();
        
        // 複製載入的設定到當前實例
        NhostSubdomain = loadedSettings.NhostSubdomain;
        NhostAdminSecret = loadedSettings.NhostAdminSecret;
        NhostRegion = loadedSettings.NhostRegion;
        EnableNotifications = loadedSettings.EnableNotifications;
        Theme = loadedSettings.Theme;
        Language = loadedSettings.Language;
        AutoSave = loadedSettings.AutoSave;
        AutoSaveInterval = loadedSettings.AutoSaveInterval;
        UseGraphQL = loadedSettings.UseGraphQL;
        CurrentApiType = loadedSettings.CurrentApiType;
    }
}