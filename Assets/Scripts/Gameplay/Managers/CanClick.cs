using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Arcaoid.Gameplay.Chart;


public class CanClick : MonoBehaviour {
    public bool canclick = false;
    public float startTime = 3f;
    public string notetype;
    public bool ispure = false;
    public Transform clicktransform;
    public GameObject truetransform;
    public GameObject Model;
    public GameObject Shadow;
    public GameObject Arctrans;
    public ArcTap TAP;
    public ArcArcTap ATAP;
    public ArcAudioManager AOFFSET;
    public ArcGameplayManager AMAN;
    public float TIM;
    public bool didsend = false;
    public LayerMask mask;
    //public BoxCollider COLLIDER;
    public ArcTimingManager ATMAN;
    public ArcScoreManager ACMAN;
    

    // Use this for initialization
    void Awake()
    {
        AOFFSET = ArcAudioManager.Instance;
        AMAN = ArcGameplayManager.Instance;
        ATMAN = ArcTimingManager.Instance;
        ACMAN = ArcScoreManager.Instance;
        if (gameObject.tag == "TapNote")
        {
            notetype = "judgebar";
            ATMAN.OnSpeedChange.AddListener(speedTAP);
        } else if (gameObject.tag == "ArcTap")
        {
            notetype = "judgeplane";
            ATMAN.OnSpeedChange.AddListener(speedATAP);
        }
        clicktransform = this.transform;
    }

    void speedTAP()
    {
        if (isActiveAndEnabled)
        {
            //print("qwq");
            var POSS = ATMAN.CalculatePositionByTiming(TAP.Timing + AOFFSET.AudioOffset);
            POSS = POSS / 1000f;
            float pos = POSS;
            transform.position = new Vector3(transform.position.x, 0, -(pos));
        }
    }

    void speedATAP()
    {
        if (isActiveAndEnabled && !(AMAN.Timing < 100))
        {
            //print("*_*");
            float t = 1f * (ATAP.Timing - ATAP.Arc.Timing) / (ATAP.Arc.EndTiming - ATAP.Arc.Timing);
            var Pose = ATMAN.CalculatePositionByTiming(ATAP.Arc.Timing + AOFFSET.AudioOffset);
            // Vector3 vecc = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(ATAP.Arc.XStart, ATAP.Arc.XEnd, t, ATAP.Arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(ATAP.Arc.YStart, ATAP.Arc.YEnd, t, ATAP.Arc.LineType)) - 0.5f), (-101f));
            // Vector3 vecc = new Vector3((ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(ATAP.Arc.XStart, ATAP.Arc.XEnd, t, ATAP.Arc.LineType))), (ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(ATAP.Arc.YStart, ATAP.Arc.YEnd, t, ATAP.Arc.LineType)) - 0.5f), -(-ATMAN.CalculatePositionByTimingAndStart(ATAP.Arc.Timing + AOFFSET.AudioOffset, AMAN.Timing + AOFFSET.AudioOffset) / 1000f));
            Vector3 vecc = new Vector3(ArcAlgorithm.ArcXToWorld(ArcAlgorithm.X(ATAP.Arc.XStart, ATAP.Arc.XEnd, t, ATAP.Arc.LineType)),
                                      ArcAlgorithm.ArcYToWorld(ArcAlgorithm.Y(ATAP.Arc.YStart, ATAP.Arc.YEnd, t, ATAP.Arc.LineType)) - 0.5f,
                                      -ATMAN.CalculatePositionByTimingAndStart(ATAP.Arc.Timing + AOFFSET.AudioOffset, ATAP.Timing + AOFFSET.AudioOffset) / 1000f - 0.6f);
            if (!float.IsNaN(vecc.x) && !float.IsNaN(vecc.y) && !float.IsNaN(vecc.z))
            {
                Arctrans.transform.position = new Vector3(0, 0, (-Pose / 1000f));
                //truetransform.transform.position = vecc;
                Model.transform.localPosition = vecc;
                if (Model.transform.localPosition.y <= 0.6f)
                {
                    //truetransform.transform.position = new Vector3(truetransform.transform.position.x, 0.6f, truetransform.transform.position.z); 
                    Model.transform.localPosition = new Vector3(vecc.x, 0.6f, vecc.z);
                }
                Vector3 p = new Vector3(vecc.x, 0, vecc.z);
                if (!float.IsNaN(p.y)&& !float.IsNaN(p.x)&& !float.IsNaN(p.z))
                    Shadow.transform.localPosition = p;
            }
        }
    }

