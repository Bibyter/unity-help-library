using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Shaders.Filters
{
    public sealed class GrayscaleFilter : MonoBehaviour
    {
        [SerializeField] Shader _shader;

        Material _material;


        private void Awake()
        {
            _material = new Material(_shader);    
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }
    }
}
