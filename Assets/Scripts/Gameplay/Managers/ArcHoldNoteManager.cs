using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using Lean.Touch;

namespace Arcaoid.Gameplay
{
    public class ArcHoldNoteManager : MonoBehaviour
    {
        public static ArcHoldNoteManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        [HideInInspector]
        public List<ArcHold> Holds = new List<ArcHold>();
        [HideInInspector]
        public readonly float[] Lanes = { 6.375f, 2.125f, -2.125f, -6.375f };
        public GameObject HoldNotePrefab;
        public Transform NoteLayer;
        public Sprite DefaultSprite, HighlightSprite;
        public float LASTBPM;
        public ArcTimingManager ATMAN;
        public LeanFingerHeld LFH;
        bool notset = true;

        public void Clean()
        {
            foreach (var t in Holds) t.Destroy();
            Holds.Clear();
        }
        public void Load(List<ArcHold> holds)
        {
            Holds = holds;
            /*foreach (var t in Holds)
            {
                //t.Instantiate();
                StartCoroutine(HOLDS(t));
               
            }*/
        }

        public IEnumerator HOLDS(ArcHold t)
        {
            int offset = ArcAudioManager.Instance.AudioOffset;
            ArcTimingManager timing = ArcTimingManager.Instance;
            int duration = t.EndTiming - t.Timing;
            t.duriationdec = duration;

           // yield return new WaitUntil(() => t.Timing - (1060.5 / ArcTimingManager.Instance.CurrentSpeed) <= ArcGameplayManager.Instance.Timing && ArcTimingManager.Instance.CurrentSpeed != 0 && !ArcTimingManager.Instance.IsBackwarding);
            yield return new WaitUntil(() => ArcTimingManager.Instance.ShouldRender(t.Timing + offset) && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding);
           
            
            if (ReferenceEquals(t.TPOBJ, null))
            {





                // float pos = (-95.994f);


                t.Position = timing.CalculatePositionByTiming(t.Timing + offset);
                float endPosition = timing.CalculatePositionByTiming(t.EndTiming + offset);

                //t.Enable = true;
                float pos = t.Position / 1000f;
                float length = (endPosition - t.Position) / 1000f;
                //var T=  Instantiate(HoldNotePrefab, new Vector3(Lanes[t.Track - 1], 0, pos), Quaternion.Euler(-90, 0, 0), NoteLayer);
                t.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("HoldNote");
                var T = t.TPOBJ;

                if (ArcTimingManager.Instance.firstclicked)
                {
                    T.transform.position = new Vector3(Lanes[t.Track - 1], 0, -pos);
                }
                else
                {
                    T.transform.position = new Vector3(Lanes[t.Track - 1], 0, pos - 200f);
                }
                T.transform.rotation = Quaternion.Euler(-90, 0, 0);
                GameObject fingerTRACK=LFH.Lane1Hit;
                switch (t.Track)
                {
                    case 1:
                        ATMAN.holdQuad1.Add(T);
                        fingerTRACK = LFH.Lane1Hit;
                        break;
                    case 2:
                        ATMAN.holdQuad2.Add(T);
                        fingerTRACK = LFH.Lane2Hit;
                        break;
                    case 3:
                        ATMAN.holdQuad3.Add(T);
                        fingerTRACK = LFH.Lane3Hit;
                        break;
                    case 4:
                        ATMAN.holdQuad4.Add(T);
                        fingerTRACK = LFH.Lane4Hit;
                        break;

                }

                T.SetActive(true);
                T.transform.parent = NoteLayer;
                //t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                T.transform.localScale = new Vector3(1.53f, length / 3.79f, 1);
                T.GetComponent<SpriteRenderer>().sprite = DefaultSprite;



               

                var SHOLD = T.transform.gameObject.GetComponent<shrinkhold>();
                SHOLD.HOLD = t;
                SHOLD.lfhTRACK = fingerTRACK;
                SHOLD.distance = T.transform.localScale.y;
                SHOLD.duriation = duration / 1000f;
                SHOLD.From = 0f;
                SHOLD.starttime = t.Timing;
                SHOLD.endtime = t.EndTiming;
                SHOLD.Track = t.Track;
            }
        }

        public void Add(ArcHold hold)
        {
            hold.Instantiate();
            Holds.Add(hold);
        }
        public void Remove(ArcHold hold)
        {
            hold.Destroy();
            Holds.Remove(hold);
        }

