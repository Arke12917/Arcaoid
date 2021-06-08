using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Lean.Touch;
using System;
using Arcaoid.Aff;
using Arcaoid.Compose;
using Arcaoid.Gameplay.Chart;

public class movecap : MonoBehaviour {
    //public Rigidbody rb;
    public ArcArcRenderer parentArc;
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
    ARCHIT temparchit;
    public BoxCollider thisobj;
    Vector3 currentpos;
    public float[] segspeed;
    public List<GameObject> Archits;
    public Transform Captransform;
    public bool didset = false;
    public ArcScoreManager ACMAN;
    public ArcTimingManager ATMAN;

    // Use this for initialization
    void OnEnable () {

        //PathToFollow = GameObject.Find(pathName).GetComponent<EditorPathScript>();
        Captransform = this.transform;
        


        // this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
        //thisobj.enabled = false;

       //Setminspeed();

        //print("Start " + parentArc.Arc.Timing);
        //print("End " + parentArc.Arc.EndTiming);

        //print("finaltime " + Movespeed);
    }

    private void Awake()
    {
        ACMAN = ArcScoreManager.Instance;
        ATMAN = ArcTimingManager.Instance;
    }


    void OnDisable ()
   {
       //transform.parent = parentArc.transform;
       StopAllCoroutines();
       isinrange = false;      
       istogether = false;
        parentArc = null;
  // firsthit = true;
   waiting=false;
        WRONG = false;
        temparchit = null;
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
        if (!(ReferenceEquals(FINGER,null)))
        {
            FINGER.arcfollowing = false;
            FINGER = null;
        }
       
        MOVE = null;


    }

