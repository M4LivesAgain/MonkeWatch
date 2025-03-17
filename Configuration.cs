using System.IO;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace BananaWatch
{
    internal static class Configuration
    {
        public static ConfigFile Config { get; private set; }

        public static ConfigEntry<Color> PressedColor { get; private set; }
        public static ConfigEntry<Color> ArrowColor { get; private set; }
        public static ConfigEntry<Color> EnterColor { get; private set; }
        public static ConfigEntry<Color> BackColor { get; private set; }
        public static ConfigEntry<bool> WrapSelectArrow { get; private set; }
        public static ConfigEntry<bool> HapticResponse { get; private set; }
        public static ConfigEntry<float> WatchClosingAngle { get; private set; }
        public static ConfigEntry<float> ButtonCooldown { get; private set; }

        static Configuration()
        {
            Config = new ConfigFile(Path.Combine(Paths.ConfigPath, "BananaWatch.cfg"), true)
            {
                SaveOnConfigSet = true
            };
            LoadSettings();
        }

        public static bool IsModEnabled(BepInEx.PluginInfo plugin) =>
            Config.Bind("MonkeWatch", plugin.Metadata.Name, true).Value;

        public static bool ToggleMod(BepInEx.PluginInfo plugin)
        {
            bool newState = !plugin.Instance.enabled;
            Config.Bind("MonkeWatch", plugin.Metadata.Name, true).Value = newState;
            plugin.Instance.enabled = newState;
            return newState;
        }

        public static void LoadSettings()
        {
            Config.Reload();

            PressedColor = BindColor("Pressed Color", 104, 104, 105);
            ArrowColor = BindColor("Arrow Color", 171, 219, 171);
            EnterColor = BindColor("Enter Color", 219, 204, 171);
            BackColor = BindColor("Back Color", 133, 133, 222);
            WrapSelectArrow = Config.Bind("MonkeWatch Config", "Selector Wrapping", true);
            WatchClosingAngle = Config.Bind("MonkeWatch Config", "Angle to close MonkeWatch", 45f);
            HapticResponse = Config.Bind("MonkeWatch Config", "Haptic Vibrations", true);
            ButtonCooldown = Config.Bind("MonkeWatch Config", "Button Cooldown", 0.25f);

        }

        private static ConfigEntry<Color> BindColor(string key, float r, float g, float b, float a = 255) =>
            Config.Bind("MonkeWatch Config", key, new Color(r / 255f, g / 255f, b / 255f, a / 255f));
    }
}
