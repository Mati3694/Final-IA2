using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Mati36.CustomIcons
{
    public class CustomIconsPref
    {
        public const string PATHPREF = "CUSTOMFOLDERPATH/";

        static public string configPath;

        [PreferenceItem("Custom Icons")]
        static public void CustomIconsPrefGUI()
        {
            GUILayout.Label("Config file", EditorStyles.boldLabel);

            if (ConfigPathIsSet && configPath != CurrentConfigPath)
                configPath = CurrentConfigPath;

            GUILayout.Label(configPath, EditorStyles.helpBox);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load File"))
            {
                string filePath;

                if (configPath != "")
                {
                    string openAtPath = Application.dataPath.Replace("Assets", "") + configPath;
                    openAtPath = Path.GetDirectoryName(openAtPath);
                    filePath = EditorUtility.OpenFilePanel("Create Custom Icons config file", openAtPath, "asset");
                }
                else
                    filePath = EditorUtility.OpenFilePanel("Create Custom Icons config file", Application.dataPath, "asset");

                if (filePath == "") return;

                string assetPath = "Assets" + filePath.Split(new string[1] { "Assets" }, System.StringSplitOptions.None)[1];

                configPath = assetPath;
                SaveConfigPath(configPath);
                CustomIconsDrawer.FindDatabaseReference();
            }

            if (GUILayout.Button("Create File"))
            {
                string filePath;

                if (configPath != "")
                {
                    string openAtPath = Application.dataPath.Replace("Assets", "") + configPath;
                    openAtPath = Path.GetDirectoryName(openAtPath);
                    filePath = EditorUtility.SaveFilePanelInProject("Create Custom Icons config file", "CustomIconsConfig", "asset", "Select or create a new config file", openAtPath);
                }
                else
                    filePath = EditorUtility.SaveFilePanelInProject("Create Custom Icons config file", "CustomIconsConfig", "asset", "Select or create a new config file", Application.dataPath);

                if (filePath == "") return;

                CustomIconsDatabase file = (CustomIconsDatabase)AssetDatabase.LoadAssetAtPath(filePath, typeof(CustomIconsDatabase));
                if (file == null)
                {
                    file = ScriptableObject.CreateInstance<CustomIconsDatabase>();
                    AssetDatabase.CreateAsset(file, filePath);
                    AssetDatabase.Refresh();
                }

                configPath = filePath;
                SaveConfigPath(configPath);
                CustomIconsDrawer.FindDatabaseReference();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("ERASE PREFS"))
            {
                if (EditorUtility.DisplayDialog("Borrando configuración", "¿Está seguro que desea borrar la configuración de este proyecto? Esta operación es irreversible.", "Sí", "No"))
                {
                    EraseConfigPath();
                    configPath = "";
                }
            }
        }


        static public void SaveConfigPath(string newPath)
        {
            EditorPrefs.SetString(PATHPREF + AppName, newPath);
        }

        static public void EraseConfigPath()
        {
            if (ConfigPathIsSet)
                EditorPrefs.DeleteKey(PATHPREF + AppName);
        }

        static public string CurrentConfigPath
        {
            get { return EditorPrefs.GetString(PATHPREF + AppName); }
        }

        static public bool ConfigPathIsSet
        {
            get { return EditorPrefs.HasKey(PATHPREF + AppName); }
        }

        static private string _appName;
        static public string AppName
        {
            get
            {
                if (_appName == null)
                {
                    string fullPath = Application.dataPath;
                    fullPath = fullPath.Split(new string[1] { "Assets" }, System.StringSplitOptions.None)[0];
                    string[] splitPath = fullPath.Split('/');
                    _appName = splitPath[splitPath.Length - 2];
                }
                return _appName;
            }
            set { _appName = value; }
        }
    }
}