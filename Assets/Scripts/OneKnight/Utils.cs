using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OneKnight {
    public static class Utils {

        public delegate float FloatGetter();

        public static void Nothing() { }

        public static float Product(float[] fs) {
            float result = 1;
            for(int i = 0; i < fs.Length; i++) {
                result *= fs[i];
            }
            return result;
        }

        public static float Product(List<float> fs) {
            float result = 1;
            for(int i = 0; i < fs.Count; i++) {
                result *= fs[i];
            }
            return result;
        }

        public static int ArgMin(List<float> args) {
            float min = args[0];
            int index = 0;
            for(int i = 1; i < args.Count; i++) {
                if(args[i] < min) {
                    min = args[i];
                    index = i;
                }
            }
            return index;
        }

        public static float Min(List<float> fs) {
            if(fs.Count == 0)
                return float.MaxValue;
            float min = fs[0];
            for(int i = 1; i < fs.Count; i++) {
                min = Mathf.Min(fs[i], min);
            }
            return min;
        }

        public static int ArgMax(List<float> args) {
            float max = args[0];
            int index = 0;
            for(int i = 1; i < args.Count; i++) {
                if(args[i] > max) {
                    max = args[i];
                    index = i;
                }
            }
            return index;
        }

        public static float Max(List<float> fs) {
            if(fs.Count == 0)
                return float.MinValue;
            float max = fs[0];
            for(int i = 1; i < fs.Count; i++) {
                max = Mathf.Max(fs[i], max);
            }
            return max;
        }

        public static float Sum(float[] fs) {
            float result = 0;
            for(int i = 0; i < fs.Length; i++) {
                result += i;
            }
            return result;
        }


        public static float Sum(List<float> fs) {
            float result = 0;
            for(int i = 0; i < fs.Count; i++) {
                result += i;
            }
            return result;
        }

        public static Vector2 Rotate(Vector2 v, float radians) {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(cos*v.x - sin*v.y, sin*v.x + cos*v.y);
        }

        public static float Distance(Ray ray, Vector3 point) {
            return Mathf.Sqrt(SqrDistance(ray, point));
        }

        public static float SqrDistance(Ray ray, Vector3 point) {
            Vector3 toPoint = point - ray.origin;
            Vector3 projectedVector = Vector3.Dot(toPoint, ray.direction) * ray.direction / ray.direction.sqrMagnitude;
            Vector3 projectedPoint = ray.origin + projectedVector;
            return Vector3.SqrMagnitude(point - projectedPoint);
        }


        public static Vector2 PredictAim(Vector2 enemyPos, Vector2 enemyVelocity, Vector2 origin, float projectileSpeed) {
            float x0 = enemyPos.x;
            float y0 = enemyPos.y;
            float x1 = origin.x;
            float y1 = origin.y;
            float vx0 = enemyVelocity.x;
            float vy0 = enemyVelocity.y;
            float v = projectileSpeed;

            float ax = vx0*vx0;
            float ay = vy0*vy0;
            float bx = 2*(x0-x1)*vx0;
            float by = 2*(y0-y1)*vy0;
            float cx = (x0-x1)*(x0-x1);
            float cy = (y0-y1)*(y0-y1);

            float a = (-v*v + ax + ay);
            float b = bx + by;
            float c = cx + cy;

            float t = (-b + Mathf.Sqrt(b*b - 4*a*c))/(2*a);
            if(t < 0)
                t = (-b - Mathf.Sqrt(b*b - 4*a*c))/(2*a);

            float vx1 = vx0 + (x0-x1)/t;
            float vy1 = vy0 + (y0-y1)/t;
            //Debug.Log("Predicted dir: " + vx1 + ", " + vy1 + " with t: " + t + " for " + enemyPos + " going " + enemyVelocity + " from " + origin + " with speed: " + projectileSpeed);
            //if you input NaNs to this, it will become 0s
            return new Vector2(vx1, vy1).normalized;

        }

        public static Vector3 Project(Vector3 vector, Vector3 onto) {
            return Vector3.Dot(vector, onto) * onto / onto.sqrMagnitude;
        }

        public static float PowInterp(float tSoFar, float maxT) {
            return (Mathf.Exp(tSoFar/maxT) - 1) / 1.781828f;
        }

        public static float LNInterp(float tSoFar, float maxT) {
            return Mathf.Log(tSoFar * 1.781828f / maxT + 1);
        }

        public static float RtInterp(float tSoFar, float maxT, float root) {
            return Mathf.Pow(tSoFar/maxT, 1/root);
        }

        public static float QuadInterp(float tSoFar, float maxT, float center) {
            if(center <= 0 || center > 1 || center == .5f) {
                throw new UnityException("Center of quadratic must be between 0 and 1, and not .5");
            }
            float a = 1/(1-2*center);
            float b = -center*center*a;
            float result = a*(tSoFar/maxT-center)*(tSoFar/maxT-center)+b;
            return result;
            //return -3.2f*(tSoFar-.75f)*(tSoFar-.75f)+1.8f;
            //y = -16/5(t-.75)^2+9/5
        }

        public static float LogisticInterp(float tSoFar, float maxT, float steepness) {
            float result = 1/(1+Mathf.Exp(-steepness*(tSoFar - maxT/2)));
            //Debug.Log("result: " + result);
            return result;
        }

        public static bool OnScreen(Vector3 screenPos) {
            return screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z > 0;
        }

        public static IEnumerator WaitOneFrame<T>(UnityAction<T> method, T arg) {
            yield return null;
            method(arg);
        }

        public static int[] TriangleStrip(int vertCount) {
            int count = (vertCount-2)*3;
            int[] tris = new int[count];
            int vi = 0;
            for(int i = 0; i < count; vi++) {
                tris[i++] = vi;
                tris[i++] = vi+1+vi%2;
                tris[i++] = vi+2-vi%2;
            }
            return tris;
        }

        public static int[] TriangleCircle(int vertCount) {
            return TriangleCircle(vertCount);
        }

        public static int[] TriangleCircle(int vertCount, bool bothSides) {
            int count;
            if(vertCount < 3)
                return new int[0];
            if(bothSides)
                count = (vertCount-2)*6;
            else
                count = (vertCount-2)*3;
            int[] tris = new int[count];
            int vi = 0;
            for(int i = 0; i < count; vi++) {
                tris[i++] = 0;
                tris[i++] = vi+1;
                tris[i++] = vi+2;
                if(bothSides) {
                    tris[i++] = 0;
                    tris[i++] = vi+2;
                    tris[i++] = vi+1;
                }
            }
            return tris;
        }

        public static Vector3 Average(IEnumerable<Vector3> verts) {
            Vector3 total = Vector3.zero;
            int count = 0;
            foreach(Vector3 v in verts) {
                total += v;
                count++;
            }
            return total/count;
        }

        public static int ArgMax(params float[] args) {
            float max = args[0];
            int index = 0;
            for(int i = 1; i < args.Length; i++) {
                if(args[i] > max) {
                    max = args[i];
                    index = i;
                }
            }
            return index;
        }

        public static float Max(out int index, params float[] args) {
            float max = args[0];
            index = 0;
            for(int i = 1; i < args.Length; i++) {
                if(args[i] > max) {
                    max = args[i];
                    index = i;
                }
            }
            return max;
        }

        public static int ArgMin(params float[] args) {
            float min = args[0];
            int index = 0;
            for(int i = 1; i < args.Length; i++) {
                if(args[i] < min) {
                    min = args[i];
                    index = i;
                }
            }
            return index;
        }

        public static float Min(out int index, params float[] args) {
            float min = args[0];
            index = 0;
            for(int i = 1; i < args.Length; i++) {
                if(args[i] < min) {
                    min = args[i];
                    index = i;
                }
            }
            return min;
        }

        //Modifies the input collection!
        public static ICollection<T> FilterAdd<T>(ICollection<T> addTo, IEnumerable<T> addFrom, Predicate<T> filter) {
            foreach(T t in addFrom) {
                if(filter(t))
                    addTo.Add(t);
            }
            return addTo;
        }

        public static int Closest(List<Collider2D> colliders, Vector2 pos, Predicate<Collider2D> filter) {
            float dist = Mathf.Infinity;
            int index = -1;
            for(int i = 0; i < colliders.Count; i++) {
                if(filter(colliders[i])) {
                    float thisDist = (colliders[i].ClosestPoint(pos)-pos).sqrMagnitude;
                    if(thisDist < dist) {
                        dist = thisDist;
                        index = i;
                    }
                }
            }
            return index;
        }

        public static IEnumerable<Vector2> UniformRadialPositions(int quantity, float radius) {
            return UniformRadialPositions(quantity, 2*Mathf.PI, radius, radius, 0);
        }

        public static IEnumerable<Vector2> UniformRadialPositions(int quantity, float totalAngle, float minRadius, float maxRadius, float adjustmentRadius) {
            for(int i = 0; i < quantity; i++) {
                float angle = i*totalAngle/quantity;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 newPos = dir* UnityEngine.Random.Range(minRadius, maxRadius);
                newPos += UnityEngine.Random.insideUnitCircle*adjustmentRadius;
                yield return newPos;
            }
        }

        public static Vector3 FromSpherical(Vector3 sphericalCoord) {
            return FromSpherical(sphericalCoord.x, sphericalCoord.y, sphericalCoord.z);
        }

        public static Vector3 FromSpherical(float theta, float rho, float phi) {
            float x = rho*Mathf.Cos(theta)*Mathf.Sin(phi);
            float y = rho*Mathf.Sin(theta)*Mathf.Sin(phi);
            float z = rho*Mathf.Cos(phi);
            return new Vector3(x, y, z);
        }

        public static Vector3 ToSpherical(Vector3 cartesian) {
            return ToSpherical(cartesian.x, cartesian.y, cartesian.z);
        }

        public static Vector3 ToSpherical(float x, float y, float z) {
            float rho = Mathf.Sqrt(x*x + y*y + z*z);
            float theta = (Mathf.Atan2(y, x) + 2*Mathf.PI) % (2*Mathf.PI);
            if(theta < 0)
                Debug.LogWarning("Theta is " + theta + " for " + x + ", " + y + ", " + z);
            float phi = Mathf.Acos(z/rho);

            if(phi > Mathf.PI) {
                phi %= Mathf.PI;
            } else if(phi < 0) {
                phi += Mathf.PI;
            }
            return new Vector3(theta, rho, phi);
        }
        public static Texture2DArray TextureArrayToShader(Material mat, string property, Texture2D[] textures) {
            return TextureArrayToShader(mat, property, new List<Texture2D>(textures), Texture2D.blackTexture);
        }

        public static Texture2DArray TextureArrayToShader(Material mat, string property, List<Texture2D> textures) {
            return TextureArrayToShader(mat, property, textures, Texture2D.blackTexture);
        }

        public static Texture2DArray TextureArrayToShader(Material mat, string property, List<Texture2D> textures, Texture2D fallback) {
            int max = 0;
            int index = -1;
            for(int i = 0; i < textures.Count; i++) {
                if(textures[i] != null && textures[i].width > max) {
                    max = textures[i].width;
                    index = i;
                }
            }
            Texture2DArray array;
            Debug.Log("Chosen texture index for formatting: " + index);
            if(index == -1) {
                array = new Texture2DArray(fallback.width, fallback.height, textures.Count, fallback.format, fallback.mipmapCount > 1);
            } else {
                array = new Texture2DArray(textures[index].width, textures[index].height, textures.Count, textures[index].format, textures[index].mipmapCount > 1);
            }
            return TextureArrayToShader(array, mat, property, textures, fallback);
        }
        public static Texture2DArray TextureArrayToShader(Texture2DArray array, Material mat, string property, Texture2D[] textures, Texture2D fallback) {
            return TextureArrayToShader(array, mat, property, new List<Texture2D>(textures), fallback);
        }

        public static Texture2DArray TextureArrayToShader(Texture2DArray array, Material mat, string property, List<Texture2D> textures, Texture2D fallback) {
            array.wrapMode = TextureWrapMode.Repeat;
            for(int i = 0; i < textures.Count; i++) {
                if(textures[i] == null)
                    Graphics.CopyTexture(fallback, 0, array, i);
                else
                    Graphics.CopyTexture(textures[i], 0, array, i);
            }
            array.Apply();
            mat.SetTexture(property, array);
            return array;
        }
    }
}
