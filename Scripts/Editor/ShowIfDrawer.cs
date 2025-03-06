#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace YaGamesSDK.Core.Editors
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionName);

            bool shouldShow = false;

            if (conditionProperty != null)
            {
                shouldShow = conditionProperty.boolValue;
            }
            else
            {
                Object target = property.serializedObject.targetObject;
                MethodInfo method = target.GetType().GetMethod(showIf.ConditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null && method.ReturnType == typeof(bool))
                {
                    shouldShow = (bool)method.Invoke(target, null);
                }
                else
                {
                    Debug.LogWarning($"[ShowIf] Condition \"{showIf.ConditionName}\" not found in {target.GetType().Name}");
                }
            }

            if (shouldShow)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionName);

            bool shouldShow = false;

            if (conditionProperty != null)
            {
                shouldShow = conditionProperty.boolValue;
            }
            else
            {
                Object target = property.serializedObject.targetObject;
                MethodInfo method = target.GetType().GetMethod(showIf.ConditionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null && method.ReturnType == typeof(bool))
                {
                    shouldShow = (bool)method.Invoke(target, null);
                }
            }

            return shouldShow ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
        }
    }
}
#endif
