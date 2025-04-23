using UnityEditor;
using UnityEngine;

namespace YaGamesSDK.Editor
{
    [CustomPropertyDrawer(typeof(InAppIdAttribute))]
    public class InAppIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var options = GetOptions();
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);
            property.stringValue = options[selectedIndex];
        }

        private string[] GetOptions()
        {
            var settings = Core.YaGamesSettings.Instance;
            if (settings.Products == null || settings.Products.Length == 0)
            {
                return new string[] { "-" };
            }

            var result = new string[settings.Products.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = settings.Products[i].Id;
            }

            return result;
        }
    }
}
