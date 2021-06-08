using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using Arcaoid.Gameplay;
using UnityEngine.Events;
using Arcaoid.Gameplay.Events;
using UnityEngine.EventSystems;

namespace Arcaoid.Gameplay
{
    namespace Events
    {
        public class OnMusicFinishedEvent : UnityEvent
        {

        }
    }

    public class ArcGameplayManager : MonoBehaviour
    {
        public static ArcGameplayManager Instance { get; private set; }

        public ShaderVariantCollection Shaders;
        //public ShaderVariantCollection PotatoShaders;
        private void Awake()
        {
            Instance = this;
            JudgeOffset = LOADMENU.judgevalue;
        }
        private void Start()
        {
            Shaders.WarmUp();
            //PotatoShaders.WarmUp();
        }

        public float JudgeOffset;
        public static bool hascameramove = false;
        public bool Auto { get; set; }
        public bool IsPlaying { get; set; }
        public float timing;
        public int Timing
        {
            get
            {
                return (int)(timing * 1000);
            }
            set
            {
                timing = value / 1000f;
                ArcAudioManager.Instance.Timing = timing;
            }
        }
        public float Timingf
        {
            get
            {
                return timing;
            }
            set
            {
                timing = value;
                ArcAudioManager.Instance.Timing = timing;
            }
        }
        public int Length { get; private set; }
        public bool IsLoaded
        {
            get
            {
                return Chart != null;
            }
        }

        public UnityEvent OnChartLoad = new UnityEvent();
        public OnMusicFinishedEvent OnMusicFinished = new OnMusicFinishedEvent();
        public ArcChart Chart { get; set; }

        private double lastDspTime = 0;
        private double deltaDspTime = 0;

        private void Update()
        {
            deltaDspTime = AudioSettings.dspTime - lastDspTime;
            lastDspTime = AudioSettings.dspTime;
            if (IsPlaying)
            {
                timing += Time.deltaTime;
                float t = ArcAudioManager.Instance.Timing;
                float delta = timing - t;
                if (Mathf.Abs(delta) > 0.016f) timing = t;
            }
            if (Timing > Length)
            {
                //OnMusicFinished.Invoke();
                //Stop();
            }
        }
        public bool Load(ArcChart chart, AudioClip audio)
        {
            if (audio == null || chart == null) return false;

            Clean();
            Chart = chart;
            Length = (int)(audio.length * 1000);

            ArcCameraManager.Instance.ResetCamera();
            ArcAudioManager.Instance.Load(audio, chart.AudioOffset);
            ArcTimingManager.Instance.Load(chart.Timings);
            ArcTimingManager.Instance.LoadF(chart.TBPM);
            ArcTapNoteManager.Instance.Load(chart.Taps);
            ArcHoldNoteManager.Instance.Load(chart.Holds);
            ArcArcManager.Instance.Load(chart.Arcs);
            ArcHoldManager.Instance.Load(chart.ArcHolds);
            ArcCameraManager.Instance.Load(chart.Cameras);
            ArcSpecialManager.Instance.Load(chart.Specials);

            if (chart.Cameras.Count == 0)
            {
                hascameramove = false;
            }
            else
            {
                hascameramove = true;
            }

            OnChartLoad.Invoke();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            //GameObject.FindGameObjectWithTag("TapNoteManager").GetComponent<ArcTapNoteManager>().RenderTapNotes();
            // GameObject.FindGameObjectWithTag("HoldNoteManager").GetComponent<ArcHoldNoteManager>().RenderHoldNotes();
            // GameObject.FindGameObjectWithTag("ArcManager").GetComponent<ArcArcManager>().RenderArcs();

            return true;
           
        }
        public void Clean()
        {
            Timing = 0;
            ArcTimingManager.Instance.Clean();
            ArcTapNoteManager.Instance.Clean();
            ArcHoldNoteManager.Instance.Clean();
            ArcArcManager.Instance.Clean();
            ArcCameraManager.Instance.Clean();
            ArcSpecialManager.Instance.Clean();
            Chart = null;
            Length = 0;
        }

        public void ResetJudge()
        {
            if (Chart != null)
            {
                foreach (var t in Chart.Arcs) { foreach (var a in t.ArcTaps) { a.Judged = false; a.Judging = false; } t.Judged = false; t.Judging = false; t.AudioPlayed = false; };
                foreach (var t in Chart.Holds) { t.Judged = false; t.Judging = false; t.AudioPlayed = false; };
                foreach (var t in Chart.Taps) { t.Judged = false; t.Judging = false; };
            }
            ArcEffectManager.Instance.ResetJudge();
        }
        public void PlayDelayed(float TIM)
        {
            /*timing = -TIM;
            ResetJudge();
            ArcAudioManager.Instance.Source.Stop();
            ArcAudioManager.Instance.Source.time = 0;
            ArcAudioManager.Instance.Source.PlayDelayed(TIM);
            IsPlaying = true;*/
            ArcAudioManager.Instance.Play();
            ArcAudioManager.Instance.SourceACTUAL.PlayDelayed(TIM);
            ArcAudioManager.Instance.Timing = timing;
            IsPlaying = true;
        }

        public void Play()
        {
            ArcAudioManager.Instance.Play();
            ArcAudioManager.Instance.SourceACTUAL.Play();
            ArcAudioManager.Instance.Timing = timing;
            IsPlaying = true;
        }

        public void Playother()
        {
            ArcAudioManager.Instance.Play();
           // ArcAudioManager.Instance.SourceACTUAL.Play();
            ArcAudioManager.Instance.Timing = timing;
            IsPlaying = true;
        }

        public void Pause()
        {
            ArcAudioManager.Instance.Pause();
            ArcAudioManager.Instance.SourceACTUAL.Pause();
            IsPlaying = false;
            ResetJudge();
        }
        public void Stop()
        {
            timing = 0;
            ArcAudioManager.Instance.Pause();
            ArcAudioManager.Instance.Timing = 0;
            IsPlaying = false;
        }

        public ArcNote FindNoteByInstance(GameObject gameObject)
        {
            foreach (var tap in Chart.Taps) if (tap.Instance.Equals(gameObject)) return tap;
            foreach (var hold in Chart.Holds) if (hold.Instance.Equals(gameObject)) return hold;
            foreach (var arc in Chart.Arcs)
            {
                if (arc.IsMyself(gameObject)) return arc;
                foreach (var arctap in arc.ArcTaps)
                {
                    if (arctap.IsMyself(gameObject)) return arctap;
                }
            }
            return null;
        }
    }
}
