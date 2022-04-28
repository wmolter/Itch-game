using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    public static class UniformGeneration {

        public static int NextSeed {
            get {
                return Random.Range(int.MinValue, int.MaxValue);
            }
        }

        public static void ResetRandom() {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }
        
        public static T RandomWeightedArray<T>(T[] items, float[] weights, float totalWeight) {
            int index = RandomIndex(weights, totalWeight);
            if(index >= 0) {
                return items[index];
            } else
                return default(T);
        }

        public static List<T> RandomWeightedArrayList<T>(T[] items, float[] weights, int max, float totalWeight) where T : class {
            List<T> gens = new List<T>();
            T obj = RandomWeightedArray(items, weights, totalWeight);
            while(obj != null && gens.Count < max) {
                gens.Add(obj);
                obj = RandomWeightedArray(items, weights, totalWeight);
            }
            return gens;
        }

        public static float TotalWeight(IEnumerable<Weighted> weights) {
            float result = 0;
            foreach(Weighted w in weights) {
                result += w.Weight;
            }
            return result;
        }

        public static Weighted RandomWeighted(IEnumerable<Weighted> weights) {
            return RandomWeighted(weights, TotalWeight(weights));
        }

        public static Weighted RandomWeighted(IEnumerable<Weighted> weights, float totalWeight) {
            float r = Random.value*totalWeight;
            float soFar = 0;
            foreach(Weighted w in weights) {
                soFar += w.Weight;
                if(r < soFar)
                    return w;
            }
            return null;
        }

        public static int RandomIndex(float[] weights, float totalWeight) {
            float r = Random.value*totalWeight;
            int index = weights.Length-1;
            float soFar = 0;
            while(index >= 0) {
                soFar += weights[index];
                if(r < soFar)
                    return index;
                index--;
            }
            return index;
        }

        public static System.Type RandomGenerationType(System.Type[] types, float[] weights, float totalWeight) {
            return RandomWeightedArray(types, weights, totalWeight);
        }

        public static List<T> RollEach<T>(T[] possible, float chance) {
            List<T> rolledItems = null;
            for(int i = 0; i < possible.Length; i++) {
                if(Random.value < chance) {
                    if(rolledItems == null)
                        rolledItems = new List<T>();
                    rolledItems.Add(possible[i]);
                }
            }
            return rolledItems;
        }

        public static List<T> RollEach<T>(T[] possible, float[] chances, float rarityModifier) {
            if(possible.Length != chances.Length)
                throw new UnityException("Possible rolls and weights must be the same length. (Did you mean to use a single weight?  There's a separate method for that.)");
            List<T> rolledItems = null;
            for(int i = 0; i < possible.Length; i++) {
                if(Random.value < chances[i]) {
                    if(rolledItems == null)
                        rolledItems = new List<T>();
                    rolledItems.Add(possible[i]);
                }
            }
            return rolledItems;
        }

    }
}