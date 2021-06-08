using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using System.Linq;
using Arcaoid.Compose;
using SecPlayerPrefs;

namespace Arcaoid.Gameplay
{
    public class ArcHoldManager : MonoBehaviour
    {
        public static ArcHoldManager Instance { get; private set; }

     
        private void Awake()
        {
            Instance = this;
            //ATMAN = ArcTimingManager.Instance;
            //AMAN = ArcAudioManager.Instance;
        }

        //[HideInInspector]
        public List<ArcArcHold> ArcHolds = new List<ArcArcHold>();
        public ArcTimingManager ATMAN;

        public GameObject ArcNotePrefab;
        public Transform ArcLayer;
        public Transform NoteLayer;
        public Color ConnectionColor;
        public Texture2D ArcTapSkin;
        public bool readytocalc = false;
        public readonly float[] Lanes = { 6.375f, 2.125f, -2.125f, -6.375f };
        public float LASTBPM;
        bool notset = true;
        public ArcAudioManager AMAN;

        //[HideInInspector]
        public float ArcJudgePos;

        public void Clean()
        {
            foreach (var t in ArcHolds) t.Destroy();
           // Arcs.Clear();
        }
        public void Load(List<ArcArcHold> archolds)
        {
            ArcHolds = archolds;
             //foreach (var t in Arcs) t.Instantiate();
           /* foreach (var t in Arcs) {

                StartCoroutine(ARCARCS(t));
               
            }*/
            /*foreach (var t in Arcs)
            {
                foreach(ArcArcTap ATAP in t.ArcTaps)
                {
                    StartCoroutine(ARCTAPS(ATAP,t));
                }
            }*/
            
           CalculateArcRelationship();
        }

