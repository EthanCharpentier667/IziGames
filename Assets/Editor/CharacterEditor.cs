using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    private Texture2D headerIcon;
    private static MethodInfo setIconForObjectMethodInfo;

    void OnEnable()
    {
        headerIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IziGames/Gizmos/character.png");
        if (setIconForObjectMethodInfo == null)
        {
            setIconForObjectMethodInfo = typeof(EditorGUIUtility).GetMethod(
                "SetIconForObject", 
                BindingFlags.Static | BindingFlags.NonPublic);
        }
        if (headerIcon != null && setIconForObjectMethodInfo != null)
        {
            MonoScript script = MonoScript.FromMonoBehaviour((Character)target);
            setIconForObjectMethodInfo.Invoke(null, new object[] { script, headerIcon });
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        if (headerIcon != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(headerIcon), GUILayout.Width(32), GUILayout.Height(32));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        
        DrawDefaultInspector();
    }
}