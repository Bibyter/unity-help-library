using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Shaders.Filters
{
    public sealed class GaussBlurFilter : MonoBehaviour
    {
        [SerializeField, Range(0, 200)] int _kernel = 21;
        [SerializeField, Range(0f, 70f)] float _spread = 5f;
        [SerializeField] Shader _shader;

        Material _material;

        public float spread
        {
            set
            {
                if (_material != null) _material.SetFloat("_Spread", _spread);
                _spread = value;
            }
            get => _spread;
        }

        public int kernel
        {
            set
            {
                if (_material != null) _material.SetInt("_Kernel", _kernel);
                _kernel = value;
            }
            get => _kernel;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                spread = _spread;
                kernel = _kernel;
            }
        }

        private void Awake()
        {
            _material = new Material(_shader);
            spread = _spread;
            kernel = _kernel;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }
    }
}
