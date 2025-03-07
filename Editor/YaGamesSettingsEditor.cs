using System.IO;
using UnityEditor;
using UnityEngine;
using YaGamesSDK.Core;

namespace YaGamesSDK.Editor
{
    [InitializeOnLoad]
    public static class YaGamesSettingsEditor
    {
        private static readonly string _assetPath = $"Assets/Resources/YaGamesSettings.asset";

        static YaGamesSettingsEditor()
        {
            if (Resources.Load<YaGamesSettings>("YaGamesSettings") == null)
            {
                CreateSettingsFile();
            }
        }

        [MenuItem("Window/YaGames Settings", false, 0)]
        private static void OpenSettings()
        {
            Selection.activeObject = YaGamesSettings.Instance;
            EditorGUIUtility.PingObject(YaGamesSettings.Instance);
        }

        private static void CreateSettingsFile()
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
        }
    }
}
