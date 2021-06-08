using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Arcaoid.Gameplay.Chart;
using UnityEngine;
using Arcaoid;
using UnityEngine.UI;
using UnityEngine.Events;
using Lean.Touch;

//using Arcaoid.Compose;

namespace Arcaoid.Gameplay
{
    public class ArcTimingManager : MonoBehaviour
    {
        public static ArcTimingManager Instance { get; private set; }
        public static bool previousarchitred=false;
        public static bool previousarcholdhitred = false;
        public static bool previousarchitreddid=false;
        public static bool previousarchitblue=false;
        public static bool previousarcholdhitblue = false;
        public static bool previousarchitbluedid=false;
        private void Awake()
        {
            allFingers = new List<LeanFinger>();
            Instance = this;
            readytorun = false;
        Shouldrun = false;
            touchPlane = new Plane();
            touchPlane.SetNormalAndPosition(hitPlane.transform.forward, hitPlane.transform.position);
            COMBO = 0;
    }
        private void Start()
        {
            speedShaderId = Shader.PropertyToID("_Speed");
            ArcTimingManager.criticalline = GameObject.FindGameObjectWithTag("CRITLINE");
            

        }

        public int DropRate = 18;
        public float BaseBpm = 100;
        public float DEFAULTBPM;
        public float CURRENTBPM;
        public float currentBPMLINE;
        public Transform BeatlineLayer;
        public GameObject BeatlinePrefab;
        public static GameObject criticalline;
        public GameObject scalenoteline;
        public Plane touchPlane;
        public Transform hitPlane;

        public List<CanClick> noteQuad1;
        public List<CanClick> noteQuad2;
        public List<CanClick> noteQuad3;
        public List<CanClick> noteQuad4;

        public List<GameObject> holdQuad1;
        public List<GameObject> holdQuad2;
        public List<GameObject> holdQuad3;
        public List<GameObject> holdQuad4;

        public List<LeanFinger> allFingers;

        public bool metronome = false;
        public static bool readytorun = false;
        public static bool Shouldrun = false;
      
        public List<ArcTiming> Timings = new List<ArcTiming>();
        public List<float> BPM = new List<float>();
        public SpriteRenderer TrackRenderer;

        private List<float> beatlineTimings = new List<float>();
        private List<SpriteRenderer> beatLineInstances = new List<SpriteRenderer>();
        private List<float> keyTimes = new List<float>();
        private readonly float[] starts = new float[25];
        private readonly float[] ends = new float[25];
        private int pairCount = 0;
        private int speedShaderId = 0;
        public int totaall;
        public bool isplaying=false;
        public static float COMBO;
        public Text COMBOVISIBLE;
        public bool finishedloading = false;
        public bool firstclicked = true;

        public bool IsStopped { get; set; }
        public bool IsBackwarding { get; set; }
        public bool WillBackward { get; set; }
        public float CurrentSpeed { get; set; }
        public float OtherSpeed { get; set; }

        public UnityEvent OnSpeedChange = new UnityEvent();

        private void Update()
        {
         /*if (IsStopped)
         {
             Time.timeScale = 0;
         }
         else if(isplaying&&finishedloading)
         {
             Time.timeScale = 1;
         }
         if (Time.frameCount % 30 == 0)
         {               
             System.GC.Collect();
         }*/

         if (COMBO != 0 && Time.timeScale != 0)
            {
                var temp = Mathf.Ceil(COMBO);
                COMBOVISIBLE.text = temp.ToString();
                if (ArcScoreManager.MAXRECALL < temp)
                {
                    ArcScoreManager.MAXRECALL = Mathf.RoundToInt(temp);
                }
            }else if (COMBO == 0)
            {
                COMBOVISIBLE.text = "";
            } 
            
            if (ReferenceEquals(Timings,null)) return;
            if (Timings.Count == 0) return;           
            UpdateChartSpeedStatus();
           
            UpdateRenderRange();
            UpdateBeatline();
            UpdateTrackSpeed();

          
           // print(CurrentSpeed);
        }

