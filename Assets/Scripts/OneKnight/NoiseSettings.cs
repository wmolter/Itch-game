using UnityEngine;
using System.Collections.Generic;

namespace OneKnight {
    [System.Serializable]
    public struct NoiseLayer {
        public float weight;
        public float bonus;
        public NoiseParams param;
    }
    [System.Serializable]
    public class NoiseSettings : ScriptableObject {

        //public CreateTexture.TextureType type;
        //public CreateTexture.TextureMode mode;
        //public CreateTexture.FunctionType function;
        
        [Header("Noise Settings")]
        [Range(2, 8192)]
        public int resolution = 128;
        public Vector3 offset;
        [Range(1, 8)]
        public int octaves = 3;
        [Range(1, 8)]
        public float lacunarity = 5;
        [Range(0, 1)]
        public float persistence = .3f;
        [Range(.001f, 200f)]
        public float frequency = 1;
        public Gradient coloring = new Gradient();

        [Range(1f, 10f)]
        public float xfreq = 1;
        [Range(1f, 10f)]
        public float yfreq = 1;
        [Range(1f, 10f)]
        public float zfreq = 1;
        public bool xyflip;

        public List<NoiseLayer> layers;

        [Header("Tiling")]
        public Vector3 tileOffset = Vector3.zero;

        [Header("Normals")]
        public float bumpiness = 1f;

        [Header("Circular Modes")]
        [Range(0, 5)]
        public float R = 1;
        [Range(0, 1)]
        public float littleRRatio = .5f;
        [Range(0, 2000)]
        public float coneConstant = 1f;

        public NoiseParams NoiseParams {
            get {
                NoiseParams result = new NoiseParams();
                result.frequency = frequency;
                result.lacunarity = lacunarity;
                result.octaves = octaves;
                result.persistence = persistence;
                return result;
            }
        }

    }
}