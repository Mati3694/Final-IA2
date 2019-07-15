using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Mati36.CustomIcons
{
    [InitializeOnLoad]
    public class CustomIconsDrawer
    {
        static CustomIconsDatabase databaseRef;

        static Texture2D _backTex;
        static Texture2D backgroundTex
        {
            get
            {
                if (_backTex == null)
                {
                    Color backgroundColor = EditorGUIUtility.isProSkin
                        ? new Color32(56, 56, 56, 255)
                        : new Color32(194, 194, 194, 255);

                    _backTex = new Texture2D(1, 1);
                    _backTex.SetPixel(0, 0, backgroundColor);
                    _backTex.Apply();

                }
                return _backTex;
            }

            set
            {
                _backTex = value;
            }
        }

        static CustomIconsDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += ItemGUI;
        }

        public static bool FindDatabaseReference()
        {
            databaseRef = GetDatabase();
            return databaseRef != null;
        }

        public static CustomIconsDatabase GetDatabase()
        {
            return (CustomIconsDatabase)AssetDatabase.LoadAssetAtPath(CustomIconsPref.CurrentConfigPath, typeof(CustomIconsDatabase));
        }

        public static string TryFindDefaultDatabase()
        {
            CustomIconsDatabase defaultDb = (CustomIconsDatabase)Resources.Load("DefaultIconsDb");
            if (defaultDb != null)
                return AssetDatabase.GetAssetPath(defaultDb);
            else
                return "";
        }

        private static void ItemGUI(string guid, Rect selectionRect)
        {
            if (guid == "") return;
            Event e = Event.current;
            if (selectionRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (e.modifiers == EventModifiers.Alt)
                    {
                        if (databaseRef == null)
                        {
                            if (!FindDatabaseReference())
                                EditorUtility.DisplayDialog("Custom Icons Error", "No se encontró archivo de configuración de Custom Icons, vaya a Edit->Preferences->CustomIcons y cree o elija uno", "Ok");
                            return;
                        }
                        Vector2 mousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                        OpenConfigWindow(guid, new Vector2(mousePos.x, mousePos.y + 10));
                    }
                }
            }
            if (databaseRef == null) { FindDatabaseReference(); return; }

            int databaseIndex = databaseRef.ContainsGUID(guid);

            if (databaseIndex != -1)
            {
                bool isLarge = IsLargeIcon(selectionRect);
                DrawIcon(selectionRect, isLarge ? databaseRef.GetAssetData(databaseIndex).icon : databaseRef.GetAssetData(databaseIndex).icon_16, isLarge);
            }
            else if (databaseRef.defaultIcon != null)
            {
                var type = File.GetAttributes(AssetDatabase.GUIDToAssetPath(guid));
                if ((type & FileAttributes.Directory) != FileAttributes.Directory) return;
                DrawIcon(selectionRect, databaseRef.defaultIcon, IsLargeIcon(selectionRect));
            }
        }

        private static void DrawIcon(Rect selectionRect, Texture2D icon, bool isLarge)
        {
            Rect iconRect;
            iconRect = selectionRect;
            if (isLarge)
                iconRect.yMax = iconRect.yMin + selectionRect.width;
            else
            {
                iconRect.xMax = selectionRect.xMin + selectionRect.height;

                if (!IsTreeView(selectionRect))
                    iconRect.center += new Vector2(3, 0);
            }

            GUI.DrawTexture(iconRect, backgroundTex);
            GUI.DrawTexture(iconRect, icon);
        }

        [MenuItem("Assets/CustomIcons/Set Asset Icon", priority = 2000)]
        static private void ContextMenu()
        {
            if (databaseRef == null) { FindDatabaseReference(); return; }
            if (Selection.assetGUIDs.Length == 0) { EditorUtility.DisplayDialog("Selection Empty", "You must select a folder", "Ok"); return; }
            string assetPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            Object objSelected = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            var type = File.GetAttributes(assetPath);
            if ((type & FileAttributes.Directory) != FileAttributes.Directory) { EditorUtility.DisplayDialog("Not a folder", "You must select a folder", "Ok"); return; }

            CustomIconsWindow.CreateWindow(new Vector2(600, 600), databaseRef, Selection.assetGUIDs[0], objSelected);
        }

        static private void OpenConfigWindow(string guid, Vector2 position)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object objSelected = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            var type = File.GetAttributes(assetPath);
            if ((type & FileAttributes.Directory) != FileAttributes.Directory) { EditorUtility.DisplayDialog("Not a folder", "You must select a folder", "Ok"); return; }

            CustomIconsWindow.CreateWindow(position, databaseRef, guid, objSelected);
        }

        static private bool IsLargeIcon(Rect r)
        {
            if (r.height < 20)
            {
                return false;
            }
            return true;
        }

        static private bool IsTreeView(Rect r)
        {
            return (r.x - 16) % 14 == 0;
        }
    }
}