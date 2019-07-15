using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mati36.CustomGUI
{
    static public class CustomGUIUtility
    {
        static private GUIStyle _headerStyle;
        static public GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle(EditorStyles.helpBox);
                    _headerStyle.padding = new RectOffset(20, 20, 10, 10);
                    _headerStyle.alignment = TextAnchor.MiddleCenter;
                    _headerStyle.fontSize = 20;
                }
                    return _headerStyle;
            }
        }

        static private GUIStyle _miniButtonStyle;
        static public GUIStyle MiniButtonStyle
        {
            get
            {
                if (_miniButtonStyle == null)
                {
                    _miniButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    _miniButtonStyle.fontStyle = FontStyle.Bold;
                }
                return _miniButtonStyle;
            }
        }


        static Color prevColor, prevColorBG;

        static public void BeginGUIColor(Color color)
        {
            prevColor = GUI.color;
            GUI.color = color;
        }

        static public void EndGUIColor()
        {
            if (prevColor == Color.clear) return;
            GUI.color = prevColor;
            prevColor = Color.clear;
        }

        static public void BeginGUIColorBG(Color color)
        {
            prevColorBG = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        static public void EndGUIColorBG()
        {
            if (prevColorBG == Color.clear) return;
            GUI.backgroundColor = prevColorBG;
            prevColorBG = Color.clear;
        }
    }
}