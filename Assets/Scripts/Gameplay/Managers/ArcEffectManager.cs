using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using E7.Native;



namespace Arcaoid.Gameplay
{
    public class ArcEffectManager : MonoBehaviour
    {
        public static ArcEffectManager Instance { get; private set; }
        public NativeAudioPointer NAtap;
        public NativeAudioPointer NAarc;
        private void Awake()
        {
            Instance = this;
            if (!Application.isEditor)
            {
                NAtap = NativeAudio.Load(TapAudio);
                NAarc = NativeAudio.Load(ArcAudio);
            }
   
            
        }
        private void Start()
        {
            for (int i = 0; i < 10; ++i)
            {
                ArcTapNoteEffectComponent n = Instantiate(TapNoteJudgeEffect, EffectLayer).GetComponent<ArcTapNoteEffectComponent>();
                tapNoteEffectInstances.Add(n);
                ArcTapNoteEffectComponent I = Instantiate(TapNoteTIMEEffect, EffectLayer).GetComponent<ArcTapNoteEffectComponent>();
                tapNoteTIME.Add(I);
            }
        }

        public GameObject TapNoteJudgeEffect;
        public GameObject TapNoteTIMEEffect;
        public GameObject[] LaneHits = new GameObject[4];
        public ParticleSystem[] HoldNoteEffects = new ParticleSystem[4];
        public Transform EffectLayer;
        public AudioClip TapAudio, ArcAudio;
        public AudioClip RECSOUND;
        public AudioSource Source;

        private bool[] holdEffectStatus = new bool[4];
        private List<ArcTapNoteEffectComponent> tapNoteEffectInstances = new List<ArcTapNoteEffectComponent>();
        private List<ArcTapNoteEffectComponent> tapNoteTIME = new List<ArcTapNoteEffectComponent>();

        public ArcTapNoteEffectComponent GetTapNoteEffectInstance()
        {
            foreach (var i in tapNoteEffectInstances) if (i.Available) return i;
            ArcTapNoteEffectComponent n = Instantiate(TapNoteJudgeEffect, EffectLayer).GetComponent<ArcTapNoteEffectComponent>();
            tapNoteEffectInstances.Add(n);
            return n;
        }

        public ArcTapNoteEffectComponent GetTapNoteEffectTIME()
        {
            foreach (var i in tapNoteTIME) if (i.Available) return i;
            ArcTapNoteEffectComponent I = Instantiate(TapNoteTIMEEffect, EffectLayer).GetComponent<ArcTapNoteEffectComponent>();
            tapNoteTIME.Add(I);
            return I;
        }

        public void SetHoldNoteEffect(int track, bool show)
        {
            if (holdEffectStatus[track - 1] != show)
            {
                holdEffectStatus[track - 1] = show;
                if (show)
                {
                    HoldNoteEffects[track - 1].Play();
                }
                else
                {
                    HoldNoteEffects[track - 1].Stop();
                    HoldNoteEffects[track - 1].Clear();
                }
                //LaneHits[track - 1].SetActive(show);
            }
        }

        public void PlayHOLDat(bool isArc = false)
        {         
            if(LOADMENU.CLICKENABLED)
            AudioSource.PlayClipAtPoint(isArc ? ArcAudio : TapAudio,new Vector3(0,0,0));
        }

        public void PlayTapNoteEffectAt(Vector2 pos, bool isfar, float TIME, bool isArc = false)
        {
            ArcTapNoteEffectComponent a = GetTapNoteEffectInstance();
            a.PlayAt(pos);
            //AudioSource.PlayClipAtPoint(isArc ? ArcAudio : TapAudio, new Vector3(0, 0, 0));

            StartCoroutine(TAPNOTEFX(TIME,isfar,isArc));
            
        }

        public IEnumerator TAPNOTEFX(float TIME, bool isfar,bool isArc = false)
        {
            yield return new WaitForSecondsRealtime(0.0f);
            if (!Application.isEditor)
            {
                if (LOADMENU.CLICKENABLED)
                {
                    if(!isfar&&TIME>= ArcGameplayManager.Instance.Timing)
                    yield return new WaitUntil(() => TIME-150f <= ArcGameplayManager.Instance.Timing);

                    if (isArc) { NAarc.Play(NativeAudio.PlayOptions.defaultOptions); } else { NAtap.Play(NativeAudio.PlayOptions.tapOptions); }
                }
            }
            else
            {
                if (LOADMENU.CLICKENABLED)
                {
                    if(!isfar)
                    yield return new WaitUntil(() => TIME - 150f <= ArcGameplayManager.Instance.Timing);

                    Source.PlayOneShot(isArc ? ArcAudio : TapAudio);
                }
            }
        }

        

        public void PlayTIMEEffectAt(Vector2 pos, string type, float time, string EorL = "NULL")
        {
            string orl = EorL;
            if (orl != "NULL")
            {
                ArcTapNoteEffectComponent F = GetTapNoteEffectTIME();
                F.StartCoroutine(F.PlayTIME(pos, type, time, orl));
                orl = "NULL";
            }
            ArcTapNoteEffectComponent a = GetTapNoteEffectTIME();
            a.StartCoroutine(a.PlayTIME(pos,type,time,orl));
            
        }

        public void PlayTapSound()
        {
            if (!Application.isEditor)
            {
                if (LOADMENU.CLICKENABLED)
                    NAtap.Play(NativeAudio.PlayOptions.tapOptions);
            }
            else
            {
                if (LOADMENU.CLICKENABLED)
                    Source.PlayOneShot(TapAudio);
            }



        }

        public IEnumerator PlayHOLDSound(float TIME)
        {
            yield return new WaitForSecondsRealtime(0.0f);
            if (!Application.isEditor)
            {
                if (LOADMENU.CLICKENABLED)
                {
                    yield return new WaitUntil(() => TIME - 150f <= ArcGameplayManager.Instance.Timing);
                    NAtap.Play(NativeAudio.PlayOptions.tapOptions);
                }
            }
            else
            {
                if (LOADMENU.CLICKENABLED)
                {
                    yield return new WaitUntil(() => TIME - 150f <= ArcGameplayManager.Instance.Timing);
                    Source.PlayOneShot(TapAudio);
                }
            }

        }

        public void PlayArcSound()
        {
            if (Application.isEditor)
            {
                if (LOADMENU.CLICKENABLED)
                    Source.PlayOneShot(ArcAudio);
            }

            else
            {
                if (LOADMENU.CLICKENABLED)
                    NAarc.Play(NativeAudio.PlayOptions.defaultOptions);
            }

        }
        public void ResetJudge()
        {
            for (int i = 1; i < 5; ++i) SetHoldNoteEffect(i, false);
        }
    }
}