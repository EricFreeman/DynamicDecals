using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LlockhamIndustries.ExtensionMethods;

namespace LlockhamIndustries.Decals
{
    /**
    * In-built projection pooling class. Use ProjectionPool.GetPool() to get a reference to a pool instance or use ProjectionPool.Default to get a reference to the default pool.
    * You can the request projections from the pool as you see fit. Once you are done with them, instead of deleting them, use the Return method to return them back to the pool.
    */
    public class ProjectionPool
    {
        //Pool Details
        private PoolInstance instance;

        public string Title
        {
            get { return instance.title; }
        }
        private int Limit
        {
            get
            {
                return instance.limits[QualitySettings.GetQualityLevel()];
            }
        }
        public int ID
        {
            get { return instance.id; }
        }

        //Parent
        internal Transform Parent
        {
            get
            {
                if (parent == null)
                {
                    //Generate multiscene gameObject
                    GameObject gameObject = new GameObject(instance.title + " Pool");
                    GameObject.DontDestroyOnLoad(gameObject);

                    //Cache transform
                    parent = gameObject.transform;
                }
                return parent;
            }
        }
        private Transform parent;

        //Pool
        internal List<PoolItem> activePool;
        internal List<PoolItem> inactivePool;

        //Constructor
        public ProjectionPool(PoolInstance Instance)
        {
            instance = Instance;
        }

        //Get Pool
        /**
         * Returns a pool with the specified name, if it exists. If it doesn't, returns the default pool.
         * @param Title The title of the pool to be returned.
         */
        public static ProjectionPool GetPool(string Title)
        {
            return DynamicDecals.System.GetPool(Title);
        }
        /**
         * Returns a pool with the specified ID, if it exists. If it doesn't, returns the default pool.
         * @param ID The ID of the pool to be returned.
         */
        public static ProjectionPool GetPool(int ID)
        {
            return DynamicDecals.System.GetPool(ID);
        }

        //Check Intersecting
        /**
        * Checks to see if a point is intersecting with any of the projections in the pool.
        * Returns true if an intersecting projection is found, otherwise returns false.
        * @param Point The type of projection being requested.
        * @param intersectionStrength How far within the bounds of the projection the point must be before it's considered an intersection. 0 will consider a point anywhere within a projections bounds as an intersections. 1 will only a point as intersecting if it is perfectly at the center of a projections bounds.
        */
        public bool CheckIntersecting(Vector3 Point, float intersectionStrength)
        {
            if (activePool != null && activePool.Count > 0)
            {
                for (int i = activePool.Count - 1; i >= 0; i--)
                {
                    if (activePool[i].Renderer != null)
                    {
                        if (activePool[i].Renderer.CheckIntersecting(Point) > intersectionStrength) return true;
                    }
                    else activePool.RemoveAt(i);
                }
            }
            return false;
        }

        //Request
        /**
        * Returns a projection of the specified type from the pool.
        * Projection will be enabled and ready to use. Use the return method once your done with it, do not delete it.
        * @param Renderer Optional - The renderer to copy from. In 90% of use cases this should be a prefab.
        * @param IncludeScripts Optional - Should the renderer being copied have it's scripts copied as well?
        */
        public ProjectionRenderer Request(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            //Request Renderers until we get a valid
            ProjectionRenderer pr = null;
            while (pr == null) pr = RequestRenderer(Renderer, IncludeBehaviours);
            return pr;
        }
        private ProjectionRenderer RequestRenderer(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            //Initialize active pool if required
            if (activePool == null) activePool = new List<PoolItem>();

            if (inactivePool != null && inactivePool.Count > 0)
            {
                //Grab first item in inactive pool
                PoolItem item = inactivePool[0];

                //Remove from inactive pool
                inactivePool.RemoveAt(0);

                //Add to active pool
                activePool.Add(item);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                return item.Renderer;
            }
            else if (activePool.Count < Limit)
            {
                //Create item
                PoolItem item = new PoolItem(this);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                //Add to active pool
                activePool.Add(item);

                return item.Renderer;
            }
            else
            {
                //Grab oldest item in active pool
                PoolItem item = activePool[0];

                //Terminate item
                item.Terminate();

                //Move to end of pool
                activePool.RemoveAt(0);
                activePool.Add(item);

                //Initialize item
                item.Initialize(Renderer, IncludeBehaviours);

                return item.Renderer;
            }
        }
    }

