using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TZ_24Play
{
    public static class Utils
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list) 
        {
            var array = list.ToArray();
            int n = array.Length;

            while(n > 1) 
            {
                n--;
                int k = Random.Range(0, n);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }

            return array;
        }
    }
}

