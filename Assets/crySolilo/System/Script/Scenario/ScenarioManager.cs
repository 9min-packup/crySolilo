using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CrySolilo
{
    public class ScenarioManager : MonoBehaviour
    {
        public Tag[] tags;
        public Dictionary<string, int> labelIndexDict = new Dictionary<string, int>();

        private bool waitForSubmit = false;

        private void Start()
        {
            LoadScenario(CRY_SOLILO.System.settingManager.setting.firstScenarioKey);
            ExecuteScenario();
        }

        public void LoadScenario(string key)
        {
            TextAsset textAsset = CRY_SOLILO.System.database.GetScenario(key);
            if (textAsset == null)
            {
                return;
            }

            char[] scenarioArray = textAsset.text.ToCharArray();
            StringBuilder strB = new StringBuilder(128);
            List<Tag> tagList = new List<Tag>();
            labelIndexDict = new Dictionary<string, int>();
            Mode mode = Mode.None;
            char c;
            bool underBar = false;
            bool rt = false;
            for (int i = 0; i < scenarioArray.Length; i++)
            {
                c = scenarioArray[i];
                if (mode == Mode.None)
                {
                    if (c == ' ')
                    {
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '\t')
                    {
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '\n')
                    {
                        underBar = false;
                        rt = true;
                    }
                    else if (c == ';')
                    {
                        mode = Mode.Comment;
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '*')
                    {
                        mode = Mode.Label;
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '#')
                    {
                        strB.Clear();
                        strB.Append(c);
                        mode = Mode.Name;
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '[')
                    {
                        strB.Clear();
                        strB.Append(c);
                        mode = Mode.Tag;
                        underBar = false;
                        rt = false;
                    }
                    else
                    {
                        if (c == '_')
                        {
                            underBar = true;
                        }
                        else
                        {
                            underBar = false;
                        }
                        rt = false;
                        strB.Clear();
                        strB.Append(c);
                        mode = Mode.Text;
                    }
                }
                else if (mode == Mode.Comment)
                {
                    if (c == '\n')
                    {
                        mode = Mode.None;
                    }
                }
                else if (mode == Mode.Text)
                {
                    if (c == ' ')
                    {
                        if (underBar)
                        {
                            strB.Append(c);
                        }
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '\t')
                    {
                        if (underBar)
                        {
                            strB.Append(c);
                        }
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '\n')
                    {
                        if (underBar)
                        {
                            strB.Append(c);
                        }
                        underBar = false;
                        rt = true;
                    }
                    else if (c == ';')
                    {
                        Tag tag = Tag.CreateTextTag(strB.ToString());
                        tagList.Add(tag);
                        strB.Clear();
                        mode = Mode.Comment;
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '#')
                    {
                        if (rt)
                        {
                            Tag tag = Tag.CreateTextTag(strB.ToString());
                            tagList.Add(tag);
                            strB.Clear();
                            strB.Append(c);
                            mode = Mode.Name;
                        }
                        else
                        {
                            strB.Append(c);
                        }
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '*')
                    {
                        if (rt)
                        {
                            Tag tag = Tag.CreateTextTag(strB.ToString());
                            tagList.Add(tag);
                            strB.Clear();
                            strB.Append(c);
                            mode = Mode.Label;
                        }
                        else
                        {
                            strB.Append(c);
                        }
                        underBar = false;
                        rt = false;
                    }
                    else if (c == '[')
                    {
                        underBar = false;
                        Tag tag = Tag.CreateTextTag(strB.ToString());
                        tagList.Add(tag);
                        strB.Clear();
                        strB.Append(c);
                        mode = Mode.Tag;
                    }
                    else
                    {
                        if (c == '_')
                        {
                            underBar = true;
                        }
                        else
                        {
                            underBar = false;
                        }
                        rt = false;
                        strB.Append(c);
                    }
                }
                else if (mode == Mode.Name)
                {

                    if (c == '\n')
                    {
                        Tag tag = Tag.CreateNameTag(strB.ToString());
                        tagList.Add(tag);
                        strB.Clear();
                        mode = Mode.None;
                    }
                    else
                    {
                        strB.Append(c);
                    }
                }
                else if (mode == Mode.Label)
                {
                    if (c == '\n')
                    {
                        Tag tag = Tag.CreateLabelTag(strB.ToString());
                        tagList.Add(tag);
                        labelIndexDict.Add(tag.properties["val"], tagList.Count - 1);
                        strB.Clear();
                        mode = Mode.None;
                    }
                    else
                    {
                        strB.Append(c);
                    }
                }
                else if (mode == Mode.Tag)
                {
                    if (c == ']')
                    {
                        strB.Append(c);
                        Tag tag = Tag.CreateTag(strB.ToString());
                        tagList.Add(tag);
                        strB.Clear();
                        mode = Mode.None;
                    }
                    else
                    {
                        strB.Append(c);
                    }
                }
            }

            tags = tagList.ToArray();
            foreach (var tag in tags)
            {
                Debug.Log(tag.ToString());
            }
        }

        public void ExecuteScenario()
        {
            StartCoroutine(ExecuteScenarioIE());
        }

        private IEnumerator ExecuteScenarioIE()
        {
            Coroutine coro = StartCoroutine(SkipTextIE());
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i].tagName == "text")
                {
                    yield return CRY_SOLILO.System.uiManager.ShowText(tags[i].properties["val"], true);
                }
                else if (tags[i].tagName == "name")
                {
                    CRY_SOLILO.System.uiManager.ShowName(tags[i].properties["val"]);
                }

                else if (tags[i].tagName == "bg")
                {
                    float time = 0;
                    string key = "";
                    if (tags[i].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[i].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[i].properties.ContainsKey("key"))
                    {
                        key = tags[i].properties["key"];
                    }
                    yield return CRY_SOLILO.System.uiManager.ShowBg(key, time);
                }
                else if (tags[i].tagName == "message_hide")
                {
                    CRY_SOLILO.System.uiManager.HideTextBox();
                }
                else if (tags[i].tagName == "message_show")
                {
                    CRY_SOLILO.System.uiManager.ShowTextBox();
                }
                else if (tags[i].tagName == "chara_show")
                {
                    float time = 0;
                    string key = "";
                    string faceKey = "";
                    float x = 0;
                    float y = 0;
                    if (tags[i].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[i].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[i].properties.ContainsKey("key"))
                    {
                        key = tags[i].properties["key"];
                    }
                    if (tags[i].properties.ContainsKey("faceKey"))
                    {
                        faceKey = tags[i].properties["faceKey"];
                    }
                    if (tags[i].properties.ContainsKey("x"))
                    {
                        x = float.Parse(tags[i].properties["x"]);
                    }
                    if (tags[i].properties.ContainsKey("y"))
                    {
                        y = float.Parse(tags[i].properties["y"]);
                    }
                    yield return CRY_SOLILO.System.uiManager.ShowCaracter(key, faceKey, new Vector2(x, y), time);
                }
                else if (tags[i].tagName == "p")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            CRY_SOLILO.System.uiManager.ClearText();
                            break;
                        }
                        yield return null;
                    }
                }
                else if (tags[i].tagName == "s")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            CRY_SOLILO.System.uiManager.ClearText();
                        }
                        yield return null;
                    }
                }
                else if (tags[i].tagName == "l")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else if (tags[i].tagName == "r")
                {
                    yield return CRY_SOLILO.System.uiManager.ShowText('\n'.ToString(), true);
                }
                else if (tags[i].tagName == "wait")
                {
                    float time = 0f;
                    if (tags[i].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[i].properties["time"]);
                        time = time / 1000.0f;
                    }
                    yield return new WaitForSeconds(time);
                }
                else if (tags[i].tagName == "jump")
                {
                    string target = "";
                    if (tags[i].properties.ContainsKey("target"))
                    {
                        target = tags[i].properties["target"];
                    }
                    if (labelIndexDict.ContainsKey(target))
                    {
                        i = labelIndexDict[target];
                    }
                }

                yield return null;
            }

            StopCoroutine(coro);
            yield break;
        }

        private IEnumerator SkipTextIE()
        {
            while (true)
            {
                if (CRY_SOLILO.System.inputManager.submit)
                {
                    CRY_SOLILO.System.uiManager.SkipText();
                }
                yield return null;
            }
        }

        public enum Mode
        {
            None,
            Comment,
            Label,
            Text,
            Name,
            Tag
        }
    }
}