using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Generation {
    public static class GaussianGeneration {

        public static int NextSeed {
            get {
                return Random.Range(int.MinValue, int.MaxValue);
            }
        }

        public static void ResetRandom() {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        private static float NextGaussian(float mean, float stdDev) {
            float u1 = 1.0f - Random.value; //uniform(0,1] random floats
            float u2 = 1.0f - Random.value;
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                         Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
            float randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        private static float NextPositiveGaussian(float stddev) {
            return System.Math.Abs(NextGaussian(0, stddev));
        }

        public static float ModifiedNextGaussian(float mean, float stdDev, float rarityModifier) {
            return NextGaussian(mean, stdDev*rarityModifier);
        }

        public static float ModifiedNextPositiveGaussian(float stdDev, float rarityModifier) {
            return NextPositiveGaussian(stdDev*rarityModifier);
        }

        public static float BoundedNextGaussian(GaussianBounds bounds, float uniformRange, float rarityModifier) {
            if(uniformRange < 0 || uniformRange > 1) {
                throw new UnityException("Uniform range must be between 0 and 1");
            }

            float unbounded = ModifiedNextGaussian(bounds.mean, bounds.stdDev, rarityModifier);
            if(unbounded < bounds.min) {
                return (float)(Random.value * (bounds.max - bounds.min) * uniformRange + bounds.min);
            }
            if(unbounded > bounds.max) {
                return (float)(bounds.max - Random.value * (bounds.max - bounds.min) * uniformRange);
            }
            return unbounded;
        }

        public static float BoundedNextGaussian(GaussianBounds bounds, float rarityModifier) {
            if(bounds.max < bounds.min)
                throw new UnityException("Invalid bounds: " + bounds.min + ", " + bounds.max);
            float unbounded = ModifiedNextGaussian(bounds.mean, bounds.stdDev, rarityModifier);
            float bounded = unbounded;
            while(bounded < bounds.min || bounded > bounds.max) {
                if(bounded < bounds.min) {
                    bounded = bounds.min + (bounds.min - bounded);
                }
                if(bounded > bounds.max) {
                    bounded = bounds.max + (bounds.max - bounded);
                }
            }
            return bounded;
        }

        public static float BoundedExponentialGaussian(float min, float max, float avg, float expdeviation, float rarityModifier) {
            float expfactor = ModifiedNextGaussian(0, expdeviation, rarityModifier);
            float raw = avg*Mathf.Exp(expfactor);
            return Mathf.Clamp(raw, min, max);

        }

        public static float BoundedExponentialGaussian(GaussianBounds bounds, float rarityModifier) {
            return BoundedExponentialGaussian(bounds.min, bounds.max, bounds.mean, bounds.stdDev, rarityModifier);
        }

        public static T RandomWeightedArray<T>(T[] items, float[] weights, float rarityModifier) {
            int index = RandomWeightedIndex(weights, rarityModifier);
            if(index >= 0) {
                return items[index];
            } else
                return default(T);
        }

        public static List<T> RandomWeightedArrayList<T>(T[] items, float[] weights, int max, float rarityModifier) where T : class {
            List<T> gens = new List<T>();
            T obj = RandomWeightedArray(items, weights, rarityModifier);
            while(obj != null && gens.Count < max) {
                gens.Add(obj);
                obj = RandomWeightedArray(items, weights, rarityModifier);
            }
            return gens;
        }

        public static int RandomWeightedIndex(float[] weights, float rarityModifier) {
            float r = NextPositiveGaussian(rarityModifier);
            int index = 0;
            while(!(index >= weights.Length) && r >= weights[index]) {
                index++;
            }
            if(index > 0) {
                return Random.Range(0, index);

            } else
                return -1;
        }

        public static System.Type RandomGenerationType(System.Type[] types, float[] weights, float rarityModifier) {
            return RandomWeightedArray(types, weights, rarityModifier);
        }
        public static bool GaussianRoll(float weight, float rarityModifier, float postModifier, float postBonus) {
            return GaussianRoll(rarityModifier, postModifier, postBonus) >= weight;
        }

        public static float GaussianRoll(float rarityModifier, float postModifier, float postBonus) {
            return GaussianRoll(rarityModifier)*postModifier + postBonus;
        }

        public static bool GaussianRoll(float weight, float rarityModifier) {
            return GaussianRoll(rarityModifier) >= weight;
        }

        public static float GaussianRoll(float rarityModifier) {
            return ModifiedNextPositiveGaussian(1, rarityModifier);
        }

        public static List<T> RollEach<T>(T[] possible, float weight, float rarityModifier) {
            List<T> rolledItems = null;
            for(int i = 0; i < possible.Length; i++) {
                if(ModifiedNextPositiveGaussian(1, rarityModifier) >= weight) {
                    if(rolledItems == null)
                        rolledItems = new List<T>();
                    rolledItems.Add(possible[i]);
                }
            }
            return rolledItems;
        }

        public static List<T> RollEach<T>(T[] possible, float[] weights, float rarityModifier) {
            if(possible.Length != weights.Length)
                throw new UnityException("Possible rolls and weights must be the same length. (Did you mean to use a single weight?  There's a separate method for that.)");
            List<T> rolledItems = null;
            for(int i = 0; i < possible.Length; i++) {
                if(ModifiedNextPositiveGaussian(1, rarityModifier) >= weights[i]) {
                    if(rolledItems == null)
                        rolledItems = new List<T>();
                    rolledItems.Add(possible[i]);
                }
            }
            return rolledItems;
        }

    }
}