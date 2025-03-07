using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using YaGamesSDK.Core;

namespace YaGamesSDK.Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = YaGamesSettings.Instance;
            settings.BuildVersion++;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            YaGames.Log($"Build version: {settings.BuildVersion}");
        }

        public void OnPostprocessBuild(BuildReport report) { }
    }
}
