using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    [System.Serializable]
    public class Tag : ISerializationCallbackReceiver
    {
        [SerializeField]
        public string tagName;

        public Dictionary<string, string> properties = new Dictionary<string, string>();

        [SerializeField]
        public List<TagProperty> propertyList;

        public void OnAfterDeserialize()
        {
            properties = new Dictionary<string, string>();
            foreach (var tagP in propertyList)
            {
                properties.Add(tagP.key, tagP.value);
            }
        }

        public void OnBeforeSerialize()
        {
            propertyList = new List<TagProperty>();
            foreach (var pair in properties)
            {
                var tagP = new TagProperty();
                tagP.key = pair.Key;
                tagP.value = pair.Value;
                propertyList.Add(tagP);
            }
        }

        public static Tag CreateTag(string tagString)
        {
            char[] splitCharsA = { '[', ' ', ']', '\n' };
            char[] splitCharsB = { '=', ' ' };
            char[] splitCharsC = { ' ', '\n', '\t', '"' };
            Tag tag = new Tag();

            string[] strArrayA = tagString.Split(splitCharsA, StringSplitOptions.RemoveEmptyEntries);
            if (strArrayA.Length >= 1)
            {
                tag.tagName = strArrayA[0];

                for (int i = 1; i < strArrayA.Length; i++)
                {
                    string[] strArrayB = strArrayA[i].Split(splitCharsB, StringSplitOptions.RemoveEmptyEntries);
                    string propatyName = strArrayB[0].Trim(splitCharsC);
                    string propatyValue = strArrayB[1].Trim(splitCharsC);
                    tag.properties.Add(propatyName, propatyValue);
                }
            }
            return tag;
        }
        public static Tag CreateTextTag(string str)
        {
            Tag tag = new Tag();
            tag.tagName = "text";
            //テキストの整形
            char[] splitChars = { ' ', '\n' };
            string showText = str.Trim(splitChars);
            showText.Replace("_ ", " ");
            tag.properties.Add("val", showText);
            return tag;
        }

        public static Tag CreateNameTag(string str)
        {
            Tag tag = new Tag();
            tag.tagName = "name";
            char[] splitChars = { '#', '\n' };
            tag.properties.Add("val", str.Trim(splitChars));
            return tag;
        }

        public static Tag CreateLabelTag(string str)
        {
            Tag tag = new Tag();
            tag.tagName = "label";
            char[] splitChars = { '*', '\n' };
            tag.properties.Add("val", str.Trim(splitChars));
            return tag;
        }

        public static Color ParseToColor(string str)
        {
            Color color = Color.black;
            char[] splitChars = { 'x' };
            string[] colorStr = str.Split(splitChars);
            if (colorStr.Length >= 2 && colorStr[1].Length == 6)
            {

                float r = (Convert.ToInt32(colorStr[1].Substring(0, 2), 16)) / 256f;
                float g = (Convert.ToInt32(colorStr[1].Substring(2, 2), 16)) / 256f;
                float b = (Convert.ToInt32(colorStr[1].Substring(4, 2), 16)) / 256f;
                color = new Color(r, g, b);
            }
            return color;
        }

        [System.Serializable]
        public class TagProperty
        {
            public string key;
            public string value;
        }

        public override string ToString()
        {
            string result = "";
            result += String.Format("tag name: {0}\n", tagName);
            if (properties.Count > 0)
            {
                result += String.Format("properties: \n");
                foreach (var pair in properties)
                {
                    result += String.Format("key: {0}, value: {1}\n", pair.Key, pair.Value);
                }
            }
            return result;
        }

    }
}