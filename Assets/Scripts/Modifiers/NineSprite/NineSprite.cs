using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * This component allows you to stretch a decal by dividing it into nine different decals (each corner, edge and the center) and stretching the individual components. This is useful for UI elements like borders or box selections.
    */
    [ExecuteInEditMode]
    public class NineSprite : MonoBehaviour
    {
        /**
        * The decal we want to use as a base. Nine copies of this will be made and used to represent your sprite. This should almost always be a prefab.
        */
        public ProjectionRenderer Sprite
        {
            get { return sprite; }
            set
            {
                if (sprite != value)
                {
                    sprite = value;
                    UpdateProperties();
                }
            }
        }

        /**
        * How large each corner / how thick each edge should be in pixels. This will adjust how we sample from the original decal.
        */
        public float BorderPixelSize
        {
            get { return borderPixelSize; }
            set
            {
                if (borderPixelSize != value)
                {
                    borderPixelSize = value;
                    UpdateProperties();
                }
            }
        }
        /**
        * How large each corner / how thick each edge should be in units (world space). This will adjust how we represent the original decal in the world.
        */
        public float BorderWorldSize
        {
            get { return borderWorldSize; }
            set
            {
                if (borderWorldSize != value)
                {
                    borderWorldSize = value;
                    UpdateTransforms();
                }
            }
        }

        //Backing fields
        [SerializeField]
        private ProjectionRenderer sprite;
        [SerializeField]
        private float borderPixelSize = 0.4f;
        [SerializeField]
        private float borderWorldSize = 0.2f;
        [SerializeField]
        private ProjectionRenderer[] spritePieces;

        //Generic methods
        private void OnDestroy()
        {
            ClearSprite();
        }

        //Update methods
        /**
        * Updates all nine decals with updated properties from the original projection renderer. This should be called whenever the original projection renderer is modified and you want
        * these changes to be reflected by the nine sprite.
        */
        public void UpdateProperties()
        {
            for (int i = 0; i < spritePieces.Length; i++)
            {
                //Copy projection from sprite
                spritePieces[i].Projection = sprite.Projection;

                //Adjust tiling / offset
                spritePieces[i].Tiling = Tiling(i);
                spritePieces[i].Offset = Offset(i);

                //Copy properties
                spritePieces[i].MaskMethod = sprite.MaskMethod;
                spritePieces[i].MaskLayer1 = sprite.MaskLayer1;
                spritePieces[i].MaskLayer2 = sprite.MaskLayer2;
                spritePieces[i].MaskLayer3 = sprite.MaskLayer3;
                spritePieces[i].MaskLayer4 = sprite.MaskLayer4;
                spritePieces[i].Properties = sprite.Properties;

                //Apply property changes
                spritePieces[i].UpdateProperties();
            }
        }
        /**
        * Updates all nine decals to account for a change in scale. This should be called when you stretch (scale) the nine-sprite, to have it rescale it's sprite pieces to match the new scale.
        */
        public void UpdateTransforms()
        {
            for (int i = 0; i < spritePieces.Length; i++)
            {
                spritePieces[i].transform.localPosition = LocalPosition(i);
                spritePieces[i].transform.localRotation = Quaternion.identity;
                spritePieces[i].transform.localScale = LocalScale(i);
            }
        }

        //Properties
        private Vector2 Tiling(int Index)
        {
            switch (Index)
            {
                case 0: return new Vector2(borderPixelSize, borderPixelSize);
                case 1: return new Vector2(1 - (2 * borderPixelSize), borderPixelSize);
                case 2: return new Vector2(borderPixelSize, borderPixelSize);
                case 3: return new Vector2(borderPixelSize, 1 - (2 * borderPixelSize));
                case 4: return new Vector2(1 - (2 * borderPixelSize), 1 - (2 * borderPixelSize));
                case 5: return new Vector2(borderPixelSize, 1 - (2 * borderPixelSize));
                case 6: return new Vector2(borderPixelSize, borderPixelSize);
                case 7: return new Vector2(1 - (2 * borderPixelSize), borderPixelSize);
                case 8: return new Vector2(borderPixelSize, borderPixelSize);
            }
            return Vector2.zero;
        }
        private Vector2 Offset(int Index)
        {
            switch (Index)
            {
                case 0: return new Vector2(0, 1 - borderPixelSize);
                case 1: return new Vector2(borderPixelSize, 1 - borderPixelSize);
                case 2: return new Vector2(1 - borderPixelSize, 1 - borderPixelSize);
                case 3: return new Vector2(0, borderPixelSize);
                case 4: return new Vector2(borderPixelSize, borderPixelSize);
                case 5: return new Vector2(1 - borderPixelSize, borderPixelSize);
                case 6: return new Vector2(0, 0);
                case 7: return new Vector2(borderPixelSize, 0);
                case 8: return new Vector2(1 - borderPixelSize, 0);
            }
            return Vector2.zero;
        }

        private Vector3 LocalPosition(int Index)
        {
            float width = borderWorldSize / transform.localScale.x / 2;
            float height = borderWorldSize / transform.localScale.y / 2;

            switch (Index)
            {
                case 0: return new Vector3(-0.5f + width, 0.5f - height, 0);
                case 1: return new Vector3(0, 0.5f - height, 0);
                case 2: return new Vector3(0.5f - width, 0.5f - height, 0);
                case 3: return new Vector3(-0.5f + width, 0, 0);
                case 4: return Vector3.zero;
                case 5: return new Vector3(0.5f - width, 0, 0);
                case 6: return new Vector3(-0.5f + width, -0.5f + height, 0);
                case 7: return new Vector3(0, -0.5f + height, 0);
                case 8: return new Vector3(0.5f - width, -0.5f + height, 0);
            }
            return Vector3.zero;
        }
        private Vector3 LocalScale(int Index)
        {
            float width = borderWorldSize / transform.localScale.x;
            float height = borderWorldSize / transform.localScale.y;

            switch (Index)
            {
                case 0: return new Vector3(width, height, 1);
                case 1: return new Vector3(1 - (2 * width), height, 1);
                case 2: return new Vector3(width, height, 1);
                case 3: return new Vector3(width, 1 - (2 * height), 1);
                case 4: return new Vector3(1 - (2 * width), 1 - (2 * height), 1);
                case 5: return new Vector3(width, 1 - (2 * height), 1);
                case 6: return new Vector3(width, height, 1);
                case 7: return new Vector3(1 - (2 * width), height, 1);
                case 8: return new Vector3(width, height, 1);
            }
            return Vector3.one;
        }

        //Setup
        private void Generate()
        {
            spritePieces = new ProjectionRenderer[9];

            spritePieces[0] = GenerateRenderer("TopLeft");
            spritePieces[1] = GenerateRenderer("TopMiddle");
            spritePieces[2] = GenerateRenderer("TopRight");
            spritePieces[3] = GenerateRenderer("MiddleLeft");
            spritePieces[4] = GenerateRenderer("MiddleMiddle");
            spritePieces[5] = GenerateRenderer("MiddleRight");
            spritePieces[6] = GenerateRenderer("BottomLeft");
            spritePieces[7] = GenerateRenderer("BottomMiddle");
            spritePieces[8] = GenerateRenderer("BottomRight");
        }
        private ProjectionRenderer GenerateRenderer(string Name)
        {
            //Generate new gameObject
            GameObject gameObject = new GameObject(Name);
            gameObject.transform.parent = transform;
            gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            gameObject.SetActive(false);

            //Generate projection renderer
            ProjectionRenderer renderer = gameObject.AddComponent<ProjectionRenderer>();

            //Add ninesprite piece so we know to delete
            gameObject.AddComponent<NineSpritePiece>();

            return renderer;
        }

        //Update
        public void UpdateNineSprite()
        {
            if (sprite != null && sprite.Projection != null)
            {
                if (spritePieces == null || spritePieces.Length != 9)
                {
                    Generate();
                    UpdateProperties();
                    UpdateTransforms();
                    for (int i = 0; i < spritePieces.Length; i++) spritePieces[i].gameObject.SetActive(true);
                }
                else
                {
                    UpdateProperties();
                    UpdateTransforms();
                }
            }
            else ClearSprite();
        }
        private void ClearSprite()
        {
            if (spritePieces != null)
            {
                for (int i = 0; i < spritePieces.Length; i++)
                {
                    DestroyImmediate(spritePieces[i]);
                }

                spritePieces = null;
            }
        }        

        #if UNITY_EDITOR
        //Gizmos
        public void OnDrawGizmos()
        {
            DrawGizmo(false);
        }
        public void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }
        private void DrawGizmo(bool Selected)
        {
            if (isActiveAndEnabled)
            {
                //Decalare color and matrix
                Color color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                Gizmos.matrix = transform.localToWorldMatrix;

                //Draw selection gizmo
                color.a = Selected ? 0.5f : 0.05f;
                Gizmos.color = color;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

        //Duplicate check
        [SerializeField]
        private int instanceID = 0;
        private void Awake()
        {
            if (!Application.isPlaying)
            {
                if (instanceID == 0)
                {
                    instanceID = GetInstanceID();
                }
                else if (instanceID != GetInstanceID())
                {
                    //Duplicate detected
                    instanceID = GetInstanceID();

                    //Grab all immediate children
                    List<Transform> children = new List<Transform>();
                    foreach (Transform child in transform) children.Add(child);

                    //Destroy all immediate child nine sprite pieces
                    for (int i = children.Count - 1; i >= 0; i--)
                    {
                        if (children[i].GetComponent<NineSpritePiece>())
                        {
                            DestroyImmediate(children[i].gameObject);
                        }
                    }

                    //Clear sprite pieces
                    spritePieces = null;

                    //Setup ninesprite
                    UpdateNineSprite();
                }
            }
        }
        #endif
    }
}