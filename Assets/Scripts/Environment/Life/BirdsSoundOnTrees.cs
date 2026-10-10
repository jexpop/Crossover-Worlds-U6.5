using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrossoverWorlds
{
    public class BirdsSoundOnTrees : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            
            /*if (other.CompareTag("Tree"))
            {*/
                Debug.Log("arbol");
            //}
        }
        
    }
}
