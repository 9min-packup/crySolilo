using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{

    public class InputManager : MonoBehaviour
    {
        public Vector3 mousePosition;
        public bool submit;


        // Update is called once per frame
        void Update()
        {
            mousePosition = Input.mousePosition;
            submit = Input.GetMouseButtonDown(0);
        }
    }
}