        public IEnumerator ARCARCS(ArcArcHold archold)
        {
            //yield return new WaitForSeconds(0.0f);

            // yield return new WaitUntil(() => arc.Timing - (1060.5 / ATMAN.CurrentSpeed) <= ArcGameplayManager.Instance.Timing&&ATMAN.CurrentSpeed!=0 && !ATMAN.IsBackwarding);
            archold.initialpos= new Vector3(ArcAlgorithm.ArcXToWorld(archold.XStart), ArcAlgorithm.ArcYToWorld(archold.YStart));
            yield return new WaitUntil(() => ATMAN.ShouldRender(archold.Timing + AMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
            
            archold.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("ARCHOLD");
            var T = archold.TPOBJ;
            T.transform.parent = ArcLayer;
            var ARENDERER = T.GetComponent<ArcHoldRenderer>();
            archold.AREND = ARENDERER;
            ARENDERER.arc = archold;
           // var M = Instantiate(ARENDERER.MVPREFAB, new Vector3(0, 0, 0), Quaternion.identity);
            var M = ObjectPooler.SharedInstance.GetPooledObject("CAPHOLD");
            M.transform.position = archold.initialpos;
            M.SetActive(true);
            var MM = M.GetComponent<movecaphold>();
            archold.MCAP = MM;
            MM.duriation = ((archold.EndTiming - (19.565f*archold.COMBO)) / 1000f) - (archold.Timing / 1000f);
            var S = M.GetComponent<SpriteRenderer>();
            MM.parentArc = ARENDERER;
            MM.Acholds = archold.ArcHoldGroup;
            ARENDERER.ActualCap = M;
            ARENDERER.ArcCap = M.transform;
            ARENDERER.ArcCapRenderer = S;
            ARENDERER.MVCAP = MM;
            if (archold.firstarc)
            {
                ARENDERER.HeadRenderer.enabled = true;
                ARENDERER.HeightIndicatorRenderer.enabled = true;
            }
            else { ARENDERER.HeadRenderer.enabled = false; ARENDERER.HeightIndicatorRenderer.enabled = false; }
            //ARENDERER.ENACP.DNHEAD.enabled = true;
            //ARENDERER.HeadRenderer.enabled = true;
            T.SetActive(true);

            ARENDERER.MVCAP.START = archold.Timing;
            ARENDERER.MVCAP.END = archold.EndTiming;
            ARENDERER.MVCAP.COMBO = archold.COMBO;
            //ARENDERER.MVCAP.CalculateCombo();
            


           
            if (ARENDERER.IsHead)
            {
                ARENDERER.HeightIndicatorRenderer.enabled = true;
            }
            ARENDERER.HeightIndicatorRenderer.transform.localPosition = new Vector3(ArcAlgorithm.ArcXToWorld(archold.XStart), 0, 0);
            ARENDERER.HeightIndicatorRenderer.transform.localScale = new Vector3(2.34f, 100 * (ArcAlgorithm.ArcYToWorld(archold.YStart) - ArcHoldRenderer.OffsetNormal / 2), 1);



            // if (arc == null) return;

            ArcTimingManager timingManager = ATMAN;
            int Offset = AMAN.AudioOffset;
            int duration = archold.EndTiming - archold.Timing;

            int v1 = duration < 1000 ? 14 : 7;
            float v2 = 1f / (v1 * duration / 1000f);
            int segSize = (int)(duration * v2);
            ARENDERER.segmentCount = (segSize == 0 ? 0 : duration / segSize) + 1;
            ARENDERER.InstantiateSegment(ARENDERER.segmentCount, ARENDERER.notetransform);
            // ARENDERER.segmentsleft = ARENDERER.segmentCount;
            int segled = ARENDERER.segmentCount;
            //thearry = new Transform[segled];
            //SEGTIME = new float[segled];

            //segmentCount = segmentCount*2;
            Vector3 start = new Vector3();
            Vector3 end = new Vector3(ArcAlgorithm.ArcXToWorld(archold.XStart),
                                        ArcAlgorithm.ArcYToWorld(archold.YStart));
            var icount = 0;

            for (int i = 0; i < ARENDERER.segmentCount - 1; ++i)
            {

                //segmentsleft -= 1;
                start = end;
                var tempend = end;
                
                end = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(archold.XStart, archold.XEnd, (i + 1f) * segSize / duration, archold.LineType)), ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(archold.YStart, archold.YEnd, (i + 1f) * segSize / duration, archold.LineType)), -timingManager.CalculatePositionByTimingAndStart(archold.Timing, archold.Timing + segSize * (i + 1)) / 1000f);
                if (icount == 0)
                {
                    ARENDERER.vecdist = new Vector3(end.x, end.y, 0);
                }
                //if (segmentsleft <= 3)
                // {
                if (((icount) != ARENDERER.segmentsleft))
                {
                    // thearry[icount] = waypointstart.transform;
                    //SEGTIME[icount] = Vector3.Distance(tempend, end);
                }
                else
                {

                }



                icount += 1;


                ARENDERER.segments[i].BuildSegmentTrace(start, end, ArcHoldRenderer.OffsetNormal, archold.Timing + segSize * i, archold.Timing + segSize * (i + 1), i + 1);
                //ARENDERER.segments[i].BuildCollider(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal);


            }


            start = end;
            end = new Vector3(ArcAlgorithm.ArcXToWorld(archold.XEnd),
                              ArcAlgorithm.ArcYToWorld(archold.YEnd),
                              -timingManager.CalculatePositionByTimingAndStart(archold.Timing, archold.EndTiming) / 1000f);
            ARENDERER.segments[ARENDERER.segmentCount - 1].BuildSegmentTrace(start, end, ArcHoldRenderer.OffsetNormal, archold.Timing + segSize * (ARENDERER.segmentCount - 1), archold.EndTiming, 0);
            //ARENDERER.segments[ARENDERER.segmentCount - 1].BuildCollider(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal);

            //var tmpct = icount;

                if (archold.Color == 0)
                {
                    ARENDERER.Color = ARENDERER.ArcBlue;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcBlue;
                }
                else if (archold.Color == 1)
                {
                    ARENDERER.Color = ARENDERER.ArcRed;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcRed;
                }
                else if (archold.Color == 2)
                {
                    ARENDERER.Color = ARENDERER.ArcGreen;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcGreen;
                }
                else if (archold.Color == 3)
                {
                    ARENDERER.Color = ARENDERER.ArcYellow;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcYellow;
                }
                else if (archold.Color == 4)
                {
                    ARENDERER.Color = ARENDERER.ArcGray;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcGray;
                }
                ARENDERER.currentcolor = ARENDERER.Color;
                ARENDERER.currentcolor.a = 0.57f;
            

