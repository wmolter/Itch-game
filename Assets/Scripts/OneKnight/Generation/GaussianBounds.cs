using UnityEngine;
using System.Collections;

namespace OneKnight.Generation {
    public class GaussianBounds {

        public float min = float.MinValue;
        public float max = float.MaxValue;
        public float mean = 0;
        public float stdDev = 1;

        public GaussianBounds() {

        }

        public GaussianBounds(float min, float max, float mean, float stdDev) {
            this.min = min;
            this.max = max;
            this.mean = mean;
            this.stdDev = stdDev;
        }

    }
}