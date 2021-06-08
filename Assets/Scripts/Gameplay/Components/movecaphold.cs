using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Lean.Touch;
using System;
using Arcaoid.Aff;
using Arcaoid.Compose;
using Arcaoid.Gameplay.Chart;

public class movecaphold : MonoBehaviour {
    public Rigidbody rb;
    public ArcHoldRenderer parentArc;
    public bool isinrange = false;

    
   
    float lerptime;


    
    public int current=0;
    public float Movespeed;
    public float distance;
    public string pathName;
    public float minimumspeed;
    public int COMBO;
    public float duriation;
    bool hasrun = false;
    private bool isready = false;
    public float TIME;
   public int currentCOMBO;
    public bool canhit = false;
    public float START;
    public float END;
    public bool stay=false;
   public bool iscolliding = false;
   public bool wasfrom=false;
    bool turnedred=false;
   public bool notemissed=false;
    public ParticleSystem JUDeffect;
    public GameObject otherArcHit;
    ARCHIT temparchit;
    public BoxCollider thisobj;
    Vector3 currentpos;
    public float[] segspeed;
    public List<GameObject> Archits;
    public Transform Captransform;
    public bool didset = false;
    public bool intocaphold = false;
    public bool capmove = true;
    public int i = 1;
    public ArcScoreManager ACMAN;
    public List<ArcArcHold> Acholds = new List<ArcArcHold>();
    public Transform Achit;
    public bool inposition = false;
    

    // Use this for initialization
    void OnEnable () {

        //PathToFollow = GameObject.Find(pathName).GetComponent<EditorPathScript>();
        Captransform = this.transform;
        


        // this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
        thisobj.enabled = false;

       //Setminspeed();

        //print("Start " + parentArc.ArcHold.Timing);
        //print("End " + parentArc.ArcHold.EndTiming);

        //print("finaltime " + Movespeed);
    }

    private void Awake()
    {
        ACMAN = ArcScoreManager.Instance;
    }


    void OnDisable ()
   {
       //transform.parent = parentArc.transform;
       StopAllCoroutines();
       isinrange = false;      
       istogether = false;
        parentArc = null;
        Achit = null;
        inposition = false;
        capmove = true;
    
  // firsthit = true;
   waiting=false;
        WRONG = false;
        temparchit = null;
        i = 1;
        intocaphold = false;
       lerptime=0;
     current = 0;
     Movespeed=0;
     distance=0;
     minimumspeed=0;
     COMBO=0;
     duriation=0;
     hasrun = false;
       isready = false;
     TIME=0;
     currentCOMBO=0;
       canhit = false;
     START=0;
     END=0;
       stay = false;
       iscolliding = false;
       wasfrom = false;
     turnedred = false;
       notemissed = false;
        otherArcHit = null;
   currentpos=Vector3.zero;
   Archits.Clear();
        overlapping = false;
        leewaystarted = false;
        hitlast = false;
       didset = false;
        isrun = false;
        lostrunning = false;
        canmove = true;
        ismoving = false;
        otherarchit = null;
        hitonce = false;
        

    firsthit = true;
     specialhit = false;
        if (!(ReferenceEquals(FINGERBOX,null)))
        {
            FINGERBOX.arcfollowing = false;
            FINGERBOX = null;
        }
          BOXPOS=null;
     ACTUALBOX=null;
        FINGER = null;
        MOVE = null;
        Acholds = null;

    }

    public void CalculateCombo()
    {
        START = parentArc.ArcHold.Timing;
        END = parentArc.ArcHold.EndTiming;
        int u = parentArc.IsHead? 0 : 1;
        double bpm = ArcTimingManager.Instance.CalculateBpmByTiming(parentArc.ArcHold.Timing);
        if (bpm <= 0) return;
        double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
        int total = (int)((parentArc.ArcHold.EndTiming - parentArc.ArcHold.Timing) / interval);
        //COMBO = total;
        if ((u ^ 1) >= total)
        {
            COMBO +=1;
            //parentArc.ArcHold.JudgeTimings.Add((int)(parentArc.ArcHold.Timing + (parentArc.ArcHold.EndTiming - parentArc.ArcHold.Timing) * 0.5f));
            return;
        }
        int n = u ^ 1;
        int t = parentArc.ArcHold.Timing;
        while (true)
        {
            t = (int)(parentArc.ArcHold.Timing + n * interval);
            if (t < parentArc.ArcHold.EndTiming)
            {
                COMBO += 1;
                
            }
            if (total == ++n) break;
        }
    }
    public bool hitlast = false;

