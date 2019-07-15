using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static public class ContextClickSelect
{
    const int MAX_OBJ_FOUND = 30;

    static ContextClickSelect()
    {
        if (EditorApplication.isPlaying) return;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        InitReflection();
    }


    static bool clickDown = false;
    static Vector2 clickDownPos;

    static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        int id = GUIUtility.GetControlID(FocusType.Passive);
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            clickDownPos = e.mousePosition;
            clickDown = true;
        }
        else if (e.type == EventType.MouseUp && e.button == 1 && clickDown)
        {
            clickDown = false;
            if (clickDownPos == e.mousePosition)
            {

                e.Use();
                Debug.Log("ContextClick".Bold()); OpenContextMenu(e.mousePosition, sceneView);
            }
        }
    }

    static void OpenContextMenu(Vector2 pos, SceneView sceneView)
    {
        var invertedPos = new Vector2(pos.x, sceneView.position.height - 16 - pos.y);

        GenericMenu contextMenu = new GenericMenu();
        GameObject obj = null;

        int matIndex;
        List<GameObject> objsFound = new List<GameObject>();
        GameObject[] currArray = null;

        for (int i = 0; i <= MAX_OBJ_FOUND; i++)
        {
            if (objsFound.Count > 0)
            {
                currArray = new GameObject[objsFound.Count];
                for (int j = 0; j < currArray.Length; j++)
                    currArray[j] = objsFound[j];
            }

            obj = PickObjectOnPos(sceneView.camera, ~0, invertedPos, currArray, null, out matIndex);
            if (obj != null)
            {
                var prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                if (prefab != null && prefab != obj)
                    objsFound.Add(prefab);
                objsFound.Add(obj);
            }
            else
                break;
        }

        foreach (var item in objsFound)
            CreateMenuRecu(contextMenu, item, objsFound, "");

        if (objsFound.Count == 0)
            AddMenuItem(contextMenu, "None", null);

        contextMenu.DropDown(new Rect(pos, Vector2.zero));
    }

    static void CreateMenuRecu(GenericMenu menu, GameObject current, List<GameObject> all, string currentPath)
    {
        List<GameObject> allMinusCurrent = new List<GameObject>(all);
        allMinusCurrent.Remove(current);

        for (int i = 0; i < allMinusCurrent.Count; i++)
        {
            if (current.transform.parent == allMinusCurrent[i].transform)
                return;
        }

        Transform child;

        AddMenuItem(menu, currentPath + (IsPrefab(current) ? "• " : "") + current.name, current.transform);
        for (int i = 0; i < current.transform.childCount; i++)
        {
            child = current.transform.GetChild(i);
            if (allMinusCurrent.Contains(child.gameObject))
                CreateMenuRecu(menu, child.gameObject, allMinusCurrent, currentPath + "         ");
        }
    }

    static bool IsPrefab(GameObject obj)
    {
        var prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
        return prefab != null && prefab == obj;
    }

    static GameObject PickObjectOnPos(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex) // PICK A GAMEOBJECT FROM SCENE VIEW AT POSITION
    {
        materialIndex = -1;
        return (GameObject)Internal_PickClosestGO.Invoke(null, new object[] { cam, layers, position, ignore, filter, materialIndex });
    }

    static void AddMenuItem(GenericMenu menu, string menuPath, Transform asset) //ADD ITEM TO MENU
    {
        menu.AddItem(new GUIContent(menuPath), false, OnItemSelected, asset);
    }

    private static void OnItemSelected(object itemSelected) // ON CLICK ITEM ON LIST
    {
        if (itemSelected != null)
            Selection.activeTransform = itemSelected as Transform;
    }

    //REFLECTION
    static private MethodInfo Internal_PickClosestGO;

    static void InitReflection()
    {
        Assembly editorAssembly = typeof(Editor).Assembly;
        System.Type handleUtilityType = editorAssembly.GetType("UnityEditor.HandleUtility");
        Internal_PickClosestGO = handleUtilityType.GetMethod("Internal_PickClosestGO", BindingFlags.Static | BindingFlags.NonPublic);
    }
}
