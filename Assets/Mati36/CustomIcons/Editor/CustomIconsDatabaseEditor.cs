using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Mati36.CustomIcons
{
    [CustomEditor(typeof(CustomIconsDatabase))]
    public class CustomIconsDatabaseEditor : Editor
    {
        //PROPERTIES
        SerializedProperty dataList;

        //STYLES
        GUIStyle titleStyle, guidStyle, assetNameStyle, buttonStyle;

        private void OnEnable()
        {
            dataList = serializedObject.FindProperty("database");
        }

        public override void OnInspectorGUI()
        {
            if (buttonStyle == null) CreateStyles();

            GUILayout.Box("", GUILayout.Height(50), GUILayout.ExpandWidth(true));
            GUI.Label(GUILayoutUtility.GetLastRect(), "Icons Database", titleStyle);
            GUILayout.Space(20);

            GUILayout.Label("Default Icon", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField(serializedObject.FindProperty("defaultIcon"));
            GUILayout.Space(20);

            if (GUILayout.Button("Clear Database"))
            {
                if (EditorUtility.DisplayDialog("Clearing Database", "Are you sure you want to erase all the database?", "Yes", "No"))
                {
                    dataList.ClearArray();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.Space(20);
            GUILayout.BeginVertical();
            for (int i = 0; i < dataList.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(EditorStyles.textArea);
                        GUILayout.BeginHorizontal();
                            GUILayout.Label(dataList.GetArrayElementAtIndex(i).FindPropertyRelative("guid").stringValue, guidStyle);
                            if (GUILayout.Button("X", buttonStyle, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                            {
                                dataList.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                return;
                            }
                        GUILayout.EndHorizontal();
                        GUILayout.Label(dataList.GetArrayElementAtIndex(i).FindPropertyRelative("assetName").stringValue, assetNameStyle);
                   
                    GUILayout.EndVertical();
                    Rect iconRect = GUILayoutUtility.GetRect(50, 50, 50, 50);
                    GUI.DrawTexture(iconRect, (Texture2D)dataList.GetArrayElementAtIndex(i).FindPropertyRelative("icon").objectReferenceValue, ScaleMode.ScaleToFit);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void CreateStyles()
        {
            titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            guidStyle = new GUIStyle();
            guidStyle.fontSize = 10;
            guidStyle.padding = new RectOffset(5, 5, 5, 5);
            guidStyle.fontStyle = FontStyle.Italic;

            assetNameStyle = new GUIStyle();
            assetNameStyle.padding = new RectOffset(5, 5, 5, 5);
            assetNameStyle.fontSize = 14;

            buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fontStyle = FontStyle.Bold;
        }
    }
}