            Vector3 pos = new Vector3(ArcAlgorithm.ArcXToWorld(archold.XStart), ArcAlgorithm.ArcYToWorld(archold.YStart));
            float offset =  ArcHoldRenderer.OffsetNormal;


            Vector3[] vertices = new Vector3[9];
            Vector2[] uv = new Vector2[9];
            int[] triangles = new int[] { 0, 1, 2, 4, 2, 0, 8, 7, 5, 0, 7, 5, 4, 2, 3 };

            if (offset == 0.9f)
            {


                vertices[1] = pos + new Vector3(offset, -offset / 2, 0);
                vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[0] = pos + new Vector3(0.4f, offset / 2, 0);
                vertices[3] = pos + new Vector3(-offset, -offset / 2, 0);
                vertices[4] = pos + new Vector3(-0.4f, offset / 2, 0);
                vertices[5] = pos + new Vector3(-0.4f, offset / 2, 0);
                vertices[6] = pos + new Vector3(0.4f, offset / 2, 0);
                vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[8] = pos + new Vector3(-offset, -offset / 2, 0);
            }
            else
            {
                vertices[1] = pos + new Vector3(offset, -offset / 2, 0);
                vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[0] = pos + new Vector3(0.1f, offset / 2, 0);
                vertices[3] = pos + new Vector3(-offset, -offset / 2, 0);
                vertices[4] = pos + new Vector3(-0.1f, offset / 2, 0);
                vertices[5] = pos + new Vector3(-0.1f, offset / 2, 0);
                vertices[6] = pos + new Vector3(0.1f, offset / 2, 0);
                vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[8] = pos + new Vector3(-offset, -offset / 2, 0);
            }

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0.5f, 1);
            uv[4] = new Vector2(0.93f, 0);
            //uv[3] = new Vector2(1,1);

            uv[5] = new Vector2();
            uv[8] = new Vector2(1, 0);
            uv[7] = new Vector2(0.5f, 1);
            uv[6] = new Vector2(1, 1);





