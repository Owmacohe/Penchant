using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Penchant.Runtime
{
    /// <summary>
    /// A class used to generate seeded random numbers and values
    /// </summary>
    public class SeededRandom
    {
        readonly int MIN_SEED_SIZE = 10;
        
        /// <summary>
        /// The seed used to generate consistent sequences of random numbers
        /// </summary>
        public string Seed
        {
            get => _seed;
            set
            {
                string temp = value;
                if (string.IsNullOrEmpty(temp)) temp = "";
                
                // We need to ensure that the seed is at least 10 characters long
                // If the seed is too short, its outputs are too similar
                _seed = temp.Trim().PadLeft(MIN_SEED_SIZE, '_');
                Reset();
            }
        }

        string _seed;
        
        /// <summary>
        /// The number of times that a random value has been called
        /// </summary>
        int calls;
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="seed">The desired seed to generate random numbers from</param>
        public SeededRandom(string seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// Random seed constructor (assigns a random seed when the object is created)
        /// </summary>
        public SeededRandom()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string seed = "";

            for (int i = 0; i < MIN_SEED_SIZE * 2; i++)
                seed += chars[UnityEngine.Random.Range(0, chars.Length)];

            Seed = seed;
        }
        
        /// <summary>
        /// A random float value between [0..1)
        /// </summary>
        public float RandomValue
        {
            get
            {
                // We use String.GetHashCode to get the unique code for this particular seed
                // We also append the number of calls to the end of the seed so that the resulting value changes each time that a new random value is called
                // The number of calls needs to be multiplied by 1 trillion so that it actually has an impact on the hash code
                // We then divide it by 4.3 billion to get it down to a normalized amount, and add 0.5 to make sure it's not negative
                float randomValue = (_seed + (calls++ * 1_000_000_000_000)).GetHashCode() / 4_300_000_000f + 0.5f;
                return randomValue;
            }
        }
        
        #region RandomRange

        /// <summary>
        /// A random float value between two bounds
        /// </summary>
        /// <param name="min">The minimum bound (inclusive)</param>
        /// <param name="max">The maximum bound (exclusive)</param>
        public float RandomRange(float min, float max) => min + (RandomValue * (max - min));
        
        /// <summary>
        /// A random float value between two bounds
        /// </summary>
        /// <param name="min">The minimum bound (inclusive)</param>
        /// <param name="max">The maximum bound (exclusive)</param>
        public float RandomRange(float min, int max) => RandomRange(min, (float)max);
        
        /// <summary>
        /// A random float value between two bounds
        /// </summary>
        /// <param name="min">The minimum bound (inclusive)</param>
        /// <param name="max">The maximum bound (exclusive)</param>
        public float RandomRange(int min, float max) => RandomRange((float)min, max);
        
        /// <summary>
        /// A random integer value between two bounds
        /// </summary>
        /// <param name="min">The minimum bound (inclusive)</param>
        /// <param name="max">The maximum bound (exclusive)</param>
        public int RandomRange(int min, int max) => (int)Mathf.Floor(RandomRange((float)min, (float)max));
        
        #endregion
        
        #region RandomEntry

        /// <summary>
        /// A random entry from a list
        /// </summary>
        /// <param name="list">The list to get the entry from</param>
        public T RandomEntry<T>(List<T> list) => list[RandomRange(0, list.Count)];
        
        /// <summary>
        /// A random entry from an array
        /// </summary>
        /// <param name="array">The array to get the entry from</param>
        public T RandomEntry<T>(T[] array) => array[RandomRange(0, array.Length)];
        
        /// <summary>
        /// A random entry from a dictionary
        /// </summary>
        /// <param name="dictionary">The dictionary to get the entry from</param>
        public U RandomEntry<T, U>(Dictionary<T, U> dictionary)
        {
            var values = dictionary.Values.ToList();
            return values[RandomRange(0, values.Count)];
        }
        
        /// <summary>
        /// A random entry from a hashtable
        /// </summary>
        /// <param name="hashtable">The hashtable to get the entry from</param>
        public T RandomEntry<T>(Hashtable hashtable)
        {
            var values = hashtable.Values.Cast<T>().ToList();
            return values[RandomRange(0, values.Count)];
        }
        
        /// <summary>
        /// A random entry from a collection
        /// </summary>
        /// <param name="collection">The collection to get the entry from<</param>
        /// <typeparam name="T">The collection type</typeparam>
        /// <typeparam name="U">The entry type</typeparam>
        public U RandomEntry<T, U>(T collection) where T : ICollection<U> =>
            collection.ToArray()[RandomRange(0, collection.Count)];
        
        #endregion
        
        #region RandomVector

        /// <summary>
        /// A random Vector2 value between [0..1), [0..1)
        /// </summary>
        public Vector2 RandomVector2 => new(RandomValue, RandomValue);
        
        /// <summary>
        /// A random Vector3 value between [0..1), [0..1), [0..1)
        /// </summary>
        public Vector3 RandomVector3 => new(RandomValue, RandomValue, RandomValue);

        /// <summary>
        /// A random Quaternion value between [0..1), [0..1), [0..1)
        /// </summary>
        public Quaternion RandomQuaternion => Quaternion.Euler(RandomVector3);
        
        #endregion

        /// <summary>
        /// A tester method to run many iterations upon a seed to determine its upper and lower bounds
        /// Generally used to validate that the system is in fact running correctly and is returning values between [0..1)
        /// </summary>
        /// <param name="iterations"></param>
        public void TestRandom(int iterations = 1_000_000)
        {
            float max = 0;
            float min = 0;

            for (int i = 0; i < iterations; i++)
            {
                var rand = RandomValue;
                
                if (rand > max) max = rand;
                if (rand < min) min = rand;
            }
            
            Debug.Log($"Seed: {_seed} | Iterations: {iterations} | Minimum: {max} | Maximum: {max}");
        }

        /// <summary>
        /// Resets the total number of calls to the random value
        /// Generally used when swapping seeds mid-game
        /// </summary>
        public void Reset()
        {
            calls = 0;
        }
    }
}