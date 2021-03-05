using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    public class ModifierManager : MonoBehaviour
    {
        //MultiScene Singleton
        public static bool Initialized
        {
            get { return singleton != null; }
        }
        public static ModifierManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    //Create our system
                    GameObject go = new GameObject("Dynamic Decals");
                    go.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    go.AddComponent<ModifierManager>();
                }
                return singleton;
            }
        }
        private static ModifierManager singleton;

        private void Start()
        {
            //Multi-scene
            if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        }
        private void OnEnable()
        {
            //Singleton
            if (singleton == null) singleton = this;
            else if (singleton != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject, true);
                return;
            }

            //Initiailize lists
            perFrameModifiers = new List<Modifier>();
            tenModifiers = new List<Modifier>();
            oneModifiers = new List<Modifier>();

            //Start co-routines
            StartCoroutine(TenTimesPerSecond());
            StartCoroutine(OncePerSecond());
        }
        private void OnDisable()
        {
            //Stop our coroutines
            StopAllCoroutines();
        }

        //Modifiers
        private List<Modifier> perFrameModifiers;
        private List<Modifier> tenModifiers;
        private List<Modifier> oneModifiers;

        private static List<Modifier> GetModifiers(Frequency p_Frequency)
        {
            switch (p_Frequency)
            {
                case Frequency.PerFrame:
                    return Singleton.perFrameModifiers;
                case Frequency.TenPerSec:
                    return Singleton.tenModifiers;
                case Frequency.OncePerSec:
                    return Singleton.oneModifiers;
            }
            return null;
        }

        public static void Register(Modifier p_Modifier)
        {
            //Grab appropriate modifiers
            List<Modifier> modifiers = GetModifiers(p_Modifier.Frequency);

            //Add new modifier
            if (!modifiers.Contains(p_Modifier))
            {
                modifiers.Add(p_Modifier);
            }
        }
        public static void Deregister(Modifier p_Modifier)
        {
            //Grab appropriate modifiers
            List<Modifier> modifiers = GetModifiers(p_Modifier.Frequency);

            //Remove new modifier
            modifiers.Remove(p_Modifier);
        }

        //Update loops
        private WaitForSeconds tenthOfASecond = new WaitForSeconds(0.1f);
        private WaitForSeconds second = new WaitForSeconds(1);

        private void Update()
        {
            for (int i = 0; i < perFrameModifiers.Count; i++)
            {
                perFrameModifiers[i].Perform();
            }
        }
        private IEnumerator TenTimesPerSecond()
        {
            while (true)
            {
                for (int i = 0; i < tenModifiers.Count; i++)
                {
                    tenModifiers[i].Perform();
                }
                yield return tenthOfASecond;
            }
        }
        private IEnumerator OncePerSecond()
        {
            while (true)
            {
                for (int i = 0; i < oneModifiers.Count; i++)
                {
                    oneModifiers[i].Perform();
                }
                yield return second;
            }
        }
    }
}