            ARENDERER.HeadFilter.mesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles.Take(9).ToArray()
            };
            
            ARENDERER.ActualCap.transform.position = new Vector3(pos.x, pos.y, NoteLayer.position.z);
            ARENDERER.ActualCap.transform.parent = null;


            ARENDERER.HeadRenderer.material = ARENDERER.headMaterialInstance;

            ARENDERER.Enable = true;

            archold.Position = timingManager.CalculatePositionByTiming(archold.Timing + AMAN.AudioOffset);
            archold.EndPosition = timingManager.CalculatePositionByTiming(archold.EndTiming + AMAN.AudioOffset);
            

            T.transform.position = new Vector3(0, 0, (-archold.Position / 1000f));
            
            
            ARENDERER.Alpha = 0.57f;
            
            

            ARENDERER.CANRENDER = true;
            archold.isrunning = false;
        }


        public void CalculateArcRelationship()
        {
            ArcTimingManager timing = ATMAN;
            foreach (ArcArcHold arc in ArcHolds)
            {
                arc.ArcHoldGroup = null;
                arc.RenderHead = true;
                arc.SHOULDHEAD = true;
            }
            foreach (ArcArcHold a in ArcHolds)
            {
                foreach (ArcArcHold b in ArcHolds)
                {
                    if (a == b) continue;
                    if ((Mathf.Abs(a.XEnd - b.XStart) < 0.1f && Mathf.Abs(a.EndTiming - b.Timing) <= 9 && a.YEnd == b.YStart) || (Mathf.Abs(a.XStart - b.XEnd) < 0.1f && Mathf.Abs(a.Timing - b.EndTiming) <= 9 && a.YStart == b.YEnd))
                    {
                        if (a.Color == b.Color)
                        {
                            if (a.ArcHoldGroup == null && b.ArcHoldGroup != null)
                            {
                                a.ArcHoldGroup = b.ArcHoldGroup;
                            }
                            else if (a.ArcHoldGroup != null && b.ArcHoldGroup == null)
                            {
                                b.ArcHoldGroup = a.ArcHoldGroup;
                            }
                            else if (a.ArcHoldGroup != null && b.ArcHoldGroup != null)
                            {
                                foreach (var t in b.ArcHoldGroup)
                                {
                                    if (!a.ArcHoldGroup.Contains(t)) a.ArcHoldGroup.Add(t);
                                }
                                b.ArcHoldGroup = a.ArcHoldGroup;
                            }
                            else if (a.ArcHoldGroup == null && b.ArcHoldGroup == null)
                            {
                                a.ArcHoldGroup = b.ArcHoldGroup = new List<ArcArcHold> { a };
                            }
                            if (!a.ArcHoldGroup.Contains(b)) a.ArcHoldGroup.Add(b);
                        }
                        if (a.IsVoid == b.IsVoid)
                        {
                            b.RenderHead = false;
                            b.SHOULDHEAD = false;
                        }
                    }
                }
            }
            foreach (ArcArcHold arc in ArcHolds)
            {
                if (arc.ArcHoldGroup == null) arc.ArcHoldGroup = new List<ArcArcHold> { arc };
                arc.ArcHoldGroup.Sort((ArcArcHold a, ArcArcHold b) => a.Timing.CompareTo(b.Timing));
                arc.ArcHoldGroup[0].firstarc = true;
                arc.ArcHoldGroup[0].RenderHead = true;  
                arc.ArcHoldGroup[0].SHOULDHEAD= true;  
                var START = arc.Timing;
               var  END = arc.EndTiming;
                if (START == END) { arc.connection = true; }
                int u = arc.RenderHead ? 0 : 1;
                double bpm = ArcTimingManager.Instance.CalculateBpmByTiming(arc.Timing);
                if (bpm <= 0) { }
                else
                {
                    double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
                    int total = (int)((arc.EndTiming - arc.Timing) / interval);
                    //COMBO = total;
                    if ((u ^ 1) >= total)
                    {
                        arc.COMBO += 1;
                        //parentArc.ArcHold.JudgeTimings.Add((int)(parentArc.ArcHold.Timing + (parentArc.ArcHold.EndTiming - parentArc.ArcHold.Timing) * 0.5f));
                        //return;
                    }
                    else
                    {
                        int n = u ^ 1;
                        int t = arc.Timing;
                        while (true)
                        {
                            t = (int)(arc.Timing + n * interval);
                            if (t < arc.EndTiming)
                            {
                                arc.COMBO += 1;

                            }
                            if (total == ++n) break;
                        }
                    }
                }
            }         
        }

        public List<ArcArcHold> ARCHOLDS = new List<ArcArcHold>();


        public void Rebuild()
        {
            foreach (var t in ArcHolds) t.Rebuild();
            CalculateArcRelationship();
        }

        public void Add(ArcArc arc)
        {
            //arc.Instantiate();
            //Arcs.Add(arc);
            //CalculateArcRelationship();
        }
        public void Remove(ArcArcHold arc)
        {
            ArcHolds.Remove(arc);
            arc.Destroy();
            CalculateArcRelationship();
        }
        
        public bool connectionrunning = false;
        
        public IEnumerator CONNECTION(ArcArc Arc, ArcArcTap ATAP)
        {
            connectionrunning = true;
           
        yield return new WaitForSecondsRealtime(0.0f);
            if (ReferenceEquals(Arc, null) || (Arc.EndTiming - Arc.Timing) == 0) yield return null;
            ArcTap[] sameTimeTapNotes;
            List<ArcTap> taps;
            taps = ArcTapNoteManager.Instance.Taps;
            sameTimeTapNotes = taps.Where((s) => Mathf.Abs(s.Timing - ATAP.Timing) <= 1).ToArray();
            foreach (ArcTap t in sameTimeTapNotes)
            {
                // LineRenderer l = UnityEngine.Object.Instantiate(ArcArcManager.Instance.ConnectionPrefab, t.TPOBJ.transform).GetComponent<LineRenderer>();
                GameObject L = ObjectPooler.SharedInstance.GetPooledObject("CONNECTION");
                L.transform.parent = t.TPOBJ.transform;
                L.SetActive(true);
                LineRenderer l = L.GetComponent<LineRenderer>();
                //l.SetPosition(1,Vector3.zero);
               
                t.LINE = l;
                float p = 1f * (ATAP.Timing - Arc.Timing) / (Arc.EndTiming - Arc.Timing);
                Vector3 pos = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(Arc.XStart, Arc.XEnd, p, Arc.LineType)),
                                             ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(Arc.YStart, Arc.YEnd, p, Arc.LineType)) - 0.5f)
                                             - new Vector3(ArcArcManager.Instance.Lanes[t.Track - 1], 0);
                if (!float.IsNaN(pos.x)&&!float.IsNaN(pos.y))
                l.SetPosition(1, new Vector3(pos.x, 0, pos.y));

                //l.startColor = l.endColor = ArcArcManager.Instance.ConnectionColor;
                //l.startColor = l.endColor = new Color(l.endColor.r, l.endColor.g, l.endColor.b, t.Alpha * 0.8f);
                Quaternion rot = Quaternion.Euler(-90, 0, 0);
                l.transform.rotation = rot;
                l.transform.localPosition = new Vector3(0,0.1f,0);
                l.transform.localScale = new Vector3(0.6535948f, 1, 1);
                //print(l.positionCount);
                



                /* var existed = t.ConnectionLines.Where((b) => b.GetPosition(1) == new Vector3(pos.x, 0, pos.y)).ToList();
                 foreach (var el in existed)
                 {
                     UnityEngine.Object.Destroy(el.gameObject);
                     t.ConnectionLines.Remove(el);
                 }*/

                //t.ConnectionLines.Add(l);

            }
            connectionrunning = false;
        }

        public bool checkedoverlaps = false;
        public bool LAMAO = false;

       

        

        public List<ArcArcTap> ArcT;
        

        private void Update()
        {

            if (ReferenceEquals(ArcHolds, null)) return;
            if (checkedoverlaps == false&&ArcTimingManager.readytorun==true)
            {
                checkedoverlaps = true;
                //CheckedLaps();
                //checkorder();
                
                    foreach (var arc in ArcHolds)
                    {                     
                        arc.isrunning = true;
                        StartCoroutine(ARCARCS(arc));
                    }               
            }
           
        }

        IEnumerator Connec(ArcArc arc, ArcArcTap AAT)
        {
            yield return new WaitUntil(() => connectionrunning == false);
            StartCoroutine(CONNECTION(arc, AAT));
        }

        public void RenderArcs()
        {
            ArcTimingManager timingManager = ATMAN;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            int offset = ArcAudioManager.Instance.AudioOffset;

            foreach (var t in ArcHolds)
            {
                //RenderArcTaps(t);
                int duration = t.EndTiming - t.Timing;
                /*if (!timingManager.ShouldRender(t.Timing + offset, duration + (t.IsVoid ? 50 : 120)) || t.Judged)
                {
                    t.Enable = false;
                    continue;
                }*/
                t.Position = timingManager.CalculatePositionByTiming(t.Timing + offset);
                t.EndPosition = timingManager.CalculatePositionByTiming(t.EndTiming + offset);
               /* if (t.Position > 100000 || t.EndPosition < -20000)
                {
                    t.Enable = false;
                    continue;
                }*/
                t.Enable = true;
                t.transform.localPosition = new Vector3(0, 0, -t.Position / 1000f);
                if (!t.IsVoid)
                {
                    t.arcRenderer.EnableEffect = currentTiming > t.Timing + offset && currentTiming < t.EndTiming + offset && !t.IsVoid && t.Judging;
                    foreach (var a in t.ArcHoldGroup)
                    {
                        if (!a.Flag)
                        {
                            a.Flag = true;
                            float alpha = 1;
                            if (a.Judging)
                            {
                                a.FlashCount = (a.FlashCount + 1) % 5;
                                if (a.FlashCount == 0) alpha = 0.85f;
                                a.arcRenderer.Highlight = true;
                            }
                            else
                            {
                                alpha = 0.65f;
                                a.arcRenderer.Highlight = false;
                            }
                            alpha *= 0.8823592f;
                            a.arcRenderer.Alpha = alpha;
                        }
                    }
                }
                else
                {
                    t.arcRenderer.EnableEffect = false;
                    t.arcRenderer.Highlight = false;
                    t.arcRenderer.Alpha = 0.318627f;
                }
                t.arcRenderer.UpdateArc();
            }
            foreach (var t in ArcHolds)
            {
                t.Flag = false;
            }
        }

       
        
    }
}