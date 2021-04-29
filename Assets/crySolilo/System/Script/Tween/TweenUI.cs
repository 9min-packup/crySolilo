using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrySolilo
{
    public static class TweenUI
    {
        public static Coroutine Fade(MonoBehaviour behaviour, Image image, Color from, Color to, float t, float firstWait, Tween.OnTweekEnd action = null, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(FadeIE(image, from, to, t, firstWait, action, inRealTime));
        }

        private static IEnumerator FadeIE(Image image, Color from, Color to, float t, float firstWait, Tween.OnTweekEnd action, bool inRealTime)
        {
            if (inRealTime)
            {
                yield return new WaitForSecondsRealtime(firstWait);

            }
            else
            {
                yield return new WaitForSeconds(firstWait);
            }
            float startTimer = GetNowTime(inRealTime);
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    image.color = CalcColorByLinear(from, to, time, t);
                    time = GetNowTime(inRealTime) - startTimer;
                    yield return null;
                }
            }
            image.color = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        public static Coroutine ClossFade(MonoBehaviour behaviour, Image imageA, Image imageB, Color from, Color to, float t, float firstWait, Tween.OnTweekEnd action = null, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(ClossFadeFadeIE(imageA, imageB, from, to, t, firstWait, action, inRealTime));
        }

        private static IEnumerator ClossFadeFadeIE(Image imageA, Image imageB, Color from, Color to, float t, float firstWait, Tween.OnTweekEnd action, bool inRealTime)
        {
            if (inRealTime)
            {
                yield return new WaitForSecondsRealtime(firstWait);

            }
            else
            {
                yield return new WaitForSeconds(firstWait);
            }
            float startTimer = GetNowTime(inRealTime);
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    imageA.color = CalcColorByLinear(to, from, time, t);
                    imageB.color = CalcColorByLinear(from, to, time, t);
                    time = GetNowTime(inRealTime) - startTimer;
                    yield return null;
                }

            }
            imageA.color = from;
            imageB.color = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static float GetNowTime(bool inRealTime = false)
        {
            if (inRealTime)
            {
                return Time.time;
            }
            else
            {
                return Time.realtimeSinceStartup;
            }

        }

        private static Color CalcColorByLinear(Color from, Color to, float t, float maxT)
        {
            if (maxT <= 0)
            {
                return to;
            }
            else
            {
                float r = from.r + ((to.r - from.r) * (t / maxT));
                float g = from.g + ((to.g - from.g) * (t / maxT));
                float b = from.b + ((to.b - from.b) * (t / maxT));
                float a = from.a + ((to.a - from.a) * (t / maxT));

                return new Color(r, b, g, a);
            }
        }
    }
}