    IEnumerator judgeholds()
    {
        //isjudging = true;
        
        yield return new WaitForSecondsRealtime(0.0f);
      
        TIME = 0f;
        float tempdur = 0;    
        isready = true;
        tempdur = duriation - 0.34f;
        if (tempdur >= 0) { } else { tempdur = duriation; }
        while ((TIME <= ((float)tempdur / COMBO)))
        {
            if (COMBO==currentCOMBO||COMBO<=currentCOMBO)
                break;
            else
            yield return null;
        }
        if (parentArc.ArcHold.firstarc)
        {
            if (canhit == true && !notemissed)
            //if (canhit == false)
            {
                //Debug.Log("PURE");
                ArcTimingManager.COMBO += 1;
                currentCOMBO += 1;
                ArcScoreManager.MAXPURES += 1;
                ArcScoreManager.PPURES += 1;
                ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                /* if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                     ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                 else
                     ArcScoreManager.CURRENTPERCENT = 100;*/
                ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "PURE", 0f);
                var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; }
                else
                {
                    ArcTimingManager.previousarcholdhitblue = true;

                }
                hitlast = true;

            }
            else
            {
                //Debug.Log("LOST");
                ArcTimingManager.COMBO = 0;
                ACMAN.PM.enabled = false;
                ACMAN.FR.enabled = false;
                currentCOMBO += 1;
                ArcScoreManager.MAXLOSTS += 1;
                notemissed = true;
                if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                {
                    ACMAN.CLEARRATE.fillAmount -= 0.02f;
                    /* if ((ArcScoreManager.CURRENTPERCENT - 2f) > 0)
                         ArcScoreManager.CURRENTPERCENT -= 2;
                     else
                         ArcScoreManager.CURRENTPERCENT = 0;*/
                }
                else if (ArcScoreManager.guagetypeno == 1)
                {
                    ACMAN.CLEARRATE.fillAmount -= 0.015f;
                    /* if ((ArcScoreManager.CURRENTPERCENT - 1.5f) > 0)
                         ArcScoreManager.CURRENTPERCENT -= 1.5f;
                     else
                         ArcScoreManager.CURRENTPERCENT = 0;*/
                }
                else if (ArcScoreManager.guagetypeno == 2)
                {
                    if ((ACMAN.NOTETOTAl <= 16) == false)
                    {
                        if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.085f;
                            /* if ((ArcScoreManager.CURRENTPERCENT - 8.5f) > 0)
                                 ArcScoreManager.CURRENTPERCENT -= 8.5f;
                             else
                                 ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.045f;
                            /*if ((ArcScoreManager.CURRENTPERCENT - 4.5f) > 0)
                                ArcScoreManager.CURRENTPERCENT -= 4.5f;
                            else
                                ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                    }
                    else
                    {
                        ACMAN.CLEARRATE.fillAmount -= 1f;
                        ArcScoreManager.CURRENTPERCENT = 0;
                    }
                }

                ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "LOST", 0f);
                var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; }
                else
                {
                    ArcTimingManager.previousarcholdhitblue = false;
                }
                hitlast = false;
            }
        }
        else
        {
            currentCOMBO += 1;
        }
        isready = false;
        
        if (COMBO != currentCOMBO&&currentCOMBO<=COMBO)
            StartCoroutine(judgeholds());
        else
        {
            //print(Acholds.Count);
           /* if (!(ReferenceEquals(FINGERBOX, null)))
            {
                FINGERBOX.arcfollowing = false;
                FINGERBOX = null;
            }*/
            
                parentArc.readytodeac = true;
            if(parentArc.ArcHold.firstarc)
            {
                
                if (Acholds.Count == i)
                {
                    
                    gameObject.SetActive(false);
                }
                else
                {
                    firsthit = false;
                    intocaphold = true;

                    Achit = null;
                    foreach (ArcHoldSegmentComponent seg in parentArc.segments)
                    {
                        seg.Alpha = 0.57f;
                    }

                    yield return new WaitUntil(() => parentArc.ArcHold.EndTiming <= ArcGameplayManager.Instance.Timing);
                    StartCoroutine(Archolds());
                }
            } 
            
        }
    }

    public IEnumerator Nothit()
    {
        //print("NOTHIT");
        yield return new WaitForSeconds(0.0f);
        foreach (ArcHoldSegmentComponent seg in Acholds[i].AREND.segments)
        {
            seg.Alpha = 0.57f;
            seg.inposition = false;
        }

    }

    public IEnumerator Washit()
    {
        //print("HIT?");
        yield return new WaitForSeconds(0.0f);
        foreach (ArcHoldSegmentComponent seg in Acholds[i].AREND.segments)
        {
            seg.Alpha = 1;
            seg.inposition = true;
        }

    }

    IEnumerator Archolds()
    {
        isready = true;
        if (currentCOMBO == COMBO||currentCOMBO>=COMBO)
        {
            inposition = false;
            Achit = null;
            currentCOMBO = 0;
            COMBO = Acholds[i].COMBO;
            duriation = ((Acholds[i].EndTiming - (19.565f * Acholds[i].COMBO)) / 1000f) - (Acholds[i].Timing / 1000f);
            if(i!=1)
            yield return new WaitUntil(() => Acholds[i-1].EndTiming <= ArcGameplayManager.Instance.Timing);
            else
            yield return new WaitUntil(() => parentArc.ArcHold.EndTiming <= ArcGameplayManager.Instance.Timing);
        }
        if (Acholds[i].MCAP != null) { }
        else { yield return new WaitUntil(() => Acholds[i].MCAP != null); }
        if (Acholds[i].EndTiming != Acholds[i].Timing)
        {
            if (COMBO != 1 && !(duriation <= 0f))
            {
                TIME = 0f;
                float tempdur = 0;

                tempdur = duriation - 0.14f;
                if (tempdur >= 0) { } else { tempdur = duriation; }
                if (currentCOMBO == 0)
                {
                    duriation = duriation - (0.5f / COMBO);
                    tempdur=tempdur - (0.5f / COMBO);
                    yield return new WaitForSeconds(0.5f);                  
                }
                while ((TIME <= ((float)tempdur / COMBO)))
                {
                    yield return null;
                }

                if (Captransform.position == Acholds[i].MCAP.transform.position || Acholds[i].wasposition)
                //if (canhit == false)
                {
                    //Debug.Log("PURE");
                    ArcTimingManager.COMBO += 1;
                    currentCOMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ArcScoreManager.PPURES += 1;
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                    ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /* if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                         ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                     else
                         ArcScoreManager.CURRENTPERCENT = 100;*/
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Acholds[i].MCAP.transform.position.x, Acholds[i].MCAP.transform.position.y + 1.2f), "PURE", 0f);
                    var temp = Acholds[i].Color == 1 ? "RED" : "BLUE";
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; }
                    else
                    {
                        ArcTimingManager.previousarcholdhitblue = true;

                    }
                    hitlast = true;

                }
                else
                {
                    //Debug.Log("LOST");
                    ArcTimingManager.COMBO = 0;
                    ACMAN.PM.enabled = false;
                    ACMAN.FR.enabled = false;
                    currentCOMBO += 1;
                    ArcScoreManager.MAXLOSTS += 1;
                    notemissed = true;
                    if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.02f;
                        /* if ((ArcScoreManager.CURRENTPERCENT - 2f) > 0)
                             ArcScoreManager.CURRENTPERCENT -= 2;
                         else
                             ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 1)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.015f;
                        /* if ((ArcScoreManager.CURRENTPERCENT - 1.5f) > 0)
                             ArcScoreManager.CURRENTPERCENT -= 1.5f;
                         else
                             ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 2)
                    {
                        if ((ACMAN.NOTETOTAl <= 16) == false)
                        {
                            if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.085f;
                                /* if ((ArcScoreManager.CURRENTPERCENT - 8.5f) > 0)
                                     ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                 else
                                     ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                            else
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.045f;
                                /*if ((ArcScoreManager.CURRENTPERCENT - 4.5f) > 0)
                                    ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                else
                                    ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                        }
                        else
                        {
                            ACMAN.CLEARRATE.fillAmount -= 1f;
                            ArcScoreManager.CURRENTPERCENT = 0;
                        }
                    }

                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Acholds[i].MCAP.transform.position.x, Acholds[i].MCAP.transform.position.y + 1.2f), "LOST", 0f);
                    var temp = Acholds[i].Color == 1 ? "RED" : "BLUE";
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; }
                    else
                    {
                        ArcTimingManager.previousarcholdhitblue = false;
                    }
                    hitlast = false;
                }
                //print("ok!");
                isready = false;
                if (COMBO != currentCOMBO && currentCOMBO <= COMBO)
                    StartCoroutine(Archolds());
                else
                {

                    i += 1;
                    if (Acholds.Count == i)
                    {

                        gameObject.SetActive(false);
                    }
                    else
                    {
                        StartCoroutine(Archolds());
                    }

                }
            }
            else
            {
                //print("combo?");
                if (!(Captransform.position == Acholds[i].MCAP.transform.position) && duriation != 0 && !(duriation < 0) && !Acholds[i].wasposition)
                    yield return new WaitForSeconds(duriation / 1.8f);
                else
                    yield return new WaitForSeconds(0.01f);

                if (duriation == 0)
                {
                    var temp = Acholds[i].Color == 1 ? "RED" : "BLUE";
                    if ((temp == "RED" && ArcTimingManager.previousarcholdhitred) || (temp == "BLUE" && ArcTimingManager.previousarcholdhitblue) || Acholds[i].wasposition)
                    {
                        //Debug.Log("pUrE");
                        ArcTimingManager.COMBO += COMBO - currentCOMBO;
                        ArcScoreManager.MAXPURES += COMBO - currentCOMBO;
                        ArcScoreManager.PPURES += COMBO - currentCOMBO;
                        //currentCOMBO = COMBO;
                        if (firsthit)
                        {
                            firsthit = false;
                            ArcEffectManager.Instance.PlayArcSound();
                        }

                        ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                        /* if ((ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                                ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                            else
                                ArcScoreManager.CURRENTPERCENT = 100;*/
                        ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                        ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Acholds[i].MCAP.transform.position.x, Acholds[i].MCAP.transform.position.y + 1.2f), "PURE", 0f);
                        if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                        currentCOMBO = COMBO;
                        hitlast = true;
                    }
                    else
                    {
                        ArcTimingManager.COMBO = 0;
                        ArcScoreManager.MAXLOSTS += (COMBO - currentCOMBO);
                        ACMAN.PM.enabled = false;
                        ACMAN.FR.enabled = false;
                        if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.02f * (COMBO - currentCOMBO);
                            /*if ((ArcScoreManager.CURRENTPERCENT - 2 * (COMBO - currentCOMBO)) > 0)
                                ArcScoreManager.CURRENTPERCENT -= 2;
                            else
                                ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else if (ArcScoreManager.guagetypeno == 1)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.015f;
                            /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                                ArcScoreManager.CURRENTPERCENT -= 1.5f;
                            else
                                ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else if (ArcScoreManager.guagetypeno == 2)
                        {
                            if (ACMAN.NOTETOTAl <= 16 == false)
                            {
                                if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                                {
                                    ACMAN.CLEARRATE.fillAmount -= 0.085f * (COMBO - currentCOMBO);
                                    /*if ((ArcScoreManager.CURRENTPERCENT - 8.5f * (COMBO - currentCOMBO)) > 0)
                                        ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                    else
                                        ArcScoreManager.CURRENTPERCENT = 0;*/
                                }
                                else
                                {
                                    ACMAN.CLEARRATE.fillAmount -= 0.045f * (COMBO - currentCOMBO);
                                    /*if ((ArcScoreManager.CURRENTPERCENT - 4.5f * (COMBO - currentCOMBO)) > 0)
                                        ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                    else
                                        ArcScoreManager.CURRENTPERCENT = 0;*/
                                }
                            }
                            else
                            {
                                ACMAN.CLEARRATE.fillAmount -= 1f;
                                ArcScoreManager.CURRENTPERCENT = 0;
                            }
                        }
                        if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; } else { ArcTimingManager.previousarcholdhitblue = false; }
                        currentCOMBO = COMBO;
                        hitlast = false;
                    }

                    //parentArc.readytodeac = true;
                }
                else
                {
                    var temp = Acholds[i].Color == 1 ? "RED" : "BLUE";
                    if ((Captransform.position == Acholds[i].MCAP.transform.position) || Acholds[i].wasposition)
                    // if (canhit == false)
                    {
                        //Debug.Log("pUrE");
                        ArcTimingManager.COMBO += COMBO - currentCOMBO;
                        ArcScoreManager.MAXPURES += COMBO - currentCOMBO;
                        ArcScoreManager.PPURES += COMBO - currentCOMBO;
                        //currentCOMBO = COMBO;
                        if (firsthit)
                        {
                            firsthit = false;
                            ArcEffectManager.Instance.PlayArcSound();
                        }

                        ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                        /* if ((ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                             ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                         else
                             ArcScoreManager.CURRENTPERCENT = 100;*/
                        ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                        ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Acholds[i].MCAP.transform.position.x, Acholds[i].MCAP.transform.position.y + 1.2f), "PURE", 0f);
                        if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                        currentCOMBO = COMBO;
                        hitlast = true;
                    }
                    else
                    {
                        ArcTimingManager.COMBO = 0;
                        ArcScoreManager.MAXLOSTS += (COMBO - currentCOMBO);
                        ACMAN.PM.enabled = false;
                        ACMAN.FR.enabled = false;
                        if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.02f * (COMBO - currentCOMBO);
                            /*if ((ArcScoreManager.CURRENTPERCENT - 2 * (COMBO - currentCOMBO)) > 0)
                                ArcScoreManager.CURRENTPERCENT -= 2;
                            else
                                ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else if (ArcScoreManager.guagetypeno == 1)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.015f;
                            /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                                ArcScoreManager.CURRENTPERCENT -= 1.5f;
                            else
                                ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else if (ArcScoreManager.guagetypeno == 2)
                        {
                            if ((ACMAN.NOTETOTAl <= 16) == false)
                            {
                                if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                                {
                                    ACMAN.CLEARRATE.fillAmount -= 0.085f * (COMBO - currentCOMBO);
                                    /*if ((ArcScoreManager.CURRENTPERCENT - 8.5f * (COMBO - currentCOMBO)) > 0)
                                        ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                    else
                                        ArcScoreManager.CURRENTPERCENT = 0;*/
                                }
                                else
                                {
                                    ACMAN.CLEARRATE.fillAmount -= 0.045f * (COMBO - currentCOMBO);
                                    /* if ((ArcScoreManager.CURRENTPERCENT - 4.5f * (COMBO - currentCOMBO)) > 0)
                                         ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                     else
                                         ArcScoreManager.CURRENTPERCENT = 0;*/
                                }
                            }
                            else
                            {
                                ACMAN.CLEARRATE.fillAmount -= 1f;
                                ArcScoreManager.CURRENTPERCENT = 0;
                            }
                        }
                        if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; } else { ArcTimingManager.previousarcholdhitblue = false; }
                        currentCOMBO = COMBO;
                        ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Acholds[i].MCAP.transform.position.x, Acholds[i].MCAP.transform.position.y + 1.2f), "LOST", 0f);
                        hitlast = false;
                    }

                    //parentArc.readytodeac = true;
                }
                isready = false;


                i += 1;
                if (Acholds.Count == i)
                {

                    gameObject.SetActive(false);
                }
                else
                {
                    StartCoroutine(Archolds());
                }
            }
        }
        else
        {
            currentCOMBO = COMBO;
            i += 1;
            if (Acholds.Count == i)
            {

                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(Archolds());
            }
        }

    }

    IEnumerator givecombo()
    {
        if (!canhit && duriation != 0 && !(duriation < 0))
            yield return new WaitForSeconds(duriation / 1.8f);
        else
            yield return new WaitForSeconds(0.01f);


        if (parentArc.ArcHold.firstarc)
        {
            //if (duriation == 0||duriation<=0.02f)
            if (duriation == 0)
            {
                var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                if ((temp == "RED" && ArcTimingManager.previousarcholdhitred) || (temp == "BLUE" && ArcTimingManager.previousarcholdhitblue))
                {
                    //Debug.Log("pUrE");
                    ArcTimingManager.COMBO += COMBO - currentCOMBO;
                    ArcScoreManager.MAXPURES += COMBO - currentCOMBO;
                    ArcScoreManager.PPURES += COMBO - currentCOMBO;
                    //currentCOMBO = COMBO;
                    if (firsthit)
                    {
                        firsthit = false;
                        ArcEffectManager.Instance.PlayArcSound();
                    }

                    ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                    /* if ((ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                            ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                        else
                            ArcScoreManager.CURRENTPERCENT = 100;*/
                    ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "PURE", 0f);
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                    currentCOMBO = COMBO;
                    hitlast = true;
                }
                else
                {
                    ArcTimingManager.COMBO = 0;
                    ArcScoreManager.MAXLOSTS += (COMBO - currentCOMBO);
                    ACMAN.PM.enabled = false;
                    ACMAN.FR.enabled = false;
                    if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.02f * (COMBO - currentCOMBO);
                        /*if ((ArcScoreManager.CURRENTPERCENT - 2 * (COMBO - currentCOMBO)) > 0)
                            ArcScoreManager.CURRENTPERCENT -= 2;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 1)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.015f;
                        /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                            ArcScoreManager.CURRENTPERCENT -= 1.5f;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 2)
                    {
                        if (ACMAN.NOTETOTAl <= 16 == false)
                        {
                            if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.085f * (COMBO - currentCOMBO);
                                /*if ((ArcScoreManager.CURRENTPERCENT - 8.5f * (COMBO - currentCOMBO)) > 0)
                                    ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                else
                                    ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                            else
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.045f * (COMBO - currentCOMBO);
                                /*if ((ArcScoreManager.CURRENTPERCENT - 4.5f * (COMBO - currentCOMBO)) > 0)
                                    ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                else
                                    ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                        }
                        else
                        {
                            ACMAN.CLEARRATE.fillAmount -= 1f;
                            ArcScoreManager.CURRENTPERCENT = 0;
                        }
                    }
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; } else { ArcTimingManager.previousarcholdhitblue = false; }
                    currentCOMBO = COMBO;
                    hitlast = false;
                }
                if (!(ReferenceEquals(FINGERBOX, null)))
                {
                    FINGERBOX.arcfollowing = false;
                    FINGERBOX = null;
                }
                parentArc.readytodeac = true;
            }
            else
            {
                var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                if (canhit == true && !notemissed)
                // if (canhit == false)
                {
                    //Debug.Log("pUrE");
                    ArcTimingManager.COMBO += COMBO - currentCOMBO;
                    ArcScoreManager.MAXPURES += COMBO - currentCOMBO;
                    ArcScoreManager.PPURES += COMBO - currentCOMBO;
                    //currentCOMBO = COMBO;
                    if (firsthit)
                    {
                        firsthit = false;
                        ArcEffectManager.Instance.PlayArcSound();
                    }

                    ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                    /* if ((ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                         ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                     else
                         ArcScoreManager.CURRENTPERCENT = 100;*/
                    ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "PURE", 0f);
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                    currentCOMBO = COMBO;
                    hitlast = true;
                }
                else
                {
                    ArcTimingManager.COMBO = 0;
                    ArcScoreManager.MAXLOSTS += (COMBO - currentCOMBO);
                    ACMAN.PM.enabled = false;
                    ACMAN.FR.enabled = false;
                    if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.02f * (COMBO - currentCOMBO);
                        /*if ((ArcScoreManager.CURRENTPERCENT - 2 * (COMBO - currentCOMBO)) > 0)
                            ArcScoreManager.CURRENTPERCENT -= 2;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 1)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.015f;
                        /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                            ArcScoreManager.CURRENTPERCENT -= 1.5f;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 2)
                    {
                        if ((ACMAN.NOTETOTAl <= 16) == false)
                        {
                            if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.085f * (COMBO - currentCOMBO);
                                /*if ((ArcScoreManager.CURRENTPERCENT - 8.5f * (COMBO - currentCOMBO)) > 0)
                                    ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                else
                                    ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                            else
                            {
                                ACMAN.CLEARRATE.fillAmount -= 0.045f * (COMBO - currentCOMBO);
                                /* if ((ArcScoreManager.CURRENTPERCENT - 4.5f * (COMBO - currentCOMBO)) > 0)
                                     ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                 else
                                     ArcScoreManager.CURRENTPERCENT = 0;*/
                            }
                        }
                        else
                        {
                            ACMAN.CLEARRATE.fillAmount -= 1f;
                            ArcScoreManager.CURRENTPERCENT = 0;
                        }
                    }
                    if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; } else { ArcTimingManager.previousarcholdhitblue = false; }
                    currentCOMBO = COMBO;
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "LOST", 0f);
                    hitlast = false;
                }
                if (!(ReferenceEquals(FINGERBOX, null)))
                {
                    FINGERBOX.arcfollowing = false;
                    FINGERBOX = null;
                }
                parentArc.readytodeac = true;
            }
                        
                    if (Acholds.Count == i)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        firsthit = false;
                foreach (ArcHoldSegmentComponent seg in parentArc.segments)
                {
                    seg.Alpha = 0.57f;
                }
                Achit = null;
                intocaphold = true;
                yield return new WaitUntil(() => parentArc.ArcHold.EndTiming <= ArcGameplayManager.Instance.Timing);
                StartCoroutine(Archolds());
                    }
                           
        }
        else { currentCOMBO = COMBO; }

        //stay = false;
        
    }
    public bool overlapping = false;
    public Vector3 faketransform;
    public IEnumerator window()
    {
        yield return new WaitForSeconds(1f);
       /* stay = false;
        iscolliding = false;    
        canhit = false;
        wasfrom = false;
        if (parentArc.isalpha && !turnedred)
        {
            if (!(ReferenceEquals(parentArc, null)))
            {
                if (!(ReferenceEquals(parentArc.gameObject, null)))
                {
                    if (!parentArc.gameObject.activeSelf == false)
                        parentArc.StartCoroutine(parentArc.Nothit());
                }
            }         
            
        }*/
    }
    bool leewaystarted = false;
    IEnumerator overlapleeway()
    {
        leewaystarted = true;
        overlapping = true;
        yield return new WaitForSeconds(2f);
        overlapping = false;
        leewaystarted = false;
        
    }
    IEnumerator DisableCAP()
    {
        yield return new WaitForSeconds(3.0f);
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
        //ONDisable();
    }
    bool isrun = false;
    IEnumerator resethit()
    {
        yield return new WaitForSeconds(0.5f);
        notemissed = false;
        //arcFin = false;
        foreach (ArcArcHold ARC in Acholds)
        {
            ARC.arcFin = false;
            notemissed = false;
        }
        //fingers = 0;
        turnedred = false;
        //parentArc.isalpha = true;
        if (parentArc != null)
        {
            foreach (ArcHoldSegmentComponent s in parentArc.segments)
            {
                s.Color = parentArc.currentcolor;
                s.Alpha = 0.57f;

            }
        }
        isrun = false;
    }

    public bool lostrunning = false;

    public IEnumerator fingerlost()
    {
        lostrunning = true;
        stay = false;
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.4f);
        if (!(ReferenceEquals(FINGERBOX, null)))
        {
            FINGERBOX.arcfollowing = false;
            FINGERBOX = null;
        }
        lostrunning = false;
    }

    public bool canmove = true;
    public bool ismoving = false;
    public Coroutine MOVE;
   


    private void Update()
    {
        /*if (!IsInvoking("UpdateMove"))
        {
            InvokeRepeating("UpdateMove", 0, 0.00833333f);
        }*/
        
        if (!(ReferenceEquals(parentArc,null))&&!intocaphold) {

            if (canmove && !ismoving&&parentArc.ArcHold.firstarc&&!intocaphold)
                MOVE = StartCoroutine(UpdateMove());
            else if (!parentArc.ArcHold.firstarc)
            {
                if (isready)
                {
                    TIME += Time.deltaTime;
                }
            }
            
        }else if (intocaphold)
        {
            if (isready)
            {
                TIME += Time.deltaTime;
            }
            if (inposition)
            {
                if(JUDeffect.isStopped)
                JUDeffect.Play();
            }
            else
            {
                if(JUDeffect.isPlaying)
                JUDeffect.Stop();
            }
            StartCoroutine(Movethecap());
        }
        /*else
        {
           if(JUDeffect.isPlaying)
            JUDeffect.Stop();
        }*/
        
       
    }

     public GameObject[] ACS;

    public Vector3 ACoob = new Vector3(600, 600, 600);

    IEnumerator Movethecap()
    {
        yield return new WaitForSeconds(0.0f);
       
        if (!(ReferenceEquals(Achit, null))&&!inposition)
        {

            if (Achit != null)
            {
                if(Achit.position!=ACoob)
                Captransform.position = Achit.position;
                else {
                    Achit = null;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (intocaphold)
        {
            if (other.tag.Equals("ARCHIT") && (Achit == null))
            {
                print("mkay");
                Achit = other.transform;
            }
        }
    }
    IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("CAPHOLD") && !inposition)
        {

            //Captransform.position = other.transform.position;
            if (Acholds[i].MCAP == null) { yield return new WaitUntil(() => Acholds[i].MCAP!=null); }
            if (Acholds[i].MCAP.transform == other.transform)
            {
                print("gotit");
                Acholds[i].wasposition = true;
                 ArcEffectManager.Instance.PlayArcSound();
                inposition = true;
                Achit = null;
                Captransform.position = Acholds[i].initialpos;
                StartCoroutine(Washit());
            }
        }
    }

    IEnumerator UpdateMove()
    {
        yield return new WaitForSeconds(0.1f);
        if (parentArc.gameObject.activeSelf == false)
        {
            parentArc.HeadRenderer.enabled = false;
        }
        if (parentArc.CANRENDER)
        {
            float tempmove1 = parentArc.ArcHold.Timing - 10000f;
            float tempend = parentArc.ArcHold.EndTiming - 10000f;
            Movespeed = ((tempend - tempmove1) / 1000f) / (parentArc.segmentsleft + 1);
           
            //if (duriation != 0)
              //  duriation -= 0.05f;
           /* if (FINGERBOX != null)
            {
                if (FINGERBOX.gameObject.activeSelf == false)
                {
                    FINGERBOX = null;
                    ACTUALBOX = null;

                }
            }*/

            if (ArcTimingManager.Shouldrun && !hasrun)
            {
                hasrun = true;
               
                    CalculateCombo();
            }
            if (notemissed&&!isrun)
            {
                isrun = true;
                StartCoroutine(resethit());
            }
            if (COMBO == currentCOMBO && COMBO != 0)
            {
                //parentArc.readytodeac = true;
                //StartCoroutine(DisableCAP());
                parentArc.ArcHold.arcFin = true;
                JUDeffect.Stop();
            }

            //put thing here
            if ((ReferenceEquals(FINGERBOX, null)) && !notemissed && isinrange && !parentArc.ArcHold.arcFin && (currentCOMBO != COMBO) && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
            {
                Array.Clear(ACS, 0, ACS.Length);
                ACS = GameObject.FindGameObjectsWithTag("ARCHIT");
                foreach (GameObject AC in ACS)
                {
                    ARCHIT ACHIT = AC.GetComponent<ARCHIT>();
                    
                        if (Vector3.Distance(transform.position,AC.transform.position) <= 3f)
                        {
                            //print((AC.transform.position - transform.position).sqrMagnitude);
                            //print(Vector3.Distance(transform.position, AC.transform.position));
                            if ((ACHIT.arcfollowing == false && (ACHIT.Color == parentArc.ArcHold.Color || ACHIT.Color == -1)) || specialhit)
                            {
                                if (firsthit && duriation > 0)
                                {
                                    firsthit = false;
                                    ArcEffectManager.Instance.PlayArcSound();
                                }


                                stay = true;
                                FINGERBOX = ACHIT;
                                if (specialhit)
                                {
                                    // StopCoroutine(RemoveSpecial());
                                    // StartCoroutine(RemoveSpecial());
                                }
                                if (!specialhit)
                                {
                                    FINGERBOX.arcfollowing = true;
                                    ACHIT.Color = parentArc.ArcHold.Color;
                                    parentArc.ArcHold.arcFin = true;
                                    //StartCoroutine(Hitonce());
                                }

                                BOXPOS = FINGERBOX.transform;
                                FINGER = FINGERBOX.finger;
                                //ACTUALBOX = FINGER.CONSTANTBOX.transform;
                                break;
                            }
                            else
                            {
                                stay = true;
                                var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                                if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                            }
                        }
                    
                    

                }

            }

            if ((parentArc.ArcHold.EndTiming - 200) + ArcAudioManager.Instance.AudioOffset <= ArcGameplayManager.Instance.Timing && parentArc.movestarted && currentCOMBO != COMBO && COMBO != 1 && parentArc.ArcHold.EndTiming != 0f)
            {
                canmove = false;
                Achit = null;
                intocaphold = true;
                

            }
                //if ((parentArc.ArcHold.EndTiming -200) + ArcAudioManager.Instance.AudioOffset <= ArcGameplayManager.Instance.Timing && parentArc.movestarted && currentCOMBO != COMBO && COMBO != 1 && parentArc.ArcHold.EndTiming != 0f&&duriation!=0&&duriation>0.02f)
                /*  if ((parentArc.ArcHold.EndTiming -200) + ArcAudioManager.Instance.AudioOffset <= ArcGameplayManager.Instance.Timing && parentArc.movestarted && currentCOMBO != COMBO && COMBO != 1 && parentArc.ArcHold.EndTiming != 0f)
                  {
                      //print(HOLD.EndTiming);
                      canmove = false;
                      //StopAllCoroutines();

                      stay = true;
                      parentArc.ArcHold.arcFin = true;
                      if (!(ReferenceEquals(FINGERBOX,null)))
                      {
                          FINGERBOX.arcfollowing = false;
                          FINGERBOX = null;
                      }
                      var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                      var temphit = false;
                      if (temp == "RED") { temphit=ArcTimingManager.previousarcholdhitred; } else { temphit=ArcTimingManager.previousarcholdhitblue; }
                      if (temphit == true)
                      {
                          ArcTimingManager.COMBO += (COMBO - currentCOMBO);
                          ArcScoreManager.MAXPURES += (COMBO - currentCOMBO);
                          ArcScoreManager.PPURES += (COMBO - currentCOMBO);
                          ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                          ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                         /* if ((ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                              ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                          else
                              ArcScoreManager.CURRENTPERCENT = 100;
                          if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                          currentCOMBO = COMBO;
                      }
                      else
                      {
                          ArcTimingManager.COMBO = 0;
                          ArcScoreManager.MAXLOSTS += (COMBO - currentCOMBO);
                          ACMAN.PM.enabled = false;
                          ACMAN.FR.enabled = false;
                          if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                          {
                              ACMAN.CLEARRATE.fillAmount -= 0.02f * (COMBO - currentCOMBO);
                             /* if ((ArcScoreManager.CURRENTPERCENT - 2 * (COMBO - currentCOMBO)) > 0)
                                  ArcScoreManager.CURRENTPERCENT -= 2;
                              else
                                  ArcScoreManager.CURRENTPERCENT = 0;
                          }else if (ArcScoreManager.guagetypeno == 1)
                          {
                              ACMAN.CLEARRATE.fillAmount -= 0.015f;
                              /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                                  ArcScoreManager.CURRENTPERCENT -= 1.5f;
                              else
                                  ArcScoreManager.CURRENTPERCENT = 0;
                          }
                          else if (ArcScoreManager.guagetypeno == 2)
                          {
                              if (ACMAN.NOTETOTAl <= 16==false)
                              {
                                  if (ACMAN.CLEARRATE.fillAmount <= 0.3 == false)
                                  {
                                      ACMAN.CLEARRATE.fillAmount -= 0.085f * (COMBO - currentCOMBO);
                                     /* if ((ArcScoreManager.CURRENTPERCENT - 8.5f * (COMBO - currentCOMBO)) > 0)
                                          ArcScoreManager.CURRENTPERCENT -= 8.5f;
                                      else
                                          ArcScoreManager.CURRENTPERCENT = 0;
                                  }
                                  else
                                  {
                                      ACMAN.CLEARRATE.fillAmount -= 0.045f * (COMBO - currentCOMBO);
                                     /* if ((ArcScoreManager.CURRENTPERCENT - 4.5f * (COMBO - currentCOMBO)) > 0)
                                          ArcScoreManager.CURRENTPERCENT -= 4.5f;
                                      else
                                          ArcScoreManager.CURRENTPERCENT = 0;
                                  }
                              }
                              else
                              {
                                  ACMAN.CLEARRATE.fillAmount -= 1f;
                                  ArcScoreManager.CURRENTPERCENT = 0;
                              }
                          }
                          if (temp == "RED") { ArcTimingManager.previousarcholdhitred = false; } else { ArcTimingManager.previousarcholdhitblue = false; }
                          currentCOMBO = COMBO;

                      }

                      yield break;

                  }*/

                if (FINGERBOX != null && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
            {
                if (Vector3.Distance(BOXPOS.position,ACTUALBOX.position)<= 3f)
                {
                    stay = true;
                    if(!specialhit)
                    BOXPOS.position = new Vector3(transform.position.x,transform.position.y,0);

                    StopCoroutine(fingerlost());

                }
                else
                {
                    stay = true;
                    if(!specialhit)
                    BOXPOS.position = new Vector3(transform.position.x, transform.position.y, 0);

                    if (!lostrunning)
                    StartCoroutine(fingerlost());
                }
            }
            else
            {
                //stay = false;
            }

            if (isinrange == true && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding && Time.timeScale != 0f)
            {
                if (stay)
                {
                    
                    iscolliding = true;

                    
                    
                    
                    canhit = true;
                    //stay = false;
                    if (!notemissed && canhit)
                    {
                        parentArc.StartCoroutine(parentArc.Returnhit());
                    }
                   /* if (notemissed)
                    {
                        turnedred = true;
                        parentArc.StartCoroutine(parentArc.MISS(FINGERBOX));
                    }*/
                    
                    
                        if (!parentArc.isalpha)
                        {
                            parentArc.StartCoroutine(parentArc.Washit());
                        }
                        if (COMBO != currentCOMBO)
                        {
                            JUDeffect.Play();
                        }
                    
                }
                else
                {

                    JUDeffect.Stop();

                    if (!gameObject.activeSelf == false)
                        StartCoroutine(window());
                   
                }
                
                if (isready)
                {

                    TIME += Time.deltaTime;
                }
                
                
            }
        }
    }

    public IEnumerator WRONGARC()
    {
        yield return new WaitForSeconds(0.0f);
        // iscolliding = false; 
       // print("WRNG");
       
        //WRONG = true;
        //notemissed = true;
       parentArc.StartCoroutine(parentArc.MISS(FINGERBOX));
        //temparchit.CAPID = 0;
       // HITID = 0;
       // temparchit.otherCAP = null;
        
    }
    public bool istogether = false;
    public bool firsthit = true;
    public bool specialhit = false;

   

    public ARCHIT FINGERBOX;
    public Transform BOXPOS;
    public Transform ACTUALBOX;
    public LeanFinger FINGER;
    

    /*private void OnDisable()
    {
        if (FINGERBOX != null)
        {
            FINGERBOX.arcfollowing = false;
            FINGERBOX = null;
        }

    }*/

    bool waiting = false;
    IEnumerator multipleinputs()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        if (Archits.Count > 1&&!overlapping)
        {
            StartCoroutine(WRONGARC());
            Archits.Clear();
        }
        waiting = false;
    }

    public bool WRONG = false;

    



   

    Transform otherarchit = null;
    bool hitonce = false;

    public Vector3 CAPscale = new Vector3(0.4f,0.395f,1);

   /* private IEnumerator OnTriggerEnter(Collider other)
    {
        /*if (other.name.Contains("ARCHIT") && !specialhit && hitonce == true && FINGERBOX != null)
        {
            //specialhit = true;
            //StopCoroutine(RemoveSpecial());
            if (other.GetComponent<ARCHIT>().Color == -1)
            {
                otherarchit = other.transform;
                yield return new WaitForSeconds(0.1f);
                if (otherarchit != null)
                {
                    print("lolno");
                    parentArc.ArcHold.arcFin = true;
                    foreach (ArcArc ARC in parentArc.ArcHold.ArcGroup)
                    {
                        ARC.arcFin = true;
                        notemissed = true;
                    }

                    if (FINGERBOX != null)
                    {
                        FINGERBOX.arcfollowing = false;
                        FINGERBOX = null;
                    }
                }
            }

        }
        yield return new WaitForSeconds(0.0f);
        if (FINGERBOX == null && !notemissed && !parentArc.ArcHold.arcFin && Captransform.localScale.y<0.5f &&(currentCOMBO != COMBO||COMBO==0) && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped&&!parentArc.ArcHold.IsVoid)
        {
            
                       
                ARCHIT ACHIT = other.GetComponent<ARCHIT>();
                try
                {
                    

                        if ((ACHIT.arcfollowing == false && (ACHIT.Color == parentArc.ArcHold.Color || ACHIT.Color == -1)) || specialhit)
                        {
                            if (firsthit && duriation > 0)
                            {
                                firsthit = false;
                                ArcEffectManager.Instance.PlayArcSound();
                            }


                            stay = true;
                            FINGERBOX = ACHIT;
                            if (specialhit)
                            {
                                // StopCoroutine(RemoveSpecial());
                                // StartCoroutine(RemoveSpecial());
                            }
                            if (!specialhit)
                            {
                                FINGERBOX.arcfollowing = true;
                                ACHIT.Color = parentArc.ArcHold.Color;
                                parentArc.ArcHold.arcFin = true;
                                //StartCoroutine(Hitonce());
                            }

                            BOXPOS = FINGERBOX.transform;
                            FINGER = FINGERBOX.finger;
                            ACTUALBOX = FINGER.CONSTANTBOX.transform;
                            
                        }
                        else
                        {
                            stay = true;
                            var temp = parentArc.ArcHold.Color == 1 ? "RED" : "BLUE";
                            if (temp == "RED") { ArcTimingManager.previousarcholdhitred = true; } else { ArcTimingManager.previousarcholdhitblue = true; }
                        }
                    
                }
                catch
                {
                    FINGERBOX = null;
                }

            

        }

    }*/

    IEnumerator removeinputs(GameObject item)
    {
        yield return new WaitForSeconds(0.05f);
        Archits.Remove(item);
    }


    public IEnumerator waitactive(int it)
    {
        yield return new WaitForSeconds(0.0f);
        if (distance == 0)
        {
            yield return new WaitForSeconds(Movespeed*parentArc.segmentsleft);
            /*if (temparchit != null && HITID != 0)
            {
                temparchit.CAPID = 0;
            }*/
        }
        else
        {

        }
        if (COMBO != 0)
        {
            yield return new WaitUntil(() => COMBO == currentCOMBO);
        }
        parentArc.StartCoroutine("deactivate");
        //transform.parent = parentArc.transform;
        transform.position = Vector3.zero;
      gameObject.SetActive(false);
        //ONDisable();
        
        
    }
}
