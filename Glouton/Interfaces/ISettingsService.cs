using Glouton.Settings;

namespace Glouton.Interfaces;

public interface ISettingsService
{
    AppSettings GetSettings();
    void UpdateSettings(AppSettings settings);
    void UpdateSetting<T>(string key, T value);
}