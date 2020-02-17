using UnityEngine;
using System.Collections;
using System;

namespace OneKnight {
    [Serializable]
    public struct IntVector3 {

        public static readonly IntVector3 up = new IntVector3(0, 1, 0);
        public static readonly IntVector3 down = new IntVector3(0, -1, 0);
        public static readonly IntVector3 left = new IntVector3(-1, 0, 0);
        public static readonly IntVector3 right = new IntVector3(1, 0, 0);
        public static readonly IntVector3 forward = new IntVector3(0, 0, 1);
        public static readonly IntVector3 back = new IntVector3(0, 0, -1);


        public static readonly IntVector3 zero = new IntVector3(0, 0, 0);
        public static readonly IntVector3 one = new IntVector3(1, 1, 1);

        public static implicit operator Vector3(IntVector3 v) {
            return new Vector3(v.x, v.y, v.z);
        }

        public static IntVector3 operator %(IntVector3 a, int b) {
            return new IntVector3(a.x % b, a.y % b, a.z % b);
        }

        public static IntVector3 operator %(IntVector3 a, IntVector3 b) {
            return new IntVector3(a.x % b.x, a.y % b.y, a.z % b.z);
        }

        public static Vector3 operator %(Vector3 a, IntVector3 b) {
            return new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);
        }

        public static bool operator ==(IntVector3 a, IntVector3 b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator != (IntVector3 a, IntVector3 b) {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }


        public static bool operator ==(Vector3 a, IntVector3 b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, IntVector3 b) {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }


        public static bool operator ==(IntVector3 a, Vector3 b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(IntVector3 a, Vector3 b) {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static IntVector3 operator +(IntVector3 a, IntVector3 b) {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(IntVector3 a, Vector3 b) {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3 a, IntVector3 b) {
            return b + a;
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b) {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        
        public static IntVector3 operator *(IntVector3 a, int b) {
            return new IntVector3(a.x * b, a.y * b, a.z * b);
        }
        
        public static IntVector3 operator *(int a, IntVector3 b) {
            return b*a;
        }

        public static Vector3 operator *(IntVector3 a, float b) {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator *(float a, IntVector3 b) {
            return b*a;
        }

        public static IntVector3 operator *(IntVector3 a, IntVector3 b) {
            return new IntVector3(a.x*b.x, a.y*b.y, a.z*b.z);
        }

        public static Vector3 operator *(Vector3 a, IntVector3 b) {
            return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
        }

        public static Vector3 operator *(IntVector3 a, Vector3 b) {
            return b*a;
        }

        public static Vector3 operator /(IntVector3 a, IntVector3 b) {
            return new Vector3(a.x*1f/b.x, a.y*1f/b.y, a.z*1f/b.z);
        }


        public static IntVector3 Abs(IntVector3 a) {
            return new IntVector3((int)Mathf.Abs(a.x), (int)Mathf.Abs(a.y), (int)Mathf.Abs(a.z));
        }

        public static IntVector3 Floor(Vector3 a) {
            return new IntVector3(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y), Mathf.FloorToInt(a.z));
        }

        public static implicit operator Vector3Int(IntVector3 a) {
            return new Vector3Int(a.x, a.y, a.z);
        }


        public int x, y, z;
        public Vector3 floatVector { get { return new Vector3(x, y, z); } }
        public float Magnitude {  get { return Mathf.Sqrt(x*x + y*y + z*z); } }

        public IntVector3(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() {
            return "(" + x + ", " + y + ", " + z + ")";
        }

    }
}