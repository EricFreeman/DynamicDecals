using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TextureExtensionMethods
{
    private static int borderWidth = 4;
    private static Color border = new Color(0.18f, 0.18f, 0.18f, 1);
    private static int checkerWidth = 20;
    private static Color backgroundOne = new Color(0.35f, 0.35f, 0.35f, 1);
    private static Color backgroundTwo = new Color(0.6f, 0.6f, 0.6f, 1);

    //Generic Preview
    public static Texture2D GetColoredProperty(this Texture2D Texture, SerializedProperty Property, Color Color, bool FillAlpha = true)
    {
        //Grab our texture
        Texture2D Tex = null;
        while (Property.objectReferenceValue != null && Tex == null)
        {
            Tex = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Grab pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color texPixel;
                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    texPixel = border;
                }
                else
                {
                    if (Tex != null)
                    {
                        texPixel = Tex.GetPixel(Mathf.FloorToInt(Tex.width / 100f * y), Mathf.FloorToInt(Tex.height / 100f * x));
                        texPixel = texPixel * Color;
                    }
                    else
                    {
                        texPixel = Color;
                    }
                    if (FillAlpha) texPixel.a = 1;
                    else texPixel = Color.Lerp(CheckerColor(checkerWidth, x, y, backgroundOne, backgroundTwo), texPixel, texPixel.a);
                }
                pixels[x * 100 + y] = texPixel;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
        return Texture;
    }

    //Transparency Preview
    public static Texture2D GetAlphaOcclusion(this Texture2D Texture, SerializedProperty Property, float Multiplier, float Cutout, int Type)
    {
        //Grab our texture
        Texture2D Albedo = null;
        while (Property.objectReferenceValue != null && Albedo == null)
        {
            Albedo = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Grab pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color result;
                float alpha;
                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    result = border;
                }
                else
                {
                    if (Albedo != null)
                    {
                        alpha = Albedo.GetPixel(Mathf.FloorToInt(Albedo.width / 100f * y), Mathf.FloorToInt(Albedo.height / 100f * x)).a;
                        alpha *= Multiplier;
                        alpha = (Type == 0) ? AlphaCutout(alpha, Cutout) : alpha;
                    }
                    else
                    {
                        alpha = AlphaCutout(Multiplier, Cutout);
                    }
                    result = Color.Lerp(CheckerColor(checkerWidth, x, y, backgroundOne, backgroundTwo), Color.white, alpha);
                }
                pixels[x * 100 + y] = result;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
        return Texture;
    }
    public static Texture2D GetShapeOcclusion(this Texture2D Texture, SerializedProperty Property, float Multiplier, float Cutout, int Type)
    {
        //Grab our texture
        Texture2D shapeTexture = null;
        while (Property.objectReferenceValue != null && shapeTexture == null)
        {
            shapeTexture = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Grab pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color result;
                float shape;
                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    result = border;
                }
                else
                {
                    if (shapeTexture != null)
                    {
                        shape = shapeTexture.GetPixel(Mathf.FloorToInt(shapeTexture.width / 100f * y), Mathf.FloorToInt(shapeTexture.height / 100f * x)).r;
                        shape *= Multiplier;
                        shape = (Type == 0) ? AlphaCutout(shape, Cutout) : shape;
                    }
                    else
                    {
                        shape = (Type == 0) ? AlphaCutout(Multiplier, Cutout) : Multiplier;
                    }
                    result = Color.Lerp(CheckerColor(checkerWidth, x, y, backgroundOne, backgroundTwo), Color.white, shape);
                }
                pixels[x * 100 + y] = result;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();

        return Texture;
    }

    //Other Previews
    public static Texture2D GetMetallic(this Texture2D Texture, SerializedProperty Property, float Metalicity)
    {
        //Grab our textures
        Texture2D Metalic = null;

        while (Property.objectReferenceValue != null && Metalic == null)
        {
            Metalic = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Calculate pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color result;
                float metalicity;

                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    result = border;
                }
                else
                {
                    //Calculate metalicity
                    if (Metalic != null)
                    {
                        metalicity = Metalic.GetPixel(Mathf.FloorToInt(Metalic.width / 100f * y), Mathf.FloorToInt(Metalic.height / 100f * x)).r;
                        metalicity *= Metalicity;
                    }
                    else
                    {
                        metalicity = Metalicity;
                    }
                    result = new Color(metalicity, metalicity, metalicity);
                    result.a = 1;
                }
                pixels[x * 100 + y] = result;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
        return Texture;
    }
    public static Texture2D GetSmoothness(this Texture2D Texture, SerializedProperty Property, float Smoothness)
    {
        //Grab our texture
        Texture2D Tex = null;
        while (Property.objectReferenceValue != null && Tex == null)
        {
            Tex = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Grab pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color texPixel;
                float smoothness;
                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    texPixel = border;
                }
                else
                {
                    if (Tex != null)
                    {
                        smoothness = Tex.GetPixel(Mathf.FloorToInt(Tex.width / 100f * y), Mathf.FloorToInt(Tex.height / 100f * x)).a;
                        smoothness *= Smoothness;
                    }
                    else
                    {
                        smoothness = Smoothness;
                    }
                    texPixel = new Color(smoothness, smoothness, smoothness, 1);
                }
                pixels[x * 100 + y] = texPixel;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
        return Texture;
    }
    public static Texture2D GetNormal(this Texture2D Texture, SerializedProperty Property, float Strength)
    {
        //Grab our texture
        Texture2D Tex = null;
        while (Property.objectReferenceValue != null && Tex == null)
        {
            Tex = AssetPreview.GetAssetPreview(Property.objectReferenceValue);
        }

        //Grab pixels
        Color[] pixels = new Color[(int)Mathf.Pow(100, 2)];
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Color texPixel;
                if (x < borderWidth || x > 100 - borderWidth || y < borderWidth || y > 100 - borderWidth)
                {
                    texPixel = border;
                }
                else
                {
                    if (Tex != null && Strength != 0)
                    {
                        texPixel = Tex.GetPixel(Mathf.FloorToInt(Tex.width / 100f * y), Mathf.FloorToInt(Tex.height / 100f * x));
                        //Unpack normals
                        Vector3 pixel = new Vector3((texPixel.r * 2) - 1, (texPixel.g * 2) - 1, (texPixel.b * 2) - 1);
                        //Modify
                        pixel.z /= Strength;
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
                    texPixel = Color.Lerp(CheckerColor(checkerWidth, x, y, backgroundOne, backgroundTwo), texPixel, texPixel.a);
                }
                pixels[x * 100 + y] = texPixel;
            }
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
        return Texture;
    }

    private static Color CheckerColor(int checkerWidth, int x, int y, Color primary, Color secondary)
    {
        if ((x / checkerWidth) % 2 != 0)
        {
            if ((y / checkerWidth) % 2 != 0)
            {
                return primary;
            }
            else
            {
                return secondary;
            }
        }
        else
        {
            if ((y / checkerWidth) % 2 != 0)
            {
                return secondary;
            }
            else
            {
                return primary;
            }
        }
    }
    private static float AlphaCutout(float alpha, float cutout)
    {
        return (alpha < cutout) ? 0 : 1;
    }
}
