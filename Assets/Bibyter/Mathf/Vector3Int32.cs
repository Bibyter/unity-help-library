using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bibyter.Mathematics
{
    [System.Serializable]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Int32 lhs, Vector3Int32 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Int32 lhs, Vector3Int32 rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Vector3Int32)) return false;

            return Equals((Vector3Int32)other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Vector3Int32 other)
        {
            return this == other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            var yHash = y.GetHashCode();
            var zHash = z.GetHashCode();
            return x.GetHashCode() ^ (yHash << 4) ^ (yHash >> 28) ^ (zHash >> 4) ^ (zHash << 28);
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

        public static readonly Vector3Int32 zero = new Vector3Int32(0, 0, 0);
        public static readonly Vector3Int32 one = new Vector3Int32(1, 1, 1);
        public static readonly Vector3Int32 up = new Vector3Int32(0, 1, 0);
        public static readonly Vector3Int32 down = new Vector3Int32(0, -1, 0);
        public static readonly Vector3Int32 left = new Vector3Int32(-1, 0, 0);
        public static readonly Vector3Int32 right = new Vector3Int32(1, 0, 0);
        public static readonly Vector3Int32 forward = new Vector3Int32(0, 0, 1);
        public static readonly Vector3Int32 back = new Vector3Int32(0, 0, -1);
    }
}