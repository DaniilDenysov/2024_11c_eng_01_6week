using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.List
{
    public static class ListExtensions
    {
        public static void Shuffle<T> (this List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0,i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
