using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Template.Models;
using TShockAPI;

namespace Template;

public class Config
{
    public static readonly string PluginDirectory = Path.Combine(TShock.SavePath, Core.PluginName);
    public static readonly string ConfigPath = Path.Combine(PluginDirectory, "config.json");

    public static Config Settings { get; set; } = new();
    #region Configs

    #endregion
    public static void Save()
    {
        string configJson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
        File.WriteAllText(ConfigPath, configJson);
    }

    public static ResponseMessage Load()
    {
        if (!Directory.Exists(PluginDirectory))
        {
            Directory.CreateDirectory(PluginDirectory);
        }
        if (!File.Exists(ConfigPath))
        {
            Save();
            return new ResponseMessage()
            {
                Text =
                    $"[{Core.PluginName}] Config file doesn't exist yet. A new one has been created.",
                Color = Color.Yellow,
            };
        }
        else
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                Config? deserializedConfig = JsonConvert.DeserializeObject<Config>(
                    json,
                    new JsonSerializerSettings()
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                    }
                );
                if (deserializedConfig != null)
                {
                    Settings = deserializedConfig;
                    Save();
                    return new ResponseMessage()
                    {
                        Text = $"[{Core.PluginName}] Loaded config.",
                        Color = Color.LimeGreen,
                    };
                }
                else
                {
                    return new ResponseMessage()
                    {
                        Text =
                            $"[{Core.PluginName}] Config file was found, but deserialization returned null.",
                        Color = Color.Red,
                    };
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Core.PluginName}] Error loading config: {ex.Message}");
                return new ResponseMessage()
                {
                    Text =
                        $"[{Core.PluginName}] Error loading config. Check logs for more details.",
                    Color = Color.Red,
                };
            }
        }
    }
}