        private void Update()
        {
            
            if (ReferenceEquals(Holds, null)) return;
            //print(Holds.Count);
           /* if (notset && ArcTimingManager.Instance.isplaying)
            {
                notset = false;
                LASTBPM = ArcTimingManager.Instance.CURRENTBPM;
                //print("set");
            }*/

            if (notset==true && ArcTimingManager.readytorun == true)
            {
                notset = false;
                
                foreach(var t in Holds)
                {
                   
                    StartCoroutine(HOLDS(t));
                   
                }
                //print("set");
            }

            /*int offset = ArcAudioManager.Instance.AudioOffset;
            ArcTimingManager timing = ArcTimingManager.Instance;
            foreach (var t in Holds)
            {
                int duration = t.EndTiming - t.Timing;
                t.duriationdec = duration;
                if(t.TPOBJ != null && ArcTimingManager.Instance.ShouldRender(t.Timing + offset) && LASTBPM != ArcTimingManager.Instance.CURRENTBPM && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
                {

                    LASTBPM = ArcTimingManager.Instance.CURRENTBPM;
                        float pos= timing.CalculatePositionByTiming(t.Timing + offset);
                        pos = pos / 1000f;
                       // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, pos);
                    
                }
                else if (t.TPOBJ == null && ArcTimingManager.Instance.ShouldRender(t.Timing + offset)&&(t.Timing!=t.EndTiming) && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
                {





                    // float pos = (-95.994f);

                    
                    t.Position = timing.CalculatePositionByTiming(t.Timing + offset);
                    float endPosition = timing.CalculatePositionByTiming(t.EndTiming + offset);

                    //t.Enable = true;
                    float pos = t.Position / 1000f;
                    float length = (endPosition - t.Position) / 1000f;
                    //var T=  Instantiate(HoldNotePrefab, new Vector3(Lanes[t.Track - 1], 0, pos), Quaternion.Euler(-90, 0, 0), NoteLayer);
                    t.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("HoldNote");
                    var T = t.TPOBJ;

                    if (ArcTimingManager.Instance.firstclicked)
                    {
                        T.transform.position = new Vector3(Lanes[t.Track - 1], 0, -pos);
                    }
                    else
                    {
                        T.transform.position = new Vector3(Lanes[t.Track - 1], 0, pos - 200f);
                    }
                    T.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    T.SetActive(true);
                    T.transform.parent = NoteLayer;
                    //t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                    T.transform.localScale = new Vector3(1.53f, length / 3.79f, 1);
                    T.GetComponent<SpriteRenderer>().sprite = DefaultSprite;



                    //t.boxCollider.center = new Vector3(0, t.boxCollider.size.y / 2);

                    var SHOLD = T.transform.gameObject.GetComponent<shrinkhold>();
                    SHOLD.HOLD = t;
                    SHOLD.distance = T.transform.localScale.y;                  
                    SHOLD.duriation = duration / 1000f;
                    SHOLD.From = 0f;
                    SHOLD.starttime = t.Timing;
                    SHOLD.endtime = t.EndTiming;
                    SHOLD.Track = t.Track;
                }
            }*/

            //if (ArcGameplayManager.Instance.Auto) JudgeHoldNotes();
           // RenderHoldNotes();
            
        }

        public void RenderHoldNotes()
        {
            ArcTimingManager timing = ArcTimingManager.Instance;
            int offset = ArcAudioManager.Instance.AudioOffset;

            foreach (var t in Holds)
            {
                var SHOLD = t.transform.gameObject.GetComponent<shrinkhold>();
                
                int duration = t.EndTiming - t.Timing;
                SHOLD.duriation = duration/1000f;
                SHOLD.starttime = t.Timing;
                SHOLD.endtime = t.EndTiming;
                SHOLD.Track = t.Track;
               /* if (!timing.ShouldRender(t.Timing + offset, duration + 120) || t.Judged)
                {
                    t.Enable = false;
                    continue;
                }*/
                
                t.Position = timing.CalculatePositionByTiming(t.Timing + offset);
                float endPosition = timing.CalculatePositionByTiming(t.EndTiming + offset);
                /*if (t.Position > 100000 || endPosition < -10000)
                {
                    t.Enable = false;
                    continue;
                }*/
                t.Enable = true;
                float pos = t.Position / 1000f;
                float length = (endPosition - t.Position) / 1000f;
                t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                t.transform.localScale = new Vector3(1.53f, length / 3.79f, 1);
                SHOLD.distance = t.transform.localScale.y;
                t.boxCollider.center = new Vector3(0, t.boxCollider.size.y / 2);

                float alpha = 1;
               /* if (t.Judging)
                {
                    t.From = Mathf.Clamp((-pos / length), 0, 1);
                    t.FlashCount = (t.FlashCount + 1) % 4;
                    if (t.FlashCount == 0) alpha = 0.85f;
                    t.Highlight = true;
                }
                else
                {
                    t.From = 0;
                    alpha = pos < 0 ? 0.5f : 1;
                    t.Highlight = false;
                }*/
                t.Alpha = alpha * 0.8627451f;
                //t.To = Mathf.Clamp((100 - pos) / length, 0, 1);
                //t.transform.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, GameObject.FindGameObjectWithTag("TimingManager").GetComponent<ArcTimingManager>().DropRate*1.4f);
            }
        }
        private void JudgeHoldNotes()
        {
            ArcTimingManager timing = ArcTimingManager.Instance;
            int offset = ArcAudioManager.Instance.AudioOffset;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            foreach (var t in Holds)
            {
                if (t.Judged) continue;
                if (currentTiming >= t.Timing + offset && currentTiming <= t.EndTiming + offset)
                {
                    t.Judging = true;
                    if (!t.AudioPlayed)
                    {
                        if (ArcGameplayManager.Instance.IsPlaying && t.ShouldPlayAudio) ArcEffectManager.Instance.PlayTapSound();
                        t.AudioPlayed = true;
                    }
                    ArcEffectManager.Instance.SetHoldNoteEffect(t.Track, true);
                }
                else if (currentTiming > t.EndTiming + offset)
                {
                    t.Judging = false;
                    t.Judged = true;
                    t.AudioPlayed = true;
                    ArcEffectManager.Instance.SetHoldNoteEffect(t.Track, false);
                }
                else
                {
                    t.ShouldPlayAudio = true;
                }
            }
        }
    }
}