using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Glouton.Settings;

internal class SettingsService : ISettingsService
{
    private readonly string _settingsPath;
    private AppSettings _currentSettings;

    public SettingsService()
    {
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        _currentSettings = new AppSettings();
        LoadSettings();
    }

    public AppSettings GetSettings()
    {
        return _currentSettings;
    }

    public void UpdateSettings(AppSettings settings)
    {
        _currentSettings = settings;
        SaveSettings();
    }

    public void UpdateSetting<T>(string key, T value)
    {
        PropertyInfo? property = typeof(AppSettings).GetProperty(key);
        if (property != null && property.CanWrite)
        {
            property.SetValue(_currentSettings, value);
            SaveSettings();
        }
    }

    private void LoadSettings()
    {

        if (File.Exists(_settingsPath))
        {
            string json = File.ReadAllText(_settingsPath);
            Dictionary<string, AppSettings>? config = JsonSerializer.Deserialize<Dictionary<string, AppSettings>>(json);
            _currentSettings = config?["AppSettings"] ?? new AppSettings();
        }
    }

    private void SaveSettings()
    {

        var config = new Dictionary<string, AppSettings>
        {
            ["AppSettings"] = _currentSettings
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(_settingsPath, json);
    }
}