        IEnumerator TAP()
        {
            yield return new WaitForSeconds(60 / DEFAULTBPM);
            ArcEffectManager.Instance.PlayArcSound();
            StartCoroutine(TAP());
        }

        public void Clean()
        {
            IsStopped = false;
            IsBackwarding = false;
            WillBackward = false;
            CurrentSpeed = 0;       
            Timings.Clear();
            TrackRenderer.sharedMaterial.SetFloat(speedShaderId, 0);
            HideExceededBeatlineInstance(0);
        }
        public void Load(List<ArcTiming> timings)
        {
            Timings = timings;
            CalculateBeatlineTimes();
        }
        public void LoadF(List<float> timings)
        {
            BPM = timings;

            var cnt = new Dictionary<float, float>();
                foreach (float value in BPM)
                {
                    if (cnt.ContainsKey(value))
                    {
                        cnt[value]++;
                    }
                    else
                    {
                        cnt.Add(value, 1);
                    }
                }
            float mostCommonValue = 0;
             float highestCount = 0;
                foreach (KeyValuePair<float, float> pair in cnt)
                {
                    if (pair.Value > highestCount)
                    {
                        mostCommonValue = pair.Key;
                    DEFAULTBPM = mostCommonValue;
                    //print(DEFAULTBPM);
                        highestCount = pair.Value;
                    }
                }
        }
        private void HideExceededBeatlineInstance(int quantity)
        {
            int count = beatLineInstances.Count;
            while (count > quantity)
            {
                beatLineInstances[count - 1].enabled = false;
                count--;
            }
        }
        private SpriteRenderer GetBeatlineInstance(int index)
        {
            while (beatLineInstances.Count < index + 1)
            {
                beatLineInstances.Add(Instantiate(BeatlinePrefab, BeatlineLayer).GetComponent<SpriteRenderer>());
            }
            return beatLineInstances[index];
        }
        public void CalculateBeatlineTimes()
        {
            beatlineTimings.Clear();
            HideExceededBeatlineInstance(0);

            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                float segment = Timings[i].Bpm == 0 ? (Timings[i + 1].Timing - Timings[i].Timing) : (60000 / Mathf.Abs(Timings[i].Bpm) * Timings[i].BeatsPerLine);
                if (segment == 0) continue;
                int n = 0;
                float j = Timings[i].Timing;
                while (j < Timings[i + 1].Timing)
                {
                    j = Timings[i].Timing + n++ * segment;
                    beatlineTimings.Add(j);
                }
            }

            if (Timings.Count >= 1)
            {
                float segmentRemain = Timings[Timings.Count - 1].Bpm == 0 ? (ArcGameplayManager.Instance.Length - Timings[Timings.Count - 1].Timing)
              : 60000 / Mathf.Abs(Timings[Timings.Count - 1].Bpm) * Timings[Timings.Count - 1].BeatsPerLine;
                if (segmentRemain != 0)
                {
                    int n = 0;
                    float j = Timings[Timings.Count - 1].Timing;
                    while (j < ArcGameplayManager.Instance.Length)
                    {
                        j = Timings[Timings.Count - 1].Timing + n++ * segmentRemain;
                        beatlineTimings.Add(j);
                    }
                }
            }

            if (Timings.Count >= 1 && Timings[0].Bpm != 0 && Timings[0].BeatsPerLine != 0)
            {
                float t = 0;
                float delta = 60000 / Mathf.Abs(Timings[0].Bpm) * Timings[0].BeatsPerLine;
                int n = 0;
                if (delta != 0)
                {
                    while (t >= -3000)
                    {
                        n++;
                        t = -n * delta;
                        beatlineTimings.Insert(0, t);
                    }
                }
            }
        }

