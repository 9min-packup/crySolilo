using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CrySolilo
{
    public class ScenarioManager : MonoBehaviour
    {
        public int scenarioIndex = 0;
        private int scenarioIndexCache = 0;
        public Tag[] tags;
        public Dictionary<string, int> labelIndexDict = new Dictionary<string, int>();

        private bool waitForSubmit = false;

        private Coroutine skipCoro, executeCoro;

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
            executeCoro = StartCoroutine(ExecuteScenarioIE());
        }

        private IEnumerator ExecuteScenarioIE()
        {
            skipCoro = StartCoroutine(SkipTextIE());
            scenarioIndex = 0;
            while (scenarioIndex < tags.Length)
            {
                if (tags[scenarioIndex].tagName == "text")
                {
                    yield return CRY_SOLILO.System.uiManager.ShowText(tags[scenarioIndex].properties["val"], true);
                }
                else if (tags[scenarioIndex].tagName == "name")
                {
                    CRY_SOLILO.System.uiManager.ShowName(tags[scenarioIndex].properties["val"]);
                }

                else if (tags[scenarioIndex].tagName == "bg")
                {
                    float time = 1.0f;
                    string key = "";
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    yield return CRY_SOLILO.System.uiManager.ShowBg(key, time);
                }
                else if (tags[scenarioIndex].tagName == "message_hide")
                {
                    CRY_SOLILO.System.uiManager.HideTextBox();
                }
                else if (tags[scenarioIndex].tagName == "message_show")
                {
                    CRY_SOLILO.System.uiManager.ShowTextBox();
                }
                else if (tags[scenarioIndex].tagName == "chara_show")
                {
                    float time = 1.0f;
                    string key = "";
                    string faceKey = "";
                    float x = 0;
                    float y = 0;
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("faceKey"))
                    {
                        faceKey = tags[scenarioIndex].properties["faceKey"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("x"))
                    {
                        x = float.Parse(tags[scenarioIndex].properties["x"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("y"))
                    {
                        y = float.Parse(tags[scenarioIndex].properties["y"]);
                    }
                    yield return CRY_SOLILO.System.uiManager.ShowCaracter(key, faceKey, new Vector2(x, y), time);
                }
                else if (tags[scenarioIndex].tagName == "chara_hide")
                {
                    float time = 1.0f;
                    string key = "";
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    yield return CRY_SOLILO.System.uiManager.HideCharacter(key, time);
                }
                else if (tags[scenarioIndex].tagName == "chara_hide_all")
                {
                    float time = 1.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    yield return CRY_SOLILO.System.uiManager.HideAllCharacter(time);
                }
                else if (tags[scenarioIndex].tagName == "playbgm")
                {
                    string key = "";
                    float volume = 1.0f;
                    float pitch = 1.0f;
                    bool loop = true;
                    bool isFade = false;
                    float fadeout = 0.0f;
                    float fadewait = 0.0f;
                    float fadein = 0.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("volume"))
                    {
                        volume = float.Parse(tags[scenarioIndex].properties["volume"]);
                        volume = volume / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("pitch"))
                    {
                        pitch = float.Parse(tags[scenarioIndex].properties["pitch"]);
                        pitch = pitch / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("loop"))
                    {
                        loop = bool.Parse(tags[scenarioIndex].properties["loop"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadeout"))
                    {
                        fadeout = float.Parse(tags[scenarioIndex].properties["fadeout"]);
                        fadeout = fadeout / 1000.0f;
                        isFade = true;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadewait"))
                    {
                        fadewait = float.Parse(tags[scenarioIndex].properties["fadewait"]);
                        fadewait = fadewait / 1000.0f;
                        isFade = true;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadein"))
                    {
                        fadein = float.Parse(tags[scenarioIndex].properties["fadein"]);
                        fadein = fadein / 1000.0f;
                        isFade = true;
                    }
                    if (isFade)
                    {
                        CRY_SOLILO.System.audioManager.PlayBGM(key, loop, volume, pitch, fadeout, fadewait, fadein);
                    }
                    else
                    {
                        CRY_SOLILO.System.audioManager.PlayBGM(key, loop, volume, pitch);
                    }
                }
                else if (tags[scenarioIndex].tagName == "stopbgm")
                {
                    bool isFade = false;
                    float fadeout = 0.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("fadeout"))
                    {
                        fadeout = float.Parse(tags[scenarioIndex].properties["fadeout"]);
                        fadeout = fadeout / 1000.0f;
                        isFade = true;
                    }
                    if (isFade)
                    {
                        CRY_SOLILO.System.audioManager.StopBGM(fadeout);
                    }
                    else
                    {
                        CRY_SOLILO.System.audioManager.StopBGM();
                    }
                }
                else if (tags[scenarioIndex].tagName == "playbgs")
                {
                    string key = "";
                    float volume = 1.0f;
                    float pitch = 1.0f;
                    bool loop = true;
                    bool isFade = false;
                    float fadeout = 0.0f;
                    float fadewait = 0.0f;
                    float fadein = 0.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("volume"))
                    {
                        volume = float.Parse(tags[scenarioIndex].properties["volume"]);
                        volume = volume / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("pitch"))
                    {
                        pitch = float.Parse(tags[scenarioIndex].properties["pitch"]);
                        pitch = pitch / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("loop"))
                    {
                        loop = bool.Parse(tags[scenarioIndex].properties["loop"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadeout"))
                    {
                        fadeout = float.Parse(tags[scenarioIndex].properties["fadeout"]);
                        fadeout = fadeout / 1000.0f;
                        isFade = true;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadewait"))
                    {
                        fadewait = float.Parse(tags[scenarioIndex].properties["fadewait"]);
                        fadewait = fadewait / 1000.0f;
                        isFade = true;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("fadein"))
                    {
                        fadein = float.Parse(tags[scenarioIndex].properties["fadein"]);
                        fadein = fadein / 1000.0f;
                        isFade = true;
                    }
                    if (isFade)
                    {
                        CRY_SOLILO.System.audioManager.PlayBGS(key, loop, volume, pitch, fadeout, fadewait, fadein);
                    }
                    else
                    {
                        CRY_SOLILO.System.audioManager.PlayBGS(key, loop, volume, pitch);
                    }
                }
                else if (tags[scenarioIndex].tagName == "stopbgs")
                {
                    bool isFade = false;
                    float fadeout = 0.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("fadeout"))
                    {
                        fadeout = float.Parse(tags[scenarioIndex].properties["fadeout"]);
                        fadeout = fadeout / 1000.0f;
                        isFade = true;
                    }
                    if (isFade)
                    {
                        CRY_SOLILO.System.audioManager.StopBGS(fadeout);
                    }
                    else
                    {
                        CRY_SOLILO.System.audioManager.StopBGS();
                    }
                }
                else if (tags[scenarioIndex].tagName == "playse")
                {
                    string key = "";
                    int buf = 0;
                    float volume = 1.0f;
                    float pitch = 1.0f;
                    bool loop = false;
                    if (tags[scenarioIndex].properties.ContainsKey("buf"))
                    {
                        buf = int.Parse(tags[scenarioIndex].properties["buf"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("volume"))
                    {
                        volume = float.Parse(tags[scenarioIndex].properties["volume"]);
                        volume = volume / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("pitch"))
                    {
                        pitch = float.Parse(tags[scenarioIndex].properties["pitch"]);
                        pitch = pitch / 100.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("loop"))
                    {
                        loop = bool.Parse(tags[scenarioIndex].properties["loop"]);
                    }
                    CRY_SOLILO.System.audioManager.PlaySE(key, buf, loop, volume, pitch);
                }

                else if (tags[scenarioIndex].tagName == "stopse")
                {
                    int buf = 0;
                    if (tags[scenarioIndex].properties.ContainsKey("buf"))
                    {
                        buf = int.Parse(tags[scenarioIndex].properties["buf"]);
                    }
                    CRY_SOLILO.System.audioManager.StopSE(buf);
                }

                else if (tags[scenarioIndex].tagName == "p")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            CRY_SOLILO.System.uiManager.ClearText();
                            break;
                        }
                        if (scenarioIndex != scenarioIndexCache)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else if (tags[scenarioIndex].tagName == "s")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            CRY_SOLILO.System.uiManager.ClearText();
                        }
                        if (scenarioIndex != scenarioIndexCache)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else if (tags[scenarioIndex].tagName == "l")
                {
                    while (true)
                    {
                        if (CRY_SOLILO.System.inputManager.submit)
                        {
                            break;
                        }
                        if (scenarioIndex != scenarioIndexCache)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else if (tags[scenarioIndex].tagName == "r")
                {
                    yield return CRY_SOLILO.System.uiManager.ShowText('\n'.ToString(), true);
                }
                else if (tags[scenarioIndex].tagName == "wait")
                {
                    float time = 0f;
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    yield return new WaitForSeconds(time);
                }
                else if (tags[scenarioIndex].tagName == "jump")
                {
                    string target = "";
                    if (tags[scenarioIndex].properties.ContainsKey("target"))
                    {
                        target = tags[scenarioIndex].properties["target"];
                    }
                    if (labelIndexDict.ContainsKey(target))
                    {
                        scenarioIndex = labelIndexDict[target];
                    }
                }

                yield return null;
                scenarioIndex++;
                scenarioIndexCache = scenarioIndex;
            }

            StopCoroutine(skipCoro);
            yield break;
        }

        public void StopScenario()
        {
            StopCoroutine(skipCoro);
            StopCoroutine(executeCoro);
        }

        public void Jump(string target)
        {
            if (labelIndexDict.ContainsKey(target))
            {
                scenarioIndex = labelIndexDict[target];
            }
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