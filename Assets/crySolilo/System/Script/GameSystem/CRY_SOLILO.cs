using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{

    public static class CRY_SOLILO
    {
        private static GameSystem system = null;
        public static GameSystem System
        {
            get
            {
                if (system == null)
                {
                    system = GameObject.FindObjectOfType<GameSystem>();

                }
                return system;
            }
        }
    }
}