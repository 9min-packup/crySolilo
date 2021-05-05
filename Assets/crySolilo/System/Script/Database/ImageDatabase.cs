using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [CreateAssetMenu(menuName = "CrySolilo/ScriptableObject/CharacterDatabase")]
    public class ImageDatabase : ScriptableObject
    {
        [SerializeField]
        public BgDataItem[] bgList;
        [SerializeField]
        public ButtonDataItem[] buttonList;
        [SerializeField]
        public CharacterDataItem[] characterList;



    }

    [System.Serializable]
    public class CharacterDataItem
    {
        public string key;
        public CharacterData characterData;
    }

    [System.Serializable]
    public class CharacterData
    {
        public string defaultFaceKey;
        public CharacterFaceData[] faces;
    }

    [System.Serializable]
    public class CharacterFaceData
    {
        public string key;
        public Sprite face;
    }

    [System.Serializable]
    public class BgDataItem
    {
        public string key;
        public Sprite bg;
    }

    [System.Serializable]
    public class ButtonDataItem
    {
        public string key;
        public Sprite button;
    }

}