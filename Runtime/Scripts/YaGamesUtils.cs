#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YaGamesSDK.Core
{
    public static class YaGamesUtils
    {
        private static readonly string _assetPath = $"Assets/Resources/YaGamesSettings.asset";

        [MenuItem("Window/YaGames Settings", false, 0)]
        private static void OpenSettings()
        {
            Selection.activeObject = YaGamesSettings.Instance;
            EditorGUIUtility.PingObject(YaGamesSettings.Instance);
        }

        public static YaGamesSettings CreateSettingsFile()
        {
            var settings = ScriptableObject.CreateInstance<YaGamesSettings>();

            string directory = Path.GetDirectoryName(_assetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(settings, _assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return settings;
        }
    }
}
#endif
