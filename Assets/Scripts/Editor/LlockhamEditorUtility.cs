using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LlockhamIndustries
{
    public static class LlockhamEditorUtility
    {
        //Global Editor Properties
        public static int TexturePreviewSize = 52;
        public static int GradientPreviewSize = 104;
        public static float TabHeight = 20;
        public static float Spacing = 6;

        public static Color BackgroundColor
        {
            get { return (EditorGUIUtility.isProSkin) ? new Color(0.15f, 0.15f, 0.15f, 1) : new Color(0.60f, 0.60f, 0.60f, 1); }
        }
        public static Color MidgroundColor
        {
            get { return (EditorGUIUtility.isProSkin) ? new Color(0.18f, 0.18f, 0.18f, 1) : new Color(0.68f, 0.68f, 0.68f, 1); }
        }
        public static Color ForegroundColor
        {
            get { return (EditorGUIUtility.isProSkin) ? new Color(0.22f, 0.22f, 0.22f, 1) : new Color(0.78f, 0.78f, 0.78f, 1); }
        }
        public static Color HeaderColor
        {
            get { return (EditorGUIUtility.isProSkin) ? new Color(0.28f, 0.28f, 0.28f, 1) : new Color(0.82f, 0.82f, 0.82f, 1); }
        }

        public static Color TextColor
        {
            get { return (EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1) : new Color(0.2f, 0.2f, 0.2f, 1); }
        }

        //Global Editor Styles
        public static GUIStyle TabLabel
        {
            get
            {
                if (tabLabel == null)
                {
                    tabLabel = new GUIStyle(EditorStyles.label);
                    tabLabel.alignment = TextAnchor.MiddleCenter;
                }
                return tabLabel;
            }
        }
        private static GUIStyle tabLabel;

        public static GUIStyle MiniTabLabel
        {
            get
            {
                if (miniTabLabel == null)
                {
                    miniTabLabel = new GUIStyle(EditorStyles.miniLabel);
                    miniTabLabel.alignment = TextAnchor.MiddleCenter;
                }
                return miniTabLabel;
            }
        }
        private static GUIStyle miniTabLabel;

        public static GUIStyle MiniLabel
        {
            get
            {
                if (miniLabel == null)
                {
                    miniLabel = new GUIStyle(EditorStyles.miniBoldLabel);
                    miniLabel.alignment = TextAnchor.LowerLeft;
                }
                return miniLabel;
            }
        }
        private static GUIStyle miniLabel;

        public static GUIStyle BoldFoldout
        {
            get
            {
                if (foldout == null) foldout = new GUIStyle(EditorStyles.foldout);
                foldout.fontStyle = FontStyle.Bold;

                return foldout;
            }
        }
        private static GUIStyle foldout;

        //Global Editor Resources
        public static Texture2D RightArrow
        {
            get
            {
                if (rightArrow == null)
                {
                    rightArrow = Resources.Load("Editor_RightArrow") as Texture2D;
                }
                return rightArrow;
            }
        }
        private static Texture2D rightArrow;

        public static Texture2D DownArrow
        {
            get
            {
                if (downArrow == null)
                {
                    downArrow = Resources.Load("Editor_DownArrow") as Texture2D;
                }
                return downArrow;
            }
        }
        private static Texture2D downArrow;

        public static Texture2D Up
        {
            get
            {
                if (up == null)
                {
                    up = Resources.Load("Editor_Up") as Texture2D;
                }
                return rightArrow;
            }
        }
        private static Texture2D up;

        public static Texture2D Down
        {
            get
            {
                if (down == null)
                {
                    down = Resources.Load("Editor_Down") as Texture2D;
                }
                return down;
            }
        }
        private static Texture2D down;

        public static Texture2D Cross
        {
            get
            {
                if (cross == null)
                {
                    cross = Resources.Load("Editor_Cross") as Texture2D;
                }
                return cross;
            }
        }
        private static Texture2D cross;

        public static Texture2D Reset
        {
            get
            {
                if (reset == null)
                {
                    reset = Resources.Load("Editor_Reset") as Texture2D;
                }
                return reset;
            }
        }
        private static Texture2D reset;

        //Texture Extension Methods
        public static Texture2D TextureFromColor(Color Color)
        {
            Texture2D Texture = new Texture2D(1, 1);
            Texture.SetPixel(0, 0, Color);
            Texture.Apply();

            return Texture;
        }
        public static Texture2D TextureFromProperty(SerializedProperty Property)
        {
            Texture2D texture = null;
            if (Property.objectReferenceValue != null)
            {
                //Brute force
                while (texture == null) texture = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
            }
            return texture;
        }

        //String Extension Methods
        public static bool ContainsCaseInsenitive(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public static string AddSpacesToType(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ' && text[i - 1] != '/') newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}

