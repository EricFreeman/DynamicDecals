using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Misc
{
    public class DestructableManager : MonoBehaviour
    {
        //Initializable MultiScene Singleton
        private static DestructableManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    GameObject go = new GameObject("Destructable Manager");
                    DontDestroyOnLoad(go);

                    go.AddComponent<DestructableManager>();
                }
                return singleton;
            }
        }
        private static DestructableManager singleton;
        private void Awake()
        {
            if (singleton == null) singleton = this;
            else if (singleton != this)
            {
                Destroy(gameObject);
            }
        }

        //Destructables
        public List<Destructable> Destructables = new List<Destructable>();
        public static void Register(Destructable Destructable)
        {
            Singleton.Destructables.Add(Destructable);
        }
        public static void Deregister(Destructable Destructable)
        {
            singleton.Destructables.Remove(Destructable);
        }

        public static void Restore()
        {
            if (singleton != null)
            {
                foreach (Destructable destructable in singleton.Destructables) destructable.Restore();
            }
        }
    }
}