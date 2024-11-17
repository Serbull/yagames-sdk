#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(YaGames))]
public class YaGamesEditor : Editor
{
    private SerializedProperty _showInterstitialOnRepeat;

    private void OnEnable()
    {
        _showInterstitialOnRepeat = serializedObject.FindProperty("_showInterstitialOnRepeat");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var exclude = new List<string>();
        if (!_showInterstitialOnRepeat.boolValue)
        {
            exclude.Add("_interstialRepeatTimer");
        }

        DrawPropertiesExcluding(serializedObject, exclude.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
