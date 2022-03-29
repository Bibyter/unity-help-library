using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Animations
{
    public sealed class LookAtCameraAnimation : MonoBehaviour
    {
        Transform _cameraTransform;

        private void Update()
        {
            transform.rotation = _cameraTransform.rotation;
        }
    }
}