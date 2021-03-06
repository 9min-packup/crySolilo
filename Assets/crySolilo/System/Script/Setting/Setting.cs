using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [CreateAssetMenu(menuName = "CrySolilo/ScriptableObject/Setting")]
    public class Setting : ScriptableObject
    {
        [SerializeField]
        public string firstScenarioKey = "first";
        [SerializeField]
        public string defaultFontKey = "Arial";
        [SerializeField]
        public int defaultFontSpeed = 20;
        [SerializeField]
        public int defaultFontSize = 28;
        [SerializeField]
        public int defaultNameFontSize = 28;
        [SerializeField]
        public Color defaultFontColor = Color.white;
        [SerializeField]
        public Vector2 textBoxSizeDelta = new Vector2(-100, -400);
        [SerializeField]
        public Vector2 textBoxPosition = new Vector2(0, -150);
        [SerializeField]
        public Vector2 textSizeDelta = new Vector2(-20, -30);
        [SerializeField]
        public Vector2 textSizePosition = new Vector2(0, 0);
        [SerializeField]
        public Vector2 nameSizeDelta = new Vector2(0, 0);
        [SerializeField]
        public Vector2 nameSizePosition = new Vector2(0, 0);
        [SerializeField]
        public Vector2 buttonMargin = new Vector2(20f, 10f);
    }
}
