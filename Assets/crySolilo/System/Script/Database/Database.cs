using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    public class Database : MonoBehaviour
    {
        private Dictionary<string, int> bgm = new Dictionary<string, int>();
        private Dictionary<string, int> bgs = new Dictionary<string, int>();
        private Dictionary<string, int> se = new Dictionary<string, int>();
        private Dictionary<string, int> bg = new Dictionary<string, int>();
        private Dictionary<string, int> character = new Dictionary<string, int>();
        private Dictionary<string, Dictionary<string, int>> characterFace = new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<string, int> font = new Dictionary<string, int>();

        [SerializeField]
        private AudioDatabase audioDatabase;
        [SerializeField]
        private ImageDatabase imageDatabase;
        [SerializeField]
        private FontDatabase fontDatabase;

        private void Awake()
        {
            for (int i = 0; i < audioDatabase.bgmList.Length; i++)
            {
                bgm.Add(audioDatabase.bgmList[i].key, i);
            }
            for (int i = 0; i < audioDatabase.bgsList.Length; i++)
            {
                bgs.Add(audioDatabase.bgsList[i].key, i);
            }
            for (int i = 0; i < audioDatabase.seList.Length; i++)
            {
                se.Add(audioDatabase.seList[i].key, i);
            }
            for (int i = 0; i < imageDatabase.bgList.Length; i++)
            {
                bg.Add(imageDatabase.bgList[i].key, i);
            }
            for (int i = 0; i < imageDatabase.characterList.Length; i++)
            {
                character.Add(imageDatabase.characterList[i].key, i);
                var faces = new Dictionary<string, int>();
                var faceList = imageDatabase.characterList[i].characterData.faces;
                for (int j = 0; j < faceList.Length; j++)
                {
                    faces.Add(faceList[j].key, j);
                }
                characterFace.Add(imageDatabase.characterList[i].key, faces);
            }
            for (int i = 0; i < fontDatabase.fontList.Length; i++)
            {
                font.Add(fontDatabase.fontList[i].key, i);
            }

        }

        public AudioClip GetBgm(string key)
        {
            int index = 0;
            if (bgm.ContainsKey(key))
            {
                index = bgm[key];
            }
            else
            {
                Debug.LogWarning("BGM Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= audioDatabase.bgmList.Length)
            {
                Debug.LogWarning("BGM Database: Index " + index + " Out Of bounds");
                return null;
            }
            return audioDatabase.bgmList[index].clip;
        }

        public AudioClip GetBgs(string key)
        {
            int index = 0;
            if (bgs.ContainsKey(key))
            {
                index = bgs[key];
            }
            else
            {
                Debug.LogWarning("BGS Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= audioDatabase.bgsList.Length)
            {
                Debug.LogWarning("BGS Database: Index " + index + " Out Of bounds");
                return null;
            }
            return audioDatabase.bgsList[index].clip;
        }

        public AudioClip GetSe(string key)
        {
            int index = 0;
            if (se.ContainsKey(key))
            {
                index = se[key];
            }
            else
            {
                Debug.LogWarning("SE Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= audioDatabase.seList.Length)
            {
                Debug.LogWarning("SE Database: Index " + index + " Out Of bounds");
                return null;
            }
            return audioDatabase.seList[index].clip;
        }

        public Sprite GetBg(string key)
        {
            int index = 0;
            if (bg.ContainsKey(key))
            {
                index = bg[key];
            }
            else
            {
                Debug.LogWarning("BG Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= imageDatabase.bgList.Length)
            {
                Debug.LogWarning("BG Database: Index " + index + " Out Of bounds");
                return null;
            }
            return imageDatabase.bgList[index].bg;
        }

        public CharacterData GetCharacter(string key)
        {
            int index = 0;
            if (character.ContainsKey(key))
            {
                index = character[key];
            }
            else
            {
                Debug.LogWarning("Character Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= imageDatabase.characterList.Length)
            {
                Debug.LogWarning("Character Database: Index " + index + " Out Of bounds");
                return null;
            }
            return imageDatabase.characterList[index].characterData;
        }

        public int GetCharacterIndex(string key)
        {
            int index = 0;
            if (character.ContainsKey(key))
            {
                index = character[key];
            }
            else
            {
                Debug.LogWarning("Character Database: Key " + key + " Not Found");
                return -1;
            }
            if (index < 0 || index >= imageDatabase.characterList.Length)
            {
                Debug.LogWarning("Character Database: Index " + index + " Out Of bounds");
                return -1;
            }
            return index;
        }

        public Sprite GetCharacterFace(string charakey, string faceKey)
        {
            int charaIndex = GetCharacterIndex(charakey);
            if (charaIndex < 0)
            {
                return null;
            }

            Dictionary<string, int> face;
            if (characterFace.ContainsKey(charakey))
            {
                face = characterFace[charakey];
            }
            else
            {
                Debug.LogWarning("CharacterFace Database: Character Key " + charakey + " Not Found");
                return null;
            }

            int faceIndex = 0;
            if (face.ContainsKey(faceKey))
            {
                faceIndex = face[faceKey];
            }
            else
            {
                Debug.LogWarning("Character Database: Face Key " + faceKey + " Not Found");
                return null;
            }
            if (faceIndex < 0 || faceIndex >= imageDatabase.characterList[charaIndex].characterData.faces.Length)
            {
                Debug.LogWarning("Character Database: Face Index " + faceIndex + " Out Of bounds");
                return null;
            }

            return imageDatabase.characterList[charaIndex].characterData.faces[faceIndex].face;
        }

        public Font GetFont(string key)
        {
            int index = 0;
            if (font.ContainsKey(key))
            {
                index = font[key];
            }
            else
            {
                Debug.LogWarning("Font Database: Key " + key + " Not Found");
                return null;
            }
            if (index < 0 || index >= fontDatabase.fontList.Length)
            {
                Debug.LogWarning("Font Database: Index " + index + " Out Of bounds");
                return null;
            }
            return fontDatabase.fontList[index].font;
        }

    }
}