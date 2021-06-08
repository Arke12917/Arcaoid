using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using SecPlayerPrefs;

namespace Arcaoid.Gameplay
{
    public class ArcTapNoteManager : MonoBehaviour
    {
        public static ArcTapNoteManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            ATMAN = ArcTimingManager.Instance;
            AUMAN = ArcAudioManager.Instance;
            alternaterender = SecurePlayerPrefs.GetBool("#ALTREND");
        }

        [HideInInspector]
        public List<ArcTap> Taps;
        [HideInInspector]
        public readonly float[] Lanes = { 6.375f, 2.125f, -2.125f, -6.375f };
        public GameObject TapNotePrefab;
        public Transform NoteLayer;
        public Transform Notebody;
        public Material ShaderdMaterial;
        public float LASTBPM;
        bool notset = true;
        public Transform CRITLINE;
        public ArcTimingManager ATMAN;
        public ArcAudioManager AUMAN;
        public bool alternaterender = false;

        public bool canload = false;

        public void Clean()
        {
            //foreach (var t in Taps) t.Destroy();
            //Taps.Clear();
        }
        public void Load(List<ArcTap> taps)
        {
            Taps = taps;
            /* foreach (var t in Taps)
             {
                 //t.Instantiate();
                /StartCoroutine(TAPS(t));

             }*/
        }
        public Transform Scalenoteline;
        public IEnumerator TAPS(ArcTap t)
        {

            yield return new WaitUntil(() => ATMAN.ShouldRender(t.Timing + AUMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
            /*if (alternaterender)
                yield return new WaitUntil(() => ATMAN.ShouldRendArc(t.Timing + AUMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
            else
                yield return new WaitUntil(() => ATMAN.ShouldRender(t.Timing + AUMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);*/


            if (ReferenceEquals(t.TPOBJ, null))
            {
                t.correctioncount = 0;
                t.cantransform = false;
                var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                POSS = POSS / 1000f;

                // yield return new WaitForSeconds((((t.Timing-(30.9761904762f*ATMAN.DropRate))/1000f)));


                // yield return new WaitForSeconds((((t.Timing - ((53f / ATMAN.DropRate) * 1000f) + (AUMAN.AudioOffset)) / 1000f)));
                //t.Instantiate();
                float pos = POSS;
                // t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                // t.Enable = true;
                //t.Alpha = 1f;
                //var T= Instantiate(TapNotePrefab,new Vector3(Lanes[t.Track - 1],0, pos),Quaternion.Euler(-90,0,0),NoteLayer);

                t.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("TapNote");
                var T = t.TPOBJ;
                //T.transform.localScale = new Vector3(1.54f,3.8f,1);
                var Ck = T.GetComponent<CanClick>();
                Ck.TAP = t;


                T.transform.parent = NoteLayer;

                T.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(pos));



                T.transform.rotation = Quaternion.Euler(-90, 0, 0);
                ATMAN.noteQuad1.Add(Ck);
                ATMAN.noteQuad2.Add(Ck);
                ATMAN.noteQuad3.Add(Ck);
                ATMAN.noteQuad4.Add(Ck);
                T.SetActive(true);
                //T.GetComponent<shrinktap>().timetime = t.Timing;

                //StartCoroutine(CONNECTION(T,t));



            }


        }

        public IEnumerator SetupArcTapConnection(ArcArcTap ATAP, ArcTap t)
        {
            yield return new WaitForSeconds(0.0f);
            // print("waited!");
            // if (ATAP.Arc == null || (ATAP.Arc.EndTiming - ATAP.Arc.Timing) == 0) yield return null;


            LineRenderer l = Instantiate(ArcArcManager.Instance.ConnectionPrefab, t.transform).GetComponent<LineRenderer>();
            float p = 1f * (t.Timing - ATAP.Arc.Timing) / (ATAP.Arc.EndTiming - ATAP.Arc.Timing);
            Vector3 pos = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(ATAP.Arc.XStart, ATAP.Arc.XEnd, p, ATAP.Arc.LineType)),
                                         ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(ATAP.Arc.YStart, ATAP.Arc.YEnd, p, ATAP.Arc.LineType)) - 0.5f)
                                         - new Vector3(ArcArcManager.Instance.Lanes[t.Track - 1], 0);
            l.SetPosition(1, new Vector3(pos.x, 0, pos.y));
            //l.startColor = l.endColor = ArcArcManager.Instance.ConnectionColor;
            //l.startColor = l.endColor = new Color(l.endColor.r, l.endColor.g, l.endColor.b, t.Alpha * 0.8f);
            l.enabled = true;
            l.transform.localPosition = new Vector3();


        }

        public void Add(ArcTap tap)
        {
            tap.Instantiate();
            Taps.Add(tap);
            //tap.SetupArcTapConnection();
        }
        public void Remove(ArcTap tap)
        {
            tap.Destroy();
            Taps.Remove(tap);
        }

        bool notcheck = false;

        private void Update()
        {

            if (ReferenceEquals(Taps, null)) return;

            if (notcheck == false && ArcTimingManager.readytorun == true)
            {
                notcheck = true;
                foreach (var t in Taps)
                {
                    StartCoroutine(TAPS(t));

                    /*var renderrange = 100000;
                    if (Time.deltaTime<5f)
                    {
                        renderrange = 200000;
                    }
                    else
                    {
                        renderrange = 100000;
                    }

                    if (ATMAN.Timings[0].Bpm > 210)
                    {
                        renderrange = 200000;
                    }
                    // print(POSS);
                    if (t.TPOBJ != null && t.infirstclicked==true && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                    {
                        // if (ATMAN.IsBackwarding)
                        //{
                        //t.TPOBJ.transform.parent = null;

                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                        POSS = POSS / 1000f;



                        var tempvector = new Vector3(1.53f, (2 + (0.01978021978f * ATMAN.DropRate)) + 9f * POSS / 100f, 1);
                        if (tempvector.y < 26.5f && tempvector.y > 0)
                            t.TPOBJ.transform.localScale = tempvector;
                        /*if (t.correctioncount < 20)
                        {

                            t.correctioncount += 1;
                        }
                        else
                        {
                            print("corrected");
                            t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS+1.5f));
                            //t.correctioncount = 0;
                        }
                        // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS+1.5f));

                        //t.TPOBJ.transform.localScale = new Vector3(1.53f, 3.8f - 0.01f * POSS / 100f, 1);






                        // else if (ATMAN.IsStopped)
                        // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, (POSS - 100f));
                    }
                    else if (t.TPOBJ != null && t.cantransform && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                    {
                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                        POSS = POSS / 1000f;
                        //t.TPOBJ.transform.localScale = new Vector3(1.53f, 3.8f -0.01f * POSS / 100f, 1);

                     var tempvector= new Vector3(1.53f, (2 + (0.01978021978f * ATMAN.DropRate)) + 9f * POSS / 100f, 1);
                        if(tempvector.y<26.5f&&tempvector.y>0)
                        t.TPOBJ.transform.localScale = tempvector;
                    }
                   else if (t.TPOBJ != null&& ATMAN.ShouldRenderr(t.Timing + AUMAN.AudioOffset)&&!ATMAN.firstclicked&&!t.infirstclicked && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                    {
                        // if (ATMAN.IsBackwarding)
                        //{
                        //t.TPOBJ.transform.parent = null;

                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);                
                        POSS = POSS / 1000f;
                        var tempint = 3;
                        if (ATMAN.CURRENTBPM > 999)
                        {
                            //print("corrected");
                            // print(t.Timing);

                            t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS + 1.5f));
                            t.cantransform = true;
                        }
                        else
                        {
                            if (t.correctioncount < ((t.Timing + 1115)/3))
                            //if (t.correctioncount < (4))
                            {

                                t.correctioncount += (t.Timing) / Time.deltaTime;
                            }
                            else
                            {
                               // print("corrected");
                                // print(t.Timing);

                                t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS + 1.5f));
                                t.cantransform = true;
                                //t.correctioncount = 0;
                            }
                        }
                        // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS+1.5f));






                        //LASTBPM = ATMAN.CURRENTBPM;
                        // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, (-POSS));
                        //t.TPOBJ.transform.parent = NoteLayer;
                        //t.TPOBJ.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        //}


                        // else if (ATMAN.IsStopped)
                        // t.TPOBJ.transform.position = new Vector3(Lanes[t.Track - 1], 0, (POSS - 100f));
                    }
                    else if (t.TPOBJ == null && (((t.Timing<=(renderrange/ ATMAN.DropRate)) && ATMAN.firstclicked) && !ATMAN.IsBackwarding && !ATMAN.IsStopped)|| t.Timing==0)
                    {
                         //print("ok");
                        //t.correctioncount = -999999999;
                        t.infirstclicked = true;
                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                        POSS = POSS / 1000f;
                        // yield return new WaitForSeconds((((t.Timing-(30.9761904762f*ATMAN.DropRate))/1000f)));


                        // yield return new WaitForSeconds((((t.Timing - ((53f / ATMAN.DropRate) * 1000f) + (AUMAN.AudioOffset)) / 1000f)));
                        //t.Instantiate();
                        float pos = POSS;
                        // t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                        // t.Enable = true;
                        //t.Alpha = 1f;
                        //var T= Instantiate(TapNotePrefab,new Vector3(Lanes[t.Track - 1],0, pos),Quaternion.Euler(-90,0,0),NoteLayer);

                        t.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("TapNote");
                        var T = t.TPOBJ;
                        //T.transform.localScale = new Vector3(1.53f, 3.8f, 1);
                        T.GetComponent<CanClick>().TAP = t;


                        T.transform.parent = NoteLayer;

                        T.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(POSS));


                        T.transform.rotation = Quaternion.Euler(-90, 0, 0);

                        T.SetActive(true);
                        T.GetComponent<CanClick>().canclick = true;
                        //T.GetComponent<shrinktap>().timetime = t.Timing;

                        //StartCoroutine(CONNECTION(T, t));

                    }
                    else if ( t.TPOBJ == null&& ATMAN.ShouldRenderr(t.Timing + AUMAN.AudioOffset)&&!ATMAN.firstclicked && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                    {
                        t.correctioncount = 0;
                        t.cantransform = false;
                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                        POSS = POSS / 1000f;
                        // yield return new WaitForSeconds((((t.Timing-(30.9761904762f*ATMAN.DropRate))/1000f)));


                        // yield return new WaitForSeconds((((t.Timing - ((53f / ATMAN.DropRate) * 1000f) + (AUMAN.AudioOffset)) / 1000f)));
                        //t.Instantiate();
                        float pos = POSS;
                        // t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                        // t.Enable = true;
                        //t.Alpha = 1f;
                        //var T= Instantiate(TapNotePrefab,new Vector3(Lanes[t.Track - 1],0, pos),Quaternion.Euler(-90,0,0),NoteLayer);

                        t.TPOBJ= ObjectPooler.SharedInstance.GetPooledObject("TapNote");
                        var T = t.TPOBJ;
                        //T.transform.localScale = new Vector3(1.54f,3.8f,1);
                        T.GetComponent<CanClick>().TAP=t;


                        T.transform.parent = NoteLayer;
                        if (ATMAN.firstclicked)
                        {
                            T.transform.position = new Vector3(Lanes[t.Track - 1], 0, -(pos-50f));
                        }
                        else
                        {
                            T.transform.position = new Vector3(Lanes[t.Track - 1], 0, (pos-600));
                        }

                        T.transform.rotation = Quaternion.Euler(-90, 0, 0);

                        T.SetActive(true);
                        //T.GetComponent<shrinktap>().timetime = t.Timing;

                        //StartCoroutine(CONNECTION(T,t));



                    }*/
                }

                //if (ArcGameplayManager.Instance.Auto) JudgeTapNotes();
                //RenderTapNotes();
            }
            if (!ATMAN.IsBackwarding && !ATMAN.IsStopped)
            {
                foreach (var t in Taps)
                {
                    if (!(ReferenceEquals(t.TPOBJ, null)))
                    {
                        var POSS = ATMAN.CalculatePositionByTiming(t.Timing + AUMAN.AudioOffset);
                        POSS = POSS / 1000f;



                        var tempvector = new Vector3(1.53f, (2 + (0.01978021978f * ATMAN.DropRate)) + 9f * POSS / 100f, 1);
                        if (tempvector.y < 26.5f && tempvector.y > 0)
                            t.TPOBJ.transform.localScale = tempvector;
                    }
                }
            }
        }

        IEnumerator CONNECTION(GameObject T, ArcTap t)
        {
            yield return new WaitForSeconds(1.12f);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("ARCTAPPOOL");
            foreach (GameObject ATAP in objs)
            {
                var ATP = ATAP.GetComponent<unparent>();


                if ((ATP.timing == t.Timing))
                {
                    //Debug.Log("InTime");
                    //StartCoroutine(SetupArcTapConnection(ATP.AATAP, t));
                    var ARCTAP = ATAP.transform.GetChild(0);

                    // Vector3 P = Vector3.Lerp(ATAP.transform.position, T.transform.position, 0.5f / (ATAP.transform.position-T.transform.position).magnitude());


                    LineRenderer l = Instantiate(ArcArcManager.Instance.ConnectionPrefab, new Vector3(0, 0, 0), Quaternion.identity, (t.transform)).GetComponent<LineRenderer>();
                    t.LINE = l;

                    //LineRenderer l = T.AddComponent<LineRenderer>();
                    // Set the width of the Line Renderer

                    l.transform.parent = Notebody;
                    l.positionCount = 2;
                    var tempos = new Vector3(ARCTAP.position.x + 1.3f, ARCTAP.position.y, ARCTAP.position.z);
                    var temposs = new Vector3(T.transform.position.x - 1.3f, T.transform.position.y, T.transform.position.z);
                    l.SetPosition(1, tempos);
                    l.SetPosition(0, temposs);




                }
            }
        }

        public void RenderTapNotes()
        {
            ArcTimingManager timing = ATMAN;
            int offset = AUMAN.AudioOffset;

            foreach (var t in Taps)
            {
                //var SHOLD = t.transform.gameObject.GetComponent<shrinktap>();

                /*if (!timing.ShouldRender(t.Timing + offset) || t.Judged)
                {
                    t.Enable = false;
                    continue;
                }*/
                t.Position = timing.CalculatePositionByTiming(t.Timing + offset);
                //SHOLD.time = t.Position;
                /* if (t.Position > 100000 || t.Position < -10000)
                 {
                     t.Enable = false;
                     continue;
                 }*/
                t.Enable = true;
                float pos = t.Position / 1000f;
                t.transform.localPosition = new Vector3(Lanes[t.Track - 1], pos, 0);
                //if (ArcCameraManager.Instance.EditorCamera)
                t.transform.localScale = new Vector3(1.53f, 2.5f, 1);
                //else
                // t.transform.localScale = new Vector3(1.53f, 2f + 5.1f * pos / 100f, 1);
                t.Alpha = pos < 90 ? 1 : (100 - pos) / 10f;
                //t.transform.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, GameObject.FindGameObjectWithTag("TimingManager").GetComponent<ArcTimingManager>().DropRate*1.4f);
                //t.OptimizeMaterial();

            }
        }
        private void JudgeTapNotes()
        {
            ArcTimingManager timing = ATMAN;
            int offset = AUMAN.AudioOffset;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            foreach (var t in Taps)
            {
                if (t.Judged) continue;
                if (currentTiming > t.Timing + offset && currentTiming <= t.Timing + offset + 150)
                {
                    t.Judged = true;
                    // if (ArcGameplayManager.Instance.IsPlaying) ArcEffectManager.Instance.PlayTapNoteEffectAt(new Vector2(Lanes[t.Track - 1], 0));
                }
                else if (currentTiming > t.Timing + offset + 150)
                {
                    t.Judged = true;
                }
            }
        }
    }
}