    public class PoolItem
    {
        //Pool
        public ProjectionPool Pool
        {
            get { return pool; }
        }
        private ProjectionPool pool;

        //Renderer
        public ProjectionRenderer Renderer
        {
            get { return renderer; }
        }
        private ProjectionRenderer renderer;
        private bool Valid
        {
            get
            {
                //Check if the object still exists
                if (renderer == null)
                {
                    if (pool.activePool != null) pool.activePool.Remove(this);
                    if (pool.inactivePool != null) pool.inactivePool.Remove(this);
                    return false;
                }

                return true;
            }
        }

        //Constructor
        public PoolItem(ProjectionPool Pool)
        {
            //Set Pool
            pool = Pool;

            //Generate GameObject
            GameObject go = new GameObject("Projection");
            go.transform.SetParent(pool.Parent);

            //Disable
            go.SetActive(false);

            //Attach Renderer
            renderer = go.AddComponent<ProjectionRenderer>();
            renderer.PoolItem = this;
        }

        //Intialize / Terminate
        internal void Initialize(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
        {
            if (Valid)
            {
                //Set parent
                renderer.transform.SetParent(pool.Parent);

                //Copy Renderer Properties
                if (Renderer != null)
                {
                    //Copy projection
                    renderer.Projection = Renderer.Projection;

                    //Copy properties
                    renderer.Tiling = Renderer.Tiling;
                    renderer.Offset = Renderer.Offset;

                    renderer.MaskMethod = Renderer.MaskMethod;
                    renderer.MaskLayer1 = Renderer.MaskLayer1;
                    renderer.MaskLayer2 = Renderer.MaskLayer2;
                    renderer.MaskLayer3 = Renderer.MaskLayer3;
                    renderer.MaskLayer4 = Renderer.MaskLayer4;

                    renderer.Properties = Renderer.Properties;

                    if (IncludeBehaviours)
                    {
                        foreach (MonoBehaviour component in Renderer.GetComponents<MonoBehaviour>())
                        {
                            //Don't copy transform and projection renderer components
                            if (component.GetType() == typeof(Transform)) continue;
                            if (component.GetType() == typeof(ProjectionRenderer)) continue;

                            MonoBehaviour comp = renderer.gameObject.AddComponent(component);
                            comp.enabled = component.enabled;
                        }
                    }

                    //Copy scale
                    renderer.transform.localScale = Renderer.transform.localScale;

                    //Copy layer & tag
                    renderer.gameObject.layer = Renderer.gameObject.layer;
                    renderer.gameObject.tag = Renderer.gameObject.tag;
                }
                else
                {
                    //Reset scale
                    renderer.transform.localScale = Vector3.one;
                }

                //Enable
                renderer.gameObject.SetActive(true);
            }
        }
        internal void Terminate()
        {
            if (Valid)
            {
                //Disable
                renderer.gameObject.SetActive(false);

                //Strip unnecessary components
                foreach (Component component in renderer.gameObject.GetComponents<Component>())
                {
                    //Don't remove transform and projection renderer components
                    if (component.GetType() == typeof(Transform)) continue;
                    if (component.GetType() == typeof(ProjectionRenderer)) continue;

                    Component.Destroy(component);
                }

                //Set parent
                renderer.transform.SetParent(pool.Parent);
            }
        }

        //Return
        public void Return()
        {
            //Remove from active pool
            pool.activePool.Remove(this);

            //Terminate
            Terminate();

            //Return projection to inactive pool
            if (pool.inactivePool == null) pool.inactivePool = new List<PoolItem>();
            pool.inactivePool.Add(this);
        }
    }
}