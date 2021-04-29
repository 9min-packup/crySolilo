using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [CreateAssetMenu(menuName = "CrySolilo/ScriptableObject/FontDatabase")]
    public class FontDatabase : ScriptableObject
    {
        [SerializeField]
        public FontDataItem[] fontList;
    }

    [System.Serializable]
    public class FontDataItem
    {
        public string key;
        public Font font;

    }
}