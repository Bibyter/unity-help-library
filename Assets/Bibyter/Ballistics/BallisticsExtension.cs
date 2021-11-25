using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter
{
    public static class BallisticsExtension
    {
        public static Vector3 GetVelocity(in Vector3 from, in Vector3 to, in Vector3 gravity, float duration)
        {
            return (to - from - (gravity * duration * duration * 0.5f)) / duration;
        }

        public static Vector3 LerpTrajectory(in Vector3 from, in Vector3 to, in Vector3 gravity, float duration, float t)
        {
            return from + ((to - from - (gravity * duration * duration * 0.5f)) / duration) * t + (gravity * t * t * 0.5f);
        }
    }
}
