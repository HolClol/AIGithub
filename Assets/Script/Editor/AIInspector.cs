using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIPhaseController))]  
public class AIInspector : Editor
{
    private GUIStyle _darkBoxStyle;
    public override void OnInspectorGUI()
    {
        if (_darkBoxStyle == null)
        {
            // try EditorStyles.helpBox first, fall back to the built-in skin
            var baseStyle = EditorStyles.helpBox
                         ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("HelpBox");
            _darkBoxStyle = new GUIStyle(baseStyle);
            _darkBoxStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 1f));
        }

        serializedObject.Update();
        var prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                EditorGUILayout.PropertyField(prop, true);
            }
            while (prop.NextVisible(false));
        }
        serializedObject.ApplyModifiedProperties();

        var mono = (AIPhaseController)target;

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(_darkBoxStyle);
        EditorGUILayout.LabelField("▶ AI Stats Overview", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▸ State Data Details", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current State", mono.currentState.ToString());
        EditorGUILayout.LabelField("Previous State", mono.previousState.ToString());

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▸ Stat Data Details", EditorStyles.boldLabel);

        if (mono.StatsController != null)
        {
            EditorGUILayout.LabelField("Current Mood", mono.StatsController.mood.ToString());
            var statData = mono.StatsController.AIStats;
            if (statData != null)
            {
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                foreach (var field in statData.GetType().GetFields(flags))
                {
                    // only show public or [SerializeField] fields
                    if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                    {
                        var val = field.GetValue(statData);
                        // If it’s a list, show count + each element
                        if (val is IList list)
                        {
                            EditorGUILayout.LabelField(field.Name, $"Count: {list.Count}");
                            EditorGUI.indentLevel++;
                            for (int i = 0; i < list.Count; i++)
                                EditorGUILayout.LabelField($"[{i}]", list[i]?.ToString() ?? "null");
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUILayout.LabelField(field.Name, val?.ToString() ?? "null");
                        }
                    }
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("StatsController not assigned!", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▸ Work Data Details", EditorStyles.boldLabel);

        if (mono.WorkController != null)
        {
            var workData = mono.WorkController.currentTask;  // adjust to your actual field name
            if (workData != null)
            {
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                foreach (var field in workData.GetType().GetFields(flags))
                {
                    // only show public or [SerializeField] fields
                    if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                    {
                        var val = field.GetValue(workData);
                        EditorGUILayout.LabelField(field.Name, val != null ? val.ToString() : "null");
                    }
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("▸ Pathfind Data Details", EditorStyles.boldLabel);

        if (mono.PathfindingController != null)
        {
            EditorGUILayout.LabelField("Current Node",
                mono.PathfindingController.CurrentPoint?.name ?? "none");
            EditorGUILayout.LabelField("Goal Node",
                mono.PathfindingController.EndPoint?.name ?? "none");
        }

        EditorGUILayout.EndVertical();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
