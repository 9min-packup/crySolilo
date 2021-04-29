using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [CreateAssetMenu(menuName = "CrySolilo/ScriptableObject/AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeField]
        public AudioDataItem[] bgmList;
        [SerializeField]
        public AudioDataItem[] bgsList;
        [SerializeField]
        public AudioDataItem[] seList;
    }

    [System.Serializable]
    public class AudioDataItem
    {
        public string key;
        public AudioClip clip;
    }
}