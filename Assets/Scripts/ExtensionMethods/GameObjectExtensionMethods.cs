using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class GameObjectExtensionMethods
    {
        public static T AddComponent<T>(this GameObject GameObject, T Source) where T : MonoBehaviour
        {
            return GameObject.AddComponent<T>().GetCopyOf(Source) as T;
        }
        public static MonoBehaviour AddComponent(this GameObject GameObject, MonoBehaviour Source)
        {
            Type type = Source.GetType();

            #if !UNITY_EDITOR && UNITY_WSA
            bool isSubclass = type.GetTypeInfo().IsSubclassOf(typeof(MonoBehaviour));
            #else
            bool isSubclass = type.IsSubclassOf(typeof(MonoBehaviour));
            #endif

            if (isSubclass)
            {
                return ((MonoBehaviour)GameObject.AddComponent(type)).GetCopyOf(Source);
            }
            else return null;
        }

        public static T GetCopyOf<T>(this MonoBehaviour Monobehaviour, T Source) where T : MonoBehaviour
        {
            //Type check
            Type type = Monobehaviour.GetType();
            if (type != Source.GetType()) return null;

            //Declare Binding Flags
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;
            
            //Iterate through all types until monobehaviour is reached
            while (type != typeof(MonoBehaviour))
            {
                //Apply Fields
                #if !UNITY_EDITOR && UNITY_WSA
                System.Reflection.FieldInfo[] fields = type.GetFields(flags).ToArray();
                #else
                System.Reflection.FieldInfo[] fields = type.GetFields(flags);
                #endif
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    field.SetValue(Monobehaviour, field.GetValue(Source));
                }

                //Move to base class
                #if !UNITY_EDITOR && UNITY_WSA
                type = type.GetTypeInfo().BaseType;
                #else
                type = type.BaseType;
                #endif
                
            }
            return Monobehaviour as T;
        }
    }
}