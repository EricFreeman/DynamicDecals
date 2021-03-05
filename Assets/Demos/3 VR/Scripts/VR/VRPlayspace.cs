using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.VR
{
    public class VRPlayspace : MonoBehaviour
    {
        public LayerMask obstructions;

        public Vector3 baseDimensions = new Vector3(2, 2, 2);
        public Vector3 nodeDimensions = new Vector3(2.4f, 2.4f, 2.4f);

        public float boundsThickness = 0.1f;
        public Material boundsMaterial;

        //Generic methods
        private void Start()
        {
            GenerateBounds();
        }

        //Positioning
        public Vector3 Position
        {
            set { transform.position = value; }
            get { return transform.position; }
        }
        public Vector3 TrialPosition(Vector3 TargetPosition)
        {
            //Calculate direction and distance
            Vector3 direction = (TargetPosition - transform.position).normalized;
            float distance = Vector3.Distance(TargetPosition, transform.position);

            //Setup box cast properties
            Vector3 currentPosition = transform.position + new Vector3(0, baseDimensions.y / 2, 0);
            //Vector3 targetPosition = TargetPosition + new Vector3(0, baseDimensions.y / 2, 0);
            RaycastHit hit;

            //Boxcast to desired position. If we encounter an obstacle, thats the extent of our movement in that direction
            if (Physics.BoxCast(currentPosition, baseDimensions * 0.4f, direction, out hit, transform.rotation, distance, obstructions))
            {
                distance = hit.distance;
            }

            //Grounded check
            while (distance > 0)
            {
                //Calculate position
                Vector3 position = transform.position + (direction * distance);

                //Test to see if it's grounded
                if (TrialGrounded(position)) return position;

                //Move back towards current position
                distance -= 0.01f;
            }

            //If we can't find a grounded position return current position
            return transform.position;
        }
        private bool TrialGrounded(Vector3 Point)
        {
            //Forward left
            Vector3 forwardLeft = Point + (transform.forward * baseDimensions.z / 2) + (-transform.right * baseDimensions.x / 2) + Vector3.up * baseDimensions.y;
            if (!Physics.Raycast(forwardLeft, Vector3.down, baseDimensions.y + 0.2f, obstructions)) return false;

            //Forward right
            Vector3 forwardRight = Point + (transform.forward * baseDimensions.z / 2) + (transform.right * baseDimensions.x / 2) + Vector3.up * baseDimensions.y;
            if (!Physics.Raycast(forwardRight, Vector3.down, baseDimensions.y + 0.2f, obstructions)) return false;

            //Back left
            Vector3 backLeft = Point + (-transform.forward * baseDimensions.z / 2) + (-transform.right * baseDimensions.x / 2) + Vector3.up * baseDimensions.y;
            if (!Physics.Raycast(backLeft, Vector3.down, baseDimensions.y + 0.2f, obstructions)) return false;

            //Back right
            Vector3 backRight = Point + (-transform.forward * baseDimensions.z / 2) + (transform.right * baseDimensions.x / 2) + Vector3.up * baseDimensions.y;
            if (!Physics.Raycast(backRight, Vector3.down, baseDimensions.y + 0.2f, obstructions)) return false;

            return true;
        }

        //Tracking nodes
        public Vector3 ClampNode(Vector3 Point)
        {
            //Convert node to local space
            Point = transform.InverseTransformPoint(Point);

            //Clamp each axis
            Point.y = Mathf.Clamp(Point.y, 0, nodeDimensions.y);
            Point.x = Mathf.Clamp(Point.x, -nodeDimensions.x / 2, nodeDimensions.x / 2);
            Point.z = Mathf.Clamp(Point.z, -nodeDimensions.z / 2, nodeDimensions.z / 2);

            //Convert back to world space
            Point = transform.TransformPoint(Point);

            return Point;
        }

        //Bounds display
        private void GenerateBounds()
        {
            //Create gameObject
            GameObject bounds = new GameObject("Bounds");
            bounds.transform.SetParent(transform);

            //Add mesh filter / renderer
            MeshFilter filter = bounds.AddComponent<MeshFilter>();
            MeshRenderer renderer = bounds.AddComponent<MeshRenderer>();

            //Mesh renderer
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial = boundsMaterial;

            //Generate mesh
            filter.sharedMesh = GenerateBoundsMesh();

        }
        private Mesh GenerateBoundsMesh()
        {
            float heightOffset = 0.01f;

            //Create new mesh
            Mesh bounds = new Mesh();

            //Setup arrays
            Vector3[] verts = new Vector3[8];
            Vector3[] normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            Vector2[] uvs = new Vector2[8];
            int[] tris = new int[24];

            //Position verts
            verts[0] = new Vector3(baseDimensions.x / 2, heightOffset, baseDimensions.z / 2);
            verts[1] = new Vector3((baseDimensions.x / 2) + boundsThickness, heightOffset, (baseDimensions.z / 2) + boundsThickness);

            verts[2] = new Vector3(-baseDimensions.x / 2, heightOffset, baseDimensions.z / 2);
            verts[3] = new Vector3((-baseDimensions.x / 2) - boundsThickness, heightOffset, (baseDimensions.z / 2) + boundsThickness);

            verts[4] = new Vector3(-baseDimensions.x / 2, heightOffset, -baseDimensions.z / 2);
            verts[5] = new Vector3((-baseDimensions.x / 2) - boundsThickness, heightOffset, (-baseDimensions.z / 2) - boundsThickness);

            verts[6] = new Vector3(baseDimensions.x / 2, heightOffset, -baseDimensions.z / 2);
            verts[7] = new Vector3((baseDimensions.x / 2) + boundsThickness, heightOffset, (-baseDimensions.z / 2) - boundsThickness);

            //Position uvs
            uvs[0] = new Vector2(0, 0);
            uvs[2] = new Vector2(0, 0);
            uvs[4] = new Vector2(0, 0);
            uvs[6] = new Vector2(0, 0);

            uvs[1] = new Vector2(1, 0);
            uvs[3] = new Vector2(1, 0);
            uvs[5] = new Vector2(1, 0);
            uvs[7] = new Vector2(1, 0);

            //Setup tris
            tris[0] = 0;
            tris[1] = 2;
            tris[2] = 1;

            tris[3] = 1;
            tris[4] = 2;
            tris[5] = 3;

            tris[6] = 2;
            tris[7] = 4;
            tris[8] = 3;

            tris[9] = 3;
            tris[10] = 4;
            tris[11] = 5;

            tris[12] = 4;
            tris[13] = 6;
            tris[14] = 5;

            tris[15] = 5;
            tris[16] = 6;
            tris[17] = 7;

            tris[18] = 6;
            tris[19] = 0;
            tris[20] = 7;

            tris[21] = 7;
            tris[22] = 0;
            tris[23] = 1;

            //Apply to mesh
            bounds.vertices = verts;
            bounds.normals = normals;
            bounds.uv = uvs;
            bounds.triangles = tris;

            //Recalculate bounding volume
            bounds.RecalculateBounds();

            return bounds;
        }
    }
}