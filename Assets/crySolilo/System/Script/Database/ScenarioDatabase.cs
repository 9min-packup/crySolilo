using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [CreateAssetMenu(menuName = "CrySolilo/ScriptableObject/ScenarioDatabase")]
    public class ScenarioDatabase : ScriptableObject
    {
        [SerializeField]
        public ScenarioDataItem[] scenarioList;
    }

    [System.Serializable]
    public class ScenarioDataItem
    {
        public string key;
        public TextAsset scenario;

    }
}
