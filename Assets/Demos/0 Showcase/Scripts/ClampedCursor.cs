using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class ClampedCursor : MonoBehaviour
    {
        public Transform cursor;
        private Vector3 initialPosition;
        private Vector3 velocity;

        private void Start()
        {
            if (cursor != null)
            {
                initialPosition = cursor.transform.position;
            }
        }
        private void Update()
        {
            if (cursor != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && InsideTransform(transform, hit.point))
                {
                    cursor.transform.position = hit.point + (hit.normal * 0.2f);
                    cursor.transform.rotation = Quaternion.LookRotation(-hit.normal);
                    velocity = Vector3.zero;
                }
                else
                {
                    cursor.transform.position = Vector3.SmoothDamp(cursor.transform.position, initialPosition, ref velocity, 0.1f);
                }
            }
        }

        private bool InsideTransform(Transform transform, Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);

            if (localPoint.x > 0.5f || localPoint.x < -0.5f) return false;
            if (localPoint.y > 0.5f || localPoint.y < -0.5f) return false;
            if (localPoint.z > 0.5f || localPoint.z < -0.5f) return false;

            return true;
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
    }
}