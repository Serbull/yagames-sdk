using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YaGamesSDK.Core;

namespace YaGamesSDK.Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            var settings = YaGamesSettings.Instance;
            settings.BuildVersion++;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            var settings = YaGamesSettings.Instance;

            if (report.summary.result == BuildResult.Cancelled || report.summary.result == BuildResult.Failed)
            {
                settings.BuildVersion--;

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssetIfDirty(settings);
            }
            else
            {
                YaGames.Log($"Build version: {YaGamesSettings.Instance.BuildVersion}");
                if (settings.ReplaceIndexHtml)
                {
                    ReplaceIndexHtml(report.summary.outputPath, Application.productName);
                }
            }
        }

        private void ReplaceIndexHtml(string buildPath, string productName)
        {
            string targetIndexPath = Path.Combine(buildPath, "index.html");
            var indexFile = Resources.Load<TextAsset>("index");

            if (indexFile == null)
            {
                YaGames.LogError("Custom index.html not found. Skipping replacement.");
                return;
            }

            string tempPath = Path.Combine(Application.temporaryCachePath, "index.html");
            File.WriteAllText(tempPath, indexFile.text);
            string customIndexPath = tempPath;

            if (!File.Exists(customIndexPath))
            {
                YaGames.LogError("Custom index.html cannot be created. Skipping replacement.");
                return;
            }

            File.Copy(customIndexPath, targetIndexPath, true);

            string fileContent = File.ReadAllText(targetIndexPath);
            fileContent = fileContent.Replace("nameplaceholder", productName);
            File.WriteAllText(targetIndexPath, fileContent);

            YaGames.Log("Custom index.html successfully copied to build folder.");
        }
    }
}
