using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class CollisionSpawner : MonoBehaviour
    {
        public GameObject[] colliders;
        public LayerMask layers;
        public Transform parent;

        void Update()
        {
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layers.value))
            {
                SpawnCollider(hit.point + (Vector3.up * 10));
            }
        }

        int colliderIndex;
        public void SpawnCollider(Vector3 Position)
        {
            if (colliders != null && colliders.Length > 0)
            {
                //Spawn current collider
                if (colliders[colliderIndex] != null)
                {
                    GameObject col = (GameObject)Instantiate(colliders[colliderIndex], Position, Quaternion.identity, parent);
                    col.name = "Collider";

                    col.GetComponent<Rigidbody>().velocity = Vector3.down * 4;
                }
                //Iterate to next collider
                colliderIndex = (colliderIndex < colliders.Length - 1) ? colliderIndex + 1 : 0;
            }
        }
    }
}