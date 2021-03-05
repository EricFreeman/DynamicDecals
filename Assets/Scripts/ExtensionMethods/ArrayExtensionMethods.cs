using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class ArrayExtensionMethods
    {
        public static T[] Insert<T>(this T[] Array, T Item, int Index)
        {
            //Make sure we have a valid item to insert
            if (Item != null)
            {
                if (Array != null)
                {
                    //Clamp index to possible values
                    Index = Mathf.Clamp(Index, 0, Array.Length);

                    //Copy old array
                    T[] oldItems = Array;
                    Array = new T[Array.Length + 1];

                    //Create new array with item inserted
                    int j = 0;
                    for (int i = 0; i < Array.Length; i++)
                    {
                        if (i != Index)
                        {
                            Array[i] = oldItems[j];
                            j++;
                        }
                        else Array[i] = Item;
                    }
                }
                else Array = new T[] { Item };
            }
            return Array;
        }
        public static T[] Add<T>(this T[] Array, T Item)
        {
            if (Item != null)
            {
                if (Array != null) Array = Array.Insert(Item, Array.Length);
                else Array = new T[] { Item };
            }
            return Array;
        }

        public static bool Contains<T>(this T[] Array, T Item)
        {
            if (Array != null && Item != null && Array.Length > 0)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (Array[i].Equals(Item)) return true;
                }
            }
            return false;
        }

        public static T[] Remove<T>(this T[] Array, T Item)
        {
            if (Array != null && Item != null && Array.Length > 0)
            {
                T[] ShrunkArray = new T[Array.Length - 1];
                bool removed = false;

                for (int i = 0; i < Array.Length; i++)
                {
                    if (!removed && Array[i] != null && Array[i].Equals(Item)) removed = true;
                    else ShrunkArray[(removed) ? i - 1 : i] = Array[i];
                }

                return (removed) ? ShrunkArray : Array;
            }
            return Array;            
        }
        public static T[] RemoveAt<T>(this T[] Array, int Index)
        {
            if (Array != null && Array.Length > 0)
            {
                if (Index >= 0 && Index < Array.Length)
                {
                    T[] oldItems = Array;
                    Array = new T[Array.Length - 1];

                    int j = 0;
                    for (int i = 0; i < oldItems.Length; i++)
                    {
                        if (i != Index)
                        {
                            Array[j] = oldItems[i];
                            j++;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Index out of Bounds");
                }
            }
            return Array;
        }

        public static T[] Resize<T>(this T[] Array, int Size)
        {
            //Make sure we have a valid item to insert
            if (Array != null)
            {
                //Copy old array
                T[] oldItems = Array;
                Array = new T[Size];

                //Create new array with item inserted
                for (int i = 0; i < Mathf.Min(Array.Length, oldItems.Length); i++)
                {
                    Array[i] = oldItems[i];
                }
            }
            return Array;
        }
    }
}