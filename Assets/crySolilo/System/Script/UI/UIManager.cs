using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrySolilo
{

    public class UIManager : MonoBehaviour
    {
        public RectTransform bgParent, charaParent, textParent, ButtonParent;
        private UIBg uibg = new UIBg();
        private Dictionary<string, UICharacter> uicharaDict = new Dictionary<string, UICharacter>();
        private UIText uiText = new UIText();
        private List<UIButton> buttonList = new List<UIButton>();

        public Coroutine ShowBg(string key, float time = 1.0f)
        {
            Sprite bgSprite = CRY_SOLILO.System.database.GetBg(key);
            if (bgSprite == null)
            {
                return null;
            }
            if (uibg.bgCoro != null)
            {
                StopCoroutine(uibg.bgCoro);
                if (uibg.bgBack != null)
                {
                    Destroy(uibg.bgBack.gameObject);
                    uibg.bgBack = null;
                    uibg.bgImageBack = null;
                }
                uibg.bgCoro = null;
            }
            GameObject obj = new GameObject(key);
            obj.transform.SetParent(bgParent);
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
            rectTransform.localPosition = Vector3.zero;
            Image image = obj.AddComponent<Image>();
            image.sprite = bgSprite;
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            uibg.bgBack = uibg.bgFore;
            uibg.bgImageBack = uibg.bgImageFore;
            uibg.bgFore = rectTransform;
            uibg.bgImageFore = image;
            uibg.key = key;

            uibg.bgCoro = TweenUI.Fade(this, uibg.bgImageFore, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), time, 0.0f, () =>
            {
                if (uibg.bgBack != null)
                {
                    Destroy(uibg.bgBack.gameObject);
                    uibg.bgBack = null;
                    uibg.bgImageBack = null;
                    uibg.bgCoro = null;
                }
            });
            return uibg.bgCoro;
        }

        public Coroutine ClearBG(float time = 1.0f)
        {
            if (uibg.bgCoro != null)
            {
                StopCoroutine(uibg.bgCoro);
                if (uibg.bgBack != null)
                {
                    Destroy(uibg.bgBack.gameObject);
                    uibg.bgBack = null;
                }
                uibg.bgCoro = null;
            }
            if (uibg.bgFore == null)
            {
                return null;
            }
            uibg.bgCoro = TweenUI.Fade(this, uibg.bgImageFore, new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), time, 0.0f, () =>
            {
                if (uibg.bgFore != null)
                {
                    Destroy(uibg.bgFore.gameObject);
                    uibg.bgFore = null;
                    uibg.bgCoro = null;
                }
            });
            return uibg.bgCoro;

        }

        public Coroutine ShowCaracter(string key, string faceKey, Vector2 position, float time = 1.0f, bool isClossFade = true)
        {
            Sprite charaSprite = CRY_SOLILO.System.database.GetCharacterFace(key, faceKey);
            if (charaSprite == null)
            {
                return null;
            }
            UICharacter uichara;
            if (uicharaDict.ContainsKey(key))
            {
                uichara = uicharaDict[key];
            }
            else
            {
                uichara = new UICharacter();
                uicharaDict.Add(key, uichara);
            }

            if (uichara.charaCoro != null)
            {
                StopCoroutine(uichara.charaCoro);
                if (uichara.charaBack != null)
                {
                    Destroy(uichara.charaBack.gameObject);
                    uichara.charaBack = null;
                    uichara.charaImageBack = null;
                }
                uichara.charaCoro = null;
            }

            GameObject obj = new GameObject(key);
            obj.transform.SetParent(bgParent);
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.localPosition = position;
            Image image = obj.AddComponent<Image>();
            image.sprite = charaSprite;
            image.preserveAspect = true;
            image.SetNativeSize();
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            uichara.charaBack = uichara.charaFore;
            uichara.charaImageBack = uichara.charaImageFore;
            uichara.charaFore = rectTransform;
            uichara.charaImageFore = image;
            uichara.charaKey = key;
            uichara.faceKey = faceKey;
            uichara.position = position;

            if (uichara.charaBack == null || !isClossFade)
            {
                uichara.charaCoro = TweenUI.Fade(this, uichara.charaImageFore, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), time, 0.0f, () =>
                {
                    if (uichara.charaBack != null)
                    {
                        Destroy(uichara.charaBack.gameObject);
                        uichara.charaBack = null;
                        uichara.charaImageBack = null;
                        uichara.charaCoro = null;
                    }
                });
            }
            else
            {
                uichara.charaCoro = TweenUI.ClossFade(this, uichara.charaImageBack, uichara.charaImageFore, new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), time, 0.0f, () =>
                {
                    if (uichara.charaBack != null)
                    {
                        Destroy(uichara.charaBack.gameObject);
                        uichara.charaBack = null;
                        uichara.charaImageBack = null;
                        uichara.charaCoro = null;
                    }
                });
            }
            return uichara.charaCoro;
        }

        public Coroutine HideCharacter(string key, float time = 1.0f)
        {
            UICharacter uichara;
            if (uicharaDict.ContainsKey(key))
            {
                uichara = uicharaDict[key];
                uicharaDict.Remove(key);
            }
            else
            {
                return null;
            }

            if (uichara.charaCoro != null)
            {
                StopCoroutine(uichara.charaCoro);
                if (uichara.charaBack != null)
                {
                    Destroy(uichara.charaBack.gameObject);
                    uichara.charaBack = null;
                    uichara.charaImageBack = null;
                }
                uichara.charaCoro = null;
            }
            if (uichara.charaFore == null)
            {
                return null;
            }
            uichara.charaCoro = TweenUI.Fade(this, uichara.charaImageFore, new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), time, 0.0f, () =>
            {
                if (uichara.charaFore != null)
                {
                    Destroy(uichara.charaFore.gameObject);
                    uichara.charaFore = null;
                    uichara.charaImageFore = null;
                    uichara.charaCoro = null;
                }
            });
            return uichara.charaCoro;
        }


        public Coroutine RotateCharacter(string key, float from, float to, float time = 1.0f)
        {
            UICharacter uichara;
            if (uicharaDict.ContainsKey(key))
            {
                uichara = uicharaDict[key];
            }
            else
            {
                return null;
            }

            if (uichara.charaCoro != null)
            {
                StopCoroutine(uichara.charaCoro);
                uichara.charaCoro = null;
            }
            if (uichara.charaFore == null)
            {
                return null;
            }
            uichara.charaCoro = TweenUI.Rotate(this, uichara.charaImageFore, from, to, time, 0.0f, () =>
            {

            });
            return uichara.charaCoro;
        }

        public Coroutine HideAllCharacter(float time = 1.0f)
        {
            UICharacter uichara = null;
            foreach (var pair in uicharaDict)
            {
                uichara = pair.Value;
                if (uichara.charaCoro != null)
                {
                    StopCoroutine(uichara.charaCoro);
                    if (uichara.charaBack != null)
                    {
                        Destroy(uichara.charaBack.gameObject);
                        uichara.charaBack = null;
                        uichara.charaImageBack = null;
                    }
                    uichara.charaCoro = null;
                }
                if (uichara.charaFore == null)
                {
                    return null;
                }
                uichara.charaCoro = TweenUI.Fade(this, uichara.charaImageFore, new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), time, 0.0f, () =>
                {
                    if (uichara.charaFore != null)
                    {
                        Destroy(uichara.charaFore.gameObject);
                        uichara.charaFore = null;
                        uichara.charaImageFore = null;
                        uichara.charaCoro = null;
                    }
                });
            }
            uicharaDict.Clear();
            if (uichara == null)
            {
                return null;
            }
            else
            {
                return uichara.charaCoro;
            }
        }


        public void ShowTextBox()
        {
            if (uiText.textBaseRect != null)
            {
                return;
            }
            GameObject objTextBase = new GameObject("TextBase");
            objTextBase.transform.SetParent(textParent);
            RectTransform rectTransform = objTextBase.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = CRY_SOLILO.System.settingManager.setting.textBoxSizeDelta;
            rectTransform.localPosition = CRY_SOLILO.System.settingManager.setting.textBoxPosition;
            Image image = objTextBase.AddComponent<Image>();
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            GameObject objText = new GameObject("Text");
            objText.transform.SetParent(rectTransform);
            RectTransform rectTransformText = objText.AddComponent<RectTransform>();
            rectTransformText.anchorMin = Vector2.zero;
            rectTransformText.anchorMax = Vector2.one;
            rectTransformText.sizeDelta = CRY_SOLILO.System.settingManager.setting.textSizeDelta;
            rectTransformText.localPosition = CRY_SOLILO.System.settingManager.setting.textSizePosition;
            Text text = objText.AddComponent<Text>();
            text.text = "";
            text.fontSize = CRY_SOLILO.System.settingManager.setting.defaultFontSize;

            GameObject objName = new GameObject("Name");
            objName.transform.SetParent(rectTransform);
            RectTransform rectTransformName = objName.AddComponent<RectTransform>();
            rectTransformName.anchorMin = Vector2.zero;
            rectTransformName.anchorMax = Vector2.one;
            rectTransformName.sizeDelta = CRY_SOLILO.System.settingManager.setting.nameSizeDelta;
            rectTransformName.localPosition = CRY_SOLILO.System.settingManager.setting.nameSizePosition;
            Text name = objName.AddComponent<Text>();
            name.text = "";
            name.fontSize = CRY_SOLILO.System.settingManager.setting.defaultNameFontSize;
            Font font = CRY_SOLILO.System.database.GetFont(CRY_SOLILO.System.settingManager.setting.defaultFontKey);
            if (font != null)
            {
                text.font = font;
                name.font = font;
            }
            text.color = CRY_SOLILO.System.settingManager.setting.defaultFontColor;
            name.color = CRY_SOLILO.System.settingManager.setting.defaultFontColor;
            uiText.textBaseRect = rectTransform;
            uiText.textBaseImage = image;
            uiText.textRect = rectTransformText;
            uiText.text = text;
            uiText.nameRect = rectTransformName;
            uiText.name = name;
        }

        public void HideTextBox()
        {
            if (uiText.textBaseRect == null)
            {
                return;
            }
            Destroy(uiText.textBaseRect.gameObject);
            uiText.textBaseRect = null;
            uiText.textBaseImage = null;
            uiText.textRect = null;
            uiText.text = null;
            uiText.nameRect = null;
            uiText.name = null;
        }

        public Coroutine ShowText(string str, bool add = false)
        {
            if (uiText.text == null)
            {
                return null;
            }
            uiText.textCoro = StartCoroutine(ShowTextIE(str, add));
            return uiText.textCoro;
        }

        public void ClearText()
        {
            if (uiText.text == null)
            {
                return;
            }
            uiText.text.text = "";
        }

        public void SkipText()
        {
            if (uiText.isTextShowing)
            {
                uiText.isTextSkip = true;
            }
        }

        public void ShowName(string name)
        {
            if (uiText.name == null)
            {
                return;
            }
            uiText.name.text = name;
        }

        private IEnumerator ShowTextIE(string str, bool add)
        {
            uiText.isTextShowing = true;
            uiText.isTextSkip = false;
            float wait = 1.0f / uiText.textSpeed;
            int startCount;
            string showString;
            if (add)
            {
                startCount = uiText.text.text.Length;
                showString = uiText.text.text + str;
            }
            else
            {
                startCount = 1;
                showString = str;
            }
            for (int count = startCount; count <= showString.Length; count++)
            {
                if (uiText.text != null)
                {
                    uiText.text.text = showString.Substring(0, count);
                }

                if (uiText.text.preferredHeight > uiText.text.rectTransform.rect.height)
                {
                    showString = showString.Substring(count - 1, showString.Length - count + 1);
                    count = 1;
                }
                if (((wait * count) >= uiText.skipAbleTime) && uiText.isTextSkip)
                {
                    break;
                }
                yield return new WaitForSecondsRealtime(wait);
            }
            uiText.isTextSkip = false;
            uiText.text.text = showString;
            yield return new WaitForSecondsRealtime(uiText.skipAbleTime);
            uiText.isTextShowing = false;
            yield break;
        }


        public void CreateButton(string key, Vector2 position, Vector2 size, Color fontColor, int fontSize, bool autoAjustSize = true, string str = null, string enterKey = null, Button.ButtonClickedEvent clickedEvent = null)
        {
            Sprite buttonSprite = CRY_SOLILO.System.database.GetButton(key);
            Sprite buttonHSprite = null;
            if (enterKey != null)
            {
                buttonHSprite = CRY_SOLILO.System.database.GetButton(enterKey);
            }
            UIButton uibutton = new UIButton();

            GameObject objButtonBase = new GameObject("ButtonBase");
            objButtonBase.transform.SetParent(ButtonParent);
            RectTransform rectTransform = objButtonBase.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.sizeDelta = size;
            rectTransform.localPosition = position;
            Image image = objButtonBase.AddComponent<Image>();
            image.color = Color.white;
            image.sprite = buttonSprite;
            image.type = Image.Type.Sliced;
            if (autoAjustSize)
            {
                image.SetNativeSize();
            }
            Button button = objButtonBase.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.SpriteSwap;
            SpriteState spriteState = new SpriteState();
            spriteState.highlightedSprite = buttonHSprite;
            button.spriteState = spriteState;
            button.onClick = clickedEvent;

            GameObject objText = new GameObject("Text");
            objText.transform.SetParent(rectTransform);
            RectTransform rectTransformText = null;
            Text text = null;
            if (str != null)
            {
                rectTransformText = objText.AddComponent<RectTransform>();
                rectTransformText.anchorMin = Vector2.zero;
                rectTransformText.anchorMax = Vector2.one;
                rectTransformText.sizeDelta = Vector2.zero;
                rectTransformText.localPosition = Vector2.zero;
                text = objText.AddComponent<Text>();
                text.text = str;
                text.fontSize = fontSize;
                Font font = CRY_SOLILO.System.database.GetFont(CRY_SOLILO.System.settingManager.setting.defaultFontKey);
                if (font != null)
                {
                    text.font = font;
                }
                text.color = fontColor;
                text.alignment = TextAnchor.MiddleCenter;
                if (autoAjustSize)
                {
                    Vector2 margin = CRY_SOLILO.System.settingManager.setting.buttonMargin;
                    rectTransform.sizeDelta = new Vector2(text.preferredWidth + margin.x, text.preferredHeight + margin.y);
                    rectTransform.sizeDelta = new Vector2(text.preferredWidth + margin.x, text.preferredHeight + margin.y);
                }
            }
            uibutton.buttonBaseRect = rectTransform;
            uibutton.position = position;
            uibutton.buttonImage = image;
            uibutton.textRect = rectTransformText;
            uibutton.text = text;
            uibutton.button = button;
            buttonList.Add(uibutton);
        }

        public void ClearAllButton()
        {
            foreach (var uiButton in buttonList)
            {
                Destroy(uiButton.buttonBaseRect.gameObject);
            }
            buttonList.Clear();
        }

        public class UIBg
        {
            public string key;
            public RectTransform bgFore, bgBack;
            public Image bgImageFore, bgImageBack;
            public Coroutine bgCoro;

        }

        public class UICharacter
        {
            public string charaKey;
            public string faceKey;
            public Vector2 position;
            public RectTransform charaFore, charaBack;
            public Image charaImageFore, charaImageBack;
            public Coroutine charaCoro;
        }

        public class UIButton
        {
            public Vector2 position;
            public RectTransform buttonBaseRect, textRect;
            public Image buttonImage;
            public Button button;
            public Text text;
        }

        public class UIText
        {
            public RectTransform textBaseRect;
            public Image textBaseImage;
            public int textSpeed = 20;
            public RectTransform textRect;
            public Text text;
            public RectTransform nameRect;
            public Text name;
            public bool isTextSkip = false;
            public bool isTextShowing = false;
            public float skipAbleTime = 0.0f;
            public Coroutine textCoro;
        }
    }
}