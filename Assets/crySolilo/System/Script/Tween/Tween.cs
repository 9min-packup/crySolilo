﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    public static class Tween
    {
        public delegate void OnTweekEnd();

        public static Coroutine Linear(MonoBehaviour behaviour, Transform trs, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action = null, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(LinearIE(trs, from, to, t, firstWait, action, inRealTime));
        }

        public static Coroutine Quadratic(MonoBehaviour behaviour, Transform trs, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action = null, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(QuadraticIE(trs, from, to, t, firstWait, action, inRealTime));
        }

        public static Coroutine Spring(MonoBehaviour behaviour, Transform trs, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action = null, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(SpringIE(trs, from, to, t, firstWait, f, action, inRealTime));
        }

        public static Coroutine SwingLoop(MonoBehaviour behaviour, Transform trs, Vector3 from, Vector3 to, float t, float firstWait, float f, bool inRealTime = false)
        {
            return behaviour.StartCoroutine(SwingLoopIE(trs, from, to, t, firstWait, f, inRealTime));
        }

        public static Coroutine VelocityLinear(MonoBehaviour behaviour, Rigidbody rb, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityLinearIE(rb, from, to, t, firstWait, action));
        }

        public static Coroutine VelocityLinear(MonoBehaviour behaviour, Rigidbody2D rb, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityLinearIE(rb, from, to, t, firstWait, action));
        }

        public static Coroutine VelocityQuadratic(MonoBehaviour behaviour, Rigidbody rb, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityQuadraticIE(rb, from, to, t, firstWait, action));
        }

        public static Coroutine VelocityQuadratic(MonoBehaviour behaviour, Rigidbody2D rb, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityQuadraticIE(rb, from, to, t, firstWait, action));
        }

        public static Coroutine VelocityConstMag(MonoBehaviour behaviour, Rigidbody rb, Vector3 from, Vector3 to, float vMagnitude, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityConstMagIE(rb, from, to, vMagnitude, action));
        }

        public static Coroutine VelocityConstMag(MonoBehaviour behaviour, Rigidbody2D rb, Vector2 from, Vector2 to, float vMagnitude, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocityConstMagIE(rb, from, to, vMagnitude, action));
        }

        public static Coroutine VelocitySpring(MonoBehaviour behaviour, Rigidbody rb, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocitySpringIE(rb, from, to, t, firstWait, f, action));
        }

        public static Coroutine VelocitySpring(MonoBehaviour behaviour, Rigidbody2D rb2d, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action = null)
        {
            return behaviour.StartCoroutine(VelocitySpringIE(rb2d, from, to, t, firstWait, f, action));
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

        private static IEnumerator LinearIE(Transform trs, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action, bool inRealTime = false)
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
                    trs.position = CalcPosByLinear(from, to, time, t);
                    time = GetNowTime(inRealTime) - startTimer;
                    yield return null;
                }
            }
            trs.position = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator QuadraticIE(Transform trs, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action, bool inRealTime = false)
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
                    trs.position = CalcPosByQuadratic(from, to, time, t);
                    time = GetNowTime(inRealTime) - startTimer;
                    yield return null;
                }
            }
            trs.position = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator SpringIE(Transform trs, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action, bool inRealTime = false)
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
                    trs.position = CalcPosBySpring(from, to, time, t, f, 1.0f / t);
                    time = GetNowTime(inRealTime) - startTimer;
                    yield return null;
                }
            }
            trs.position = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator SwingLoopIE(Transform trs, Vector3 from, Vector3 to, float t, float firstWait, float f, bool inRealTime = false)
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
                while (true)
                {
                    trs.position = CalcPosBySpring(from, from + (to - from) / 2.0f, time, t, f, 0.0f);
                    time = GetNowTime(inRealTime) - startTimer;
                    time %= 2 * Mathf.PI;
                    yield return null;
                }
            }
            trs.position = to;
            yield break;
        }

        private static IEnumerator VelocityLinearIE(Rigidbody rb, Vector3 from, Vector3 to, float t, float firstWait, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            rb.position = from;
            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb.velocity = CalcVelByLinear(rb.position, to, time, t);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb.velocity = Vector3.zero;

            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocityLinearIE(Rigidbody2D rb2d, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            rb2d.position = from;
            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb2d.velocity = CalcVelByLinear(rb2d.position, to, time, t);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb2d.velocity = Vector2.zero;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocityConstMagIE(Rigidbody rb, Vector3 from, Vector3 to, float vMagnitude, OnTweekEnd action)
        {
            rb.position = from;
            if (vMagnitude > 0.0f)
            {
                float startTimer = Time.time;
                float time = 0.0f;
                float maxT = ((to - rb.position).magnitude / vMagnitude) * 2.0f;
                Vector3 diff = to - rb.position;
                while ((diff.magnitude > 0.0f) && (time <= maxT))
                {
                    Vector3 velocity = (to - rb.position).normalized * vMagnitude;
                    if (velocity.magnitude > diff.magnitude)
                    {
                        velocity = diff;
                    }
                    rb.velocity = velocity;

                    diff = to - rb.position;
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb.velocity = Vector3.zero;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocityConstMagIE(Rigidbody2D rb2d, Vector2 from, Vector2 to, float vMagnitude, OnTweekEnd action)
        {
            rb2d.position = from;
            if (vMagnitude > 0.0f)
            {
                float startTimer = Time.time;
                float time = 0.0f;
                float maxT = ((to - rb2d.position).magnitude / vMagnitude) * 2.0f;
                Vector2 diff = to - rb2d.position;
                while ((diff.magnitude > 0.0f) && (time <= maxT))
                {
                    Vector2 velocity = (to - rb2d.position).normalized * vMagnitude;
                    if (velocity.magnitude > diff.magnitude)
                    {
                        velocity = diff;
                    }
                    rb2d.velocity = velocity;

                    diff = to - rb2d.position;
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb2d.velocity = Vector2.zero;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocityQuadraticIE(Rigidbody2D rb2d, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            rb2d.position = from;
            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb2d.velocity = CalcVelByQuadratic(rb2d.position, to, 0.0f, t - time);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb2d.velocity = Vector2.zero;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocityQuadraticIE(Rigidbody rb, Vector2 from, Vector2 to, float t, float firstWait, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            rb.position = from;
            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb.velocity = CalcVelByQuadratic(rb.position, to, 0.0f, t - time);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb.velocity = Vector2.zero;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocitySpringIE(Rigidbody rb, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb.velocity = t * (CalcPosBySpring(from, to, time, t, f, 1.0f / t) - rb.position);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb.position = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }

        private static IEnumerator VelocitySpringIE(Rigidbody2D rb2d, Vector3 from, Vector3 to, float t, float firstWait, float f, OnTweekEnd action)
        {
            yield return new WaitForSeconds(firstWait);

            float startTimer = Time.time;
            float time = 0.0f;
            if (t > 0.0f)
            {
                while (time <= t)
                {
                    rb2d.velocity = t * (CalcPosBySpring(from, to, time, t, f, 1.0f / t) - (Vector3)rb2d.position);
                    time = Time.time - startTimer;
                    yield return null;
                }
            }
            rb2d.position = to;
            if (action != null)
            {
                action();
            }
            yield break;
        }


        private static Vector3 CalcPosByLinear(Vector3 from, Vector3 to, float t, float maxT)
        {
            if (maxT <= 0)
            {
                return to;
            }
            else
            {
                return from + ((to - from) * (t / maxT));
            }
        }

        private static Vector3 CalcPosByQuadratic(Vector3 from, Vector3 to, float t, float maxT)
        {
            if (maxT <= 0)
            {
                return to;
            }
            else
            {
                Vector3 A = (to - from) / (maxT * maxT);
                float B = (-1) * (maxT - t) * (maxT - t);
                return to + A * B;
            }
        }

        private static Vector3 CalcPosBySpring(Vector3 from, Vector3 to, float t, float maxT, float f, float zeta)
        {
            if (maxT <= 0)
            {
                return to;
            }
            else
            {
                Vector3 diffV = to - from;
                return to - (diffV * (Mathf.Cos(f * t)) * Mathf.Exp((-zeta) * f * t));
            }
        }

        private static Vector3 CalcVelByLinear(Vector3 from, Vector3 to, float t, float maxT)
        {
            if (t >= maxT)
            {
                return Vector3.zero;
            }
            else
            {
                return (to - from) / (maxT - t);
            }
        }

        private static Vector3 CalcVelByQuadratic(Vector3 from, Vector3 to, float t, float maxT)
        {
            if (t >= maxT)
            {
                return Vector3.zero;
            }
            else
            {
                Vector3 A = (to - from) / (maxT * maxT);
                float B = (-2) * (t - maxT);
                return A * B;
            }
        }

        private static Vector3 CalcVelBySpring(Vector3 from, Vector3 to, float t, float maxT, float f, float zeta)
        {
            if (maxT <= 0)
            {
                return Vector3.zero;
            }
            else
            {
                Vector3 diffV = to - from;
                return diffV * Mathf.Exp((-zeta) * f * t) * f * (((zeta) * Mathf.Cos(f * t)) + (Mathf.Sin(f * t)));
            }
        }

    }
}
