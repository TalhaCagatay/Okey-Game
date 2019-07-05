using System;
using System.Collections;
using System.Collections.Generic;

namespace TÇI
{
    /// <summary>
    /// This class is responsible for shuffling stones
    /// </summary>
    public static class ShuffleStones<T>
    {
        // A Function to generate a 
        // random permutation of arr[] 
        public static void Shuffle(T[] arr, int n)
        {
            // Creating a object 
            // for Random class 
            Random r = new Random();

            // Start from the last element and 
            // swap one by one. We don't need to 
            // run for the first element  
            // that's why i > 0 
            for (int i = n - 1; i > 0; i--)
            {
                // Pick a random index 
                // from 0 to i 
                int j = r.Next(0, i + 1);

                // Swap arr[i] with the 
                // element at random index 
                T temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
            // Prints the random array 
            for (int i = 0; i < n; i++)
                Console.Write(arr[i] + " ");
        }
    }
}
