using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneKnight {
    public delegate float NoiseMethod(Vector3 point, float frequency);
    public delegate Vector4 FullNoiseMethod(Vector3 point, float frequency);
    public delegate float TileNoiseMethod(Vector3 point, int frequency, IntVector3 period, Vector3 offset);
    public enum NoiseMethodType {
        Value1D, Value2D, Value3D, Perlin1D, Perlin2D, TilePerlin2D, Perlin3D, White3D, Voronoi3D, TileVoronoi3D, VoronoiCell3D, TileVoronoiCell3D, VoronoiTimesCell3D, VoronoiBorderFast3D, TileVoronoiBorderFast3D, VoronoiBorderAccurate3D, TileVoronoiBorderAccurate3D
    }

    [System.Serializable]
    public struct NoiseParams {
        public NoiseMethodType method;
        [Range(1, 8)]
        public int octaves;
        public float frequency;
        [Range(1, 10)]
        public float lacunarity;
        [Range(0, 1)]
        public float persistence;
    }

    public static class Noise {

        public static NoiseMethod[] valueMethods = {
        Value1D,
        Value2D,
        Value3D
    };

        public static NoiseMethod[] perlinMethods = {
        Perlin1D,
        Perlin2D,
        Perlin3D
    };

        public static NoiseMethod[][] noiseMethods = {
        valueMethods,
        perlinMethods
    };

        private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

        private const int hashMask = 255;

        private static float[] gradients1D = {
        1f, -1f
    };

        private const int gradientMask1D = 1;

        private static Vector2[] gradients2D = {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2(1f, -1f).normalized,
        new Vector2(-1f, -1f).normalized
    };

        private const int gradientMask2D = 7;

        private static Vector3[] gradients3D = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };

        private const int gradientsMask3D = 15;

        private static float sqr2 = Mathf.Sqrt(2);

        public static NoiseMethod GetMethod(NoiseMethodType type) {
            switch(type) {
                case NoiseMethodType.Value1D:
                    return Value1D;
                case NoiseMethodType.Value2D:
                    return Value2D;
                case NoiseMethodType.Value3D:
                    return Value3D;
                case NoiseMethodType.Perlin1D:
                    return Perlin1D;
                case NoiseMethodType.Perlin2D:
                    return Perlin2D;
                case NoiseMethodType.Perlin3D:
                    return Perlin3D;
                case NoiseMethodType.Voronoi3D:
                    return Voronoi3D;
                case NoiseMethodType.VoronoiCell3D:
                    return VoronoiCell3D;
                case NoiseMethodType.VoronoiTimesCell3D:
                    return Voronoi3DTimesCell;
                case NoiseMethodType.VoronoiBorderFast3D:
                    return VoronoiBorder3D;
                case NoiseMethodType.VoronoiBorderAccurate3D:
                    return VoronoiBorderAccurate3D;
                default:
                    return null;
            }
        }

        public static TileNoiseMethod GetTileMethod(NoiseMethodType type) {
            switch(type) {
                case NoiseMethodType.TilePerlin2D:
                    return PerlinTile2D;
                case NoiseMethodType.TileVoronoi3D:
                    return Voronoi3DTile;
                case NoiseMethodType.TileVoronoiCell3D:
                    return VoronoiCell3DTile;
                case NoiseMethodType.TileVoronoiBorderFast3D:
                    return VoronoiBorder3DTile;
                case NoiseMethodType.TileVoronoiBorderAccurate3D:
                    return VoronoiBorderAccurate3DTile;
                default:
                    return null;
            }
        }

        public static float Value1D(Vector3 point, float frequency) {
            point *= frequency;
            int i0 = Mathf.FloorToInt(point.x);
            float t = point.x - i0;
            i0 &= hashMask;
            int i1 = i0 + 1;

            int h0 = hash[i0];
            int h1 = hash[i1];

            t = Smooth(t);
            return Mathf.Lerp(h0, h1, t) * (1f / hashMask);
        }

        public static float Value2D(Vector3 point, float frequency) {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            float tx = point.x - ix0;
            float ty = point.y - iy0;
            ix0 &= hashMask;
            iy0 &= hashMask;
            int ix1 = ix0+1;
            int iy1 = iy0+1;
            int h0 = hash[ix0];
            int h1 = hash[ix1];
            int h00 = hash[h0 + iy0];
            int h10 = hash[h1 + iy0];
            int h01 = hash[h0 + iy1];
            int h11 = hash[h1 + iy1];

            tx = Smooth(tx);
            ty = Smooth(ty);

            return Mathf.Lerp(
                Mathf.Lerp(h00, h10, tx),
                Mathf.Lerp(h01, h11, tx),
                ty) * 1f/hashMask;
        }

        public static float Value3D(Vector3 point, float frequency) {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            int iz0 = Mathf.FloorToInt(point.z);
            float tx = point.x - ix0;
            float ty = point.y - iy0;
            float tz = point.z - iz0;
            ix0 &= hashMask;
            iy0 &= hashMask;
            iz0 &= hashMask;
            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;
            int iz1 = iz0 + 1;

            int h0 = hash[ix0];
            int h1 = hash[ix1];
            int h00 = hash[h0 + iy0];
            int h10 = hash[h1 + iy0];
            int h01 = hash[h0 + iy1];
            int h11 = hash[h1 + iy1];
            int h000 = hash[h00 + iz0];
            int h100 = hash[h10 + iz0];
            int h010 = hash[h01 + iz0];
            int h110 = hash[h11 + iz0];
            int h001 = hash[h00 + iz1];
            int h101 = hash[h10 + iz1];
            int h011 = hash[h01 + iz1];
            int h111 = hash[h11 + iz1];

            tx = Smooth(tx);
            ty = Smooth(ty);
            tz = Smooth(tz);
            return Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(h000, h100, tx), Mathf.Lerp(h010, h110, tx), ty),
                Mathf.Lerp(Mathf.Lerp(h001, h101, tx), Mathf.Lerp(h011, h111, tx), ty),
                tz) * (1f / hashMask);
        }

        private static float Smooth(float t) {
            return t*t*t*(t*(t*6f - 15f) + 10f);
        }

        public static float Perlin1D(Vector3 point, float frequency) {
            point *= frequency;
            int i0 = Mathf.FloorToInt(point.x);
            float t0 = point.x - i0;
            float t1 = t0 - 1f;
            i0 &= hashMask;
            int i1 = i0 + 1;

            float g0 = gradients1D[hash[i0]  & gradientMask1D];
            float g1 = gradients1D[hash[i1] & gradientMask1D];

            float v0 = g0*t0;
            float v1 = g1*t1;

            float t = Smooth(t0);
            return Mathf.Lerp(v0, v1, t) * 2f;
        }

        public static float Perlin2D(Vector3 point, float frequency) {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            float tx0 = point.x - ix0;
            float ty0 = point.y - iy0;
            float tx1 = tx0 - 1f;
            float ty1 = ty0 - 1f;
            ix0 &= hashMask;
            iy0 &= hashMask;
            int ix1 = ix0+1;
            int iy1 = iy0+1;

            int h0 = hash[ix0];
            int h1 = hash[ix1];
            Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientMask2D];
            Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientMask2D];
            Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientMask2D];
            Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientMask2D];

            float v00 = Dot(g00, tx0, ty0);
            float v10 = Dot(g10, tx1, ty0);
            float v01 = Dot(g01, tx0, ty1);
            float v11 = Dot(g11, tx1, ty1);

            float tx = Smooth(tx0);
            float ty = Smooth(ty0);

            return Mathf.Lerp(
                Mathf.Lerp(v00, v10, tx),
                Mathf.Lerp(v01, v11, tx),
                ty) * sqr2;
        }

        public static float PerlinTile2D(Vector3 point, int frequency, IntVector3 period, Vector3 offset) {
            point *= frequency;
            period *= frequency;

            float x = ((point.x % period.x) + period.x) % period.x;
            float y = ((point.y % period.y) + period.y) % period.y;

            int ix1 = Mathf.FloorToInt((x+1) % period.x);
            int iy1 = Mathf.FloorToInt((y+1) % period.y);
            point = new Vector2(x, y);
            int ix0 = Mathf.FloorToInt(x);
            int iy0 = Mathf.FloorToInt(y);
            float tx0 = x - ix0;
            float ty0 = y - iy0;
            float tx1 = tx0 - 1f;
            float ty1 = ty0 - 1f;
            ix0 += (int)offset.x;
            iy0 += (int)offset.y;
            ix1 += (int)offset.x;
            iy1 += (int)offset.y;
            ix0 &= hashMask;
            iy0 &= hashMask;
            ix1 &= hashMask;
            iy1 &= hashMask;
            int h0 = hash[ix0];
            int h1 = hash[ix1];
            Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientMask2D];
            Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientMask2D];
            Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientMask2D];
            Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientMask2D];
            float v00 = Dot(g00, tx0, ty0);
            float v10 = Dot(g10, tx1, ty0);
            float v01 = Dot(g01, tx0, ty1);
            float v11 = Dot(g11, tx1, ty1);

            float tx = Smooth(tx0);
            float ty = Smooth(ty0);

            return Mathf.Lerp(
                Mathf.Lerp(v00, v10, tx),
                Mathf.Lerp(v01, v11, tx),
                ty) * sqr2;
        }

        private static float Dot(Vector2 g, float x, float y) {
            return g.x * x + g.y * y;
        }

        private static float Dot(Vector3 g, float x, float y, float z) {
            return g.x * x + g.y * y + g.z * z;
        }

        public static float Perlin3D(Vector3 point, float frequency) {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            int iz0 = Mathf.FloorToInt(point.z);
            float tx0 = point.x - ix0;
            float ty0 = point.y - iy0;
            float tz0 = point.z - iz0;
            float tx1 = tx0 - 1f;
            float ty1 = ty0 - 1f;
            float tz1 = tz0 - 1f;
            ix0 &= hashMask;
            iy0 &= hashMask;
            iz0 &= hashMask;
            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;
            int iz1 = iz0 + 1;

            int h0 = hash[ix0];
            int h1 = hash[ix1];
            int h00 = hash[h0 + iy0];
            int h10 = hash[h1 + iy0];
            int h01 = hash[h0 + iy1];
            int h11 = hash[h1 + iy1];
            Vector3 g000 = gradients3D[hash[h00 + iz0] & gradientsMask3D];
            Vector3 g100 = gradients3D[hash[h10 + iz0] & gradientsMask3D];
            Vector3 g010 = gradients3D[hash[h01 + iz0] & gradientsMask3D];
            Vector3 g110 = gradients3D[hash[h11 + iz0] & gradientsMask3D];
            Vector3 g001 = gradients3D[hash[h00 + iz1] & gradientsMask3D];
            Vector3 g101 = gradients3D[hash[h10 + iz1] & gradientsMask3D];
            Vector3 g011 = gradients3D[hash[h01 + iz1] & gradientsMask3D];
            Vector3 g111 = gradients3D[hash[h11 + iz1] & gradientsMask3D];

            float v000 = Dot(g000, tx0, ty0, tz0);
            float v100 = Dot(g100, tx1, ty0, tz0);
            float v010 = Dot(g010, tx0, ty1, tz0);
            float v110 = Dot(g110, tx1, ty1, tz0);
            float v001 = Dot(g001, tx0, ty0, tz1);
            float v101 = Dot(g101, tx1, ty0, tz1);
            float v011 = Dot(g011, tx0, ty1, tz1);
            float v111 = Dot(g111, tx1, ty1, tz1);

            float tx = Smooth(tx0);
            float ty = Smooth(ty0);
            float tz = Smooth(tz0);
            return Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(v000, v100, tx), Mathf.Lerp(v010, v110, tx), ty),
                Mathf.Lerp(Mathf.Lerp(v001, v101, tx), Mathf.Lerp(v011, v111, tx), ty),
                tz);
        }

        static Vector3 whiteNoiseA = new Vector3(72.987f, -12.11f, 2.23445f);
        static Vector3 whiteNoiseB = new Vector3(11.35832f, 93.56274f, 72.22f);
        static Vector3 whiteNoiseC = new Vector3(128, 0.7949f, 64.293f);
        static float bias = 5.2734f;
        static float sinFreq = 23.291427f;
        const float whiteNoiseConstant = -33.0734f;
        public static float WhiteNoise3D(Vector3 point) {
            return WhiteNoise3D(point, whiteNoiseA);
        }

        public static float WhiteNoise3D(Vector3 point, Vector3 dotWith) {
            IntVector3 floored = IntVector3.Floor(point);
            float value = Vector3.Dot(floored, dotWith) + bias;
            value = Mathf.Sin(sinFreq*value)*whiteNoiseConstant;
            value = value - Mathf.Floor(value);
            return value;
        }

        public static Vector3 WhiteNoise3Dto3D(Vector3 point) {
            return new Vector3(WhiteNoise3D(point, whiteNoiseA), WhiteNoise3D(point, whiteNoiseB), WhiteNoise3D(point, whiteNoiseC));
        }

        public static float Voronoi3D(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition =  cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist)
                            minDist = dist;
                    }
                }
            }
            return 2*minDist-1;
        }

        private static IntVector3 CellCoords(Vector3 point, IntVector3 period, Vector3 offset) {
            return IntVector3.Floor(offset + ((point % period) + period) % period);
        }

        public static float Voronoi3DTile(Vector3 point, int frequency, IntVector3 period, Vector3 offset) {
            point *= frequency;
            period *= frequency;
            IntVector3 floored = CellCoords(point, period, offset);
            float minDist = 10;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        IntVector3 cell = floored + new IntVector3(x, y, z);
                        // tile the neighboring cell position, but calculate distance as though it's next to you
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(CellCoords(point+new IntVector3(x, y, z), period, offset));
                        Vector3 toCell = cellPosition - (offset + ((point % period) + period) % period);
                        float dist = toCell.magnitude;
                        if(dist < minDist)
                            minDist = dist;
                    }
                }
            }
            return 2*minDist-1;
        }

        public static float VoronoiCell3D(Vector3 point, float frequency) {
            Vector3 minCell = VoronoiCellHelper(point, frequency);
            return 2*WhiteNoise3D(minCell) - 1;
        }

        private static Vector3 VoronoiCellHelper(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = cell;
                        }
                    }
                }
            }
            return minCell;
        }

        public static float VoronoiCell3DTile(Vector3 point, int frequency, IntVector3 period, Vector3 offset) {
            point *= frequency;
            period *= frequency;
            IntVector3 floored = CellCoords(point, period, offset);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(CellCoords(point+new IntVector3(x, y, z), period, offset));
                        Vector3 toCell = cellPosition - (offset + ((point % period) + period) % period);
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = CellCoords(cell, period, offset);
                        }
                    }
                }
            }
            return 2*WhiteNoise3D(minCell) - 1;
        }

        public static float VoronoiBorder3D(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            float secondMin = 10;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            secondMin = minDist;
                            minDist = dist;
                        }
                    }
                }
            }
            return 2*(secondMin - minDist) - 1;
        }

        public static float VoronoiBorder3DTile(Vector3 point, int frequency, IntVector3 period, Vector3 offset) {
            point *= frequency;
            period *= frequency;
            IntVector3 floored = CellCoords(point, period, offset);
            float minDist = 10;
            float secondMin = 10;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(CellCoords(point+new IntVector3(x, y, z), period, offset));
                        Vector3 toCell = cellPosition - (offset + ((point % period) + period) % period);
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            secondMin = minDist;
                            minDist = dist;
                        }
                    }
                }
            }
            return 2*(secondMin - minDist) - 1;
        }

        public static float VoronoiBorderAccurate3D(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            Vector3 toMinCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = cell;
                            toMinCell = toCell;
                        }
                    }
                }
            }

            minDist = 10;

            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        
                        if(cell != minCell) {
                            //vector between the two cell centers, halved
                            Vector3 toCenter = (toMinCell + toCell)*.5f;
                            Vector3 cellDiff = Vector3.Normalize(toCell - toMinCell);
                            float edgeDist = Vector3.Dot(toCenter, cellDiff);
                            minDist = Mathf.Min(minDist, edgeDist);
                        }
                    }
                }
            }
            return 2*minDist - 1;
        }

        public static float VoronoiBorderAccurate3DTile(Vector3 point, int frequency, IntVector3 period, Vector3 offset) {
            point *= frequency;
            period *= frequency;
            IntVector3 floored = CellCoords(point, period, offset);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            Vector3 toMinCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(CellCoords(point+new IntVector3(x, y, z), period, offset));
                        Vector3 toCell = cellPosition - (offset + ((point % period) + period) % period);
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = cell;
                            toMinCell = toCell;
                        }
                    }
                }
            }

            minDist = 10;

            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(CellCoords(point+new IntVector3(x, y, z), period, offset));
                        Vector3 toCell = cellPosition - (offset + ((point % period) + period) % period);

                        if(cell != minCell) {
                            //vector between the two cell centers, halved
                            Vector3 toCenter = (toMinCell + toCell)*.5f;
                            Vector3 cellDiff = Vector3.Normalize(toCell - toMinCell);
                            float edgeDist = Vector3.Dot(toCenter, cellDiff);
                            minDist = Mathf.Min(minDist, edgeDist);
                        }
                    }
                }
            }
            return 2*minDist - 1;
        }

        public static Vector4 Voronoi3DAndCell(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            Vector3 toMinCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = cell;
                            toMinCell = toCell;
                        }
                    }
                }
            }
            return 2 * new Vector4(minDist, WhiteNoise3D(minCell), .5f, .5f) - Vector4.one;
        }

        public static Vector4 Voronoi3DAll(Vector3 point, float frequency) {
            point *= frequency;
            IntVector3 floored = IntVector3.Floor(point);
            float minDist = 10;
            Vector3 minCell = Vector3.zero;
            Vector3 toMinCell = Vector3.zero;
            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;
                        float dist = toCell.magnitude;
                        if(dist < minDist) {
                            minDist = dist;
                            minCell = cell;
                            toMinCell = toCell;
                        }
                    }
                }
            }

            float borderDist = 10;

            for(int x = -1; x < 2; x++) {
                for(int y = -1; y < 2; y++) {
                    for(int z = -1; z < 2; z++) {
                        Vector3 cell = floored + new Vector3(x, y, z);
                        Vector3 cellPosition = cell + WhiteNoise3Dto3D(cell);
                        Vector3 toCell = cellPosition - point;

                        if(cell != minCell) {
                            //vector between the two cell centers, halved
                            Vector3 toCenter = (toMinCell + toCell)*.5f;
                            Vector3 cellDiff = Vector3.Normalize(toCell - toMinCell);
                            float edgeDist = Vector3.Dot(toCenter, cellDiff);
                            borderDist = Mathf.Min(borderDist, edgeDist);
                        }
                    }
                }
            }
            return 2 * new Vector4(minDist, WhiteNoise3D(minCell), borderDist, .5f) - Vector4.one;
        }


        public static float Voronoi3DTimesCell(Vector3 point, float frequency) {
            Vector4 result = Voronoi3DAndCell(point, frequency);
            return (1-(result.x+1)/2)*result.y;
        }

        public static float Sum(NoiseMethod method, Vector3 point, float frequency, int octaves, float lacunarity, float persistence) {
            float value = 0;
            float amplitude = 1f;
            float range = 0;
            for(int i = 0; i < octaves; i++) {
                range += amplitude;
                value += method(point, frequency) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }
            return value / range;
        }

        public static float Sum(NoiseMethod method, Vector3 point, NoiseParams input) {
            return Sum(method, point, input.frequency, input.octaves, input.lacunarity, input.persistence);
        }

        public static float Sum(Vector3 point, NoiseParams input) {
            return Sum(GetMethod(input.method), point, input.frequency, input.octaves, input.lacunarity, input.persistence);
        }

        public static float SumTile(TileNoiseMethod method, Vector3 point, NoiseParams input, IntVector3 period, Vector3 offset) {
            float value = 0;
            float amplitude = 1f;
            float range = 0;
            float frequency = input.frequency;

            for(int i = 0; i < input.octaves; i++) {
                range += amplitude;
                value += method(point, (int)frequency, period, offset) * amplitude;
                amplitude *= input.persistence;
                frequency *= input.lacunarity;
            }
            return value / range;
        }

        public static float SumTile(Vector3 point, NoiseParams input, IntVector3 period, Vector3 offset) {
            return SumTile(GetTileMethod(input.method), point, input, period, offset);
        }

        public static Vector4 SumAll(FullNoiseMethod method, Vector3 point, float frequency, int octaves, float lacunarity, float persistence) {
            Vector4 value = Vector4.zero;
            float amplitude = 1f;
            float range = 0;
            for(int i = 0; i < octaves; i++) {
                range += amplitude;
                value += method(point, frequency) * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }
            return value / range;
        }

        public static Vector4 SumAll(FullNoiseMethod method, Vector3 point, NoiseParams input) {
            return SumAll(method, point, input.frequency, input.octaves, input.lacunarity, input.persistence);
        }

        public static float CrossFade(NoiseMethod method, Vector3 point1, Vector3 point2, float t, float frequency, int octaves, float lacunarity, float persistence) {
            float a = Sum(method, point1, frequency, octaves, lacunarity, persistence);
            float b = Sum(method, point2, frequency, octaves, lacunarity, persistence);
            return Mathf.Lerp(a, b, t);
        }

        public static float VoronoiCellLayer(Vector3 point, float vorFreq, NoiseParams param) {
            Vector3 toUse = VoronoiCellHelper(point, vorFreq);
            return Sum(toUse, param);
        }


        public static float LayerNoiseSample(Vector3 point, List<NoiseLayer> layers) {
            float val = 0;
            foreach(NoiseLayer layer in layers) {
                val += layer.weight*(Sum(point, layer.param)+layer.bonus);
            }
            return val;
        }

        public static float TileLayerNoiseSample(Vector3 point, List<NoiseLayer> layers, Vector3 tileOffset) {
            float val = 0;
            foreach(NoiseLayer layer in layers) {
                val += layer.weight*(SumTile(point, layer.param, IntVector3.one, tileOffset)+layer.bonus);
            }
            return val;
        }
    }
}