    public IEnumerator checkclick()
    {
        yield return new WaitForSeconds(0.0f);
        canclick = true;
        ispure = false;
    }
    //fixes taps behind arcs and holds
    void sendray()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 150f, mask))
        {
            if (hit.collider.tag == "ArcCaps")
            {
                var CAP = hit.collider.GetComponent<movecap>();
                if (!CAP.parentArc.Arc.IsVoid)
                {
                    if (Mathf.Abs(TAP.Timing - CAP.parentArc.Arc.Timing) <= 175f && Mathf.Abs(TAP.Timing - CAP.parentArc.Arc.Timing) != 0)
                    {
                        TAP.shouldwait = true;
                        TAP.TIME = (int)((TAP.Timing - CAP.parentArc.Arc.Timing) / 1.2) + CAP.parentArc.Arc.Timing;
                        //print("SHOULD WAIT");
                        //print(ACTAP.TIME);
                    }
                }
            }
            else
            {
                var CAP = hit.collider.GetComponent<shrinkhold>();
                if (CAP != null)
                {
                    if (Mathf.Abs(TAP.Timing - CAP.HOLD.EndTiming) <= 175f && Mathf.Abs(TAP.Timing - CAP.HOLD.EndTiming) != 0)
                    {
                        TAP.holdwait = true;
                        TAP.TIME = (int)((TAP.Timing - CAP.HOLD.EndTiming) / 1.2) + CAP.HOLD.EndTiming;
                        //print("HOLD WAIT");
                        //print(ACTAP.TIME);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (ATMAN.IsBackwarding&&canbackward==true)
        {
            canbackward = false;
           StartCoroutine(backwarded());
        }else if (canbackward == false&&!ATMAN.IsBackwarding)
        {
            StopAllCoroutines();
        }
    }

    bool canbackward = true;

    IEnumerator backwarded()
    {
        
        if (notetype == "judgebar")
        {
            yield return new WaitUntil(() => !ATMAN.ShouldRender(TAP.Timing + AOFFSET.AudioOffset));
        }
        else
        {
            if(ArcArcManager.Instance.alternaterender)
             yield return new WaitUntil(() => ATMAN.ShouldRendArc(ATAP.Timing + AOFFSET.AudioOffset));
            else
             yield return new WaitUntil(() => !ATMAN.ShouldRender(ATAP.Timing + AOFFSET.AudioOffset));

            //yield return new WaitUntil(() => ATMAN.ShouldRendArc(ATAP.Timing + AOFFSET.AudioOffset) && !ATMAN.IsBackwarding && !ATMAN.IsStopped);
        }

        if (notetype == "judgebar")
        {


            //TAP.TPOBJ = null;
            if (!(ReferenceEquals(TAP.LINE, null)))
            {
                TAP.LINE.transform.parent = null;
                TAP.LINE.gameObject.SetActive(false);
            }
            
            
            TAP.TPOBJ = null;
            ATMAN.noteQuad1.Remove(this);
            ATMAN.noteQuad2.Remove(this);
            ATMAN.noteQuad3.Remove(this);
            ATMAN.noteQuad4.Remove(this);
            ArcTapNoteManager.Instance.StartCoroutine(ArcTapNoteManager.Instance.TAPS(TAP));
            gameObject.SetActive(false);
        }
        else if (notetype == "judgeplane")
        {
            //transform.parent.gameObject.SetActive(false);
            ATAP.TPOBJ = null;
            ATMAN.noteQuad1.Remove(this);
            ATMAN.noteQuad2.Remove(this);
            ATMAN.noteQuad3.Remove(this);
            ATMAN.noteQuad4.Remove(this);
            ArcArcManager.Instance.StartCoroutine(ArcArcManager.Instance.ARCTAPS(ATAP,ATAP.Arc));
            truetransform.SetActive(false);
        }
    }

    IEnumerator ATAPS()
    {
        yield return new WaitUntil(() => ATAP != null);
        TIM = ATAP.Timing;
        yield return new WaitUntil(() => ATAP.Timing + AOFFSET.AudioOffset - 202 - AMAN.JudgeOffset <= AMAN.Timing);

        

        didclick = true;
        canclick = true;

        yield return new WaitUntil(() => ATAP.Timing + AOFFSET.AudioOffset + 100 <= AMAN.Timing);

        ran = true;

        ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(clicktransform.position.x, clicktransform.position.y + 1.2f), "LOST", 0f);
        ArcTimingManager.COMBO = 0;
        ArcScoreManager.MAXLOSTS += 1;
        ACMAN.PM.enabled = false;
        ACMAN.FR.enabled = false;
        if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
        {
            ACMAN.CLEARRATE.fillAmount -= 0.02f;

        }
        else if (ArcScoreManager.guagetypeno == 1)
        {
            ACMAN.CLEARRATE.fillAmount -= 0.015f;

        }
        else if (ArcScoreManager.guagetypeno == 2)
        {
            if ((ACMAN.NOTETOTAl <= 16) == false)
            {
                if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                {
                    ACMAN.CLEARRATE.fillAmount -= 0.085f;

                }
                else
                {
                    ACMAN.CLEARRATE.fillAmount -= 0.045f;
                }
            }
            else
            {
                ACMAN.CLEARRATE.fillAmount -= 1f;
                ArcScoreManager.CURRENTPERCENT = 0;
            }
        }

        if (notetype == "judgebar")
        {

            //TAP.TPOBJ = null;
            if (!(ReferenceEquals(TAP.LINE, null)))
            {
                TAP.LINE.transform.parent = null;
                TAP.LINE.gameObject.SetActive(false);
            }

            ATMAN.noteQuad1.Remove(this);
            ATMAN.noteQuad2.Remove(this);
            ATMAN.noteQuad3.Remove(this);
            ATMAN.noteQuad4.Remove(this);
            gameObject.SetActive(false);
        }
        else if (notetype == "judgeplane")
        {
            //transform.parent.gameObject.SetActive(false);
            ATMAN.noteQuad1.Remove(this);
            ATMAN.noteQuad2.Remove(this);
            ATMAN.noteQuad3.Remove(this);
            ATMAN.noteQuad4.Remove(this);
            truetransform.SetActive(false);
        }

    }

    IEnumerator TAPS()
    {
        yield return new WaitUntil(() => TAP!=null);
        TIM = TAP.Timing;
        yield return new WaitUntil(() => TAP.Timing + AOFFSET.AudioOffset - 202 -AMAN.JudgeOffset <= AMAN.Timing);       
          
            if (didsend == false)
            {
                didsend = true;
                //sendray();
            }
            
                didclick = true;
                canclick = true;

        yield return new WaitUntil(() => TAP.Timing + AOFFSET.AudioOffset + 100 <= AMAN.Timing);
      
            ran = true;

            ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(clicktransform.position.x, clicktransform.position.y + 1.2f), "LOST", 0f);
            ArcTimingManager.COMBO = 0;
            ArcScoreManager.MAXLOSTS += 1;
            ACMAN.PM.enabled = false;
            ACMAN.FR.enabled = false;
            if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
            {
                ACMAN.CLEARRATE.fillAmount -= 0.02f;
               
            }
            else if (ArcScoreManager.guagetypeno == 1)
            {
                ACMAN.CLEARRATE.fillAmount -= 0.015f;
                
            }
            else if (ArcScoreManager.guagetypeno == 2)
            {
                if ((ACMAN.NOTETOTAl <= 16) == false)
                {
                    if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.085f;
                        
                    }
                    else
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.045f;                      
                    }
                }
                else
                {
                    ACMAN.CLEARRATE.fillAmount -= 1f;
                    ArcScoreManager.CURRENTPERCENT = 0;
                }
            }

            if (notetype == "judgebar")
            {

                //TAP.TPOBJ = null;
                if (!(ReferenceEquals(TAP.LINE, null)))
                {
                    TAP.LINE.transform.parent = null;
                    TAP.LINE.gameObject.SetActive(false);
                }

                ATMAN.noteQuad1.Remove(this);
                ATMAN.noteQuad2.Remove(this);
                ATMAN.noteQuad3.Remove(this);
                ATMAN.noteQuad4.Remove(this);
                gameObject.SetActive(false);
            }
            else if (notetype == "judgeplane")
            {
                //transform.parent.gameObject.SetActive(false);
                ATMAN.noteQuad1.Remove(this);
                ATMAN.noteQuad2.Remove(this);
                ATMAN.noteQuad3.Remove(this);
                ATMAN.noteQuad4.Remove(this);
                truetransform.SetActive(false);
            }

        
    }
    
    //void OnTriggerEnter(Collider other)
    //{


    //    Transform othertransform = other.transform;
      
        
    //    /*if (other.gameObject.name.Equals(notetype)&&canclick == false && gameObject.activeSelf == true&& !ATMAN.IsBackwarding && !ATMAN.IsStopped)
    //    {
    //        try
    //        {
    //            StartCoroutine(checkclick());
    //        }
    //        catch
    //        {
    //        }
    //    }*/
    //   /* else if(other.gameObject.name.Equals(notetype+"pure") && !ATMAN.IsBackwarding && !ATMAN.IsStopped)
    //    {
    //        ispure = true;
    //    }*/
    //    if (other.gameObject.name.Equals("failcollider")&& !ATMAN.IsBackwarding&& !ATMAN.IsStopped)
    //    {
    //        ran = true;
            
    //        /*if (notetype == "judgebar")
    //        {
    //            print(AMAN.Timing + ":" + TAP.Timing);
    //        }*/
    //        ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(clicktransform.position.x, clicktransform.position.y + 1.2f), "LOST", 0f);
    //        ArcTimingManager.COMBO = 0;
    //        ArcScoreManager.MAXLOSTS += 1;
    //        ACMAN.PM.enabled = false;
    //        ACMAN.FR.enabled = false;
    //        if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
    //        {
    //            ACMAN.CLEARRATE.fillAmount -= 0.02f;
    //            /*if ((ArcScoreManager.CURRENTPERCENT - 2f) > 0)
    //                ArcScoreManager.CURRENTPERCENT -= 2;
    //            else
    //                ArcScoreManager.CURRENTPERCENT = 0;*/
    //        }
    //        else if (ArcScoreManager.guagetypeno == 1)
    //        {
    //            ACMAN.CLEARRATE.fillAmount -= 0.015f;
    //           /* if ((ArcScoreManager.CURRENTPERCENT - 1.5f) > 0)
    //                ArcScoreManager.CURRENTPERCENT -= 1.5f;
    //            else
    //                ArcScoreManager.CURRENTPERCENT = 0;*/
    //        }
    //        else if (ArcScoreManager.guagetypeno == 2)
    //        {
    //            if ((ACMAN.NOTETOTAl <= 16) == false)
    //            {
    //                if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
    //                {
    //                    ACMAN.CLEARRATE.fillAmount -= 0.085f;
    //                    /*if ((ArcScoreManager.CURRENTPERCENT - 8.5f) > 0)
    //                        ArcScoreManager.CURRENTPERCENT -= 8.5f;
    //                    else
    //                        ArcScoreManager.CURRENTPERCENT = 0;*/
    //                }
    //                else
    //                {
    //                    ACMAN.CLEARRATE.fillAmount -= 0.045f;
    //                   /* if ((ArcScoreManager.CURRENTPERCENT - 4.5f) > 0)
    //                        ArcScoreManager.CURRENTPERCENT -= 4.5f;
    //                    else
    //                        ArcScoreManager.CURRENTPERCENT = 0;*/
    //                }
    //            }
    //            else
    //            {
    //                ACMAN.CLEARRATE.fillAmount -= 1f;
    //                ArcScoreManager.CURRENTPERCENT = 0;
    //            }
    //        }

    //        if (notetype == "judgebar")
    //        {

    //            TAP.TPOBJ = null;
    //            if (!(ReferenceEquals(TAP.LINE, null)))
    //            {
    //                TAP.LINE.transform.parent = null;
    //                TAP.LINE.gameObject.SetActive(false);
    //            }

    //            ATMAN.noteQuad1.Remove(this.gameObject);
    //            ATMAN.noteQuad2.Remove(this.gameObject);
    //            ATMAN.noteQuad3.Remove(this.gameObject);
    //            ATMAN.noteQuad4.Remove(this.gameObject);
    //            gameObject.SetActive(false);
    //        }else if (notetype == "judgeplane")
    //        {
    //            transform.parent.gameObject.SetActive(false);
    //            ATMAN.noteQuad1.Remove(this.gameObject);
    //            ATMAN.noteQuad2.Remove(this.gameObject);
    //            ATMAN.noteQuad3.Remove(this.gameObject);
    //            ATMAN.noteQuad4.Remove(this.gameObject);
    //            truetransform.SetActive(false);
    //        }
            
    //    }else if (other.gameObject.name.Contains("Game0bject"))
    //    }
    //}

    public bool ran = false;
    public bool didclick = false;
    private void OnDisable()
    {
        canclick = false;
        canbackward = true;
        ispure = false;
        didsend = false;
        ran = false;
        TIM = 0;
        didclick = false;
    }
    void OnEnable()
    {
        canclick = false;
        ispure = false;
        ran = false;
        if (gameObject.tag == "TapNote")
        {
            StartCoroutine(TAPS());
        }else if (gameObject.tag == "ArcTap")
        {
            StartCoroutine(ATAPS());
        }
        //TIM = 0;

        //startTime = 3.0f;
    }
}
