using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Arcaoid.Gameplay
{
    public class ArcArcSegmentComponent : MonoBehaviour
    {
        public Color ShadowColor;
        public Material ArcMaterial, ShadowMaterial;
        public MeshRenderer SegmentRenderer, ShadowRenderer;
        public const float OffsetNormal = 0.9f;
        public const float OffsetVoid = 0.15f;
        public MeshCollider ArcCollider;
        public MeshFilter SegmentFilter, ShadowFilter;
        public Texture2D DefaultTexture, HighlightTexture;
        public float OFFSET;
        public readonly Color ArcVoid = new Color(0.5686275f, 0.4705882f, 0.6666667f, 0.20f);
        public int FromTiming, ToTiming;
        public Vector3 FromPos, ToPos;
        public bool wasfrom = false;

        private void OnDisable()
        {


            //ArcCollider.sharedMesh.Clear();
            if (!(ReferenceEquals(SegmentFilter, null)) && !(ReferenceEquals(ShadowFilter, null)))
            {
                SegmentFilter.mesh.Clear();
                ShadowFilter.mesh.Clear();
            }
            
            // resetonce = false;
         
            OFFSET = 0;
            FromTiming = 0;
            ToTiming = 0;
            FromPos = Vector3.zero;
            ToPos = Vector3.zero;
            From = 0;
            ongoingfrom = 0;
            currentFrom = 0;
            wasfrom = false;
            //zscale.localScale = Vector3.one;
              enable = false;
          usingArcInstanceMaterial = true;
          usingShadowInstanceMaterial = true;
          highlighted = false;
          renderQueue = 3000;
        AAR=null;
          shrinkrender = false;
          distance=0;
          distance2=0;
          minimumspeed=0;
          finaltime=0;
          didlerp = false;
         MVCAP=null;
        actualfrom=0;
          shouldrender = false;


    }
        
        IEnumerator DISABLE()
        {
            yield return new WaitForSeconds(2.0f);
            gameObject.SetActive(false);
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
                    SegmentRenderer.enabled = value;
                    ShadowRenderer.enabled = value;
                }
            }
        }
        public float From
        {
            get
            {
                return currentFrom;
            }
            set
            {
                if (currentFrom != value)
                {
                    currentFrom = value;
                    bodyMaterialInstance.SetFloat(fromShaderId, value);
                    shadowMaterialInstance.SetFloat(fromShaderId, value);
                }
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
                    bodyMaterialInstance.SetColor(colorShaderId, value);
                    Color c = ShadowColor;
                    c.a = value.a * 0.3f;
                    shadowMaterialInstance.SetColor(colorShaderId, c);
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
                    bodyMaterialInstance.SetColor(colorShaderId, currentColor);
                    Color c = ShadowColor;
                    c.a = value * 0.3f;
                    shadowMaterialInstance.SetColor(colorShaderId, c);
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
                    bodyMaterialInstance.mainTexture = highlighted ? HighlightTexture : DefaultTexture;
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
                    bodyMaterialInstance.renderQueue = value;
                    shadowMaterialInstance.renderQueue = value;
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
                    bodyMaterialInstance.SetInt(highlightShaderId, value ? 1 : 0);
                    selected = value;
                }
            }
        }

        public Material CurrentArcMaterial
        {
            set
            {
                if (ReferenceEquals(value, null))
                {
                    if (usingArcInstanceMaterial == false)
                    {
                        SegmentRenderer.material = bodyMaterialInstance;
                        usingArcInstanceMaterial = true;
                    }
                }
                else
                {
                    if (usingArcInstanceMaterial == true)
                    {
                        SegmentRenderer.material = value;
                        usingArcInstanceMaterial = false;
                    }
                }
            }
        }
        public Material CurrentShadowMaterial
        {
            set
            {
                if (ReferenceEquals(value, null))
                {
                    if (usingShadowInstanceMaterial == false)
                    {
                        ShadowRenderer.material = shadowMaterialInstance;
                        usingShadowInstanceMaterial = true;
                    }
                }
                else
                {
                    if (usingShadowInstanceMaterial == true)
                    {
                        ShadowRenderer.material = value;
                        usingShadowInstanceMaterial = false;
                    }
                }
            }
        } 

        private bool enable = false;
        private bool selected = false;
        private bool usingArcInstanceMaterial = true;
        private bool usingShadowInstanceMaterial = true;
        private bool highlighted = false;
        private int fromShaderId;
        private int colorShaderId;
        private int highlightShaderId;
        private int renderQueue = 3000;
        private float currentFrom = 0;
        private Color currentColor;
        public Material bodyMaterialInstance, shadowMaterialInstance;
        public ArcArcRenderer AAR;
        public bool shrinkrender = false;
        public float distance;
        public float distance2;
        public float minimumspeed;
        public float finaltime;
        public bool speedwasset = false;
        public Renderer REND;
        public Transform RBNT;
        public bool didlerp = false;
        public movecap MVCAP;
        float actualfrom;
        public bool shouldrender = false;
        public Transform zscale;
        
        void Awake()
        {
            REND = this.GetComponent<Renderer>();
            RBNT = GameObject.FindGameObjectWithTag("NOTEBODY").GetComponent<Transform>();
            bodyMaterialInstance = Instantiate(ArcMaterial);
            zscale = this.transform;
            shadowMaterialInstance = Instantiate(ShadowMaterial);
            SegmentRenderer.material = bodyMaterialInstance;
            ShadowRenderer.material = shadowMaterialInstance;
            SegmentRenderer.sortingLayerName = "Arc";
            SegmentRenderer.sortingOrder = 1;
            ShadowRenderer.sortingLayerName = "Arc";
            ShadowRenderer.sortingOrder = 0;
            fromShaderId = Shader.PropertyToID("_From");
            colorShaderId = Shader.PropertyToID("_Color");
            highlightShaderId = Shader.PropertyToID("_Highlight");
        }

        void OnEnable()
        {
            
            //Movespeed = ((AAR.Arc.EndTiming - AAR.Arc.Timing) / 1000f);        
            didlerp = false;
            lerpyrunning = false;
            
            
                AAR = transform.GetComponentInParent<ArcArcRenderer>();
            if (!(ReferenceEquals(AAR, null)))
            {
                if (!(ReferenceEquals(AAR.ActualCap, null)))
                    MVCAP = AAR.ActualCap.GetComponent<movecap>();
            }
              

            
            SegmentRenderer.enabled = true;
            ShadowRenderer.enabled = true;
            //resetonce = false;
        }

        public float ongoingfrom=0;

        private void Update()
        {
            float z = AAR.transform.position.z;
            if (-FromPos.z < z+200 && -ToPos.z >= z-200 && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding&&Time.timeScale!=0)
            {
                ongoingfrom = (z + FromPos.z) / (-ToPos.z + FromPos.z);
                if (AAR.Arc.IsVoid)               
                {
                    From = ongoingfrom;
                }
                else if(shrinkrender)
                {
                    if (!wasfrom)
                    {
                        wasfrom = true;
                       // zscale.localScale = new Vector3(zscale.localScale.x, zscale.localScale.y, 1-ongoingfrom);
                        From = ongoingfrom;
                    }
                    //zscale.localScale = new Vector3(zscale.localScale.x, zscale.localScale.y, 1-((z + FromPos.z) / (-ToPos.z + FromPos.z)));
                   From = (z + FromPos.z) / (-ToPos.z + FromPos.z);

                    //s.From = 0;
                }
                
            }
            if (!(ReferenceEquals(AAR, null)))
            {
                if (OFFSET == 0.15f)
                {
                    Color = ArcVoid;
                }
                if (OFFSET == 0.15f)
                {

                    //shrinkrender = true;
                }
                else
                {
                    if (MVCAP.iscolliding && MVCAP.notemissed == false)
                    {

                        shrinkrender = true;
                        StopCoroutine(LERPY());

                    }
                    else
                    {
                        if (!lerpyrunning)
                            StartCoroutine(LERPY());
                    }
                }
               
            }
        }
        private void FdUpdate()
        {
            if (!(ReferenceEquals(AAR, null)))
            {
                if (OFFSET == 0.15f)
                {
                    Color = ArcVoid;
                }
                /*if (OFFSET == 0.15f)
                {
                    if (Color != ArcVoid)
                    {
                        Color = ArcVoid;
                    }
                }*/
                //this.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                /* if (!MVCAP.notemissed&&Color==MVCAP.parentArc.MISSED)
                 {
                     
                     else
                     {
                         if (Color != MVCAP.parentArc.currentcolor)
                         {
                             Color = MVCAP.parentArc.currentcolor;
                         }
                     }
                     resetonce = true;
                 }*/


                if (OFFSET == 0.15f)
                    {
                    
                        //shrinkrender = true;
                    }
                    else
                    {
                        if (MVCAP.iscolliding && MVCAP.notemissed == false)
                        {

                            shrinkrender = true;
                        StopCoroutine(LERPY());

                        }
                        else
                        {
                        if(!lerpyrunning)
                        StartCoroutine(LERPY());
                        }



                        
                        
                            /*if (shouldrender && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding && Time.timeScale != 0)
                            {
                                float tempmove1 = AAR.Arc.Timing - 10000f;
                                float tempend = AAR.Arc.EndTiming - 10000f;
                                Movespeed = ((ToTiming - FromTiming) / 1000f);
                                actualfrom = Mathf.MoveTowards(actualfrom, 1f, Time.deltaTime * (1f / Movespeed));
                        
                    }*/
                        

                    }
                   /* if (shrinkrender && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding)
                    {
                  


                        //didlerp = true;
                        //StartCoroutine(LERPY());
                        float tempmove1 = AAR.Arc.Timing - 10000f;
                        float tempend = AAR.Arc.EndTiming - 10000f;
                        Movespeed = ((ToTiming - FromTiming) / 1000f);
                       

                            if (!AAR.Arc.IsVoid)
                            {
                                if (From != actualfrom && !MVCAP.wasfrom)
                                {
                                    MVCAP.wasfrom = true;
                                    From = actualfrom;
                                }
                            }
                            From = Mathf.MoveTowards(From, 1f, Time.deltaTime * (1f / Movespeed));

                            //From = 0;
                        
                    }*/
                
            }
        }
        public bool lerpyrunning = false;

       IEnumerator LERPY()
        {
            lerpyrunning = true;
            yield return new WaitForSeconds(1f);
            shrinkrender = false;
            wasfrom = false;
            lerpyrunning = false;
            
        }

        void Setminspeed()
        {
            foreach (float DIST in AAR.SEGTIME)
            {
                minimumspeed += DIST;
            }
            minimumspeed = Movespeed / minimumspeed;
            //print(minimumspeed);
            finaltime = distance * minimumspeed;
            distance2 = finaltime -  (minimumspeed);
            
            

            //0.0000000009313226f
            //0.000000001396984f
            // float average = (0.0000000009313226f + 0.000000001396984f + 0.000000003259629f) / 3f;

            float timing = ToTiming-FromTiming;
               finaltime =  minimumspeed;
               
            
            
        }

        /*private void OnTriggerEnter(Collider other)
        {
            if (other.name.Contains("trig?"))
            {
                
                if (OFFSET == 0.9f)
                {
                    shouldrender = true;
                }
                else
                {
                    shrinkrender = true;
                }

                if (OFFSET == 0.9f && segvalue == 1)
                {

                    //AAR.StartCoroutine(AAR.Washit());
                    //Alpha = 1;
                }
            }
        }*/

      
        private void OnDestroy()
        {
            Destroy(bodyMaterialInstance);
        }

        public void BuildSegment(Vector3 fromPos, Vector3 toPos, float offset, int from, int to)
        {
           FromTiming = from;
            ToTiming = to;
            FromPos = fromPos;
            ToPos = toPos;
            OFFSET = offset;

            if (fromPos == toPos) return;

            //BuildCollider();

            Vector3[] vertices = new Vector3[12];
            Vector2[] uv = new Vector2[12];
            int[] triangles = new int[] { 0,3,2,0,2,1,4,0,1,4,1,5,4,7,6,4,6,5,11,8,9,11,9,10 };

          
           

            vertices[0] = fromPos + new Vector3(0.4f, offset / 2, 0);
            vertices[1] = toPos + new Vector3(0.4f, offset / 2, 0);
            vertices[2] = toPos + new Vector3(offset, -offset / 2, 0);
            vertices[3] = fromPos + new Vector3(offset, -offset / 2, 0);
            vertices[4] = fromPos + new Vector3(-0.4f, offset / 2, 0);
            vertices[5] = toPos + new Vector3(-0.4f, offset / 2, 0);
            vertices[6] = toPos + new Vector3(-offset, -offset / 2, 0);
            vertices[7] = fromPos + new Vector3(-offset, -offset / 2, 0);

            vertices[8] = fromPos + new Vector3(0.4f, offset / 2, 0);
            vertices[9] = toPos + new Vector3(0.4f, offset / 2, 0);
            vertices[11] = fromPos + new Vector3(-0.4f, offset / 2, 0);
            vertices[10] = toPos + new Vector3(-0.4f, offset / 2, 0);


            uv[0] = new Vector2();
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);
            uv[4] = new Vector2();
            uv[5] = new Vector2(0, 1);
            uv[6] = new Vector2(1, 1);
            uv[7] = new Vector2(1, 0);

            uv[11] = new Vector2();
            uv[10] = new Vector2(0, 1);
            uv[9] = new Vector2(1, 1);
            uv[8] = new Vector2(1, 0);

            /*   Vector3[] vertices = new Vector3[6];
               Vector2[] uv = new Vector2[6];
               int[] triangles = new int[] { 0, 3, 2, 0, 2, 1, 0, 5, 4, 0, 4, 1 };

               vertices[0] = fromPos + new Vector3(0, offset / 2, 0);
               uv[0] = new Vector2();
               vertices[1] = toPos + new Vector3(0, offset / 2, 0);
               uv[1] = new Vector2(0, 1);
               vertices[2] = toPos + new Vector3(offset, -offset / 2, 0);
               uv[2] = new Vector2(1, 1);
               vertices[3] = fromPos + new Vector3(offset, -offset / 2, 0);
               uv[3] = new Vector2(1, 0);
               vertices[4] = toPos + new Vector3(-offset, -offset / 2, 0);
               uv[4] = new Vector2(1, 1);
               vertices[5] = fromPos + new Vector3(-offset, -offset / 2, 0);
               uv[5] = new Vector2(1, 0);*/

            SegmentFilter.mesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles 
            };

            



            Vector3[] shadowvertices = new Vector3[4];
            Vector2[] shadowuv = new Vector2[4];
            int[] shadowtriangles = new int[6];

            shadowvertices[0] = fromPos + new Vector3(-offset, -fromPos.y, 0);
            shadowuv[0] = new Vector2();
            shadowvertices[1] = toPos + new Vector3(-offset, -toPos.y, 0); ;
            shadowuv[1] = new Vector2(0, 1);
            shadowvertices[2] = toPos + new Vector3(offset, -toPos.y, 0);
            shadowuv[2] = new Vector2(1, 1);
            shadowvertices[3] = fromPos + new Vector3(offset, -fromPos.y, 0);
            shadowuv[3] = new Vector2(1, 0);

            shadowtriangles[0] = 0;
            shadowtriangles[1] = 1;
            shadowtriangles[2] = 2;
            shadowtriangles[3] = 0;
            shadowtriangles[4] = 2;
            shadowtriangles[5] = 3;

            ShadowFilter.mesh = new Mesh()
            {
                vertices = shadowvertices,
                uv = shadowuv,
                triangles = shadowtriangles
            }; 
        }

        public void BuildCollider(Vector3 fromPos, Vector3 toPos, float offset)
        {
            //if (arc.Timing > arc.EndTiming) return;
           
            List<Vector3> vert = new List<Vector3>();
            List<int> tri = new List<int>();

            

            Vector3 pos = fromPos;
            vert.Add(pos + new Vector3(-offset, -offset / 2, 0));
            vert.Add(pos + new Vector3(0, offset / 2, 0));
            vert.Add(pos + new Vector3(offset, -offset / 2, 0));

            int t = 0;
            
                pos = toPos;
                vert.Add(pos + new Vector3(-offset, -offset / 2, 0));
                vert.Add(pos + new Vector3(0, offset / 2, 0));
                vert.Add(pos + new Vector3(offset, -offset / 2, 0));

                tri.AddRange(new int[] { t + 1, t, t + 3, t + 1, t + 3, t, t + 1, t + 3, t + 4, t + 1, t + 4, t + 3,
                    t + 1, t + 2, t + 5, t + 1, t + 5, t + 2, t + 1, t + 5, t + 4, t + 1, t + 4, t + 5 });
                //t += 3;

           
            ArcCollider.sharedMesh = new Mesh()
            {
                vertices = vert.ToArray(),
                triangles = tri.ToArray()

            };
            // ArcCollider.sharedMesh.Clear();

          

        }



        public float Movespeed;
        public int segvalue;

        public void BuildSegmentTrace(Vector3 fromPos, Vector3 toPos, float offset, int from, int to, int segcount)
        {
            FromTiming = from;
            ToTiming = to;
            FromPos = fromPos;
            ToPos = toPos;
            OFFSET = offset;
            segvalue = segcount;

            if (fromPos == toPos) return;

            Vector3[] vertices = new Vector3[12];
            Vector2[] uv = new Vector2[12];
            int[] triangles = new int[] { 0, 3, 2, 0, 2, 1, 4, 0, 1, 4, 1, 5, 4, 7, 6, 4, 6, 5, 11, 8, 9, 11, 9, 10 };
            if (offset==0.9f)
            {


                vertices[0] = fromPos + new Vector3(0.4f, offset / 2, 0);
                vertices[1] = toPos + new Vector3(0.4f, offset / 2, 0);
                vertices[2] = toPos + new Vector3(offset, -offset / 2, 0);
                vertices[3] = fromPos + new Vector3(offset, -offset / 2, 0);
                vertices[4] = fromPos + new Vector3(-0.4f, offset / 2, 0);
                vertices[5] = toPos + new Vector3(-0.4f, offset / 2, 0);
                vertices[6] = toPos + new Vector3(-offset, -offset / 2, 0);
                vertices[7] = fromPos + new Vector3(-offset, -offset / 2, 0);

                vertices[8] = fromPos + new Vector3(0.4f, offset / 2, 0);
                vertices[9] = toPos + new Vector3(0.4f, offset / 2, 0);
                vertices[11] = fromPos + new Vector3(-0.4f, offset / 2, 0);
                vertices[10] = toPos + new Vector3(-0.4f, offset / 2, 0);
            }
            else
            {
                vertices[0] = fromPos + new Vector3(0.1f, offset / 2, 0);
                vertices[1] = toPos + new Vector3(0.1f, offset / 2, 0);
                vertices[2] = toPos + new Vector3(offset, -offset / 2, 0);
                vertices[3] = fromPos + new Vector3(offset, -offset / 2, 0);
                vertices[4] = fromPos + new Vector3(-0.1f, offset / 2, 0);
                vertices[5] = toPos + new Vector3(-0.1f, offset / 2, 0);
                vertices[6] = toPos + new Vector3(-offset, -offset / 2, 0);
                vertices[7] = fromPos + new Vector3(-offset, -offset / 2, 0);

                vertices[8] = fromPos + new Vector3(0.1f, offset / 2, 0);
                vertices[9] = toPos + new Vector3(0.1f, offset / 2, 0);
                vertices[11] = fromPos + new Vector3(-0.1f, offset / 2, 0);
                vertices[10] = toPos + new Vector3(-0.1f, offset / 2, 0);
            }

            uv[0] = new Vector2();
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);
            uv[4] = new Vector2();
            uv[5] = new Vector2(0, 1);
            uv[6] = new Vector2(1, 1);
            uv[7] = new Vector2(1, 0);

            uv[11] = new Vector2();
            uv[10] = new Vector2(0, 1);
            uv[9] = new Vector2(1, 1);
            uv[8] = new Vector2(1, 0);

            /*   Vector3[] vertices = new Vector3[6];
               Vector2[] uv = new Vector2[6];
               int[] triangles = new int[] { 0, 3, 2, 0, 2, 1, 0, 5, 4, 0, 4, 1 };

               vertices[0] = fromPos + new Vector3(0, offset / 2, 0);
               uv[0] = new Vector2();
               vertices[1] = toPos + new Vector3(0, offset / 2, 0);
               uv[1] = new Vector2(0, 1);
               vertices[2] = toPos + new Vector3(offset, -offset / 2, 0);
               uv[2] = new Vector2(1, 1);
               vertices[3] = fromPos + new Vector3(offset, -offset / 2, 0);
               uv[3] = new Vector2(1, 0);
               vertices[4] = toPos + new Vector3(-offset, -offset / 2, 0);
               uv[4] = new Vector2(1, 1);
               vertices[5] = fromPos + new Vector3(-offset, -offset / 2, 0);
               uv[5] = new Vector2(1, 0);*/

            SegmentFilter.mesh = new Mesh()
            {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };

            Vector3[] shadowvertices = new Vector3[4];
            Vector2[] shadowuv = new Vector2[4];
            int[] shadowtriangles = new int[6];

            shadowvertices[0] = fromPos + new Vector3(-offset, -fromPos.y, 0);
            shadowuv[0] = new Vector2();
            shadowvertices[1] = toPos + new Vector3(-offset, -toPos.y, 0); ;
            shadowuv[1] = new Vector2(0, 1);
            shadowvertices[2] = toPos + new Vector3(offset, -toPos.y, 0);
            shadowuv[2] = new Vector2(1, 1);
            shadowvertices[3] = fromPos + new Vector3(offset, -fromPos.y, 0);
            shadowuv[3] = new Vector2(1, 0);

            shadowtriangles[0] = 0;
            shadowtriangles[1] = 1;
            shadowtriangles[2] = 2;
            shadowtriangles[3] = 0;
            shadowtriangles[4] = 2;
            shadowtriangles[5] = 3;

            ShadowFilter.mesh = new Mesh()
            {
                vertices = shadowvertices,
                uv = shadowuv,
                triangles = shadowtriangles
            };
            SegmentRenderer.enabled = true;
            ShadowRenderer.enabled = true;
        }

      

    }
}