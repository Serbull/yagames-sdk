using UnityEngine;

namespace YaGamesSDK.Core
{
    public class BuildTimeShower : MonoBehaviour
    {
        private string _buildTime;
        private GUIStyle _labelStyle;

        private void Start()
        {
            _buildTime = YaGamesSettings.Instance.BuildTime;

            _labelStyle = new GUIStyle();
            _labelStyle.alignment = TextAnchor.LowerRight;
            _labelStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            _labelStyle.fontSize = Mathf.RoundToInt(screenHeight * 0.015f);

            float posX = screenWidth - screenHeight * 0.005f;
            float posY = screenHeight - screenHeight * 0.005f;

            GUI.Label(new Rect(posX, posY, 0, 0), _buildTime, _labelStyle);
        }
    }
}
