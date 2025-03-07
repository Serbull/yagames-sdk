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
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
            {
                YaGames.Log($"Build version: {YaGamesSettings.Instance.BuildVersion}");
            }
            else
            {
                var settings = YaGamesSettings.Instance;
                settings.BuildVersion--;

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
            }
        }
    }
}
