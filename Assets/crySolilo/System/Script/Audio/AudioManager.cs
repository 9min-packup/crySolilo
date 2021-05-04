using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CrySolilo
{
    public class AudioManager : MonoBehaviour
    {
        [Range(0.0f, 1.0f), SerializeField]
        private float bgmVolume = 1.0f;
        [Range(0.0f, 1.0f), SerializeField]
        private float bgsVolume = 1.0f;
        [Range(0.0f, 1.0f), SerializeField]
        private float seVolume = 1.0f;

        public AudioMixer mixer;

        public string bgmKey;
        public string bgsKey;

        public AudioSource BGM_Speaker, BGS_Speaker;
        public AudioSource[] SE_Speakers;
        private Coroutine bgmCoro, bgsCoro;

        public bool isBgmFade, isBgsFade;

        private void Awake()
        {
            SetBgmVolume(bgmVolume);
            SetBgsVolume(bgsVolume);
            SetSeVolume(seVolume);
        }

        public float ToPercent(float volume)
        {
            return Mathf.Pow(10f, volume / 20f);
        }
        public float ToVolume(float percent)
        {
            float decibel = 20f * Mathf.Log10(percent);
            return Mathf.Max(decibel, -80f);
        }

        public void SetBgmVolume(float percent)
        {
            bgmVolume = percent;
            mixer.SetFloat("BGM", ToVolume(percent));
        }
        public void SetBgsVolume(float percent)
        {
            bgsVolume = percent;
            mixer.SetFloat("BGS", ToVolume(percent));
        }
        public void SetSeVolume(float percent)
        {
            seVolume = percent;
            mixer.SetFloat("SE", ToVolume(percent));
        }

        public void PlayBGM(string key, bool isLoop, float volume, float pitch)
        {
            AudioClip audioClip = CRY_SOLILO.System.database.GetBgm(key);
            if (audioClip == null)
            {
                return;
            }
            Play(BGM_Speaker, audioClip, isLoop, volume, pitch);
        }

        public void PlayBGM(string key, bool isLoop, float volume, float pitch, float fadeOut, float wait, float fadeIn)
        {
            AudioClip audioClip = CRY_SOLILO.System.database.GetBgm(key);
            if (audioClip == null)
            {
                return;
            }
            PlayBGM(BGM_Speaker, audioClip, isLoop, BGM_Speaker.volume, volume, pitch, fadeOut, wait, fadeIn);
        }

        public void StopBGM()
        {
            Stop(BGM_Speaker);
        }

        public void StopBGM(float fadeOut)
        {
            StopBGM(BGM_Speaker, BGM_Speaker.volume, fadeOut);
        }

        public void PlayBGS(string key, bool isLoop, float volume, float pitch)
        {
            if (bgmKey == key)
            {
                return;
            }
            AudioClip audioClip = CRY_SOLILO.System.database.GetBgs(key);
            if (audioClip == null)
            {
                return;
            }
            bgmKey = key;
            Play(BGM_Speaker, audioClip, isLoop, volume, pitch);
        }

        public void PlayBGS(string key, bool isLoop, float volume, float pitch, float fadeOut, float wait, float fadeIn)
        {
            if (bgsKey == key)
            {
                return;
            }
            AudioClip audioClip = CRY_SOLILO.System.database.GetBgs(key);
            if (audioClip == null)
            {
                return;
            }
            bgsKey = key;
            PlayBGS(BGS_Speaker, audioClip, isLoop, BGS_Speaker.volume, volume, pitch, fadeOut, wait, fadeIn);
        }

        public void StopBGS()
        {
            bgmKey = "";
            Stop(BGM_Speaker);
        }

        public void StopBGS(float fadeOut)
        {
            bgsKey = "";
            StopBGS(BGS_Speaker, BGS_Speaker.volume, fadeOut);
        }

        public void PlaySE(string key, int index, bool isLoop, float volume, float pitch)
        {
            if (index < 0 || index >= SE_Speakers.Length)
            {
                Debug.LogWarning("SE Speaker Index Out of bounds.  ");
            }

            AudioClip audioClip = CRY_SOLILO.System.database.GetSe(key);
            if (audioClip == null)
            {
                return;
            }
            Play(SE_Speakers[index], audioClip, isLoop, volume, pitch);
        }

        public void StopSE()
        {
            int index = 0;
            if (index < 0 || index >= SE_Speakers.Length)
            {
                Debug.LogWarning("SE Speakers is empty.  ");
            }

            Stop(SE_Speakers[index]);
        }


        public void StopSE(int index)
        {
            if (index < 0 || index >= SE_Speakers.Length)
            {
                Debug.LogWarning("SE Speaker Index Out of bounds.  ");
            }

            Stop(SE_Speakers[index]);
        }

        private void Play(AudioSource source, AudioClip clip, bool isLoop, float volume, float pitch)
        {
            source.clip = clip;
            source.loop = isLoop;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }

        private void Stop(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.loop = false;
        }


        private void PlayBGM(AudioSource source, AudioClip clip, bool isLoop, float volumeFrom, float volumeTo, float pitch, float fadeOut, float wait, float fadeIn)
        {
            if (bgmCoro != null && isBgmFade)
            {
                StopCoroutine(bgmCoro);
            }

            bgmCoro = StartCoroutine(PlayWithFadeBGM(source, clip, isLoop, volumeFrom, volumeTo, pitch, fadeOut, wait, fadeIn));
        }

        private void PlayBGS(AudioSource source, AudioClip clip, bool isLoop, float volumeFrom, float volumeTo, float pitch, float fadeOut, float wait, float fadeIn)
        {
            if (bgsCoro != null && isBgsFade)
            {
                StopCoroutine(bgsCoro);
            }

            bgsCoro = StartCoroutine(PlayWithFadeBGS(source, clip, isLoop, volumeFrom, volumeTo, pitch, fadeOut, wait, fadeIn));
        }

        private void StopBGM(AudioSource source, float volumeFrom, float fadeOut)
        {
            if (bgmCoro != null && isBgmFade)
            {
                StopCoroutine(bgmCoro);
            }

            bgmCoro = StartCoroutine(StopWithFadeBGM(source, volumeFrom, fadeOut));
        }

        private void StopBGS(AudioSource source, float volumeFrom, float fadeOut)
        {
            if (bgsCoro != null && isBgsFade)
            {
                StopCoroutine(bgsCoro);
            }

            bgsCoro = StartCoroutine(StopWithFadeBGS(source, volumeFrom, fadeOut));
        }

        private IEnumerator PlayWithFadeBGM(AudioSource source, AudioClip clip, bool isLoop, float volumeFrom, float volumeTo, float pitch, float fadeOut, float wait, float fadeIn)
        {
            isBgmFade = true;

            if (fadeOut <= 0.0f)
            {
                source.Stop();
            }
            else
            {
                //fade out
                float from = Mathf.Clamp(volumeFrom, 0.0f, 1.0f);
                float to = 0.0f;
                float diff = to - from;
                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }

                source.volume = to;
            }

            if (wait > 0.0f)
            {
                float startTime, timer = 0.0f;
                startTime = Time.time;
                while (timer <= wait)
                {
                    timer = Time.time - startTime;
                    yield return null;
                }
            }
            source.clip = clip;
            source.loop = isLoop;
            source.pitch = pitch;
            source.Play();
            if (fadeIn <= 0.0f)
            {
                source.volume = 1.0f;
            }
            else
            {
                //fade int
                float from = 0.0f;
                float to = Mathf.Clamp(volumeTo, 0.0f, 1.0f);
                float diff = to - from;

                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }
                source.volume = to;
            }
            isBgmFade = false;
            yield break;
        }

        private IEnumerator PlayWithFadeBGS(AudioSource source, AudioClip clip, bool isLoop, float volumeFrom, float volumeTo, float pitch, float fadeOut, float wait, float fadeIn)
        {

            if (fadeOut <= 0.0f)
            {
                source.Stop();
            }
            else
            {
                //fade out
                float from = Mathf.Clamp(volumeFrom, 0.0f, 1.0f);
                float to = 0.0f;
                float diff = to - from;
                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }
                source.volume = to;
            }

            if (wait > 0.0f)
            {
                float startTime, timer = 0.0f;
                startTime = Time.time;
                while (timer <= wait)
                {
                    timer = Time.time - startTime;
                    yield return null;
                }
            }
            source.clip = clip;
            source.loop = isLoop;
            source.pitch = pitch;
            source.Play();

            if (fadeIn <= 0.0f)
            {
                source.volume = 1.0f;
            }
            else
            {
                //fade int
                float from = 0.0f;
                float to = Mathf.Clamp(volumeTo, 0.0f, 1.0f);
                float diff = to - from;

                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }
                source.volume = to;
            }
            isBgsFade = false;
            yield break;
        }


        private IEnumerator StopWithFadeBGM(AudioSource source, float volumeFrom, float fadeOut)
        {
            isBgmFade = true;

            if (fadeOut <= 0.0f)
            {
                source.Stop();
            }
            else
            {
                //fade out
                float from = Mathf.Clamp(volumeFrom, 0.0f, 1.0f);
                float to = 0.0f;
                float diff = to - from;
                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }

                source.volume = to;
            }

            source.Stop();
            source.clip = null;
            source.loop = false;
            isBgmFade = false;
            yield break;
        }

        private IEnumerator StopWithFadeBGS(AudioSource source, float volumeFrom, float fadeOut)
        {

            if (fadeOut <= 0.0f)
            {
                source.Stop();
            }
            else
            {
                //fade out
                float from = Mathf.Clamp(volumeFrom, 0.0f, 1.0f);
                float to = 0.0f;
                float diff = to - from;
                float startTime, timer = 0.0f;
                startTime = Time.time;
                source.volume = from;
                while (timer <= fadeOut)
                {
                    source.volume = from + (diff * (timer / fadeOut));
                    timer = Time.time - startTime;
                    yield return null;
                }
                source.volume = to;
            }

            source.Stop();
            source.clip = null;
            source.loop = false;
            isBgsFade = false;
            yield break;
        }


    }
}