using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Selector))]
    [RequireComponent(typeof(GenericCameraController))]
    public class SelectionController : MonoBehaviour
    {
        public LayerMask Layers;

        //Backing fields
        private Selector selector;
        private GenericCameraController controller;

        //Generic methods
        private void Awake()
        {
            selector = GetComponent<Selector>();
            controller = GetComponent<GenericCameraController>();
        }
        private void Update()
        {
            if (controller.Camera != null && Input.GetMouseButtonDown(1))
            {
                //Set up ray
                Ray ray = controller.Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //Cast for target position
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Layers.value))
                {
                    CommandSelectables(hit.point);
                }
            }
        }

        private void CommandSelectables(Vector3 Point)
        {
            //Grab selectables
            List<Selectable> selection = selector.Selection;

            if (selection != null)
            {
                //Iterate over selection
                foreach (Selectable selectable in selection)
                {
                    Locomotion locomotion = selectable.GetComponent<Locomotion>();
                    if (locomotion != null)
                    {
                        CommandUnit(locomotion, Point);
                    }
                }
            }
        }
        private void CommandUnit(Locomotion Unit, Vector3 Point)
        {
            //Flatten point
            Point.y = 0;

            //Flatten position
            Vector3 position = Unit.transform.position;
            position.y = 0;

            //Calculate direction
            Vector3 direction = (position - Point).normalized;

            Unit.Movement = direction;
            Unit.Direction = direction;
        }
    }
}