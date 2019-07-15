using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Mati36.CustomIcons
{
    public class CustomIconsWelcomeWindow : ScriptableWizard
    {
        //WINDOW
        static CustomIconsWelcomeWindow w;

        //STYLES
        static GUIStyle titleStyle, normalStyle;

        //IMAGES
        static Texture folderSprite;
        Rect imageRect;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += CheckFirstUse;
        }

        private static void CheckFirstUse()
        {
            EditorApplication.update -= CheckFirstUse;
            if (!CustomIconsPref.ConfigPathIsSet)
            {
                string defaultPath = CustomIconsDrawer.TryFindDefaultDatabase();
                if (defaultPath != "")
                    CustomIconsPref.SaveConfigPath(defaultPath);
                CreateWindow();
            }
            else
            {
                if (!CustomIconsDrawer.FindDatabaseReference())
                    EditorUtility.DisplayDialog("Custom Icons Error", "No se encuentra el archivo de configuración, ¿movió las carpetas del plugin CustomIcons?\n" +
                        "Configure nuevamente la ruta de configuración en Edit->Preferences->CustomIcons", "Ok");
            }
        }
        
        [MenuItem("Test/Custom Icons Welcome")]
        public static void CreateWindow()
        {
            w = DisplayWizard<CustomIconsWelcomeWindow>("¡Bienvenido a CustomIcons!");
            w.Show();
            w.position = new Rect(400, 200, 400, 300);
            w.maxSize = new Vector2(400, 300);
            w.minSize = new Vector2(400, 300);

            CreateStyles();
        }

        private static void CreateStyles()
        {
            titleStyle = new GUIStyle();
            titleStyle.fontSize = 14;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.padding = new RectOffset(10, 10, 20, 20);
            titleStyle.wordWrap = true;

            normalStyle = new GUIStyle();
            normalStyle.fontSize = 10;
            normalStyle.padding = titleStyle.padding;
            normalStyle.wordWrap = true;

            folderSprite = (Texture)Resources.Load("unityHelpIcon");
        }

        private void OnGUI()
        {
            if (titleStyle == null) CreateStyles();
            if (w == null) w = this;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            GUILayout.Label("¡Bienvenido!\nGracias por usar la herramienta CustomIcons", titleStyle);
            GUILayout.EndHorizontal();

            if (!CustomIconsPref.ConfigPathIsSet)
            {
                GUILayout.Label("Para comenzar a usar la herramienta, abra el menu Edit->Preferences->CustomIcons y elija un archivo de configuración, o cree uno nuevo.", normalStyle);
                GUILayout.Label("Una vez que tenga el archivo de configuración, simplemente haga Alt+Clic en cualquier carpeta que desee para setearle un icono", normalStyle);
            }
            else
                GUILayout.Label("Para comenzar a usar la herramienta, simplemente haga Alt+Clic en cualquier carpeta que desee para setearle un icono", normalStyle);

            imageRect = GUILayoutUtility.GetRect(100, 100);
            GUI.DrawTexture(imageRect, folderSprite, ScaleMode.ScaleToFit);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Entendido", GUILayout.ExpandWidth(true)))
                w.Close();

            GUILayout.EndVertical();
        }
    }
}