using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Rigidbody))]
    public class Destructable : MonoBehaviour
    {
        //Backing Fields
        private Rigidbody rb;
        private Vector3 position;
        private Quaternion rotation;

        public void OnEnable()
        {
            //Grab our rigidbody
            rb = GetComponent<Rigidbody>();

            //Cache destructable state
            position = transform.position;
            rotation = transform.rotation;

            //Register to manager
            Register();
        }
        public void OnDisable()
        {
            Deregister();
        }

        public void Register()
        {
            DestructableManager.Register(this);
        }
        public void Deregister()
        {
            DestructableManager.Deregister(this);
        }

        public void Restore()
        {
            rb.position = position;
            rb.rotation = rotation;
        }
        public void Destroy()
        {
            //Todo
        }
    }
}