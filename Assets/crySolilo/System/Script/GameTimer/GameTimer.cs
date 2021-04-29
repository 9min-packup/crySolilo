using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Room4183Kit
{
    public class GameTimer : MonoBehaviour
    {
        public bool timerSwitch = true;
        [Space]
        public DateTime startDateTime;
        public DateTime nowDateTime;
        public string hhmmss = "00:00:00";
        public double second;
        public double max_second = 3599999d;
        public TimeSpan timeSpan;
        //    [SerializeField] private float secondMax = 3599999;

        // Start is called before the first frame update
        private void Awake()
        {
            RefreshTimer();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (timerSwitch)
            {
                second += Time.fixedDeltaTime;
                if (second > max_second)
                {
                    second = max_second;
                }

                RefreshTimer();
            }
        }

        public void RefreshTimer()
        {
            timeSpan = new TimeSpan(0, 0, (int)second);
            hhmmss = string.Format("{0:00}:{1:00}:{2:00}", (timeSpan.Hours + (timeSpan.Days * 24)), timeSpan.Minutes, timeSpan.Seconds);
        }

        public void ResetTimer()
        {
            second = 0f;
            RefreshTimer();
        }

    }
}