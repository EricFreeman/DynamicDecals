using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * The abstract base of all printers. Prints copies of projections given a position & rotation.
*/
    public class Printer : MonoBehaviour
    {
        /**
        * The projections to be printed. Multiple can be queued at once and are printed based on the print method.
        */
        public ProjectionRenderer[] prints = new ProjectionRenderer[1];
        /**
        * The layers associated with each of the projections to be printed. Used when multiple prints are availble and the print method is set to layer. Each print will be printed when it's associated layer is being printed on.
        */
        public LayerMask[] printLayers;
        /**
        * The tags associated with each of the projections to be printed. Used when multiple prints are availble and the print method is set to tag. Each print will be printed when the surface being printed on has the given tag.
        */
        public string[] printTags;
        /**
        * Determines which projections from the queue are printed. Layer, will switch prints based on the layer hit. Random will select a different projection at random each print and print it. All will print all of the projections every print. 
        */
        public PrintSelection printMethod;

        #region Pool
        /**
        * The pool determines which projection pool the printed projections will belong to.
        */
        public ProjectionPool Pool
        {
            get
            {
                if (pool == null) pool = DynamicDecals.System.GetPool(poolID);
                return pool;
            }
            set
            {
                poolID = value.ID;
            }
        }
        private ProjectionPool pool;

        [SerializeField]
        private int poolID = 0;
        #endregion
        #region Properties
        //Parent
        /**
        * The print parent determines where within the scenes heirarchy the printed projections will be placed. Default will place them in the default pooling heriarchy. Surface will parent them to whatever they are printed upon, which can be useful when you want projections to attach and move with the surface they're printer upon.
        */
        public PrintParent parent;

        //Print behaviours
        /**
        * Printing with behaviours allows any behaviours attached to the printed renderer to be transfered to the prints.
        * This allows us to attach behaviours that, say, fade a projection out over time, or destroy a projection once we are 500 units away from it.
        */
        public bool printBehaviours;

        //Destroy on print
        /**
        * If enabled, the printer and the attached gameobject will be destroyed after the printer prints. Useful for single print items like projectiles.
        */
        public bool destroyOnPrint;

        //Frequency
        /**
        * Frequency time restricts how often the printer can print projections (in seconds). ie a value of 0.1f will prevent the printer from printing more than once every 0.1 seconds. 
        */
        public float frequencyTime;
        /**
        * Frequency distance restricts how close a printer can print a projection to its previous print. ie. a value of 0.1f will prevent the printer from printing if it's previous print was within 0.1 units of the print location.
        */
        public float frequencyDistance;

        [SerializeField]
        protected PrinterOverlap[] overlaps = new PrinterOverlap[0];
        #endregion

        private float timeSincePrint = Mathf.Infinity;
        private Vector3 lastPrintPos = Vector3.zero;
        private void Update()
        {
            timeSincePrint += Time.deltaTime;
        }

        /**
        * The simplest method of printing available. Use this when you know exactly where you want to print.
        * Printing is still subject to frequency & interection checks, and any fading or culling specified will still be applied.
        * @param Position The position to print the projection at, in world space.
        * @param Rotation The orientation of the printed projection, in world space.
        * @param Surface The transform the projection will be childed to. Will be ignored unless printer has Print Parent set to surface (Not default).
        */
        public void Print(Vector3 Position, Quaternion Rotation, Transform Surface, int Layer = 0)
        {
            //Projection Check
            if (prints == null || prints.Length < 1)
            {
                Debug.LogError("No Projections to print. Please set at least one projection to print.");
                return;
            }

            //Frequency Check
            if (timeSincePrint >= frequencyTime && Vector3.Distance(Position, lastPrintPos) >= frequencyDistance)
            {
                //Intersection Check
                if (overlaps.Length > 0)
                {
                    for (int i = 0; i < overlaps.Length; i++)
                    {
                        //Get the pool being refered to
                        ProjectionPool pool = DynamicDecals.System.GetPool(overlaps[i].poolId);

                        //Validity & Intersection Check
                        if (pool.ID == overlaps[i].poolId && pool.CheckIntersecting(Position, overlaps[i].intersectionStrength))
                        {
                            //Destroy on print
                            if (destroyOnPrint) Destroy(gameObject);
                            return;
                        }
                    }
                }

                //Print using print method
                switch (printMethod)
                {
                    case PrintSelection.Layer:
                        if (printLayers == null || printLayers.Length == 0)
                        {
                            PrintProjection(prints[0], Position, Rotation, Surface);
                        }
                        else
                        {
                            //If the layer mask contains the hit layer, print the decal associated with it.
                            for (int i = 0; i < printLayers.Length; i++)
                            {
                                if (printLayers[i] == (printLayers[i] | (1 << Layer))) PrintProjection(prints[i], Position, Rotation, Surface);
                            }
                        }
                        break;
                    case PrintSelection.Tag:
                        if (printLayers == null || printLayers.Length == 0)
                        {
                            PrintProjection(prints[0], Position, Rotation, Surface);
                        }
                        else
                        {
                            bool printed = false;
                            //If the surface is of the tag, print the decal associated with it.
                            for (int i = 1; i < printTags.Length; i++)
                            {
                                if (printTags[i] == Surface.tag)
                                {
                                    PrintProjection(prints[i], Position, Rotation, Surface);
                                    printed = true;
                                }
                            }

                            //If the surface has no relevant tag, print the default print.
                            if (!printed)
                            {
                                PrintProjection(prints[0], Position, Rotation, Surface);
                            }
                        }
                        break;
                    case PrintSelection.Random:
                        //Generate an int between one and the prints length
                        int index = Random.Range(0, prints.Length);
                        //Print the projection at that index
                        PrintProjection(prints[index], Position, Rotation, Surface);
                        break;
                    case PrintSelection.All:
                        //Print each projection once
                        foreach (ProjectionRenderer projection in prints)
                        {
                            PrintProjection(projection, Position, Rotation, Surface);
                        }
                        break;
                }

                //Destroy on print
                if (destroyOnPrint) Destroy(gameObject);

                //Cache frequency data
                timeSincePrint = 0;
                lastPrintPos = Position;
            }
        }
        private void PrintProjection(ProjectionRenderer Projection, Vector3 Position, Quaternion Rotation, Transform Surface)
        {
            if (Projection != null)
            {
                ProjectionRenderer proj = Pool.Request(Projection, printBehaviours);

                //Set Transform Data
                proj.transform.position = Position;
                proj.transform.rotation = Rotation;

                //Set Parent
                if (parent == PrintParent.Surface)
                {
                    //Create a sub parent
                    //Fixes Non-Uniform scaling and is generally cleaner
                    Transform subParent = null;
                    foreach (Transform child in Surface)
                    {
                        if (child.name == "Projections") subParent = child;
                    }
                    if (subParent == null)
                    {
                        subParent = new GameObject("Projections").transform;
                        subParent.SetParent(Surface);
                    }
                    proj.transform.SetParent(subParent);
                }
            }
        }
    }
    //Overlap
    [System.Serializable]
    public struct PrinterOverlap
    {
        public int poolId;
        public float intersectionStrength;
    }

    //Parent type selection
    public enum PrintParent { Default, Surface }
    //Print selection method
    public enum PrintSelection { All, Random, Layer, Tag }
}