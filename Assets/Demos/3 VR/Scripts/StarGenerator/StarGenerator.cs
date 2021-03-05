using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(MeshFilter))]
    public class StarGenerator : MonoBehaviour
    {
        public int seed = 64;
        public int count = 1000;
        public float radius = 500;
        public int octaves = 3;
        public float size = 1;

        private MeshFilter meshfilter;

        public void GenerateQuadStars()
        {
            //Set seed
            Random.InitState(seed);

            //Grab meshfilter
            meshfilter = GetComponent<MeshFilter>();

            //Grab or generate mesh
            if (meshfilter.sharedMesh == null)
            {
                Mesh newMesh = new Mesh();
                newMesh.name = "Stars";
                meshfilter.sharedMesh = newMesh;
            }
            Mesh mesh = meshfilter.sharedMesh;

            //Clear mesh
            mesh.Clear();

            //Setup arrays
            Vector3[] verts = new Vector3[count * 4];
            Vector3[] normals = new Vector3[count * 4];
            Vector2[] uvs = new Vector2[count * 4];
            int[] tris = new int[count * 6];

            //Add stars to arrays
            for (int i = 0; i < count; i++)
            {
                GenerateQuadStar(verts, normals, uvs, tris, i);
            }

            //Apply to mesh
            mesh.vertices = verts;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = tris;

            //Recalulate bounds
            mesh.RecalculateBounds();
        }
        private void GenerateQuadStar(Vector3[] Verts, Vector3[] Normals, Vector2[] UVs, int[] Tris, int Index)
        {
            //Generate direction
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                //Generate size
                float minSize = radius * 0.0006f;
                float intensity = size;
                for (int i = 0; i < octaves; i++)
                {
                    intensity *= Random.Range(0.1f, 1f);
                }
                float starSize = Mathf.Max(intensity, minSize);
                float brightness = Mathf.Clamp01(intensity / minSize);

                //Calculate position & rotation
                Vector3 position = transform.position + direction * radius;
                Quaternion rotation = Quaternion.LookRotation(-direction);

                //Generate our verts
                Verts[Index * 4] = position + (rotation * new Vector3(-1, -1, 0) * starSize);
                UVs[Index * 4] = new Vector2(brightness, 0);
                Normals[Index * 4] = -direction;

                Verts[(Index * 4) + 1] = position + (rotation * new Vector3(-1, 1, 0) * starSize);
                UVs[(Index * 4) + 1] = new Vector2(brightness, 0);
                Normals[(Index * 4) + 1] = -direction;

                Verts[(Index * 4) + 2] = position + (rotation * new Vector3(1, 1, 0) * starSize);
                UVs[(Index * 4) + 2] = new Vector2(brightness, 0);
                Normals[(Index * 4) + 2] = -direction;

                Verts[(Index * 4) + 3] = position + (rotation * new Vector3(1, -1, 0) * starSize);
                UVs[(Index * 4) + 3] = new Vector2(brightness, 0);
                Normals[(Index * 4) + 3] = -direction;

                //Generate our tris
                Tris[Index * 6] = Index * 4;
                Tris[(Index * 6) + 1] = (Index * 4) + 2;
                Tris[(Index * 6) + 2] = (Index * 4) + 1;

                Tris[(Index * 6) + 3] = Index * 4;
                Tris[(Index * 6) + 4] = (Index * 4) + 3;
                Tris[(Index * 6) + 5] = (Index * 4) + 2;
            }
        }
    }
}