        public float CalculatePositionByTiming(int timing)
        {
            return CalculatePositionByTimingAndStart(ArcGameplayManager.Instance.Timing, timing);
        }
        public float CalculatePositionByTimingAndStart(int startTiming, int timing)
        {
            int offset = ArcAudioManager.Instance.AudioOffset;
            int currentTiming = startTiming > timing ? timing : startTiming;
            int targetTiming = startTiming > timing ? startTiming : timing;
            bool reverse = startTiming > timing;
            float position = 0;
            int start = 0, end = 0;
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (currentTiming >= Timings[i].Timing + offset && currentTiming < Timings[i + 1].Timing + offset) { start = i; break; }
            }
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (targetTiming >= Timings[i].Timing + offset && targetTiming < Timings[i + 1].Timing + offset) { end = i; break; }
            }
            if (Timings.Count != 0)
            {
                if (currentTiming >= Timings[Timings.Count - 1].Timing + offset) start = Timings.Count - 1;
                if (targetTiming >= Timings[Timings.Count - 1].Timing + offset) end = Timings.Count - 1;
            }
            if (start == end)
            {
                position += (targetTiming - currentTiming) * Timings[start].Bpm / BaseBpm * DropRate;
            }
            else
            {
                for (int i = start; i <= end; ++i)
                {
                    if (i == start) position += (Timings[i + 1].Timing + offset - currentTiming) * Timings[i].Bpm / BaseBpm * DropRate;
                    else if (i != start && i != end) position += (Timings[i + 1].Timing - Timings[i].Timing) * Timings[i].Bpm / BaseBpm * DropRate;
                    else if (i == end) position += (targetTiming - Timings[i].Timing - offset) * Timings[i].Bpm / BaseBpm * DropRate;
                }
            }
            return reverse ? -position : position;
        }
        public int CalculateTimingByPosition(float position, int depth = 0)
        {
            int start = 0;
            int end = Timings.Count - 1;
            int breakPos = -1;
            float startPosition = 0;
            float endPosition = position;
            ArcGameplayManager gm = ArcGameplayManager.Instance;
            int offset = ArcAudioManager.Instance.AudioOffset;
            int currentTiming = gm.Timing;
            int songLength = gm.Length;
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (currentTiming >= Timings[i].Timing + offset && currentTiming < Timings[i + 1].Timing + offset) start = i;
            }
            if (currentTiming >= Timings[end].Timing + offset) start = end;

            int depthCount = 0;
            float delta = 0, endTime = 0;
            if (start != end)
            {
                for (int i = start; i <= end; ++i)
                {
                    if (i == start)
                    {
                        delta = (Timings[i + 1].Timing + offset - currentTiming) * (Timings[i].Bpm / BaseBpm) * DropRate;
                        if ((startPosition + delta <= endPosition && startPosition >= endPosition) || (startPosition + delta >= endPosition && startPosition <= endPosition)) { if (depth == depthCount) { breakPos = i; break; } else { depthCount++; } }
                        startPosition += delta;
                    }
                    else if (i != end && i != start)
                    {
                        delta = (Timings[i + 1].Timing - Timings[i].Timing) * (Timings[i].Bpm / BaseBpm) * DropRate;
                        if ((startPosition + delta < endPosition && startPosition > endPosition) || (startPosition + delta > endPosition && startPosition < endPosition)) { if (depth == depthCount) { breakPos = i; break; } else { depthCount++; } }
                        startPosition += delta;
                    }
                    else if (i == end)
                    {
                        delta = (songLength - Timings[i].Timing - offset) * (Timings[i].Bpm / BaseBpm) * DropRate;
                        if ((startPosition + delta < endPosition && startPosition > endPosition) || (startPosition + delta > endPosition && startPosition < endPosition)) { if (depth == depthCount) { breakPos = i; break; } else { depthCount++; } }
                        startPosition += delta;
                    }
                }
            }
            else if (start == end)
            {
                delta = (endPosition - startPosition);
                endTime = delta / ((Timings[end].Bpm / BaseBpm) * DropRate) + currentTiming;
                if (endTime > songLength) return songLength;
                else return (int)endTime;
            }
            if (breakPos == start)
            {
                delta = (endPosition - startPosition);
                if (delta == 0)
                {
                    if (Timings[breakPos].Bpm == 0) endTime = Timings[breakPos].Timing + offset;
                    else endTime = currentTiming;
                }
                else endTime = delta / ((Timings[breakPos].Bpm / BaseBpm) * DropRate) + currentTiming;
            }
            else if (breakPos != -1)
            {
                delta = (endPosition - startPosition);
                endTime = delta / ((Timings[breakPos].Bpm / BaseBpm) * DropRate) + Timings[breakPos].Timing + offset;
            }
            else if (breakPos == -1) endTime = songLength;
            if (endTime > songLength) return songLength;
            else return (int)endTime;
        }
        public float CalculateBpmByTiming(int timing)
        {
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (timing >= Timings[i].Timing && timing < Timings[i + 1].Timing) return Timings[i].Bpm;
            }
            return Timings.Last().Bpm;
        }
        public bool startedupdating = false;
        private void UpdateChartSpeedStatus()
        {
            int offset = ArcAudioManager.Instance.AudioOffset;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            IsBackwarding = false;
            IsStopped = false;
            WillBackward = false;
            
            if (Timings.Count == 0)
            {
                CurrentSpeed = 1;
                return;
            }
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (currentTiming >= Timings[i].Timing + offset && currentTiming < Timings[i + 1].Timing + offset)
                {
                    CurrentSpeed = Timings[i].Bpm / BaseBpm;
                    CurrentSpeed = Timings[i].Bpm / BaseBpm;
                    currentBPMLINE = Timings[i].BeatsPerLine;
                    CURRENTBPM = Timings[i].Bpm;
                    startedupdating = true;

                    if (prevspeed != CurrentSpeed)
                    {
                        prevspeed = CurrentSpeed;
                        OnSpeedChange.Invoke();
                    }

                    if (CurrentSpeed < 0) IsBackwarding = true;
                    else if (CurrentSpeed == 0) IsStopped = true;
                    if (IsStopped)
                    {
                        int k = i;
                        while (k < Timings.Count - 1)
                        {
                            k++;
                            if (Timings[k].Bpm / BaseBpm < 0)
                            {
                                WillBackward = true;
                                break;
                            }
                            else if (Timings[k].Bpm / BaseBpm > 0)
                            {
                                WillBackward = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        WillBackward = false;
                    }
                    
                    return;
                }
            }
            if (currentTiming >= Timings[Timings.Count - 1].Timing + offset)
            {
                CurrentSpeed = Timings[Timings.Count - 1].Bpm / BaseBpm;
                currentBPMLINE =  Timings[Timings.Count - 1].BeatsPerLine;
                CURRENTBPM = Timings[Timings.Count-1].Bpm;

                if (CurrentSpeed < 0) IsBackwarding = true;
                else if (CurrentSpeed == 0) IsStopped = true;
            }
           
        }

        public float prevspeed;

        /*private void UpdateOtherSpeed()
        {
            int offset = ArcAudioManager.Instance.AudioOffset;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            IsBackwarding = false;
            IsStopped = false;
            WillBackward = false;

            if (Timings.Count == 0)
            {
                OtherSpeed = 1;
                return;
            }
            for (int i = 0; i < Timings.Count - 1; ++i)
            {
                if (currentTiming >= Timings[i].Timing  && currentTiming < Timings[i + 1].Timing )
                {
                    OtherSpeed = Timings[i].Bpm / BaseBpm;
                    if (OtherSpeed < 0) IsBackwarding = true;
                    else if (OtherSpeed == 0) IsStopped = true;
                    if (IsStopped)
                    {
                        int k = i;
                        while (k < Timings.Count - 1)
                        {
                            k++;
                            if (Timings[k].Bpm / BaseBpm < 0)
                            {
                                WillBackward = true;
                                break;
                            }
                            else if (Timings[k].Bpm / BaseBpm > 0)
                            {
                                WillBackward = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        WillBackward = false;
                    }
                    return;
                }
            }
            if (currentTiming >= Timings[Timings.Count - 1].Timing)
            {
                OtherSpeed = Timings[Timings.Count - 1].Bpm / BaseBpm;
                if (OtherSpeed < 0) IsBackwarding = true;
                else if (OtherSpeed == 0) IsStopped = true;
            }
        }*/

        private void UpdateRenderRange()
        {
            pairCount = 0;
            keyTimes.Clear();

            keyTimes.Add(ArcGameplayManager.Instance.Timing);
            keyTimes.Add(ArcGameplayManager.Instance.Timing);

            keyTimes.Add(CalculateTimingByPosition(0, 0));
            keyTimes.Add(CalculateTimingByPosition(100000, 0));

            for (int i = 0; i < keyTimes.Count; i += 2)
            {
                starts[pairCount] = keyTimes[i];
                ends[pairCount] = keyTimes[i + 1];
                pairCount++;
            }
        }
        private void UpdateBeatline()
        {
            int index = 0;
            int offset = ArcAudioManager.Instance.AudioOffset;
            foreach (float t in beatlineTimings)
            {
                if (!ShouldRender((int)(t + offset), 0))
                {
                    continue;
                }
                SpriteRenderer s = GetBeatlineInstance(index);
                s.enabled = true;
                float z = CalculatePositionByTiming((int)(t + offset)) / 1000f;               
                var tempp = new Vector3(0, 0, -z);
                s.transform.localPosition = Vector3.SlerpUnclamped(transform.localPosition,tempp,1);
                s.transform.localScale = new Vector3(1700, 20 + z);
                index++;
            }
            HideExceededBeatlineInstance(index);
        }
        private void UpdateTrackSpeed()
        {
            TrackRenderer.sharedMaterial.SetFloat(speedShaderId, ArcGameplayManager.Instance.IsPlaying ? CurrentSpeed : 0);
        }

        public void Add(ArcTiming timing)
        {
            Timings.Add(timing);
            Timings.Sort((ArcTiming a, ArcTiming b) => a.Timing.CompareTo(b.Timing));
        }
        public void Remove(ArcTiming timing)
        {
            Timings.Remove(timing);
        }

        public bool ShouldRender(int timing, int delay = 0)
        {
            for (int i = 0; i < pairCount; ++i)
            {
                
                if (timing >= (starts[i]) - delay && timing <= (ends[i])) return true;
            }
            return false;
        }

        public bool ShouldRendArc(int timing, int delay = 0)
        {
            if (Mathf.Sign(CurrentSpeed) == 1)
            {
                for (int i = 0; i < pairCount; ++i)
                {

                    if (timing >= (starts[i]) - delay && timing <= (ends[i]) && ((timing - ArcGameplayManager.Instance.Timing <= (93000 / DropRate) && timing - ArcGameplayManager.Instance.Timing >= (-93000 / DropRate)))) { return true; }


                }
            }
            
            return false;
        }

        public bool ShouldRenderr(int timing, int delay = 0)
        {
            for (int i = 0; i < pairCount; ++i)
            {
                if (timing >= (starts[i]+300) - delay && timing <= (ends[i])) return true;
            }
            return false;
        }

        public bool ShouldRenderrr(int timing, int delay = 0)
        {
            for (int i = 0; i < pairCount; ++i)
            {

                if (timing >= (starts[i]) - delay && timing <= (ends[i]))
                {
                    return true;
                }
               

               
            }
            return false;
        }


    }
}