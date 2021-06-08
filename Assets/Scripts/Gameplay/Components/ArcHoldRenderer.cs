using System.Collections.Generic;
using System.Linq;
using System;
using Arcaoid.Gameplay;
using Arcaoid.Gameplay.Chart;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Lean.Touch;
namespace Arcaoid.Gameplay
{
    public class ArcHoldRenderer : MonoBehaviour
    {
        public const float OffsetNormal = 0.9f;
        public const float OffsetVoid = 0.15f;

        public Material arcMaterial;
        public Material shadowMaterial;
        public GameObject SegmentPrefab;
        public Color ArcRed;
        public Color ArcBlue;
        public Color ArcGreen;
        public Color ArcYellow;
        public Color ArcGray;
        public float GBOFFSET;
        public bool VOID = false;

        public Color MISSED;
        public Color currentcolor;
        public Color ShadowColor;
        public readonly Color ArcVoid = new Color(0.5686275f, 0.4705882f, 0.6666667f, 0.20f);

        public MeshCollider ArcCollider, HeadCollider;
        public MeshFilter HeadFilter;
        public MeshRenderer HeadRenderer;
        public Transform Head;
        public Transform emptytrans;
        public GameObject MVPREFAB;
        public GameObject ActualCap;
        public GameObject wayprefab;
        public SpriteRenderer HeightIndicatorRenderer;
        public Transform ArcCap;
        public SpriteRenderer ArcCapRenderer;
        public ParticleSystem JudgeEffect;
        public Texture2D DefaultTexture, HighlightTexture;
        public float segmentsleft;
        public float arcexdist;
        public Vector3 vecdist;
        public float smallfloat;
        public float[] SEGTIME;

        private void OnDisable()
        {
            segmentsleft = 0;
            arcexdist = 0;
            vecdist = new Vector3(0,0,0);
            smallfloat = 0;
            arc = null;
            VOID = false;
            capisrun = false;
            didscale = false;
            segments.Clear();
           
                //headColliderMesh.Clear();
                HeadFilter.mesh.Clear();
            
            Array.Clear(arctarget,0,arctarget.Length);
         CANRENDER = false;
            canbackward = true;
            washead = false;
            extrahits = false;
            readytodeac = false;
            movestarted = false;
            headisrunning = false;
            ACAPisrunning = false;
            UPCCAP = null;
            HEADS = null;

            if (!(ReferenceEquals(ArcHold, null)) && !(ReferenceEquals(ArcHold.TPOBJ, null)))
            {
                ArcHold.TPOBJ = null;
            }
            
            

    }

       

