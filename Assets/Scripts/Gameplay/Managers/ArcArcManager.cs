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
    public class ArcArcManager : MonoBehaviour
    {
        public static ArcArcManager Instance { get; private set; }
        
        public bool voidarcsallowed = true;
        public bool alternaterender = false;
        private void Awake()
        {
            Instance = this;
            voidarcsallowed = SecurePlayerPrefs.GetBool("#VOIDARCS");
            alternaterender = SecurePlayerPrefs.GetBool("#ALTREND");
            ATMAN = ArcTimingManager.Instance;
            AMAN = ArcAudioManager.Instance;
        }

        //[HideInInspector]
        public List<ArcArc> Arcs = new List<ArcArc>();
        public ArcTimingManager ATMAN;

        public GameObject ArcNotePrefab, ArcTapPrefab, ConnectionPrefab;
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
            foreach (var t in Arcs) t.Destroy();
            // Arcs.Clear();
        }
        public void Load(List<ArcArc> arcs)
        {
            Arcs = arcs;
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

        public IEnumerator ARCARCS(ArcArc arc)
        {
            //yield return new WaitForSeconds(0.0f);

            // yield return new WaitUntil(() => arc.Timing - (1060.5 / ATMAN.CurrentSpeed) <= ArcGameplayManager.Instance.Timing&&ATMAN.CurrentSpeed!=0 && !ATMAN.IsBackwarding);
            yield return new WaitUntil(() => ATMAN.ShouldRender(arc.Timing + AMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);

            arc.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("ARC");
            var T = arc.TPOBJ;
            T.transform.parent = ArcLayer;
            var ARENDERER = T.GetComponent<ArcArcRenderer>();
            arc.AREND = ARENDERER;
            ARENDERER.arc = arc;
            // var M = Instantiate(ARENDERER.MVPREFAB, new Vector3(0, 0, 0), Quaternion.identity);
            var M = ObjectPooler.SharedInstance.GetPooledObject("CAP");
            M.transform.position = Vector3.zero;
            M.SetActive(true);
            var MM = M.GetComponent<movecap>();
            var S = M.GetComponent<SpriteRenderer>();
            MM.parentArc = ARENDERER;
            ARENDERER.ENACP.Movecap = MM;
            if (arc.OverlapFound == true)
            {
                MM.specialhit = true;
            }
            else { MM.specialhit = false; }

           // ARENDERER.ENACP.CAPCOLLIDER = M.GetComponent<BoxCollider>();
            ARENDERER.ENACP.Actualcap = S;
            ARENDERER.ActualCap = M;
            ARENDERER.ArcCap = M.transform;
            ARENDERER.ArcCapRenderer = S;
            ARENDERER.MVCAP = MM;
            ARENDERER.HeadRenderer.enabled = true;
            ARENDERER.HeightIndicatorRenderer.enabled = true;
            //ARENDERER.ENACP.DNHEAD.enabled = true;
            //ARENDERER.HeadRenderer.enabled = true;
            T.SetActive(true);





            //ARENDERER.Build();

            if (!ARENDERER.arc.IsVoid)
            {
                ARENDERER.MVCAP.COMBO = 0;
                ARENDERER.MVCAP.CalculateCombo();
            }


            if (arc.IsVoid)
            {
                ARENDERER.HeightIndicatorRenderer.enabled = false;
                ARENDERER.VOID = true;
                //return;
            }
            else if (ARENDERER.IsHead)
            {
                ARENDERER.HeightIndicatorRenderer.enabled = true;
            }
            ARENDERER.HeightIndicatorRenderer.transform.localPosition = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), 0, 0);
            ARENDERER.HeightIndicatorRenderer.transform.localScale = new Vector3(2.34f, 100 * (ArcAlgorithm.ArcYToWorld(arc.YStart) - ArcArcRenderer.OffsetNormal / 2), 1);



            // if (arc == null) return;

            ArcTimingManager timingManager = ATMAN;
            int Offset = AMAN.AudioOffset;
            int duration = arc.EndTiming - arc.Timing;

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
            Vector3 end = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart),
                                        ArcAlgorithm.ArcYToWorld(arc.YStart));
            var icount = 0;

            for (int i = 0; i < ARENDERER.segmentCount - 1; ++i)
            {

                //segmentsleft -= 1;
                start = end;
                var tempend = end;

                end = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, (i + 1f) * segSize / duration, arc.LineType)), ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, (i + 1f) * segSize / duration, arc.LineType)), -timingManager.CalculatePositionByTimingAndStart(arc.Timing, arc.Timing + segSize * (i + 1)) / 1000f);
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


                ARENDERER.segments[i].BuildSegmentTrace(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal, arc.Timing + segSize * i, arc.Timing + segSize * (i + 1), i + 1);
                //ARENDERER.segments[i].BuildCollider(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal);


            }


            start = end;
            end = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XEnd),
                              ArcAlgorithm.ArcYToWorld(arc.YEnd),
                              -timingManager.CalculatePositionByTimingAndStart(arc.Timing, arc.EndTiming) / 1000f);
            ARENDERER.segments[ARENDERER.segmentCount - 1].BuildSegmentTrace(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal, arc.Timing + segSize * (ARENDERER.segmentCount - 1), arc.EndTiming, 0);
            //ARENDERER.segments[ARENDERER.segmentCount - 1].BuildCollider(start, end, arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal);

            //var tmpct = icount;



            if (arc.IsVoid)
            {
                ARENDERER.Color = ARENDERER.ArcVoid;
            }
            else
            {
                if (arc.Color == 0)
                {
                    ARENDERER.Color = ARENDERER.ArcBlue;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcBlue;
                }
                else if (arc.Color == 1)
                {
                    ARENDERER.Color = ARENDERER.ArcRed;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcRed;
                }
                else if (arc.Color == 2)
                {
                    ARENDERER.Color = ARENDERER.ArcGreen;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcGreen;
                }
                else if (arc.Color == 3)
                {
                    ARENDERER.Color = ARENDERER.ArcYellow;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcYellow;
                }
                else if (arc.Color == 4)
                {
                    ARENDERER.Color = ARENDERER.ArcGray;
                    ARENDERER.HeightIndicatorRenderer.color = ARENDERER.ArcGray;
                }
                ARENDERER.currentcolor = ARENDERER.Color;
                ARENDERER.currentcolor.a = 0.57f;
            }

            Vector3 pos = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
            float offset = arc.IsVoid ? ArcArcRenderer.OffsetVoid : ArcArcRenderer.OffsetNormal;


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
            /* if (offset == 0.9f)
             {


                 vertices[1] = pos + new Vector3(offset, -offset / 2, 0 + 1);
                 vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2 + 1);
                 vertices[0] = pos + new Vector3(0.4f, offset / 2, 0 + 1);
                 vertices[3] = pos + new Vector3(-offset, -offset / 2, 0 + 1);
                 vertices[4] = pos + new Vector3(-0.4f, offset / 2, 0 + 1);
                 vertices[5] = pos + new Vector3(-0.4f, offset / 2, 0 + 1);
                 vertices[6] = pos + new Vector3(0.4f, offset / 2, 0 + 1);
                 vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2 + 1);
                 vertices[8] = pos + new Vector3(-offset, -offset / 2, 0 + 1);
             }
             else
             {
                 vertices[1] = pos + new Vector3(offset, -offset / 2, 0 + 1);
                 vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2 + 1);
                 vertices[0] = pos + new Vector3(0.1f, offset / 2, 0 + 1);
                 vertices[3] = pos + new Vector3(-offset, -offset / 2, 0 + 1);
                 vertices[4] = pos + new Vector3(-0.1f, offset / 2, 0 + 1);
                 vertices[5] = pos + new Vector3(-0.1f, offset / 2, 0 + 1);
                 vertices[6] = pos + new Vector3(0.1f, offset / 2, 0 + 1);
                 vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2 + 1);
                 vertices[8] = pos + new Vector3(-offset, -offset / 2, 0 + 1);
             }*/

            /* ARENDERER.HeadCollider.sharedMesh = new Mesh()
             {
                 vertices = vertices,
                 uv = uv,
                 triangles = triangles
             };*/
            ARENDERER.ActualCap.transform.position = new Vector3(pos.x, pos.y, NoteLayer.position.z);
            ARENDERER.ActualCap.transform.parent = null;


            ARENDERER.HeadRenderer.material = ARENDERER.headMaterialInstance;


            //int duration = t.EndTiming - t.Timing;
            /*if (!timingManager.ShouldRender(t.Timing + offset, duration + (t.IsVoid ? 50 : 120)) || t.Judged)
            {
                t.Enable = false;
                continue;
            }*/
            ARENDERER.Enable = true;

            arc.Position = timingManager.CalculatePositionByTiming(arc.Timing + AMAN.AudioOffset);
            arc.EndPosition = timingManager.CalculatePositionByTiming(arc.EndTiming + AMAN.AudioOffset);
            /* if (t.Position > 100000 || t.EndPosition < -20000)
             {
                 t.Enable = false;
                 continue;
             }*/
            //arc.arcRenderer.Enable = true;

            T.transform.position = new Vector3(0, 0, (-arc.Position / 1000f));
            //ARENDERER.ENACP.DNHEAD.enabled = true;
            //T.transform.localPosition = new Vector3(0, 0, (((-0.5333f * 3f) * timingManager.CURRENTBPM)) - NoteLayer.position.z);

            /* if (!arc.IsVoid)
                 {
                     arc.arcRenderer.EnableEffect = currentTiming > arc.Timing + offfset && currentTiming < arc.EndTiming + offfset && !arc.IsVoid && arc.Judging;
                     /*foreach (var a in arc.ArcGroup)
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
                     arc.arcRenderer.EnableEffect = false;
                     arc.arcRenderer.Highlight = false;*/
            if (arc.IsVoid)
            {
                ARENDERER.Alpha = 0.318627f;
            }
            else
            {
                ARENDERER.Alpha = 0.57f;
            }
            //}

            ARENDERER.CANRENDER = true;
            arc.isrunning = false;
        }

        public IEnumerator ARCTAPS(ArcArcTap AAT, ArcArc arc)
        {
            yield return new WaitForSeconds(0.0f);
            // yield return new WaitUntil(() => AAT.Timing-(1060.5 /ATMAN.CurrentSpeed)<= ArcGameplayManager.Instance.Timing && ATMAN.CurrentSpeed != 0 && !ATMAN.IsBackwarding);

            if (alternaterender)
                yield return new WaitUntil(() => ATMAN.ShouldRendArc(AAT.Timing + AMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
            else
                yield return new WaitUntil(() => ATMAN.ShouldRender(AAT.Timing + AMAN.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
            /*while(!ATMAN.ShouldRender(AAT.Timing + AMAN.AudioOffset))
            {
                yield return null;
            }
            while (ATMAN.IsStopped)
            {
                yield return null;
            }
            while (ATMAN.IsBackwarding)
            {
                yield return null;
            }*/
            //print("contiuned?");
            var offset = AMAN.AudioOffset;
            if (ReferenceEquals(AAT.TPOBJ, null))
            {
                //yield return new WaitForSeconds((((AAT.Timing - ((53f / ATMAN.DropRate) * 1000f) + (ArcAudioManager.Instance.AudioOffset)) / 1000f)));


                //var AT= Instantiate(ArcTapPrefab,LocalPosition,Quaternion.identity);

                AAT.correctioncount = 0;
                AAT.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("ARCTAPPOOL");

                var AT = AAT.TPOBJ;
                AAT.TPOBJ = AAT.TPOBJ.transform.Find("ArcTap").gameObject;
                var Model = AAT.TPOBJ.transform.Find("Model").gameObject;
                AAT.Arc = arc;

                var Pose = ATMAN.CalculatePositionByTiming(arc.Timing + AMAN.AudioOffset);


                //var LocalPosition = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), -95.994f);
                float t = 1f * (AAT.Timing - arc.Timing) / (arc.EndTiming - arc.Timing);

                /*if (ATMAN.firstclicked)
                {

                    Vector3 vecc= new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), -(-ATMAN.CalculatePositionByTimingAndStart(arc.Timing + offset, ArcGameplayManager.Instance.Timing + offset) / 1000f));
                    if (!float.IsNaN(vecc.x) && !float.IsNaN(vecc.y) && !float.IsNaN(vecc.z))
                        AT.transform.position = vecc;

                }
                else
                {

                    Vector3 vecc = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), (-101f));
                    if (!float.IsNaN(vecc.x) && !float.IsNaN(vecc.y) && !float.IsNaN(vecc.z))
                        AT.transform.position = vecc;


                }*/

                Vector3 vecc = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType)), ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f,
                                  -ATMAN.CalculatePositionByTimingAndStart(arc.Timing + offset, AAT.Timing + offset) / 1000f - 0.6f);
                if (!float.IsNaN(vecc.x) && !float.IsNaN(vecc.y) && !float.IsNaN(vecc.z))
                {
                    AAT.TPOBJ.transform.position = new Vector3(0, 0, (-Pose / 1000f));
                    Model.transform.localPosition = vecc;
                }

                if (Model.transform.localPosition.y <= 0.6f)
                {

                    if (!float.IsNaN(vecc.x) && !float.IsNaN(vecc.y) && !float.IsNaN(vecc.z))
                        Model.transform.localPosition = new Vector3(vecc.x, 0.6f, vecc.z);

                }
                AT.transform.parent = NoteLayer;


                /*if (Model.transform.localPosition.x - 1.05f <= 0f && Model.transform.localPosition.y + 0.5f >= 3.2f)
                {
                    ATMAN.noteQuad2.Add(Model);
                }
                if (Model.transform.localPosition.x + 1.05f >= 0f && Model.transform.localPosition.y + 0.5f >= 3.2f)
                {
                    ATMAN.noteQuad1.Add(Model);
                }
                if (Model.transform.localPosition.x - 1.05f <= 0f && Model.transform.localPosition.y - 0.5f <= 3.2f)
                {
                    ATMAN.noteQuad4.Add(Model);
                }
                if (Model.transform.localPosition.x + 1.05f >= 0f && Model.transform.localPosition.y - 0.5f <= 3.2f)
                {
                    ATMAN.noteQuad3.Add(Model);
                }*/

               /* ATMAN.noteQuad1.Add(Model);
                ATMAN.noteQuad2.Add(Model);
                ATMAN.noteQuad3.Add(Model);
                ATMAN.noteQuad4.Add(Model);*/

                AT.SetActive(true);
                //var unparent = AT.GetComponent<unparent>();
                var CanClick = Model.GetComponent<CanClick>();
                CanClick.ATAP = AAT;

                ATMAN.noteQuad1.Add(CanClick);
                ATMAN.noteQuad2.Add(CanClick);
                ATMAN.noteQuad3.Add(CanClick);
                ATMAN.noteQuad4.Add(CanClick);

                //unparent.timing = AAT.Timing;
                //unparent.AATAP = AAT;


                //p.y = 0;
                var ShadowRenderer = AAT.TPOBJ.transform.gameObject.GetComponentInChildren<SpriteRenderer>();
                AAT.ShadowRenderer = ShadowRenderer;
                var Shadow = ShadowRenderer.transform;

                Vector3 p = new Vector3(vecc.x, 0, vecc.z);
                ShadowRenderer.enabled = true;
                if (!float.IsNaN(p.y) && !float.IsNaN(p.x) && !float.IsNaN(p.z))
                    Shadow.localPosition = p;


                StartCoroutine(CONNECTION(arc, AAT));

            }


        }

        public void CalculateArcRelationship()
        {
            ArcTimingManager timing = ATMAN;
            foreach (ArcArc arc in Arcs)
            {
                arc.ArcGroup = null;
                arc.RenderHead = true;
                arc.SHOULDHEAD = true;
            }
            foreach (ArcArc a in Arcs)
            {
                foreach (ArcArc b in Arcs)
                {
                    if (a == b) continue;
                    if (Mathf.Abs(a.XEnd - b.XStart) < 0.1f && Mathf.Abs(a.EndTiming - b.Timing) <= 9 && a.YEnd == b.YStart)
                    {
                        if (a.Color == b.Color && a.IsVoid == b.IsVoid)
                        {
                            if (a.ArcGroup == null && b.ArcGroup != null)
                            {
                                a.ArcGroup = b.ArcGroup;
                            }
                            else if (a.ArcGroup != null && b.ArcGroup == null)
                            {
                                b.ArcGroup = a.ArcGroup;
                            }
                            else if (a.ArcGroup != null && b.ArcGroup != null)
                            {
                                foreach (var t in b.ArcGroup)
                                {
                                    if (!a.ArcGroup.Contains(t)) a.ArcGroup.Add(t);
                                }
                                b.ArcGroup = a.ArcGroup;
                            }
                            else if (a.ArcGroup == null && b.ArcGroup == null)
                            {
                                a.ArcGroup = b.ArcGroup = new List<ArcArc> { a };
                            }
                            if (!a.ArcGroup.Contains(b)) a.ArcGroup.Add(b);
                        }
                        if (a.IsVoid == b.IsVoid)
                        {
                            b.RenderHead = false;
                            b.SHOULDHEAD = false;
                        }
                    }
                }
            }
            foreach (ArcArc arc in Arcs)
            {
                if (arc.ArcGroup == null) arc.ArcGroup = new List<ArcArc> { arc };
                arc.ArcGroup.Sort((ArcArc a, ArcArc b) => a.Timing.CompareTo(b.Timing));
            }
            foreach (ArcArc arc in Arcs)
            {
                //arc.CalculateJudgeTimings();
            }
        }

        public List<ArcArc> ARCS = new List<ArcArc>();



        public void CalculateArcRel()
        {
            ArcTimingManager timing = ATMAN;
            foreach (ArcArc arc in Arcs)
            {
                arc.ArcGroup = null;
                arc.RenderHead = true;
            }
            foreach (ArcArc a in Arcs)
            {
                foreach (ArcArc b in Arcs)
                {
                    if (a == b) continue;
                    if (Mathf.Abs(a.XEnd - b.XStart) < 0.1f && Mathf.Abs(a.EndTiming - b.Timing) <= 9 && a.YEnd == b.YStart)
                    {
                        if (a.Color == b.Color && a.IsVoid == b.IsVoid)
                        {
                            if (a.ArcGroup == null && b.ArcGroup != null)
                            {
                                a.ArcGroup = b.ArcGroup;
                            }
                            else if (a.ArcGroup != null && b.ArcGroup == null)
                            {
                                b.ArcGroup = a.ArcGroup;
                            }
                            else if (a.ArcGroup != null && b.ArcGroup != null)
                            {
                                foreach (var t in b.ArcGroup)
                                {
                                    if (!a.ArcGroup.Contains(t)) a.ArcGroup.Add(t);
                                }
                                b.ArcGroup = a.ArcGroup;
                            }
                            else if (a.ArcGroup == null && b.ArcGroup == null)
                            {
                                a.ArcGroup = b.ArcGroup = new List<ArcArc> { a };
                            }
                            if (!a.ArcGroup.Contains(b)) a.ArcGroup.Add(b);
                        }
                        if (a.IsVoid == b.IsVoid)
                        {
                            b.RenderHead = false;
                        }
                    }
                }
            }
            foreach (ArcArc arc in Arcs)
            {
                if (arc.ArcGroup == null) arc.ArcGroup = new List<ArcArc> { arc };
                arc.ArcGroup.Sort((ArcArc a, ArcArc b) => a.Timing.CompareTo(b.Timing));
            }
            foreach (ArcArc arc in Arcs)
            {

                //arc.CalculateJudgeTimings();
            }
            readytocalc = true;
            ArcTimingManager.Shouldrun = true;
            //print("can calc!");
        }

        public void Rebuild()
        {
            foreach (var t in Arcs) t.Rebuild();
            CalculateArcRelationship();
        }

        public void Add(ArcArc arc)
        {
            //arc.Instantiate();
            //Arcs.Add(arc);
            //CalculateArcRelationship();
        }
        public void Remove(ArcArc arc)
        {
            Arcs.Remove(arc);
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
                if (!float.IsNaN(pos.x) && !float.IsNaN(pos.y))
                    l.SetPosition(1, new Vector3(pos.x, 0, pos.y));

                //l.startColor = l.endColor = ArcArcManager.Instance.ConnectionColor;
                //l.startColor = l.endColor = new Color(l.endColor.r, l.endColor.g, l.endColor.b, t.Alpha * 0.8f);
                Quaternion rot = Quaternion.Euler(-90, 0, 0);
                l.transform.rotation = rot;
                l.transform.localPosition = new Vector3(0, 0.1f, 0);
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

        void CheckedLaps()
        {
            //print(Arcs.Count);
            foreach (var arc in Arcs)
            {
                //print("startcheck");
                if (!arc.IsVoid)
                {
                    foreach (var b in Arcs)
                    {
                        if (arc == b) continue;
                        if ((b.Timing == arc.Timing) && b.Color != arc.Color && (Mathf.Abs(b.XStart - arc.XStart) <= 0.15f) && (Mathf.Abs(b.YStart - arc.YStart) <= 0.15f) && !b.IsVoid)
                        {
                            bool arcfound = false;
                            foreach (var c in Arcs)
                            {
                                if (c == b || c == arc)
                                    continue;

                                if (Mathf.Abs(c.EndTiming - b.Timing) <= 27 && c.Timing < b.Timing && !c.IsVoid && c.Color != b.Color)
                                {
                                    arcfound = true;
                                }
                                else
                                {
                                }

                            }
                            if (arcfound == false)
                            {
                                b.OverlapFound = true;
                                arc.OverlapFound = true;
                                // print("FOUNDOVERLAP");
                            }
                            else
                            {
                                //print("NOOVERLAP");
                            }
                        }
                    }
                }
            }

        }

        void checkorder()
        {
            ArcT = new List<ArcArcTap>();
            foreach (var arc in Arcs)
            {

                foreach (ArcArcTap AAT in arc.ArcTaps)
                {
                    ArcT.Add(AAT);
                }
            }
            foreach (var ACTAP in ArcT)
            {
                foreach (var arc in Arcs)
                {
                    if (arc.Timing < ACTAP.Timing && !arc.IsVoid)
                    {
                        if (Math.Abs(ACTAP.Timing - arc.Timing) <= 175f && Math.Abs(ACTAP.Timing - arc.Timing) != 0)
                        {
                            ACTAP.shouldwait = true;
                            ACTAP.TIME = (int)(0.6f * arc.Timing);
                            //print("SHOULD WAIT");
                            //print(ACTAP.TIME);
                        }
                    }
                }
            }
            foreach (var tap in ArcTapNoteManager.Instance.Taps)
            {
                foreach (var arc in Arcs)
                {
                    if (arc.Timing < tap.Timing)
                    {
                        if (Math.Abs(tap.Timing - arc.Timing) <= 175f)
                        {
                            /*ACTAP.shouldwait = true;
                            ACTAP.TIME = (int)(0.6f * arc.Timing);
                            print("SHOULD WAIT");
                            print(ACTAP.TIME);*/
                        }
                    }
                }
            }

        }

        public List<ArcArcTap> ArcT;


        private void Update()
        {

            if (ReferenceEquals(Arcs, null)) return;
            if (checkedoverlaps == false && ArcTimingManager.readytorun == true)
            {
                checkedoverlaps = true;
                CheckedLaps();
                //checkorder();
                if (voidarcsallowed == true)
                {
                    foreach (var arc in Arcs)
                    {

                        foreach (ArcArcTap AAT in arc.ArcTaps)
                        {
                            StartCoroutine(ARCTAPS(AAT, arc));
                        }
                        arc.isrunning = true;
                        StartCoroutine(ARCARCS(arc));
                    }
                }
                else
                {
                    foreach (var arc in Arcs)
                    {

                        foreach (ArcArcTap AAT in arc.ArcTaps)
                        {
                            StartCoroutine(ARCTAPS(AAT, arc));
                        }
                        arc.isrunning = true;
                        if (!arc.IsVoid)
                            StartCoroutine(ARCARCS(arc));
                    }
                }
            }
            //print(ATMAN.CurrentSpeed);
            //print(ATMAN.DropRate);


            /* foreach (var arc in Arcs)
             {

                /* foreach (ArcArcTap AAT in arc.ArcTaps)
                 {
                     var offset =AMAN.AudioOffset;
                     if (AAT.TPOBJ != null && ATMAN.ShouldRender(AAT.Timing + ArcAudioManager.Instance.AudioOffset)&&!ATMAN.IsBackwarding&&!ATMAN.IsStopped)
                     {
                         //if (ATMAN.IsBackwarding)
                         // {
                         //LASTBPM = ATMAN.CURRENTBPM;
                         //float t = 1f * (AAT.Timing - arc.Timing) / (arc.EndTiming - arc.Timing);
                         //AAT.TPOBJ.transform.position = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType)), ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f, -(-ATMAN.CalculatePositionByTimingAndStart(arc.Timing + offset, ArcGameplayManager.Instance.Timing + offset) / 1000f));
                         if (AAT.correctioncount < 6)
                         {


                             AAT.correctioncount += 1;
                         }
                         //}
                     }
                     else if (AAT.TPOBJ == null && ATMAN.ShouldRender(AAT.Timing + offset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                     {
                         //yield return new WaitForSeconds((((AAT.Timing - ((53f / ATMAN.DropRate) * 1000f) + (ArcAudioManager.Instance.AudioOffset)) / 1000f)));


                         //var AT= Instantiate(ArcTapPrefab,LocalPosition,Quaternion.identity);
                         AAT.correctioncount = 0;
                         AAT.TPOBJ = ObjectPooler.SharedInstance.GetPooledObject("ARCTAPPOOL");
                         var AT = AAT.TPOBJ;


                         //var LocalPosition = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), -95.994f);
                         float t = 1f * (AAT.Timing - arc.Timing) / (arc.EndTiming - arc.Timing);
                         if (ATMAN.firstclicked)
                         {
                             try
                             {
                                 AT.transform.position = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), -(-ATMAN.CalculatePositionByTimingAndStart(arc.Timing + offset, ArcGameplayManager.Instance.Timing + offset) / 1000f));
                             }
                             catch
                             {

                             }
                         }
                         else
                         {
                             try
                             {
                                 AT.transform.position = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, t, arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, t, arc.LineType)) - 0.5f), (-101f));
                             }
                             catch
                             {

                             }
                         }
                         if (AT.transform.position.y <= 0.6f)
                         {
                             AT.transform.position = new Vector3(AT.transform.position.x,0.6f,AT.transform.position.z);
                         }
                         AT.transform.parent = NoteLayer;
                         AT.SetActive(true);
                         var unparent = AT.GetComponent<unparent>();
                         var CanClick = AT.GetComponentInChildren<CanClick>();
                         CanClick.ATAP = AAT;

                         unparent.timing = AAT.Timing;
                         unparent.AATAP = AAT;


                         //p.y = 0;
                         var ShadowRenderer = AT.transform.gameObject.GetComponentInChildren<SpriteRenderer>();
                         var Shadow = ShadowRenderer.transform;

                         Vector3 p = new Vector3(Shadow.localPosition.x, -(AT.transform.position.y), Shadow.localPosition.z);
                         ShadowRenderer.enabled = true;
                         Shadow.localPosition = p;


                         StartCoroutine(CONNECTION(arc, AAT));

                     }
                 }*/



            /* int offfset = AMAN.AudioOffset;
            if (arc.TPOBJ == null && ATMAN.ShouldRender(arc.Timing + offfset)&&!arc.isrunning && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
             {
                 arc.isrunning = true;
                 StartCoroutine(ARCARCS(arc));

             }
         }*/


            //if (ArcGameplayManager.Instance.Auto) JudgeArcs();
            //ArcJudgePos = 0;
            //RenderArcs();
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

            foreach (var t in Arcs)
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
                    foreach (var a in t.ArcGroup)
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
            foreach (var t in Arcs)
            {
                t.Flag = false;
            }
        }
        private void RenderArcTaps(ArcArc arc)
        {
            int timing = ArcGameplayManager.Instance.Timing;
            ArcTimingManager timingManager = ATMAN;
            int offset = ArcAudioManager.Instance.AudioOffset;

            foreach (ArcArcTap t in arc.ArcTaps)
            {
                /*if (!timingManager.ShouldRender(t.Timing + offset, 50) || t.Judged)
                {
                    t.Enable = false;
                    continue;
                }*/
                /*if (timing > t.Timing + offset + 50)
                {
                    t.Enable = false;
                    continue;
                }*/
                float pos = timingManager.CalculatePositionByTiming(t.Timing + offset) / 1000f;
                if (pos > -10 && pos <= 90)
                {
                    t.Alpha = 1;
                    t.Enable = true;
                }
                else if (pos > 90 && pos <= 100)
                {
                    t.Enable = true;
                    t.Alpha = (100 - pos) / 10f;
                }
                else
                {
                    t.Enable = true;
                }
            }
        }

        private void JudgeArcs()
        {
            int currentTiming = ArcGameplayManager.Instance.Timing;
            int offset = ArcAudioManager.Instance.AudioOffset;
            foreach (ArcArc arc in Arcs)
            {
                JudgeArcTaps(arc);
                if (arc.Judged) continue;
                if (currentTiming > arc.EndTiming + offset)
                {
                    arc.Judged = true;
                }
                else if (currentTiming > arc.Timing + offset && currentTiming <= arc.EndTiming + offset)
                {
                    if (!arc.IsVoid)
                    {
                        if (!arc.AudioPlayed)
                        {
                            if (ArcGameplayManager.Instance.IsPlaying && arc.ShouldPlayAudio) ArcEffectManager.Instance.PlayArcSound();
                            arc.AudioPlayed = true;
                        }
                    }
                    foreach (var a in arc.ArcGroup) a.Judging = true;
                }
                else
                {
                    arc.ShouldPlayAudio = true;
                }
            }
        }
        private void JudgeArcTaps(ArcArc arc)
        {
            int currentTiming = ArcGameplayManager.Instance.Timing;
            int offset = ArcAudioManager.Instance.AudioOffset;
            foreach (ArcArcTap t in arc.ArcTaps)
            {
                if (t.Judged) continue;
                if (currentTiming > t.Timing + offset && currentTiming <= t.Timing + offset + 150)
                {
                    t.Judged = true;
                    //if (ArcGameplayManager.Instance.IsPlaying) ArcEffectManager.Instance.PlayTapNoteEffectAt(new Vector2(t.LocalPosition.x, t.LocalPosition.y + 0.5f), true);
                }
                else if (currentTiming > t.Timing + offset + 150)
                {
                    t.Judged = true;
                }
            }
        }
    }
}