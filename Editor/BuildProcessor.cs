using System.IO;
using System.IO.Compression;
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
                    ReplaceIndexHtml(settings, report.summary.outputPath);
                }

                if (settings.ArchiveBuild)
                {
                    ArchiveBuildFolder(report.summary.outputPath);
                }
            }
        }

        private void ReplaceIndexHtml(YaGamesSettings settings, string buildPath)
        {
            string targetIndexPath = Path.Combine(buildPath, "index.html");
            var indexFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Packages/com.serbull.yagames-sdk/Editor/Resources/index.html");

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

            //Replace name with product name
            fileContent = fileContent.Replace("nameplaceholder", Application.productName);

            //Replace IS showing on starting
            if (!settings.ShowInterstitialOnGameStart)
            {
                string line = "ysdk.adv.showFullscreenAdv();";
                fileContent = fileContent.Replace(line, $"//{line}");
            }

            File.WriteAllText(targetIndexPath, fileContent);

            YaGames.Log("Custom index.html successfully copied to build folder.");
        }

        private void ArchiveBuildFolder(string buildPath)
        {
            string zipPath = buildPath.TrimEnd(Path.DirectorySeparatorChar) + ".zip";

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
                YaGames.Log("Old archive deleted.");
            }

            ZipFile.CreateFromDirectory(buildPath, zipPath);
            YaGames.Log($"Build folder archived: {zipPath}");
        }
    }
}
