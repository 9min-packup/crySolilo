using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CrySolilo
{
    public class ScenarioManager : MonoBehaviour
    {
        public int scenarioIndex = 0;
        public Tag[] tags;
        public Dictionary<string, int> labelIndexDict = new Dictionary<string, int>();

        private Coroutine skipCoro, executeCoro;

        private bool jumpRequest = false;
        private string jumpRequestStorageTarget = null;
        private string jumpRequestLabelTarget = null;

        private bool executing = false;

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
        }

        public void ExecuteScenario(int index = 0)
        {
            executeCoro = StartCoroutine(ExecuteScenarioIE(index));
        }

        private IEnumerator ExecuteScenarioIE(int index = 0)
        {
            executing = true;
            skipCoro = StartCoroutine(SkipTextIE());
            scenarioIndex = index;
            jumpRequest = false;
            jumpRequestStorageTarget = null;
            jumpRequestLabelTarget = null;
            while ((scenarioIndex < tags.Length) || jumpRequest)
            {
                if (jumpRequest)
                {
                    if (jumpRequestStorageTarget != null)
                    {
                        LoadScenario(jumpRequestStorageTarget);
                    }
                    if (jumpRequestLabelTarget != null)
                    {
                        if (labelIndexDict.ContainsKey(jumpRequestLabelTarget))
                        {
                            scenarioIndex = labelIndexDict[jumpRequestLabelTarget];
                        }
                        else
                        {
                            scenarioIndex = 0;
                        }
                    }
                    else
                    {
                        scenarioIndex = 0;
                    }
                    jumpRequest = false;
                    jumpRequestStorageTarget = null;
                    jumpRequestLabelTarget = null;
                    yield return null;
                    continue;
                }
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

                else if (tags[scenarioIndex].tagName == "chara_rotate")
                {
                    float time = 1.0f;
                    string key = "";
                    float from = 0.0f;
                    float to = 0.0f;
                    if (tags[scenarioIndex].properties.ContainsKey("time"))
                    {
                        time = float.Parse(tags[scenarioIndex].properties["time"]);
                        time = time / 1000.0f;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("from"))
                    {
                        from = float.Parse(tags[scenarioIndex].properties["from"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("to"))
                    {
                        to = float.Parse(tags[scenarioIndex].properties["to"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("key"))
                    {
                        key = tags[scenarioIndex].properties["key"];
                    }
                    yield return CRY_SOLILO.System.uiManager.RotateCharacter(key, from, to, time);
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

                else if (tags[scenarioIndex].tagName == "button")
                {
                    string graphic = "";
                    float x = 0.0f;
                    float y = 0.0f;
                    float width = 100.0f;
                    float height = 40.0f;
                    Color color = CRY_SOLILO.System.settingManager.setting.defaultFontColor;
                    int fontSize = CRY_SOLILO.System.settingManager.setting.defaultFontSize;
                    bool autoAjustSize = true;
                    string text = null;
                    string enterimg = null;
                    string target = null;
                    string storage = null;
                    Button.ButtonClickedEvent clickedEvent = new Button.ButtonClickedEvent();

                    if (tags[scenarioIndex].properties.ContainsKey("graphic"))
                    {
                        graphic = tags[scenarioIndex].properties["graphic"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("x"))
                    {
                        x = float.Parse(tags[scenarioIndex].properties["x"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("y"))
                    {
                        y = float.Parse(tags[scenarioIndex].properties["y"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("width"))
                    {
                        width = float.Parse(tags[scenarioIndex].properties["width"]);
                        autoAjustSize = false;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("height"))
                    {
                        height = float.Parse(tags[scenarioIndex].properties["height"]);
                        autoAjustSize = false;
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("color"))
                    {
                        color = Tag.ParseToColor(tags[scenarioIndex].properties["color"]);
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("text"))
                    {
                        text = tags[scenarioIndex].properties["text"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("enterimg"))
                    {
                        enterimg = tags[scenarioIndex].properties["enterimg"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("target"))
                    {
                        target = tags[scenarioIndex].properties["target"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("storage"))
                    {
                        storage = tags[scenarioIndex].properties["storage"];
                    }
                    clickedEvent.AddListener(() =>
                    {
                        Jump(storage, target);
                        CRY_SOLILO.System.uiManager.ClearAllButton();
                    });
                    CRY_SOLILO.System.uiManager.CreateButton(graphic, new Vector2(x, y), new Vector2(width, height), color, fontSize, autoAjustSize, text, enterimg, clickedEvent);

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
                        if (jumpRequest)
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
                        if (jumpRequest)
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
                        if (jumpRequest)
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
                    string target = null;
                    string storage = null;
                    if (tags[scenarioIndex].properties.ContainsKey("target"))
                    {
                        target = tags[scenarioIndex].properties["target"];
                    }
                    if (tags[scenarioIndex].properties.ContainsKey("storage"))
                    {
                        storage = tags[scenarioIndex].properties["storage"];
                    }
                    Jump(storage, target);
                }

                yield return null;
                scenarioIndex++;
            }

            StopCoroutine(skipCoro);
            executing = false;
            yield break;
        }

        public void StopScenario()
        {
            StopCoroutine(skipCoro);
            StopCoroutine(executeCoro);
        }

        public void Jump(string storageTarget = null, string labelTarget = null)
        {
            if (executing)
            {
                jumpRequest = true;
                jumpRequestStorageTarget = storageTarget;
                jumpRequestLabelTarget = labelTarget;
            }
            else
            {
                if (jumpRequestStorageTarget != null)
                {
                    LoadScenario(jumpRequestStorageTarget);
                }
                int index = 0;
                if (jumpRequestLabelTarget != null)
                {
                    if (labelIndexDict.ContainsKey(jumpRequestLabelTarget))
                    {
                        scenarioIndex = labelIndexDict[jumpRequestLabelTarget];
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else
                {
                    index = 0;
                }


                ExecuteScenario(index);
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