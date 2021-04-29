using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrySolilo
{

    public class UIManager : MonoBehaviour
    {
        public RectTransform bgParent, charaParent, textParent;
        private UIBg uibg = new UIBg();
        private Dictionary<string, UICharacter> uicharaDict = new Dictionary<string, UICharacter>();
        private UIText uiText = new UIText();


        public void ShowBg(string key, float time = 1.0f)
        {
            Sprite bgSprite = CRY_SOLILO.System.database.GetBg(key);
            if (bgSprite == null)
            {
                return;
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
        }

        public void ClearBG(float time = 1.0f)
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
                return;
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

        }

        public void ShowCaracter(string key, string faceKey, Vector2 position, float time = 1.0f, bool isClossFade = true)
        {
            Sprite charaSprite = CRY_SOLILO.System.database.GetCharacterFace(key, faceKey);
            if (charaSprite == null)
            {
                return;
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
        }

        public void HideCharacter(string key, float time = 1.0f)
        {
            UICharacter uichara;
            if (uicharaDict.ContainsKey(key))
            {
                uichara = uicharaDict[key];
            }
            else
            {
                return;
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
                return;
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
            rectTransformText.localPosition = Vector2.zero;
            Text text = objText.AddComponent<Text>();
            text.text = "";
            text.fontSize = CRY_SOLILO.System.settingManager.setting.defaultFontSize;
            Font font = CRY_SOLILO.System.database.GetFont(CRY_SOLILO.System.settingManager.setting.defaultFontKey);
            if (font != null)
            {
                text.font = font;
            }
            text.color = CRY_SOLILO.System.settingManager.setting.defaultFontColor;
            uiText.textBaseRect = rectTransform;
            uiText.textBaseImage = image;
            uiText.textRect = rectTransformText;
            uiText.text = text;
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
        }

        public void ShowText(string str)
        {


        }

        public IEnumerator ShowTextIE(string str)
        {

            yield break;
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

        public class UIText
        {
            public RectTransform textBaseRect;
            public Image textBaseImage;
            public int textSpeed = 20;
            public RectTransform textRect;
            public Text text;
            public Coroutine textCoro;
        }
    }
}