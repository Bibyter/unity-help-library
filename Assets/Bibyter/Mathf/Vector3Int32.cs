using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Vector3Int32
    {
        public int x, y, z;

        public Vector3Int32(int v)
        {
            this.x = v; this.y = v; this.z = v;
        }

        public Vector3Int32(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static Vector3Int32 Create(in Vector3Int v)
        {
            return new Vector3Int32() { x = v.x, y = v.y, z = v.z };
        }

        public int sqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return x * x + y * y + z * z; } }

        public float magnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return Mathf.Sqrt((float)(x * x + y * y + z * z)); } }

        [System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int32 operator +(in Vector3Int32 a, in Vector3Int32 b)
        {
            return new Vector3Int32 { x = a.x + b.x, y = a.y + b.y, z = a.z + b.z };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int32 operator -(in Vector3Int32 a, in Vector3Int32 b)
        {
            return new Vector3Int32(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int32 operator *(in Vector3Int32 a, int b)
        {
            return new Vector3Int32(a.x * b, a.y * b, a.z * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int32 operator /(in Vector3Int32 a, int b)
        {
            return new Vector3Int32(a.x / b, a.y / b, a.z / b);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int32 RoundToInt(in Vector3 v)
        {
            return new Vector3Int32(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z)
            );
        }

        public static Vector3Int32 Max(in Vector3Int32 a, in Vector3Int32 b)
        {
            return new Vector3Int32(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Vector3Int32 Min(in Vector3Int32 a, in Vector3Int32 b)
        {
            return new Vector3Int32(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }
    }
}