    public void CalculateCombo()
    {
        START = parentArc.Arc.Timing;
        END = parentArc.Arc.EndTiming;
        if (parentArc.Arc.IsVoid) return;
        int u = parentArc.IsHead? 0 : 1;
        double bpm = ArcTimingManager.Instance.CalculateBpmByTiming(parentArc.Arc.Timing);
        if (bpm <= 0) return;
        double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
        int total = (int)((parentArc.Arc.EndTiming - parentArc.Arc.Timing) / interval);
        //COMBO = total;
        if ((u ^ 1) >= total)
        {
            COMBO +=1;
            //parentArc.Arc.JudgeTimings.Add((int)(parentArc.Arc.Timing + (parentArc.Arc.EndTiming - parentArc.Arc.Timing) * 0.5f));
            return;
        }
        int n = u ^ 1;
        int t = parentArc.Arc.Timing;
        while (true)
        {
            t = (int)(parentArc.Arc.Timing + n * interval);
            if (t < parentArc.Arc.EndTiming)
            {
                COMBO += 1;
                //parentArc.Arc.JudgeTimings.Add(t);
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
        tempdur = duriation - 0.14f;
        if (tempdur >= 0) { } else { tempdur = duriation; }
        while ((TIME <= ((float)tempdur / COMBO)))
        {
            yield return null;
        }
        if (canhit == true&&!notemissed)
        //if (canhit == false)
        {
            //Debug.Log("PURE");
            if(firsthit)
                                {
                firsthit = false;
                ArcEffectManager.Instance.PlayArcSound();
            }
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
            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
            if (temp == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true;
                
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
            if (ArcScoreManager.guagetypeno == 0|| ArcScoreManager.guagetypeno == 3|| ArcScoreManager.guagetypeno == 4)
            {
                ACMAN.CLEARRATE.fillAmount -= 0.02f;
               /* if ((ArcScoreManager.CURRENTPERCENT - 2f) > 0)
                    ArcScoreManager.CURRENTPERCENT -= 2;
                else
                    ArcScoreManager.CURRENTPERCENT = 0;*/
            }
            else if (ArcScoreManager.guagetypeno == 1) { ACMAN.CLEARRATE.fillAmount -= 0.015f;
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
            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
            if (temp == "RED") { ArcTimingManager.previousarchitred = false; } else { ArcTimingManager.previousarchitblue = false; 
            }
            hitlast = false;
        }

        isready = false;
        if (COMBO != currentCOMBO)
            StartCoroutine(judgeholds());
        else
        {
            if (!(ReferenceEquals(FINGER, null)))
            {
                FINGER.arcfollowing = false;
                FINGER= null;
            }
            parentArc.readytodeac = true;
        }
    }

    

    IEnumerator givecombo()
    {
        if (!canhit && duriation != 0 && !(duriation < 0))
            yield return new WaitForSeconds(duriation / 1.8f);
        else
            yield return new WaitForSeconds(0.01f);



        //if (duriation == 0||duriation<=0.02f)
        if (duriation == 0)
        {
            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
            if ((temp=="RED"&&ArcTimingManager.previousarchitred)|| (temp == "BLUE" && ArcTimingManager.previousarchitblue))
            {
                //Debug.Log("pUrE");
                ArcTimingManager.COMBO += COMBO-currentCOMBO;
                ArcScoreManager.MAXPURES += COMBO-currentCOMBO;
                ArcScoreManager.PPURES += COMBO-currentCOMBO;
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
                if (temp == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true; }
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
                if (temp == "RED") { ArcTimingManager.previousarchitred = false; } else { ArcTimingManager.previousarchitblue = false; }
                currentCOMBO = COMBO;
                hitlast = false;
            }
            if (!(ReferenceEquals(FINGER, null)))
            {
                FINGER.arcfollowing = false;
                FINGER = null;
            }
            parentArc.readytodeac = true;
        }
        else
        {
            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
            if (canhit == true&&!notemissed)
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
                if (temp == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true; }
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
                if (temp == "RED") { ArcTimingManager.previousarchitred = false; } else { ArcTimingManager.previousarchitblue = false; }
                currentCOMBO = COMBO;
                ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(Captransform.position.x, Captransform.position.y + 1.2f), "LOST", 0f);              
                hitlast = false;
            }
            if (!(ReferenceEquals(FINGER, null)))
            {
                FINGER.arcfollowing = false;
                FINGER = null;
            }
            parentArc.readytodeac = true;
        }
        //stay = false;
        
    }
    public bool overlapping = false;
    public Vector3 faketransform;
    public IEnumerator window()
    {
        yield return new WaitForSeconds(1f);
        stay = false;
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
            
        }
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
        foreach (ArcArc ARC in parentArc.Arc.ArcGroup)
        {
            ARC.arcFin = false;
            notemissed = false;
        }
        //fingers = 0;
        turnedred = false;
        //parentArc.isalpha = true;
        foreach (ArcArcSegmentComponent s in parentArc.segments)
        {
            s.Color = parentArc.currentcolor;
            s.Alpha = 0.57f;
            
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
        if (!(ReferenceEquals(FINGER, null)))
        {
            FINGER.arcfollowing = false;
            FINGER = null;
            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
            if (temp == "RED") { ArcTimingManager.previousarchitred = false; } else { ArcTimingManager.previousarchitblue = false; }
            currentCOMBO = COMBO;

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
        
        if (!(ReferenceEquals(parentArc,null))) {
            if (canmove && !ismoving && !parentArc.Arc.IsVoid)
            {
                //ismoving = true;
                //StartCoroutine(UpdateMove());
                UpdateMove();
            }         
        }
        else
        {
           if(JUDeffect.isPlaying)
            JUDeffect.Stop();
        }
       
    }

    

 
   void UpdateMove()
    {
        //yield return new WaitForSeconds(0.0f);
        if (parentArc.gameObject.activeSelf == false)
        {
            parentArc.HeadRenderer.enabled = false;
        }
        if (parentArc.CANRENDER&&!parentArc.Arc.IsVoid)
        {
            float tempmove1 = parentArc.Arc.Timing - 10000f;
            float tempend = parentArc.Arc.EndTiming - 10000f;
            Movespeed = ((tempend - tempmove1) / 1000f) / (parentArc.segmentsleft + 1);
            duriation = (parentArc.Arc.EndTiming / 1000f) - (parentArc.Arc.Timing / 1000f);
            if (duriation != 0)
                duriation -= 0.05f;
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
                if (!parentArc.Arc.IsVoid)
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
                parentArc.Arc.arcFin = true;
                JUDeffect.Stop();
            }

            //put thing here
            if ((ReferenceEquals(FINGER, null)) && !notemissed && isinrange && !parentArc.Arc.arcFin && (currentCOMBO != COMBO) && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
            {

                if (ATMAN.allFingers.Count != 0)
                {
                    
                    foreach (LeanFinger finger in ATMAN.allFingers)
                    {
                        

                        if (Vector3.Distance(transform.position, finger.fingerVec) <= 3f)
                        {
                            //print((AC.transform.position - transform.position).sqrMagnitude);
                            //print(Vector3.Distance(transform.position, AC.transform.position));
                            
                            //if ((finger.arcfollowing == false && (finger.Color == parentArc.Arc.Color || finger.Color == -1)) || specialhit)
                            if ((finger.arcfollowing == false && (finger.Color == parentArc.Arc.Color || finger.Color == -1)))
                            {
                                if (firsthit && duriation > 0)
                                {
                                    firsthit = false;
                                    ArcEffectManager.Instance.PlayArcSound();
                                }


                                stay = true;
                                FINGER = finger;
                                if (specialhit)
                                {
                                    // StopCoroutine(RemoveSpecial());
                                    // StartCoroutine(RemoveSpecial());
                                }
                                if (!specialhit)
                                {
                                    FINGER.arcfollowing = true;
                                    FINGER.Color = parentArc.Arc.Color;
                                    parentArc.Arc.arcFin = true;
                                    //StartCoroutine(Hitonce());
                                }

                                break;
                            }
                            else
                            {
                                stay = true;
                                var tempac = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
                                if (tempac == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true; }
                            }
                        }



                    }
                }

            }


            //if ((parentArc.Arc.EndTiming -200) + ArcAudioManager.Instance.AudioOffset <= ArcGameplayManager.Instance.Timing && parentArc.movestarted && currentCOMBO != COMBO && COMBO != 1 && parentArc.Arc.EndTiming != 0f&&duriation!=0&&duriation>0.02f)
            if ((parentArc.Arc.EndTiming -200) + ArcAudioManager.Instance.AudioOffset <= ArcGameplayManager.Instance.Timing && parentArc.movestarted && currentCOMBO != COMBO && COMBO != 1 && parentArc.Arc.EndTiming != 0f)
            {
                //print(HOLD.EndTiming);
                canmove = false;
                StopAllCoroutines();
               
                stay = true;
                parentArc.Arc.arcFin = true;
                if (!(ReferenceEquals(FINGER,null)))
                {
                    FINGER.arcfollowing = false;
                    FINGER = null;
                }
                var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
                var temphit = false;
                if (temp == "RED") { temphit=ArcTimingManager.previousarchitred; } else { temphit=ArcTimingManager.previousarchitblue; }
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
                        ArcScoreManager.CURRENTPERCENT = 100;*/
                    if (temp == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true; }
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
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }else if (ArcScoreManager.guagetypeno == 1)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.015f;
                        /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                            ArcScoreManager.CURRENTPERCENT -= 1.5f;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
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
                    if (temp == "RED") { ArcTimingManager.previousarchitred = false; } else { ArcTimingManager.previousarchitblue = false; }
                    currentCOMBO = COMBO;

                }

                ismoving = false;
                //yield break;
                //StartCoroutine(DisableCAP());
                return;

            }

            if (FINGER != null && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped)
            {
                
                if (Vector3.Distance(FINGER.fingerVec,Captransform.position)<= 3f)
                {
                    stay = true;
                    //if(!specialhit)
                    //BOXPOS.position = new Vector3(transform.position.x,transform.position.y,0);

                    StopCoroutine(fingerlost());

                }
                else
                {
                    stay = true;
                   // if(!specialhit)
                   // BOXPOS.position = new Vector3(transform.position.x, transform.position.y, 0);

                    if (!lostrunning)
                    StartCoroutine(fingerlost());
                }
            }
            else
            {
                stay = false;
              
            }

            if (isinrange == true && !ArcTimingManager.Instance.IsStopped && !ArcTimingManager.Instance.IsBackwarding && Time.timeScale != 0f)
            {
                if (stay)
                {
                    
                    iscolliding = true;

                    if (gameObject.activeSelf == true)
                    {
                        StopCoroutine(window());
                    }
                    
                    
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
        ismoving = false;
    }

    public IEnumerator WRONGARC()
    {
        yield return new WaitForSeconds(0.0f);
        // iscolliding = false; 
       // print("WRNG");
       
        //WRONG = true;
        notemissed = true;
       parentArc.StartCoroutine(parentArc.MISS(FINGER));
        //temparchit.CAPID = 0;
       // HITID = 0;
       // temparchit.otherCAP = null;
        
    }
    public bool istogether = false;
    public bool firsthit = true;
    public bool specialhit = false;

   

   
   
    
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
                    parentArc.Arc.arcFin = true;
                    foreach (ArcArc ARC in parentArc.Arc.ArcGroup)
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
        if (FINGERBOX == null && !notemissed && !parentArc.Arc.arcFin && Captransform.localScale.y<0.5f &&(currentCOMBO != COMBO||COMBO==0) && Time.timeScale != 0 && !ArcTimingManager.Instance.IsBackwarding && !ArcTimingManager.Instance.IsStopped&&!parentArc.Arc.IsVoid)
        {
            
                       
                ARCHIT ACHIT = other.GetComponent<ARCHIT>();
                try
                {
                    

                        if ((ACHIT.arcfollowing == false && (ACHIT.Color == parentArc.Arc.Color || ACHIT.Color == -1)) || specialhit)
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
                                ACHIT.Color = parentArc.Arc.Color;
                                parentArc.Arc.arcFin = true;
                                //StartCoroutine(Hitonce());
                            }

                            BOXPOS = FINGERBOX.transform;
                            FINGER = FINGERBOX.finger;
                            ACTUALBOX = FINGER.CONSTANTBOX.transform;
                            
                        }
                        else
                        {
                            stay = true;
                            var temp = parentArc.Arc.Color == 1 ? "RED" : "BLUE";
                            if (temp == "RED") { ArcTimingManager.previousarchitred = true; } else { ArcTimingManager.previousarchitblue = true; }
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
