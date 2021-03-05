using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LlockhamIndustries;

namespace LlockhamIndustries.Decals
{    
    //Property group wrappers
    public abstract class PropertyGroupDrawer
    {
        protected static Vector2 Texture = new Vector2(64, 64);
        protected static Vector2 Gradient = new Vector2(16, 52);

        protected ProjectionEditor editor;
        protected GUIContent label;

        public abstract void Initialize();
        public abstract void Terminate();
        public abstract void Mark();

        public abstract void OnGUI(Rect Rect);
        public abstract void OnGUILayout();
    }

    public abstract class ShapeDrawer : PropertyGroupDrawer
    {
        internal ShapeTexturePreview preview;

        protected SerializedProperty texture;
        protected SerializedProperty multiplier;
        protected SerializedProperty transparencyType;
        protected SerializedProperty cutoff;

        public ShapeDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor)
        {
            preview = new ShapeTexturePreview();

            label = Label;
            editor = Editor;

            transparencyType = TransparencyType;
            cutoff = Cutoff;

            texture = PropertyGroup.FindPropertyRelative("texture");
            multiplier = PropertyGroup.FindPropertyRelative("multiplier");
        }

        public override void Terminate()
        {
            preview.Terminate();
        }
        public override void Mark()
        {
            preview.Mark();
        }
    }
    public abstract class AlbedoDrawer : PropertyGroupDrawer
    {
        internal AlbedoTexturePreview preview;

        protected SerializedProperty texture;
        protected SerializedProperty color;
        protected SerializedProperty transparencyType;
        protected SerializedProperty cutoff;

        public AlbedoDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor)
        {
            preview = new AlbedoTexturePreview();

            editor = Editor;
            label = Label;

            transparencyType = TransparencyType;
            cutoff = Cutoff;

            texture = PropertyGroup.FindPropertyRelative("texture");
            color = PropertyGroup.FindPropertyRelative("color");
        }

        public override void Terminate()
        {
            preview.Terminate();
        }
        public override void Mark()
        {
            preview.Mark();
        }
    }
    public abstract class GlossDrawer : PropertyGroupDrawer
    {
        internal GreyScaleTexturePreview preview;

        protected SerializedProperty texture;
        protected SerializedProperty glossiness;

        public GlossDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor)
        {
            preview = new GreyScaleTexturePreview();

            label = Label;
            editor = Editor;

            texture = PropertyGroup.FindPropertyRelative("texture");
            glossiness = PropertyGroup.FindPropertyRelative("glossiness");
        }

        public override void Terminate()
        {
            preview.Terminate();
        }
        public override void Mark()
        {
            preview.Mark();
        }
    }
    public abstract class MetallicDrawer : PropertyGroupDrawer
    {
        internal GreyScaleTexturePreview gloss;
        internal GreyScaleTexturePreview metallic;

        protected SerializedProperty texture;
        protected SerializedProperty metallicity;
        protected SerializedProperty glossiness;

        public MetallicDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor)
        {
            gloss = new GreyScaleTexturePreview();
            metallic = new GreyScaleTexturePreview();

            editor = Editor;
            label = Label;

            texture = PropertyGroup.FindPropertyRelative("texture");
            metallicity = PropertyGroup.FindPropertyRelative("metallicity");
            glossiness = PropertyGroup.FindPropertyRelative("glossiness");
        }

        public override void Terminate()
        {
            gloss.Terminate();
            metallic.Terminate();
        }
        public override void Mark()
        {
            gloss.Mark();
            metallic.Mark();
        }
    }
    public abstract class SpecularDrawer : PropertyGroupDrawer
    {
        internal GreyScaleTexturePreview gloss;
        internal ColorTexturePreview specular;

        protected SerializedProperty texture;
        protected SerializedProperty color;
        protected SerializedProperty glossiness;

        public SpecularDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor)
        {
            gloss = new GreyScaleTexturePreview();
            specular = new ColorTexturePreview();

            editor = Editor;
            label = Label;

            texture = PropertyGroup.FindPropertyRelative("texture");
            color = PropertyGroup.FindPropertyRelative("color");
            glossiness = PropertyGroup.FindPropertyRelative("glossiness");
        }

        public override void Terminate()
        {
            gloss.Terminate();
            specular.Terminate();
        }
        public override void Mark()
        {
            gloss.Mark();
            specular.Mark();
        }
    }
    public abstract class NormalDrawer : PropertyGroupDrawer
    {
        internal NormalTexturePreview preview;

        protected SerializedProperty texture;
        protected SerializedProperty strength;

        public NormalDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor)
        {
            preview = new NormalTexturePreview();

            editor = Editor;
            label = Label;

            texture = PropertyGroup.FindPropertyRelative("texture");
            strength = PropertyGroup.FindPropertyRelative("strength");
        }

        public override void Terminate()
        {
            preview.Terminate();
        }
        public override void Mark()
        {
            preview.Mark();
        }
    }
    public abstract class EmissiveDrawer : PropertyGroupDrawer
    {
        internal EmissiveTexturePreview preview;

        protected SerializedProperty texture;
        protected SerializedProperty color;
        protected SerializedProperty intensity;

        public EmissiveDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor)
        {
            preview = new EmissiveTexturePreview();

            editor = Editor;
            label = Label;

            texture = PropertyGroup.FindPropertyRelative("texture");
            color = PropertyGroup.FindPropertyRelative("color");
            intensity = PropertyGroup.FindPropertyRelative("intensity");
        }

        public override void Terminate()
        {
            preview.Terminate();
        }
        public override void Mark()
        {
            preview.Mark();
        }
    }

    //Texture drawers
    public class ShapeTextureDrawer : ShapeDrawer
    {
        public ShapeTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor) : base(Label, PropertyGroup, TransparencyType, Cutoff, Editor) { }

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(4, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            multiplier.floatValue = EditorGUI.FloatField(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), multiplier.floatValue);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));
            
            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, multiplier, transparencyType, cutoff);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class AlbedoTextureDrawer : AlbedoDrawer
    {
        public AlbedoTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor) : base(Label, PropertyGroup, TransparencyType, Cutoff, Editor){}

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4 , Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);            

            //Properties
            EditorGUI.BeginChangeCheck();

            color.colorValue = EditorGUI.ColorField(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), color.colorValue);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));
            
            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, color, transparencyType, cutoff);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class GlossTextureDrawer : GlossDrawer
    {
        public GlossTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor) : base(Label, PropertyGroup, Editor) { }

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(4, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            glossiness.floatValue = EditorGUI.Slider(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), glossiness.floatValue, 0, 1);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));

            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, TextureChannel.r, glossiness);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class MetallicTextureDrawer : MetallicDrawer
    {
        public MetallicTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor) : base(Label, PropertyGroup, Editor) { }

        public override void Initialize()
        {
            gloss.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
            metallic.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), "Glossiness (A)", LlockhamEditorUtility.MiniLabel);
            glossiness.floatValue = EditorGUI.Slider(new Rect(4, 36, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), glossiness.floatValue, 0, 1);

            EditorGUI.LabelField(new Rect(4, 60, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), "Metallicity (R)", LlockhamEditorUtility.MiniLabel);
            metallicity.floatValue = EditorGUI.Slider(new Rect(4, 74, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), metallicity.floatValue, 0, 1);

            EditorGUI.PropertyField(new Rect(4, 100, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));
            
            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                gloss.Mark();
                metallic.Mark();
            }

            //Preview
            gloss.Update(texture, TextureChannel.a, glossiness);
            metallic.Update(texture, TextureChannel.r, metallicity);

            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), gloss.Texture);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 20 + LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), metallic.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max((LlockhamEditorUtility.TexturePreviewSize * 2) + 20 + LlockhamEditorUtility.Spacing, 124 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class SpecularTextureDrawer : SpecularDrawer
    {
        public SpecularTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor) : base(Label, PropertyGroup, Editor) {}

        public override void Initialize()
        {
            gloss.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
            specular.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), "Glossiness (A)", LlockhamEditorUtility.MiniLabel);
            glossiness.floatValue = EditorGUI.Slider(new Rect(4, 36, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), glossiness.floatValue, 0, 1);

            EditorGUI.LabelField(new Rect(4, 60, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), "Spec Color (RGB)", LlockhamEditorUtility.MiniLabel);
            color.colorValue = EditorGUI.ColorField(new Rect(4, 80, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), color.colorValue);

            EditorGUI.PropertyField(new Rect(4, 100, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));

            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                gloss.Mark();
                specular.Mark();
            }

            //Preview
            gloss.Update(texture, TextureChannel.a, glossiness);
            specular.Update(texture, color);

            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), gloss.Texture);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 20 + LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), specular.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max((LlockhamEditorUtility.TexturePreviewSize * 2) + 20 + LlockhamEditorUtility.Spacing, 124 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class NormalTextureDrawer : NormalDrawer
    {
        public NormalTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor) : base(Label, PropertyGroup, Editor) {}

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            strength.floatValue = EditorGUI.Slider(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), strength.floatValue, 0, 4);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));
            
            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, strength);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class EmissiveTextureDrawer : EmissiveDrawer
    {
        
        public EmissiveTextureDrawer(GUIContent Label, SerializedProperty PropertyGroup, ProjectionEditor Editor) : base(Label, PropertyGroup, Editor) {}

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Texture.x), Mathf.FloorToInt(Texture.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            intensity.floatValue = EditorGUI.Slider(new Rect(4, 24, (Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12)) * 0.7f, 16), intensity.floatValue, 0, 4);
            color.colorValue = EditorGUI.ColorField(new Rect(4 + (Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12)) * 0.7f, 24, (Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12)) * 0.3f, 16), color.colorValue);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));
            
            
            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, color, intensity);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }

    //Gradient drawers
    public class ShapeGradientDrawer : ShapeDrawer
    {
        public ShapeGradientDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor) : base(Label, PropertyGroup, TransparencyType, Cutoff, Editor) { }

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Gradient.x), Mathf.FloorToInt(Gradient.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(4, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            multiplier.floatValue = EditorGUI.FloatField(new Rect(4, 24, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), multiplier.floatValue);
            EditorGUI.PropertyField(new Rect(4, 44, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), texture, new GUIContent(""));

            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, multiplier, transparencyType, cutoff);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 4), 12, LlockhamEditorUtility.TexturePreviewSize, LlockhamEditorUtility.TexturePreviewSize), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }
    public class AlbedoGradientDrawer : AlbedoDrawer
    {
        public AlbedoGradientDrawer(GUIContent Label, SerializedProperty PropertyGroup, SerializedProperty TransparencyType, SerializedProperty Cutoff, ProjectionEditor Editor) : base(Label, PropertyGroup, TransparencyType, Cutoff, Editor) { }

        public override void Initialize()
        {
            preview.Initialize(Mathf.FloorToInt(Gradient.x), Mathf.FloorToInt(Gradient.y));
        }
        public override void OnGUI(Rect Rect)
        {
            EditorGUI.DrawRect(new Rect(Rect.x, Rect.y + 2, Rect.width, Rect.height - 6), LlockhamEditorUtility.MidgroundColor);
            GUI.BeginGroup(Rect);

            //Header
            EditorGUI.LabelField(new Rect(0, 4, Rect.width - (LlockhamEditorUtility.TexturePreviewSize + 12), 16), label, EditorStyles.boldLabel);

            //Properties
            EditorGUI.BeginChangeCheck();

            color.colorValue = EditorGUI.ColorField(new Rect(4, 24, (Rect.width / 2) - 12, 16), color.colorValue);
            EditorGUI.PropertyField(new Rect(8 + (Rect.width / 2), 24, (Rect.width / 2) - 12, 16), texture, new GUIContent(""));

            if (EditorGUI.EndChangeCheck())
            {
                editor.Mark();
                preview.Mark();
            }

            //Preview
            preview.Update(texture, color, transparencyType, cutoff);
            EditorGUI.DrawPreviewTexture(new Rect(Rect.width - (LlockhamEditorUtility.GradientPreviewSize + 4), 40, LlockhamEditorUtility.GradientPreviewSize, Rect.height - 44), preview.Texture);

            GUI.EndGroup();
        }
        public override void OnGUILayout()
        {
            Rect Rect = GUILayoutUtility.GetRect(0, Mathf.Max(LlockhamEditorUtility.TexturePreviewSize + 16 + LlockhamEditorUtility.Spacing, 68 + LlockhamEditorUtility.Spacing));
            OnGUI(Rect);
        }
    }

    //Texture preview classes
    internal abstract class TexturePreview
    {
        //Texture Previews
        protected static Color Border = new Color(0.14f, 0.14f, 0.14f, 1);
        protected static int BorderWidth = 3;
        protected static int TextureSize = 64;
        protected static int CheckerSize = 16;

        protected static Color CheckerPrimary = new Color(0.35f, 0.35f, 0.35f, 1);
        protected static Color CheckerSecondary = new Color(0.6f, 0.6f, 0.6f, 1);
        protected static Color CheckerColor(int x, int y)
        {
            if ((x / CheckerSize) % 2 != 0)
            {
                if ((y / CheckerSize) % 2 != 0)
                {
                    return CheckerPrimary;
                }
                else
                {
                    return CheckerSecondary;
                }
            }
            else
            {
                if ((y / CheckerSize) % 2 != 0)
                {
                    return CheckerSecondary;
                }
                else
                {
                    return CheckerPrimary;
                }
            }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }
        protected Texture2D texture;

        public void Mark()
        {
            marked = true;
        }
        protected bool marked = true;

        public void Initialize(int Width, int Height)
        {
            if (texture == null) texture = new Texture2D(Width, Height);
        }
        public void Terminate()
        {
            if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
        }
    }

    internal class ShapeTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty ShapeTexture, SerializedProperty Multiplier, SerializedProperty TransparencyType, SerializedProperty Cutoff)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(ShapeTexture);
                float mul = Multiplier.floatValue;

                bool cutout = (TransparencyType.enumValueIndex == 0);
                float cutoff = Cutoff.floatValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            //Alpha
                            float alpha = 0;

                            //Texture & color alpha
                            if (tex != null)
                            {
                                alpha = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x)).a;
                                alpha = alpha * mul;
                            }
                            //Color alpha
                            else
                            {
                                alpha = mul;
                            }

                            //Blend between black and what based on alpha
                            if (cutout) alpha = (alpha < cutoff) ? 0 : 1;
                            texPixel = Color.Lerp(Color.black, Color.white, alpha);
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }
    internal class AlbedoTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty AlbedoTexture, SerializedProperty AlbedoColor, SerializedProperty TransparencyType, SerializedProperty Cutoff)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(AlbedoTexture);
                Color color = AlbedoColor.colorValue;

                bool cutout = (TransparencyType.enumValueIndex == 0);
                float cutoff = Cutoff.floatValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            //Texture
                            if (tex != null)
                            {
                                texPixel = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x));
                                texPixel = texPixel * color;
                            }
                            //Color
                            else
                            {
                                texPixel = color;
                            }

                            //Checker
                            if (cutout) texPixel.a = (texPixel.a < cutoff) ? 0 : 1;
                            texPixel = Color.Lerp(CheckerColor(x, y), texPixel, texPixel.a);
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }
    internal class NormalTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty Texture, SerializedProperty Strength)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(Texture);
                float strength = Strength.floatValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            if (tex != null && strength != 0)
                            {
                                texPixel = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x));
                                //Unpack normals
                                Vector3 pixel = new Vector3((texPixel.r * 2) - 1, (texPixel.g * 2) - 1, (texPixel.b * 2) - 1);
                                //Modify
                                pixel.z /= strength;
                                pixel = pixel.normalized;
                                //Apply back to pixel
                                texPixel.r = (pixel.x + 1) / 2;
                                texPixel.g = (pixel.y + 1) / 2;
                                texPixel.b = (pixel.z + 1) / 2;
                            }
                            else
                            {
                                texPixel = Color.black;
                            }
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }
    internal class EmissiveTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty Texture, SerializedProperty Color, SerializedProperty Intensity)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(Texture);
                Color color = Color.colorValue;
                float intensity = Intensity.floatValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            //Texture
                            if (tex != null)
                            {
                                texPixel = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x));
                                texPixel = texPixel * (color * intensity);
                            }
                            //Color
                            else
                            {
                                texPixel = (color * intensity);
                            }
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }

    internal class ColorTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty Texture, SerializedProperty Color)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(Texture);
                Color color = Color.colorValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            //Texture
                            if (tex != null)
                            {
                                texPixel = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x));
                                texPixel = texPixel * color;
                            }
                            //Color
                            else
                            {
                                texPixel = color;
                            }
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }
    internal class GreyScaleTexturePreview : TexturePreview
    {
        public void Update(SerializedProperty Texture, TextureChannel Channel, SerializedProperty Multiplier)
        {
            if (marked)
            {
                Texture2D tex = LlockhamEditorUtility.TextureFromProperty(Texture);
                float mul = Multiplier.floatValue;

                Color[] pixels = new Color[texture.width * texture.height];
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        //Determine our pixel color
                        Color texPixel;

                        //Border
                        if (x < BorderWidth || x > texture.width - BorderWidth || y < BorderWidth || y > texture.height - BorderWidth)
                        {
                            texPixel = Border;
                        }
                        else
                        {
                            float metallicity = 0;

                            //Texture
                            if (tex != null)
                            {
                                switch (Channel)
                                {
                                    case TextureChannel.r:
                                        metallicity = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x)).r;
                                        break;
                                    case TextureChannel.g:
                                        metallicity = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x)).g;
                                        break;
                                    case TextureChannel.b:
                                        metallicity = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x)).b;
                                        break;
                                    case TextureChannel.a:
                                        metallicity = tex.GetPixel(Mathf.FloorToInt(tex.width / texture.width * y), Mathf.FloorToInt(tex.height / texture.height * x)).a;
                                        break;
                                }
                                
                                metallicity = metallicity * mul;
                            }
                            //Color
                            else
                            {
                                metallicity = mul;
                            }
                            
                            texPixel = Color.Lerp(Color.black, Color.white, metallicity);
                        }

                        //Write to pixel
                        pixels[x * texture.width + y] = texPixel;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                //No longer marked
                marked = false;
            }
        }
    }
    internal enum TextureChannel { r, g, b, a };
}