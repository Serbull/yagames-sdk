using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace YaGamesSDK
{
    public class Flags
    {
        [DllImport("__Internal")]
        private static extern string LoadFlagsExtern();

        private static Dictionary<string, string> _flags = new();

        public static bool IsLoaded { get; private set; }
        public static event Action OnLoaded;

        public void FlagsLoaded(string flags)
        {
            YaGames.Log($"Flags loaded: {flags}");

            _flags = JsonConvert.DeserializeObject<Dictionary<string, string>>(flags);

            IsLoaded = true;
            OnLoaded?.Invoke();
        }

        public static void Load()
        {
            if (IsLoaded) return;

#if UNITY_EDITOR
            IsLoaded = true;
            OnLoaded?.Invoke();
#else
            LoadFlagsExtern();
#endif
        }

        public static bool HasFlag(string flag)
        {
            return _flags.ContainsKey(flag);
        }

        public static string GetFlag(string flag, string defalutValue)
        {
#if UNITY_EDITOR
            return defalutValue;
#else
            if (_flags.ContainsKey(flag))
            {
                return _flags[flag];
            }
            else
            {
                YaGames.LogError($"Not exist flag: {flag}");
                return defalutValue;
            }
#endif
        }

        public static int GetFlag(string flag, int defalutValue)
        {
            var value = GetFlag(flag, defalutValue.ToString());
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                YaGames.LogError($"Cannot (int) parse flag: {flag} -> {_flags[flag]}");
                return defalutValue;
            }
        }

        public static float GetFlag(string flag, float defalutValue)
        {
            var value = GetFlag(flag, defalutValue.ToString());
            if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            else
            {
                YaGames.LogError($"Cannot (float) parse flag: {flag} -> {_flags[flag]}");
                return defalutValue;
            }
        }
    }
}