        public ArcArcHold ArcHold
        {
            get
            {
                return arc;
            }
            set
            {
                arc = value;
                //Build();
            }
        }
        public Color Color
        {
            get
            {
                return currentColor;
            }
            set
            {
                if (currentColor != value)
                {
                    currentColor = value;
                    arcBodySharedMaterial.SetColor(colorShaderId, currentColor);
                    headMaterialInstance.SetColor(colorShaderId, currentColor);
                    foreach (var s in segments)
                    {
                        s.Color = currentColor;
                    }
                }
            }
        }
        public float Alpha
        {
            get
            {
                return currentColor.a;
            }
            set
            {
                if (currentColor.a != value)
                {
                    currentColor.a = value;
                    arcBodySharedMaterial.SetColor(colorShaderId, currentColor);
                    headMaterialInstance.SetColor(colorShaderId, currentColor);
                    Color c = ShadowColor;
                    c.a = value * 0.3f;
                    arcShadowSharedMaterial.SetColor(colorShaderId, c);
                    foreach (var s in segments)
                    {
                        s.Alpha = value;
                    }
                }
            }
        }
        public bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                if (enable != value)
                {
                    enable = value;
                    EnableHead = value;
                    //EnableHeightIndicator = value;
                    foreach (ArcHoldSegmentComponent s in segments) s.Enable = value;
                    //EnableArcCap = value;
                    if (!value) EnableEffect = false;
                    //ArcCollider.enabled = value;
                }
            }
        }
        public bool EnableHead
        {
            get
            {
                return headEnable;
            }
            set
            {
                if (headEnable != value)
                {
                    headEnable = value;
                    //HeadRenderer.enabled = value;
                    //HeadCollider.enabled = value;
                }
            }
        }
        public bool EnableHeightIndicator
        {
            get
            {
                return heightIndicatorEnable;
            }
            set
            {
                if (heightIndicatorEnable != value)
                {
                    heightIndicatorEnable = value;
                    HeightIndicatorRenderer.enabled = value;
                }
            }
        }
        public bool EnableArcCap
        {
            get
            {
                return arcCapEnable;
            }
            set
            {
                if (arcCapEnable != value)
                {
                    arcCapEnable = value;
                    ArcCapRenderer.enabled = value;
                }
            }
        }
        public bool Highlight
        {
            get
            {
                return highlighted;
            }
            set
            {
                if (highlighted != value)
                {
                    highlighted = value;
                    headMaterialInstance.mainTexture = highlighted ? HighlightTexture : DefaultTexture;
                    arcBodySharedMaterial.mainTexture = highlighted ? HighlightTexture : DefaultTexture;
                    foreach (var s in segments) s.Highlight = value;
                }
            }
        }
        public bool EnableEffect
        {
            get
            {
                return effect;
            }
            set
            {
                if (effect != value)
                {
                    effect = value;
                    if (value)
                    {
                        JudgeEffect.Play();
                    }
                    else
                    {
                        JudgeEffect.Stop();
                        JudgeEffect.Clear();
                    }
                }
            }
        }
        public int RenderQueue
        {
            get
            {
                return renderQueue;
            }
            set
            {
                if (renderQueue != value)
                {
                    renderQueue = value;
                    arcBodySharedMaterial.renderQueue = value;
                    headMaterialInstance.renderQueue = value;
                    arcShadowSharedMaterial.renderQueue = value;
                    foreach (var s in segments) s.RenderQueue = value;
                }
            }
        }
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    int h = value ? 1 : 0;
                    headMaterialInstance.SetInt(highlightShaderId, h);
                    arcBodySharedMaterial.SetInt(highlightShaderId, h);
                    foreach (var s in segments) s.Selected = value;
                    selected = value;
                }
            }
        }
        public bool IsHead
        {
            get
            {
                return arc.RenderHead;
            }
        }

        private void Awake()
        {
            notetransform = GameObject.FindGameObjectWithTag("NOTEBODY");
            ATMAN = ArcTimingManager.Instance;
            NOTEtransform = notetransform.transform;
            Arctransform = this.transform;
            arcBodySharedMaterial = Instantiate(arcMaterial);
            headMaterialInstance = Instantiate(arcMaterial);
            arcShadowSharedMaterial = Instantiate(shadowMaterial);
            HeadRenderer.sortingLayerName = "Arc";
            HeadRenderer.sortingOrder = 1;
            colorShaderId = Shader.PropertyToID("_Color");
            highlightShaderId = Shader.PropertyToID("_Highlight");
            ATMAN.OnSpeedChange.AddListener(spedchange);


        }

        public ArcTimingManager ATMAN;

        public IEnumerator Washit()
        {
            //print("HIT?");
            yield return new WaitForSeconds(0.0f);
            foreach (ArcHoldSegmentComponent seg in segments)
            {
                seg.Alpha = 1;
                isalpha = true;
            }

        }

        public IEnumerator Returnhit()
        {
           // print("RETURNED");
            yield return new WaitForSeconds(0.0f);
            foreach (ArcHoldSegmentComponent seg in segments)
            {
                seg.Color = currentColor;
                seg.Alpha = 1;
                isalpha = true;
            }

        }


        public IEnumerator Nothit()
        {
            //print("NOTHIT");
            yield return new WaitForSeconds(0.0f);
            foreach (ArcHoldSegmentComponent seg in segments)
            {
                seg.Alpha = 0.57f;
                isalpha = false;
            }

        }
        public IEnumerator MISS(ARCHIT FINGERBOX)
        {
            //print("MIISED");
            yield return new WaitForSeconds(0.0f);
            if (!(ReferenceEquals(FINGERBOX, null)))
            {
                FINGERBOX.arcfollowing = false;
                FINGERBOX.wrongArc = false;
                FINGERBOX.onArc = false;
                
                FINGERBOX.Color = -1;
                FINGERBOX = null;
            }
            foreach (ArcHoldSegmentComponent seg in segments)
            {
                seg.Color = MISSED;
                seg.Alpha = 0.57f;
                isalpha = false;
            }
           
        }

        public bool isalpha = false;

        private void OnDestroy()
        {
            Destroy(arcBodySharedMaterial);
            Destroy(headMaterialInstance);
            Destroy(arcShadowSharedMaterial);
        }

        private int highlightShaderId;
        private int colorShaderId;
        public int segmentCount = 0;
        private int renderQueue = 3000;
        private bool enable;
        private bool selected;
        private bool headEnable;
        private bool heightIndicatorEnable;
        private bool arcCapEnable;
        private bool highlighted;
        private bool effect;
        public ArcArcHold arc;
        private Color currentColor;
        public List<ArcHoldSegmentComponent> segments = new List<ArcHoldSegmentComponent>();
        public Material headMaterialInstance;
        private Material arcBodySharedMaterial;
        private Material arcShadowSharedMaterial;
        public Mesh headColliderMesh;
        public Transform [] arctarget;
       // public enablecap ENACP;
        public bool CANRENDER = false;
        

        public void InstantiateSegment(int quantity, GameObject notebody)
        {
            int count = segments.Count;
            if (count == quantity) return;
            else if (count < quantity)
            {
                for (int i = 0; i < quantity - count; ++i)
                {
                    //GameObject g = Instantiate(SegmentPrefab, transform);
                    GameObject g = ObjectPooler.SharedInstance.GetPooledObject("SEGMENTHOLD");
                    g.transform.parent = this.transform;
                    g.transform.localPosition = new Vector3(Arctransform.position.x,Arctransform.position.y, 0);

                    if (g.transform.position.z != 0)
                    {
                        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                    }
                    g.SetActive(true);
                    segments.Add(g.GetComponent<ArcHoldSegmentComponent>());
                    g.GetComponent<ArcHoldSegmentComponent>().Color = currentColor;
                }
            }
            else if (count > quantity)
            {
                for (int i = 0; i < count - quantity; ++i)
                {
                    //Destroy(segments.Last().gameObject);
                    //segments.RemoveAt(segments.Count - 1);
                }
            }
            foreach (ArcHoldSegmentComponent s in segments)
            {
               // s.transform.SetAsLastSibling();
            }
        }

        public void Build()
        {
            //BuildHeightIndicator();
           // BuildSegments();
            //BuildHead();
            //BuildCollider();
        }
        public void BuildHeightIndicator()
        {
            if (arc.IsVoid)
            {
                EnableHeightIndicator = false;
                return;
            }
            HeightIndicatorRenderer.transform.localPosition = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), 0, 0);
            HeightIndicatorRenderer.transform.localScale = new Vector3(2.34f, 100 * (ArcAlgorithm.ArcYToWorld(arc.YStart) - OffsetNormal / 2), 1);

            if (arc.Color == 0)
            {
                HeightIndicatorRenderer.color = ArcBlue;
            }
            else if (arc.Color == 1)
            {
                HeightIndicatorRenderer.color = ArcRed;
            }
            else if (arc.Color == 2)
            {
                HeightIndicatorRenderer.color = ArcGreen;
            }
            else if (arc.Color == 3)
            {
                HeightIndicatorRenderer.color = ArcYellow;
            }
            else if (arc.Color == 4)
            {
                HeightIndicatorRenderer.color = ArcGray;
            }
        }
        public Color raycolor = Color.red;
        public List<Transform> Pathobjs = new List<Transform>();
        public Transform[] thearry;
       
        public IEnumerator deactivate()
        {
            yield return new WaitForSeconds(1.5f);
            Arctransform.position = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
        }

        public void BuildSegments()
        {
            if (ReferenceEquals(arc,null)) return;

            ArcTimingManager timingManager = ATMAN;
            int offset = ArcAudioManager.Instance.AudioOffset;
            int duration = arc.EndTiming - arc.Timing;

            int v1 = duration < 1000 ? 14 : 7;
            float v2 = 1f / (v1 * duration / 1000f);
            int segSize = (int)(duration * v2);
            segmentCount = (segSize == 0 ? 0 : duration / segSize) + 1;
            //InstantiateSegment(segmentCount);
            segmentsleft = segmentCount;
            int segled = segmentCount;
            thearry = new Transform[segled];
            SEGTIME = new float[segled];

            //segmentCount = segmentCount*2;
            Vector3 start = new Vector3();
            Vector3 end = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart),
                                        ArcAlgorithm.ArcYToWorld(arc.YStart));
            var icount = 0;

            for (int i = 0; i < segmentCount - 1; ++i)
            {
               
                //segmentsleft -= 1;
                start = end;
                var tempend = end;
                if (icount == 1)
                {
                    //arcexdist = Vector3.Distance(vecdist, tempend);
                }
                end = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(arc.XStart, arc.XEnd, (i + 1f) * segSize / duration, arc.LineType)),
                                  ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(arc.YStart, arc.YEnd, (i + 1f) * segSize / duration, arc.LineType)),
                                  -timingManager.CalculatePositionByTimingAndStart(arc.Timing + offset, arc.Timing + offset + segSize * (i + 1)) / 1000f);

                
                //SEGTIME[icount] = (float)(i + 1f) * segSize / duration;

              
                    //GameObject waypointstart = Instantiate(wayprefab);
                    //waypointstart.transform.position = new Vector3(end.x, end.y, 0);

                   // emptytrans.transform.position = waypointstart.transform.position;
                    if (icount == 0)
                    {
                        vecdist = new Vector3(end.x, end.y, 0);
                    }
                //if (segmentsleft <= 3)
               // {
                    if (((icount) != segmentsleft))
                    {
                       // thearry[icount] = waypointstart.transform;
                        //SEGTIME[icount] = Vector3.Distance(tempend, end);
                    }
                    else
                    {

                    }
                //}
                /*else
                {
                    if (((icount + 2) != segmentsleft))
                    {
                        thearry[icount] = waypointstart.transform;
                        SEGTIME[icount] = Vector3.Distance(tempend, end);
                    }
                    else
                    {

                    }
                }*/
                
                
                    icount += 1;
                
                



                segments[i].BuildSegmentTrace(start, end, arc.IsVoid ? OffsetVoid : OffsetNormal, arc.Timing + segSize * i, arc.Timing + segSize * (i + 1),i+1);
                //segments[i].BuildCollider(start, end, arc.IsVoid ? OffsetVoid : OffsetNormal);
               

            }
            

                start = end;
            end = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XEnd),
                              ArcAlgorithm.ArcYToWorld(arc.YEnd),
                              -timingManager.CalculatePositionByTimingAndStart(arc.Timing + offset, arc.EndTiming + offset) / 1000f);
           segments[segmentCount - 1].BuildSegmentTrace(start, end, arc.IsVoid ? OffsetVoid : OffsetNormal, arc.Timing + segSize * (segmentCount - 1), arc.EndTiming,0);
            //segments[segmentCount-1].BuildCollider(start, end, arc.IsVoid ? OffsetVoid : OffsetNormal);
           
            var tmpct = icount;
            //icount -= 1;
            //SEGTIME[tmpct] = Vector3.Distance(emptytrans.position, end);


            
                if (arc.Color == 0)
                {
                    Color = ArcBlue;
                }
                else if (arc.Color == 1)
                {
                    Color = ArcRed;
                }
                else if (arc.Color == 2)
                {
                    Color = ArcGreen;
                }
                else if (arc.Color == 3)
                {
                    Color = ArcYellow;
                }
                else if (arc.Color == 4)
                {
                    Color = ArcGray;
                }
            
        }
        public void BuildHead()
        {
            Vector3 pos = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
            float offset =  OffsetNormal;

/*
            Vector3[] vertices = new Vector3[3];
            Vector2[] uv = new Vector2[3];
            int[] triangles = new int[] { 0, 1, 2, };


            vertices[0] = pos + new Vector3(-0.4f, offset / 2, 0);
            vertices[1] = pos + new Vector3(0, -offset / 2, offset / 2);
           // vertices[0] = pos + new Vector3(0.4f, offset / 2, 0);
            vertices[2] = pos + new Vector3(-offset, -offset / 2, 0);
           // vertices[4] = pos + new Vector3(-0.4f, offset / 2, 0);
           // vertices[5] = pos + new Vector3(-0.4f, offset / 2, 0);
           // vertices[6] = pos + new Vector3(0.4f, offset / 2, 0);
           // vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2);
           */
               Vector3[] vertices = new Vector3[9];
               Vector2[] uv = new Vector2[9];
               int[] triangles = new int[] { 0,1,2,4,2,0,8,7,5,0,7,5,4,2,3};

            if (offset == 0.9f)
            {


                vertices[1] = pos + new Vector3(offset, -offset / 2, 0);
                vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[0] = pos + new Vector3(0.4f, offset / 2, 0  );
                vertices[3] = pos + new Vector3(-offset, -offset / 2, 0 );
                vertices[4] = pos + new Vector3(-0.4f, offset / 2, 0 );
                vertices[5] = pos + new Vector3(-0.4f, offset / 2, 0);
                vertices[6] = pos + new Vector3(0.4f, offset / 2, 0 );
                vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2);
                vertices[8] = pos + new Vector3(-offset, -offset / 2, 0 );
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

            uv[0] = new Vector2(0,0);
            uv[1] = new Vector2(1, 0);
           uv[2] = new Vector2(0.5f, 1);
            uv[4] = new Vector2(0.93f,0);
            //uv[3] = new Vector2(1,1);

            uv[5] = new Vector2();
            uv[8] = new Vector2(1, 0);
            uv[7] = new Vector2(0.5f, 1);
            uv[6] = new Vector2(1, 1);



            /* Vector3[] vertices = new Vector3[4];
             Vector2[] uv = new Vector2[4];
             int[] triangles = new int[] { 0, 2, 1, 0, 3, 2, 0, 1, 2, 0, 2, 3 };

             vertices[0] = pos + new Vector3(0, offset / 2, 0);
             uv[0] = new Vector2();
             vertices[1] = pos + new Vector3(offset, -offset / 2, 0);
             uv[1] = new Vector2(1, 0);
             vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2);
             uv[2] = new Vector2(1, 1);
             vertices[3] = pos + new Vector3(-offset, -offset / 2, 0);
             uv[3] = new Vector2(1, 1);*/

            HeadFilter.mesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles.Take(9).ToArray()
            };
            if (offset == 0.9f)
            {


                vertices[1] = pos + new Vector3(offset, -offset / 2, 0 +1 );
                vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2+1 );
                vertices[0] = pos + new Vector3(0.4f, offset / 2, 0 +1);
                vertices[3] = pos + new Vector3(-offset, -offset / 2, 0 +1);
                vertices[4] = pos + new Vector3(-0.4f, offset / 2, 0 +1);
                vertices[5] = pos + new Vector3(-0.4f, offset / 2, 0 +1);
                vertices[6] = pos + new Vector3(0.4f, offset / 2, 0 +1);
                vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2 +1);
                vertices[8] = pos + new Vector3(-offset, -offset / 2, 0 +1);
            }
            else
            {
                vertices[1] = pos + new Vector3(offset, -offset / 2, 0 +1);
                vertices[2] = pos + new Vector3(0, -offset / 2, offset / 2+1 );
                vertices[0] = pos + new Vector3(0.1f, offset / 2, 0 +1);
                vertices[3] = pos + new Vector3(-offset, -offset / 2, 0+1);
                vertices[4] = pos + new Vector3(-0.1f, offset / 2, 0+1 );
                vertices[5] = pos + new Vector3(-0.1f, offset / 2, 0 +1);
                vertices[6] = pos + new Vector3(0.1f, offset / 2, 0 +1);
                vertices[7] = pos + new Vector3(0, -offset / 2, offset / 2+1 );
                vertices[8] = pos + new Vector3(-offset, -offset / 2, 0+1 );
            }

            /*HeadCollider.sharedMesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };*/
            ActualCap.transform.position = new Vector3(pos.x,pos.y,0);
            ActualCap.transform.parent = null;


            HeadRenderer.material = headMaterialInstance;
        }
        public void BuildCollider()
        {
            //if (arc.Timing > arc.EndTiming) return;

            List<Vector3> vert = new List<Vector3>();
            List<int> tri = new List<int>();

            float offset = OffsetNormal;

            Vector3 pos = segments[0].FromPos;
            vert.Add(pos + new Vector3(-offset, -offset / 2, 0));
            vert.Add(pos + new Vector3(0, offset / 2, 0));
            vert.Add(pos + new Vector3(offset, -offset / 2, 0));

            int t = 0;
            foreach (var seg in segments)
            {
                pos = seg.ToPos;
                vert.Add(pos + new Vector3(-offset, -offset / 2, 0));
                vert.Add(pos + new Vector3(0, offset / 2, 0));
                vert.Add(pos + new Vector3(offset, -offset / 2, 0));

                tri.AddRange(new int[] { t + 1, t, t + 3, t + 1, t + 3, t, t + 1, t + 3, t + 4, t + 1, t + 4, t + 3,
                    t + 1, t + 2, t + 5, t + 1, t + 5, t + 2, t + 1, t + 5, t + 4, t + 1, t + 4, t + 5 });
                t += 3;
            }
             
            ArcCollider.sharedMesh = new Mesh()
            {
                vertices = vert.ToArray(),
                triangles = tri.ToArray()
            }; 
        }

        public void UpdateArc()
        {
            if (!enable) return;

            //UpdateHead();
            //UpdateSegments();
            //UpdateHeightIndicator();
            //UpdateArcCap();
            changecappos();
        }
        public void changecappos()
        {
            //print("heyy!");
            //ActualCap.transform.position = Head.position;
            //if(Head.gameObject.ise)
            //ActualCap.transform.localScale = new Vector3(arc.IsVoid ? 0.21f : 0.35f, arc.IsVoid ? 0.21f : 0.35f);
        }

      

        private void UpdateSegments()
        {
            int currentTiming = ArcGameplayManager.Instance.Timing;
            ArcTimingManager timingManager = ATMAN;
            int offset = ArcAudioManager.Instance.AudioOffset;
            float z = arc.transform.localPosition.z;

            foreach (ArcHoldSegmentComponent s in segments)
            {
                if (-s.ToPos.z < z)
                {
                    
                    
                        s.Enable = true;
                        continue;
                   
                }
                if (-s.FromPos.z < z && -s.ToPos.z >= z)
                {
                    s.Enable = true;
                    s.CurrentArcMaterial = null;
                    s.CurrentShadowMaterial = null;
                    s.Alpha = currentColor.a;
                    if (arc.Judging)
                    {
                        s.From = (z + s.FromPos.z) / (-s.ToPos.z + s.FromPos.z);
                    }
                    else
                    {
                        s.From = 0;
                    }
                    continue;
                }
                float pos = -(z + s.FromPos.z);
                /*if (pos > 90 && pos < 100)
                {
                    s.Enable = true;
                    s.CurrentArcMaterial = null;
                    s.CurrentShadowMaterial = null;
                    s.Alpha = currentColor.a * (100 - pos) / 10f;
                    s.From = 0;
                }
                else if (pos > 100 || pos < -20)
                {
                    s.Enable = false;
                }
                else
                {*/
                    s.Enable = true;
                    s.Alpha = currentColor.a;
                    s.From = 0;
                    s.CurrentArcMaterial = arcBodySharedMaterial;
                    s.CurrentShadowMaterial = arcShadowSharedMaterial;
                //}
            }
        }
        public bool washead = false;
        private IEnumerator UpdateHead()
        {
            headisrunning = true;
            yield return new WaitForSeconds(0.0f);
            if (!arc.SHOULDHEAD||!MVCAP.firsthit==true)
            {
                HeadRenderer.enabled = false;
                HeightIndicatorRenderer.enabled = false;
                headisrunning = false;
                yield break;
            }

            float Position = ATMAN.CalculatePositionByTiming(ArcHold.Timing + ArcAudioManager.Instance.AudioOffset);
            int currentTiming = ArcGameplayManager.Instance.Timing;
            int offset = ArcAudioManager.Instance.AudioOffset;

            if (Position > 100000 || Position < -10000)
            {
                HeadRenderer.enabled = false;
                HeightIndicatorRenderer.enabled = false;
                headisrunning = false;
                yield break;
            }
            HeadRenderer.enabled  = true;
            washead = true;
            HeightIndicatorRenderer.enabled = true;
            //print("active?");
            if (Position > 90000 && Position <= 100000)
            {
                Head.localPosition = new Vector3();
                HeadRenderer.material = headMaterialInstance;
                Color c = currentColor;
                c.a = currentColor.a * (100000 - Position) / 100000;
                headMaterialInstance.SetColor(colorShaderId, c);
            }
            else if (Position < 0)
            {
                headMaterialInstance.SetColor(colorShaderId, currentColor);
                if (arc.Judging)
                {
                    if (segmentCount >= 1)
                    {
                        ArcHoldSegmentComponent s = segments[0];
                        int duration = s.ToTiming - s.FromTiming;
                        float t = duration == 0 ? 0 : ((-arc.Position / 1000f) / (-s.ToPos.z));
                        if (t > 1)
                        {
                            // HeadRenderer.enabled = false;
                            headisrunning = false;
                            yield break;
                        }
                        else if (t < 0) t = 0;
                        Head.localPosition = (s.ToPos - s.FromPos) * t;

                    }
                }
                else
                {
                    Head.localPosition = new Vector3();
                }
            }
            else
            {
                headMaterialInstance.SetColor(colorShaderId, currentColor);
                Head.localPosition = new Vector3();
            }
            headisrunning = false;
        }

        void spedchange()
        {
            if (gameObject.activeSelf==true)
            {
                
                    //print("owo");
                    arc.Position = ATMAN.CalculatePositionByTiming(arc.Timing + ArcAudioManager.Instance.AudioOffset);
                    if(!float.IsNaN(arc.Position))
                    transform.position = new Vector3(0, 0, (-arc.Position / 1000f));
                
            }
        }

        private void UpdateHeightIndicator()
        {
            if ((arc.YEnd == arc.YStart && !arc.SHOULDHEAD))
            {
                EnableHeightIndicator = false;
                return;
            }

            float pos = Arctransform.position.z;
            int currentTiming = ArcGameplayManager.Instance.Timing;
            if (pos < -90 && pos > -100)
            {
                Color c = currentColor;
                c.a = currentColor.a * (pos + 100) / 10;
                EnableHeightIndicator = true;
                HeightIndicatorRenderer.color = c;
            }
            else if (pos < -100 || pos > 10)
            {
                EnableHeightIndicator = true;
            }
            else
            {
                if (arc.Judging && pos > 0) EnableHeightIndicator = false;
                else EnableHeightIndicator = true;
                HeightIndicatorRenderer.color = currentColor;
            }
            
        }
        public GameObject notetransform;

        float updateInterval = 0.005f;
        public Rigidbody CAPBODY;
        private void OnEnable()
        {
            
            //CAPBODY = ArcCap.gameObject.GetComponent<Rigidbody>();

            //InvokeRepeating("UpdateInterval", updateInterval, updateInterval);
        }

        bool headisrunning = false;
        bool ACAPisrunning = false;
        bool canbackward = true;

        private void Update()
        {
            UpdateArcCap();
            if(!headisrunning)
            HEADS=StartCoroutine(UpdateHead());
            //UpdateHeightIndicator();
            if (ATMAN.IsBackwarding&&gameObject.activeSelf==true&&canbackward==true)
            {
                canbackward = false;
                StartCoroutine(backwarded());
            }
            else if (canbackward == false && !ATMAN.IsBackwarding)
            {
                StopAllCoroutines();
            }

        }

        /*private void FixedUpdate()
        {
            if (ArcCap.position == currenttarget)
            {
                //capisrun = false;
            }
            if (CANRENDER)
                
            if (ArcCap.position.z != 0)
            {
                //ArcCap.position = new Vector3(ArcCap.position.x, ArcCap.position.y, 0);
            }
        }*/

        public IEnumerator backwarded()
        {
            yield return new WaitUntil(() => !ATMAN.ShouldRender(ArcHold.Timing + ArcAudioManager.Instance.AudioOffset));
            MVCAP.gameObject.SetActive(false);
            //Arctransform.position = new Vector3(0, 0, 0);
            foreach (ArcHoldSegmentComponent s in segments)
            {
                s.gameObject.SetActive(false);
                s.transform.parent = null;
                s.transform.localPosition = Vector3.zero;

            }
            segments.Clear();
            
            //CAPBODY = null;
            //print("deactivateedd");
            ArcHoldManager.Instance.StartCoroutine(ArcHoldManager.Instance.ARCARCS(ArcHold));
            gameObject.SetActive(false);

        }

        void Start()
        {
            //print(Time.deltaTime);
            //CAPBODY = ArcCap.gameObject.GetComponent<Rigidbody>();
           
            updateInterval = Time.deltaTime;
        }
        /*public void LateUpdate()
        {
            if(CANRENDER)
            UpdateArcCap();
         
            //changecappos();


        }*/
        public Transform NOTEtransform;
        public Transform Arctransform;
        
        public void UpdateArcCap()
        {
            //if (!capisrun)
            //{
                //capisrun = true;
                //StartCoroutine(UPARCCAP());
                if(!ACAPisrunning)
                UPCCAP = StartCoroutine(UPARCCAP());
                
            //}
        }

        bool capisrun = false;
        bool capismoving = false;
        bool didscale = false;
        public Vector3 currenttarget;
        public Coroutine UPCCAP;
        public Coroutine HEADS;
        IEnumerator UPARCCAP()
        {
            
            ACAPisrunning = true;
            yield return new WaitForSeconds(0.0f);
            //if (CANRENDER)
            //{
            //print("MMKAY");
            if (!ATMAN.IsBackwarding && !ATMAN.IsStopped)
            {


                int currentTiming = ArcGameplayManager.Instance.Timing;
                int duration = arc.EndTiming - arc.Timing;
                int offset = ArcAudioManager.Instance.AudioOffset;

                //if (NOTEtransform.position.z + (Arctransform.position.z + 200f) > NOTEtransform.position.z && NOTEtransform.position.z + Arctransform.position.z < NOTEtransform.position.z + (ArcGameplayManager.Instance.Length / 3))
                //{
                float Position = ATMAN.CalculatePositionByTiming(ArcHold.Timing + offset);
                ArcHold.Position = Position;
                float EndPosition = ATMAN.CalculatePositionByTiming(ArcHold.EndTiming + offset);


                if (duration == 0)
                {
                    EnableArcCap = false;
                    //capisrun = false;
                    ACAPisrunning = false;
                    StopCoroutine(UPCCAP);
                    
                }
                if (ArcHold.firstarc)
                {
                    if (Position < 35 && EndPosition > 35 && Time.timeScale != 0)
                    {
                        if (!movestarted && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                        {
                            //movestarted = true;



                            /*if (MVCAP.specialhit == true)
                                print("OVERLAPPED!");*/

                            ArcCap.position = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
                            if (!ArcHold.firstarc) { ArcCap.localScale = new Vector3(0.4f, 0.395f, 1f); }
                            if (MVCAP.duriation != 0f)
                            {
                                MVCAP.thisobj.enabled = true;
                            }

                            MVCAP.isinrange = true;
                            // if (MVCAP.COMBO != 1 && !(MVCAP.duriation == 0f) && MVCAP.currentCOMBO != MVCAP.COMBO && MVCAP.duriation >= 0.02f)

                            if (MVCAP.COMBO != 1 && !(MVCAP.duriation <= 0f) && MVCAP.currentCOMBO != MVCAP.COMBO)
                            {
                                MVCAP.StartCoroutine("judgeholds");
                            }
                            else
                            {
                                MVCAP.StartCoroutine("givecombo");
                            }


                        }
                    }
                }
                else
                {
                    if (Position < 60 && EndPosition > 60 && Time.timeScale != 0)
                    {
                        if (!movestarted && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                        {
                            //movestarted = true;



                            /*if (MVCAP.specialhit == true)
                                print("OVERLAPPED!");*/

                            ArcCap.position = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
                            if (!ArcHold.firstarc) { ArcCap.localScale = new Vector3(0.4f, 0.395f, 1f); }
                            if (MVCAP.duriation != 0f)
                            {
                                MVCAP.thisobj.enabled = true;
                            }

                            MVCAP.isinrange = true;
                            // if (MVCAP.COMBO != 1 && !(MVCAP.duriation == 0f) && MVCAP.currentCOMBO != MVCAP.COMBO && MVCAP.duriation >= 0.02f)

                            if (MVCAP.COMBO != 1 && !(MVCAP.duriation <= 0f) && MVCAP.currentCOMBO != MVCAP.COMBO)
                            {
                                MVCAP.StartCoroutine("judgeholds");
                            }
                            else
                            {
                                MVCAP.StartCoroutine("givecombo");
                            }


                        }
                    }
                }

                if (Position > 0 && Position < 100000 && Time.timeScale != 0)
                {
                    if (arc.RenderHead&&ArcHold.firstarc)
                    {
                        
                        float p = 1 - Position / 100000;
                        float scale = (0.395f + 0.5f * (1 - p));
                        EnableArcCap = true;
                        
                        
                        //if (!didscale)
                        //{
                            //didscale = true;
                            ArcCap.localScale = new Vector3(scale, scale,1f);
                        ArcCapRenderer.color = new Color(1, 1, 1, p);
                        //ArcCap.localScale = new Vector3(1, 1);
                        //ArcCap.DOScale(0.395f,(((Arc.Timing+380f+offset) *1f-ArcGameplayManager.Instance.Timing*1f)/1000f));


                        ///}


                        ArcCap.position = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
                    }
                    else if (!ArcHold.connection) { ArcCap.position = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart)); }
                    else
                    {
                        EnableArcCap = false;                        
                    }
                }
                /* if (NOTEtransform.position.z+(Arctransform.position.z+100f) > NOTEtransform.position.z&& NOTEtransform.position.z +Arctransform.position.z <NOTEtransform.position.z+ 0f)
                 {

                     if (IsHead && !arc.IsVoid)
                     {
                         float p = 1 - NOTEtransform.position.z - Arctransform.position.z / 1.5f;
                         float scale = 0.35f + 0.5f * (1 - p);
                         EnableArcCap = true;
                         //ArcCapRenderer.color = new Color(1, 1, 1, p);
                         if (Mathf.Sign(p) == 1&&scale<10f&&scale>-1f)
                         {
                             //ArcCap.localScale = new Vector3(scale, scale, 1f);
                         }
                         ArcCap.position = new Vector3(ArcAlgorithm.ArcXToWorld(arc.XStart), ArcAlgorithm.ArcYToWorld(arc.YStart));
                     }
                     else
                     {
                         EnableArcCap = false;
                     }
                 }*/
                else if (Position < 0 && EndPosition > 0 && Time.timeScale != 0)
                {
                    //print("on");

                    if (!movestarted && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
                    {
                        movestarted = true;
                    }
                    HeightIndicatorRenderer.enabled = false;
                    if (ArcHold.firstarc)
                    {
                        EnableArcCap = true;
                        ArcCapRenderer.color = new Color(1, 1, 1, 1f);
                    }
                    ArcCap.localScale = new Vector3(0.4f, 0.395f, 1f);

                    if (ArcHold.firstarc)
                    {
                        foreach (var s in segments)
                        {
                            if (Position / 1000f < s.FromPos.z && Position / 1000f >= s.ToPos.z)
                            {
                                float t = ((s.FromPos.z - Position / 1000f) / (s.FromPos.z - s.ToPos.z));



                                // if (!capismoving)
                                // {



                                if (s.FromPos != s.ToPos && s.ToTiming - s.FromTiming != 0)
                                {

                                    //if (!capisrun)
                                    //{
                                    //capisrun = true;

                                    var tempvec = new Vector3(s.ToPos.x, s.ToPos.y, 0);

                                    //currenttarget = tempvec;
                                    var othervec = new Vector3(s.FromPos.x + (s.ToPos.x - s.FromPos.x) * t,
                                                              s.FromPos.y + (s.ToPos.y - s.FromPos.y) * t);
                                    //currenttarget = othervec;

                                    ArcCap.position = new Vector3(s.FromPos.x + (s.ToPos.x - s.FromPos.x) * t,
                                                        s.FromPos.y + (s.ToPos.y - s.FromPos.y) * t);

                                    // }


                                }

                                //CAPBODY.MovePosition(capv - vecc);


                                // StartCoroutine(movingcap(s.ToPos,s,Position));

                                //}


                                break;
                            }
                        }
                    }
                }
                else
                {
                   /* if (movestarted)
                    {
                        StartCoroutine(DisableARC());
                    }*/
                    if (movestarted && MVCAP.currentCOMBO == MVCAP.COMBO&&Time.timeScale!=0)
                    {
                        if (Time.timeScale == 0)
                        {
                            //EnableArcCap = true;
                        }
                        else
                        {
                            EnableArcCap = false;
                        }

                        MVCAP.JUDeffect.Stop();
                        //ArcCap.gameObject.SetActive(false);
                        StartCoroutine(DisableARC());
                    }
                    else
                    {
                        if(!ArcHold.firstarc)
                        EnableArcCap = false;

                    }
                }
            }
            else EnableArcCap = false;
            ACAPisrunning = false;
            //capisrun = false;
            //}

        }

        IEnumerator movingcap(Vector3 topos, ArcHoldSegmentComponent s, float t)
        {
            
            yield return new WaitForSeconds(0.0f);
            
            //ArcCap.position = Vector2.MoveTowards(ArcCap.position, topos,Time.deltaTime*16);
           ArcCap.position= new Vector3(s.FromPos.x + (s.ToPos.x - s.FromPos.x) * t,
                                                       s.FromPos.y + (s.ToPos.y - s.FromPos.y) * t);

        }

        bool extrahits = false;
        public bool readytodeac = false;
       IEnumerator DisableARC()
        {
            
            

            //MVCAP.transform.position = Vector3.zero;
            if(!ArcHold.firstarc)
                MVCAP.gameObject.SetActive(false);
            else { EnableArcCap = true; }
            //MVCAP.transform.parent = this.transform;
            yield return new WaitForSeconds(0.3f);
            Arctransform.position = new Vector3(0, 0, 0);
            foreach (ArcHoldSegmentComponent s in segments)
            {
                s.gameObject.SetActive(false);
                //s.Color = Color;
                
                s.transform.parent = null;
                s.transform.localPosition = Vector3.zero;
                //print(s.transform.position.z);
                

            }
            segments.Clear();
            
            //CAPBODY = null;
            //print("deactivateedd");
            gameObject.SetActive(false);
        }


        public movecaphold MVCAP;
        public bool movestarted = false;
       

        public bool IsMyself(GameObject gameObject)
        {
            return ArcCollider.gameObject.Equals(gameObject) || HeadCollider.gameObject.Equals(gameObject);
        }
    }
}