using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace YaGamesSDK.Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) { }

        public void OnPostprocessBuild(BuildReport report)
        {
            string buildTime = DateTime.Now.ToString("yyMMdd/HHmm");

            Core.YaGamesSettings.Instance.BuildTime = buildTime;
            YaGames.Log($"Build time: {buildTime}");
        }
    }
}
