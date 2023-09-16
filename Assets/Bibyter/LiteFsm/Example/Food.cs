using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiteFsm.Example
{
    public sealed class Food : MonoBehaviour
    {
        private void Start()
        {
            Respawn();
        }

        public void Respawn()
        {
            transform.position = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
        }
    }
}