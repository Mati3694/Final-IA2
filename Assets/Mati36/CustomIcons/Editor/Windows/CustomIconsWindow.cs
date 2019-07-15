using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;


namespace Mati36.CustomIcons
{
    public class CustomIconsWindow : ScriptableWizard
    {
        //WINDOW
        static CustomIconsWindow w;
        static CustomIconsDatabase databaseRef;

        //CURRENT DATA
        static private Texture2D selectedTex;
        static private Texture2D selectedTex_16;
        static string selectedGUID;
        static Object selectedObj;
        static int databaseIndex = -1;

        //STYLES
        static GUIStyle titleStyle, guidStyle;

        public static void CreateWindow(Vector2 r, CustomIconsDatabase database, string selectedGUID, Object selectedObj)
        {
            if (w != null) w.Close();

            w = (CustomIconsWindow)DisplayWizard("Custom Asset Icon", typeof(CustomIconsWindow), "");
            CustomIconsWindow.selectedGUID = selectedGUID;
            CustomIconsWindow.selectedObj = selectedObj;

            selectedTex = null;
            selectedTex_16 = null;

            Vector2 size = new Vector2(300, 220);

            Rect correctedRect = new Rect(r.x, r.y, 300, 200);
            correctedRect.y -= correctedRect.size.y + 15;
            correctedRect.x -= 20;
            w.position = correctedRect;
            w.maxSize = size;
            w.minSize = size;

            databaseRef = database;
            databaseIndex = databaseRef.ContainsGUID(selectedGUID);

            if (databaseRef != null && databaseIndex != -1)
                databaseRef.SetName(databaseIndex, selectedObj.name);

            CreateStyles();
        }


        private void OnEnable() //EN CASO DE QUE PIERDA REFS AL RECARGAR
        {
            if (databaseRef == null)
                databaseRef = CustomIconsDrawer.GetDatabase();

            databaseIndex = databaseRef.ContainsGUID(selectedGUID);
            if (databaseIndex != -1)
                databaseRef.SetName(databaseIndex, selectedObj.name);
        }

        private void OnGUI()
        {
            GUILayout.Label("GUID: " + selectedGUID, guidStyle);

            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            GUILayout.Label(selectedObj.name, titleStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUIStyle texStyle = new GUIStyle();
            texStyle.margin = new RectOffset(10, 10, 10, 10);
            Rect r = GUILayoutUtility.GetRect(100, 60, 100, 80, texStyle);

            if (selectedTex == null && databaseIndex != -1)
            {
                selectedTex = databaseRef.GetAssetData(databaseIndex).icon;
                selectedTex_16 = databaseRef.GetAssetData(databaseIndex).icon_16;
            }

            if (selectedTex != null)
                GUI.DrawTexture(r, selectedTex, ScaleMode.ScaleToFit);
            else
                GUI.DrawTexture(r, EditorGUIUtility.whiteTexture, ScaleMode.ScaleToFit);


            GUILayout.BeginVertical(texStyle);

            //Large Icon
            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon");
            selectedTex = (Texture2D)EditorGUILayout.ObjectField(selectedTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();
            //16 Icon
            GUILayout.BeginHorizontal();
            GUILayout.Label("x16 Icon");
            selectedTex_16 = (Texture2D)EditorGUILayout.ObjectField(selectedTex_16, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (selectedTex != null && GUILayout.Button("Apply"))
            {
                if (databaseIndex == -1)
                    databaseRef.AddAssetData(selectedGUID, selectedTex, selectedTex_16 != null ? selectedTex_16 : selectedTex, selectedObj.name);
                else
                    databaseRef.SetIcon(databaseIndex, selectedTex, selectedTex_16 != null ? selectedTex_16 : selectedTex);

                EditorUtility.SetDirty(databaseRef);
                w.Close();
            }

            if (databaseIndex != -1 && GUILayout.Button("Revert To Default"))
            {
                databaseRef.DeleteAssetData(databaseIndex);
                EditorUtility.SetDirty(databaseRef);
                w.Close();
            }
            GUILayout.EndHorizontal();
        }

        static private void CreateStyles()
        {
            titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.padding = new RectOffset(5, 5, 5, 5);

            guidStyle = new GUIStyle();
            guidStyle.fontSize = 10;
            guidStyle.padding = new RectOffset(5, 5, 5, 5);
            guidStyle.fontStyle = FontStyle.Italic;
        }
    }
}