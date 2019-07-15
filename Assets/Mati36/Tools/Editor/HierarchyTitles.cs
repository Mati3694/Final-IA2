using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class HierarchyTitles
{
    static GUIStyle titleStyle;
    static HierarchyTitles()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawItem;
        if (titleStyle == null)
            titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontStyle = FontStyle.Bold;
    }

    private static void DrawItem(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;
        if (!obj.name.StartsWith("//")) return;

        GUI.Box(selectionRect, GUIContent.none);
        GUI.Label(selectionRect, obj.name.Remove(0, 2), titleStyle);
    }
}
