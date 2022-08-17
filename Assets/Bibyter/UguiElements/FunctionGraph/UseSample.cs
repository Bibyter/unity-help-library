using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.UiElements.FunctionGraph
{
    public class UseSample : MonoBehaviour
    {
        [SerializeField] UiFunctionGraph _functionGraph;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _functionGraph.AddValue(0, Mathf.Sin(Time.time * 3f) * 300f);
                _functionGraph.AddValue(1, Mathf.Cos(Time.time * 3f) * 300f);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _functionGraph.ClearAllLines();
                _functionGraph.OrdinateAxisReset();
            }
        }
    }
}