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
            if (report.summary.result == BuildResult.Cancelled || report.summary.result == BuildResult.Failed)
            {
                var settings = YaGamesSettings.Instance;
                settings.BuildVersion--;

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
            }
            else
            {
                YaGames.Log($"Build version: {YaGamesSettings.Instance.BuildVersion}");
            }
        }
    }
}
