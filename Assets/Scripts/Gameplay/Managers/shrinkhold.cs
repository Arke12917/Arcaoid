using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using System.Diagnostics;
using Lean.Touch;
//using UnityEngine.XR.WSA.Input;

namespace Arcaoid.Gameplay
{
    public class shrinkhold : MonoBehaviour
    {
        private float currentFrom = 0;
        public SpriteRenderer REND;
        public Renderer render;
        public float TIME;
        public bool ableToHit = false;
        bool fingerWaiting = false;
        public bool shrinkrender = false;
        public bool shouldshrink = false;
        public bool comboadded = false;
        public bool hasrun = false;
        public bool isready = false;
        public bool canhit = false;
        public float duriation;
        public float distance;
        public float starttime;
        public float endtime;
        public int COMBO;
        private float fromongoing;
        bool wasfrom = false;
        public int currentCOMBO;
        private int fromShaderId;
        public Material bodyMaterialInstance;
        public Material HoldMaterial;
        public int Track;
        public float count;
        private bool shrinkingrun = false;
        public bool firsthit = true;
        public bool hitByLane = false;
        private bool stay = false;
        private bool iscolliding = false;
        private bool stophitbox = false;
        public Collider otherbox;
        public GameObject lfhTRACK;
        public GameObject otherboxGM;
        public Transform holdtransform;
        public ArcHold HOLD;
        public LeanFingerHeld LFH;
        public ArcTimingManager ATMAN;
        public ArcAudioManager AUMAN;

        public ArcGameplayManager AMAN;
        public ArcScoreManager ACMAN;



        // Use this for initialization
        void Awake()
        {
            ATMAN = ArcTimingManager.Instance;
            AUMAN = ArcAudioManager.Instance;
            AMAN = ArcGameplayManager.Instance;
            ACMAN = ArcScoreManager.Instance;
            //REND = this.GetComponent<SpriteRenderer>();
            //render = this.gameObject.GetComponent<Renderer>();
            bodyMaterialInstance = REND.material;
            //fromShaderId = Shader.PropertyToID("_From");
            holdtransform = this.transform;
            // ATMAN.OnSpeedChange.AddListener(holdchange);
            //LFH = GameObject.FindGameObjectWithTag("FACTIV").GetComponent<LeanFingerHeld>();


        }

        private void OnEnable()
        {
            render.material.SetFloat("_Alpha", 0.862f);
            //From = 0f;
        }

        public void holdchange()
        {
            if (isActiveAndEnabled && !(AMAN.Timing < 100))
            {
                //print("uwu");
                HOLD.Position = ATMAN.CalculatePositionByTiming(HOLD.Timing + AUMAN.AudioOffset);
                // float endPosition = ATMAN.CalculatePositionByTiming(HOLD.EndTiming + AUMAN.AudioOffset);
                // float length = (endPosition - HOLD.Position) / 1000f;
                transform.position = new Vector3(transform.position.x, 0, -HOLD.Position);
                //transform.localScale = new Vector3(1.53f, length / 3.79f, 1);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            ableToHit = false;
            fingerWaiting = false;
            currentCOMBO = 0;
            colentered = false;
            HOLD = null;
            lfhTRACK = null;
            didhit = false;
            hitByLane = false;
            currentFrom = 0;
            shrinkrender = false;
            shouldshrink = false;
            comboadded = false;
            hasrun = false;
            isready = false;
            canhit = false;
            hitlast = false;
            canbackward = true;
            duriation = 0;
            distance = 0;
            starttime = 0;
            endtime = 0;
            fromongoing = 0;
            wasfrom = false;
            fingerscount = 0;

            //From = 0f;
            // bodyMaterialInstance.SetFloat(fromShaderId, 0f);
            currentCOMBO = 0;
            COMBO = 0;
            Track = 0;
            count = 0;
            shrinkingrun = false;
            firsthit = true;
            stay = false;
            iscolliding = false;
            canupdate = true;
            shralling = false;
            stophitbox = false;
            if (!(ReferenceEquals(render, null)))
            {
                if (!(ReferenceEquals(render.material, null)))
                    render.material.SetFloat("_Alpha", 0.862f);
            }
        }



        public void CalculateCombo()
        {

            int u = 0;
            double bpm = ATMAN.CalculateBpmByTiming((int)starttime);
            if (bpm <= 0) return;
            double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
            int total = (int)((endtime - starttime) / interval);
            if ((u ^ 1) >= total)
            {
                COMBO += 1;
                //JudgeTimings.Add((int)(Timing + (EndTiming - Timing) * 0.5f));
                return;
            }
            int n = u ^ 1;
            int t = (int)starttime;
            while (true)
            {
                t = (int)(starttime + n * interval);
                if (t < endtime)
                {
                    COMBO += 1;
                }
                if (total == ++n)
                {
                    break;
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
                }
            }
        }
        // Update is called once per frame


        /* IEnumerator SHRALL()
         {
             yield return new WaitUntil(() => HOLD.Timing<=ArcGameplayManager.Instance.Timing);
             if (!IsInvoking("SHRINKALL"))
             {
                 InvokeRepeating("SHRINKALL", 0f, 0.00833333f);
             }
         }*/

        IEnumerator backwarded()
        {
            yield return new WaitUntil(() => !ArcTimingManager.Instance.ShouldRender(HOLD.Timing + AUMAN.AudioOffset));
            if (!(ReferenceEquals(otherbox, null)))
            {
                //otherbox.enabled = false;
                //otherbox.GetComponent<holdset>().disabled = false;
                /* holdset sh = otherbox.gameObject.GetComponent<holdset>();
                 if (sh.iscolliding && sh.TRACK == HOLD.Track)
                 {
                     sh.finger.HoldFin = false;
                 }
                 else
                 {
                     //sh.finger.HoldFin = true;
                     //StartCoroutine(DestroyBOX(otherbox.gameObject));
                 }*/
            }

            //ting here

            ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
            //HOLD.TPOBJ = null;        
            switch (HOLD.Track)
            {
                case 1:
                    LFH.TRACKOVL1.SetActive(false);
                    break;
                case 2:
                    LFH.TRACKOVL2.SetActive(false);
                    break;
                case 3:
                    LFH.TRACKOVL3.SetActive(false);
                    break;
                case 4:
                    LFH.TRACKOVL4.SetActive(false);
                    break;
            }
            switch (HOLD.Track)
            {
                case 1:
                    ATMAN.holdQuad1.Remove(this.gameObject);
                    break;
                case 2:
                    ATMAN.holdQuad2.Remove(this.gameObject);
                    break;
                case 3:
                    ATMAN.holdQuad3.Remove(this.gameObject);
                    break;
                case 4:
                    ATMAN.holdQuad4.Remove(this.gameObject);
                    break;

            }
            HOLD.TPOBJ = null;
            ArcHoldNoteManager.Instance.StartCoroutine(ArcHoldNoteManager.Instance.HOLDS(HOLD));
            gameObject.SetActive(false);
        }

        bool canbackward = true;
        public bool shralling = false;

        private void FixedUpdate()
        {
            //if (canupdate)
                FixedUpdatething();

            if (canbackward == false && !ATMAN.IsBackwarding)
            {

                StopAllCoroutines();
            }
        }

        IEnumerator WaitForFinger()
        {
            yield return new WaitForSeconds(0.25f);
            ableToHit = false;
            fingerWaiting = false;
        }

        void FixedUpdatething()
        {
            if (ArcTimingManager.readytorun && !hasrun)
            {
                hasrun = true;
                CalculateCombo();
            }



            if (ATMAN.IsBackwarding && gameObject.activeSelf && canbackward == true)
            {

                canbackward = false;
                canupdate = false;
                StartCoroutine(backwarded());
                return;
            }

            if (hitByLane)
            {
                if (lfhTRACK.activeSelf == true&&HOLD.Timing + AUMAN.AudioOffset <= AMAN.Timing)
                {
                    StopCoroutine(WaitForFinger());
                    ableToHit = true;
                    fingerWaiting = false;
                }
                else
                {
                    if (!fingerWaiting)
                    {
                        fingerWaiting = true;
                        StartCoroutine(WaitForFinger());
                    }
                }

                if (ableToHit)
                {
                    fingerscount += 1;

                    stay = true;
                    colentered = true;
                }
                else
                {
                    stay = false;
                    fingerscount -= 1;
                    switch (HOLD.Track)
                    {
                        case 1:
                            LFH.TRACKOVL1.SetActive(false);
                            break;
                        case 2:
                            LFH.TRACKOVL2.SetActive(false);
                            break;
                        case 3:
                            LFH.TRACKOVL3.SetActive(false);
                            break;
                        case 4:
                            LFH.TRACKOVL4.SetActive(false);
                            break;
                    }
                }


                if (stay)
                {
                    iscolliding = true;
                    //stay = false;
                }
                else
                {
                    iscolliding = false;
                    wasfrom = false;
                }

                if (iscolliding)
                {
                    //StopCoroutine(unHit());
                    canhit = true;
                    shrinkingrun = false;
                }
                else if (shouldshrink)
                {
                    //canhit = true;
                    canhit = false;

                }

                if (canhit)
                {
                    shrinkrender = true;
                    if (firsthit)
                    {
                        firsthit = false;
                        StartCoroutine(ArcEffectManager.Instance.PlayHOLDSound(starttime - Mathf.Abs(AUMAN.AudioOffset)));
                    }
                    render.material.SetFloat("_Alpha", 0.862f);
                    REND.sprite = ArcHoldNoteManager.Instance.HighlightSprite;
                    ArcEffectManager.Instance.SetHoldNoteEffect(Track, true);
                }
                else
                {
                    if (shouldshrink && currentCOMBO != COMBO)
                    {
                        render.material.SetFloat("_Alpha", 0.43f);


                        REND.sprite = ArcHoldNoteManager.Instance.DefaultSprite;
                        ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);


                    }
                    else
                    {
                        render.material.SetFloat("_Alpha", 0.862f);

                    }
                    if (currentCOMBO != COMBO)
                        shrinkrender = false;

                }



                if (shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
                {
                    if ((HOLD.EndTiming + AUMAN.AudioOffset) <= ArcGameplayManager.Instance.Timing && currentCOMBO == COMBO && COMBO != 0 && HOLD.EndTiming != 0f)
                    {
                        /*if (!(ReferenceEquals(otherbox, null)))
                        {
                            //otherbox.enabled = false;
                            //otherbox.GetComponent<holdset>().disabled = false;
                            if (!(ReferenceEquals(otherbox.gameObject, null)))
                            {
                                holdset sh = otherbox.gameObject.GetComponent<holdset>();
                                if (sh.iscolliding && sh.TRACK == HOLD.Track)
                                {
                                    sh.finger.HoldFin = false;
                                }
                                else
                                {
                                    //sh.finger.HoldFin = true;
                                    //StartCoroutine(DestroyBOX(otherbox.gameObject));
                                }
                            }

                        }*/

                        //ting here

                        ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
                        //HOLD.TPOBJ = null;        
                        switch (HOLD.Track)
                        {
                            case 1:
                                LFH.TRACKOVL1.SetActive(false);
                                break;
                            case 2:
                                LFH.TRACKOVL2.SetActive(false);
                                break;
                            case 3:
                                LFH.TRACKOVL3.SetActive(false);
                                break;
                            case 4:
                                LFH.TRACKOVL4.SetActive(false);
                                break;
                        }
                        switch (HOLD.Track)
                        {
                            case 1:
                                ATMAN.holdQuad1.Remove(this.gameObject);
                                break;
                            case 2:
                                ATMAN.holdQuad2.Remove(this.gameObject);
                                break;
                            case 3:
                                ATMAN.holdQuad3.Remove(this.gameObject);
                                break;
                            case 4:
                                ATMAN.holdQuad4.Remove(this.gameObject);
                                break;

                        }
                        gameObject.SetActive(false);

                    }



                }

                if (shrinkrender && shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
                {




                    /*if (!IsInvoking("SHRINK"))
                    {
                        InvokeRepeating("SHRINK", 0f, 0.00833333f);
                    }*/

                    switch (HOLD.Track)
                    {
                        case 1:
                            LFH.TRACKOVL1.SetActive(true);
                            break;
                        case 2:
                            LFH.TRACKOVL2.SetActive(true);
                            break;
                        case 3:
                            LFH.TRACKOVL3.SetActive(true);
                            break;
                        case 4:
                            LFH.TRACKOVL4.SetActive(true);
                            break;
                    }
                    //From = Mathf.MoveTowards(From, 1f, Time.deltaTime * (1f / duriation));                 




                }
                //print("ohter!");

                /*else if (other.name == "LINESHRINK")
                {
                    shouldshrink = true;
                    shrinkrange = true;
                }*/
            }
            if (HOLD != null)
            {
                if (HOLD.Timing + AUMAN.AudioOffset <= ArcGameplayManager.Instance.Timing && !shralling)
                {
                    shralling = true;

                    
                    
                   // if (!(COMBO == 1&&endtime-starttime<230.0f))
                    if (!(COMBO == 1))
                    {
                        //print("judge");
                        StartCoroutine(judgeholds());
                    }
                    else
                    {
                        //print("give");
                        StartCoroutine(givecombo());
                    }

                    shouldshrink = true;
                    shrinkrange = true;

                    //print(duriation / (60 / ATMAN.DEFAULTBPM));
                }


                else if ((HOLD.EndTiming - 200) + AUMAN.AudioOffset <= ArcGameplayManager.Instance.Timing && currentCOMBO != COMBO && HOLD.EndTiming != 0f && COMBO != 1)
                {
                    
                    //print(HOLD.EndTiming);
                    canupdate = false;
                    StopAllCoroutines();
                    if (hitlast == true||stay)
                    {
                        ArcTimingManager.COMBO += (COMBO - currentCOMBO);
                        ArcScoreManager.MAXPURES += (COMBO - currentCOMBO);
                        ArcScoreManager.PPURES += (COMBO - currentCOMBO);
                        ArcScoreManager.Score += (ArcScoreManager.BASESCORE * (COMBO - currentCOMBO)) + (1 * (COMBO - currentCOMBO));
                        ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount * (COMBO - currentCOMBO);
                        /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT * (COMBO - currentCOMBO)) < 100)
                            ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                        else
                            ArcScoreManager.CURRENTPERCENT = 100;*/
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
                        }
                        else if (ArcScoreManager.guagetypeno == 1)
                        {
                            ACMAN.CLEARRATE.fillAmount -= 0.015f;
                            /* if ((ArcScoreManager.CURRENTPERCENT - 1.5f > 0))
                                 ArcScoreManager.CURRENTPERCENT -= 1.5f;
                             else
                                 ArcScoreManager.CURRENTPERCENT = 0;*/
                        }
                        else if (ArcScoreManager.guagetypeno == 2)
                        {
                            if ((ACMAN.NOTETOTAl <= 16) == false)
                            {
                                if ((ACMAN.CLEARRATE.fillAmount <= 0.3) == false)
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
                        currentCOMBO = COMBO;

                    }
                    DEac = StartCoroutine(Deactivate());
                    return;


                }
            }
            if (shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
            {
                if (isready)
                {
                    TIME += Time.deltaTime;
                }
            }


        }

        public Coroutine DEac;

        /*void SHRINKALL()
        {
            if (fromongoing != 1)
            {
                fromongoing= Mathf.MoveTowards(fromongoing, 1f, 0.00833333f * (1f / duriation));
               // From = fromongoing;
            }
            else
                CancelInvoke("SHRINKALL");
        }*/

        /* void SHRINK()
         {
             if (From != 1)
                 From = Mathf.MoveTowards(From, 1f, 0.00833333f * (1f / duriation));
             else
                 CancelInvoke("SHRINK");
         }*/

        IEnumerator Deactivate()
        {
            yield return new WaitUntil(() => HOLD.EndTiming + AUMAN.AudioOffset <= ArcGameplayManager.Instance.Timing);
            ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
            /*if ((object)otherbox!=null&&colentered)
            {
                
                if (otherboxGM.activeSelf == true)
                {
                    //otherbox.enabled = false;
                    //otherbox.GetComponent<holdset>().disabled = false;
                    //holdset sh = otherbox.gameObject.GetComponent<holdset>();

                    // sh.finger.HoldFin = true;
                    if (!(ReferenceEquals(otherboxGM, null)))
                    {
                        //if ((object)otherbox.gameObject!= null)
                        otherboxGM.transform.position = boxoob;
                    }

                    yield return new WaitForFixedUpdate();

                    if (!(ReferenceEquals(otherboxGM, null)))
                        otherboxGM.SetActive(false);
                    otherboxGM = null;
                    otherbox = null;
                }        
               
            }*/

            //ting here


            //HOLD.TPOBJ = null;        
            switch (HOLD.Track)
            {
                case 1:
                    LFH.TRACKOVL1.SetActive(false);
                    break;
                case 2:
                    LFH.TRACKOVL2.SetActive(false);
                    break;
                case 3:
                    LFH.TRACKOVL3.SetActive(false);
                    break;
                case 4:
                    LFH.TRACKOVL4.SetActive(false);
                    break;
            }
            switch (HOLD.Track)
            {
                case 1:
                    ATMAN.holdQuad1.Remove(this.gameObject);
                    break;
                case 2:
                    ATMAN.holdQuad2.Remove(this.gameObject);
                    break;
                case 3:
                    ATMAN.holdQuad3.Remove(this.gameObject);
                    break;
                case 4:
                    ATMAN.holdQuad4.Remove(this.gameObject);
                    break;

            }
            gameObject.SetActive(false);
        }

        IEnumerator unHit()
        {
            shrinkingrun = true;
            //yield return new WaitForSecondsRealtime(0.3f);
            yield return new WaitForSecondsRealtime(36.0000144f * Time.fixedDeltaTime);
            canhit = false;

            shrinkingrun = false;
        }

        IEnumerator judgeholds()
        {
            float tempdur = 0;

            tempdur = duriation - 0.1f;


            //isjudging = true;

            yield return new WaitForSecondsRealtime(0.0f);
            //yield return new WaitUntil(() => HOLD.Timing + AUMAN.AudioOffset <= ArcGameplayManager.Instance.Timing);

            TIME = 0f;
            if (currentCOMBO != COMBO)
            {
                var shouldhit = false;
                //bool didwait= false;
                /*if (!canhit)
                {
                   
                }
                else
                {
                    shouldhit = true;
                }*/
                isready = true;
                if (currentCOMBO == 0 && !didhit)
                {
                    yield return new WaitForSeconds(0.4f);
                }
                else
                {
                    while ((TIME <= ((float)tempdur / COMBO)))
                    {
                        yield return null;
                    }
                }

                if (canhit == true || shouldhit)
                {
                    ArcTimingManager.COMBO += 1;
                    currentCOMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ArcScoreManager.PPURES += 1;
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(holdtransform.position.x, holdtransform.position.y + 1.2f), "PURE", 0f);
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                    ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    hitlast = true;
                    shouldhit = false;

                }
                else
                {
                    render.material.SetFloat("_Alpha", 0.43f);
                    ArcTimingManager.COMBO = 0;
                    ArcScoreManager.MAXLOSTS += 1;
                    ACMAN.PM.enabled = false;
                    ACMAN.FR.enabled = false;
                    currentCOMBO += 1;
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(holdtransform.position.x, holdtransform.position.y + 1.2f), "LOST", 0f);
                    if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.02f;
                        /*if ((ArcScoreManager.CURRENTPERCENT - 2f) > 0)
                            ArcScoreManager.CURRENTPERCENT -= 2;
                        else
                            ArcScoreManager.CURRENTPERCENT = 0;*/
                    }
                    else if (ArcScoreManager.guagetypeno == 1)
                    {
                        ACMAN.CLEARRATE.fillAmount -= 0.015f;
                        /*if ((ArcScoreManager.CURRENTPERCENT - 1.5f) > 0)
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
                                /* if ((ArcScoreManager.CURRENTPERCENT - 4.5f) > 0)
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
                    hitlast = false;
                }
                //if()
                isready = false;
                // yield return new WaitForSeconds((60/ATMAN.DEFAULTBPM)- (0.0879f));
                //yield return new WaitUntil(() => (TIME >= (60 / ATMAN.CURRENTBPM)-(0.082f)));
                //print(From);

                //isjudging = false;
                StartCoroutine(judgeholds());
            }
            else if(currentCOMBO==COMBO&&COMBO!=0&&canupdate)
            {
                canupdate=false;
                DEac = StartCoroutine(Deactivate());
            }
        }

        IEnumerator givecombo()
        {

            if (!didhit)
            {
                yield return new WaitForSeconds(duriation / 1.4f);
            }
            /*if (!canhit)
                yield return new WaitForSeconds(duriation / 1.8f);
            else
                shouldhit = true;*/


            if (COMBO != 0 && COMBO != currentCOMBO)
            {
                if (canhit == true || didhit)
                {
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ArcScoreManager.PPURES += 1;
                    currentCOMBO += 1;
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(holdtransform.position.x, holdtransform.position.y + 1.2f), "PURE", 0f);
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                    ACMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/
                    hitlast = true;
                }
                else
                {
                    render.material.SetFloat("_Alpha", 0.43f);
                    ArcTimingManager.COMBO = 0;
                    ArcScoreManager.MAXLOSTS += 1;
                    ACMAN.PM.enabled = false;
                    ACMAN.FR.enabled = false;
                    currentCOMBO += 1;
                    ArcEffectManager.Instance.PlayTIMEEffectAt(new Vector2(holdtransform.position.x, holdtransform.position.y + 1.2f), "LOST", 0f);
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
                    hitlast = false;
                }
                ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
            }
            
                canupdate = false;
                DEac = StartCoroutine(Deactivate());
            

        }

        public bool hitlast = false;
        public bool didhit = false;
        public int fingerscount = 0;

        bool canupdate = true;

        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.name.Contains("HILD"))
        //    {
        //        if (fingerscount > 1) { fingerscount -= 1; }
        //        else
        //        {
        //            stay = false;
        //            fingerscount -= 1;
        //        }
        //        if (stay)
        //        {
        //            iscolliding = true;
        //            //stay = false;
        //        }
        //        else
        //        {

        //                iscolliding = false;
        //                wasfrom = false;


        //        }

        //        if (iscolliding)
        //        {
        //            //StopCoroutine(unHit());
        //            canhit = true;
        //            shrinkingrun = false;
        //        }
        //        else if (shouldshrink)
        //        {
        //            //canhit = true;
        //            canhit = false;
        //            switch (HOLD.Track)
        //            {
        //                case 1:
        //                    LFH.TRACKOVL1.SetActive(false);
        //                    break;
        //                case 2:
        //                    LFH.TRACKOVL2.SetActive(false);
        //                    break;
        //                case 3:
        //                    LFH.TRACKOVL3.SetActive(false);
        //                    break;
        //                case 4:
        //                    LFH.TRACKOVL4.SetActive(false);
        //                    break;
        //            }

        //        }

        //        if (canhit)
        //        {
        //            shrinkrender = true;
        //            if (firsthit)
        //            {
        //                firsthit = false;
        //                StartCoroutine(ArcEffectManager.Instance.PlayHOLDSound(starttime - Mathf.Abs(AUMAN.AudioOffset)));
        //            }
        //            render.material.SetFloat("_Alpha", 0.862f);
        //            REND.sprite = ArcHoldNoteManager.Instance.HighlightSprite;
        //            ArcEffectManager.Instance.SetHoldNoteEffect(Track, true);
        //        }
        //        else
        //        {
        //            if (shouldshrink && currentCOMBO != COMBO)
        //            {
        //                render.material.SetFloat("_Alpha", 0.43f);


        //                REND.sprite = ArcHoldNoteManager.Instance.DefaultSprite;
        //                ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);


        //            }
        //            else
        //            {
        //                render.material.SetFloat("_Alpha", 0.862f);

        //            }
        //            if (currentCOMBO != COMBO)
        //                shrinkrender = false;

        //        }



        //        if (shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
        //        {
        //            if ((HOLD.EndTiming + AUMAN.AudioOffset) <= ArcGameplayManager.Instance.Timing && currentCOMBO == COMBO && COMBO != 0 && HOLD.EndTiming != 0f)
        //            {
        //                if (!(ReferenceEquals(otherbox, null)))
        //                {
        //                    //otherbox.enabled = false;
        //                    //otherbox.GetComponent<holdset>().disabled = false;
        //                    if (!(ReferenceEquals(otherbox.gameObject, null)))
        //                    {
        //                        holdset sh = otherbox.gameObject.GetComponent<holdset>();


        //                        if (sh.iscolliding && sh.TRACK == HOLD.Track)
        //                        {
        //                            sh.finger.HoldFin = false;
        //                        }
        //                        else
        //                        {
        //                            //sh.finger.HoldFin = true;
        //                            //StartCoroutine(DestroyBOX(otherbox.gameObject));

        //                        }
        //                    }
        //                }

        //                //ting here

        //                ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
        //                //HOLD.TPOBJ = null;        
        //                switch (HOLD.Track)
        //                {
        //                    case 1:
        //                        LFH.TRACKOVL1.SetActive(false);
        //                        break;
        //                    case 2:
        //                        LFH.TRACKOVL2.SetActive(false);
        //                        break;
        //                    case 3:
        //                        LFH.TRACKOVL3.SetActive(false);
        //                        break;
        //                    case 4:
        //                        LFH.TRACKOVL4.SetActive(false);
        //                        break;
        //                }
        //                switch (HOLD.Track)
        //                {
        //                    case 1:
        //                        ATMAN.holdQuad1.Remove(this.gameObject);
        //                        break;
        //                    case 2:
        //                        ATMAN.holdQuad2.Remove(this.gameObject);
        //                        break;
        //                    case 3:
        //                        ATMAN.holdQuad3.Remove(this.gameObject);
        //                        break;
        //                    case 4:
        //                        ATMAN.holdQuad4.Remove(this.gameObject);
        //                        break;

        //                }
        //                gameObject.SetActive(false);

        //            }



        //        }

        //        if (shrinkrender && shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
        //        {





        //            /*if (!IsInvoking("SHRINK"))
        //            {
        //                InvokeRepeating("SHRINK", 0f, 0.00833333f);
        //            }*/

        //            /*switch (HOLD.Track)
        //            {
        //                case 1:
        //                    LFH.TRACKOVL1.SetActive(false);
        //                    break;
        //                case 2:
        //                    LFH.TRACKOVL2.SetActive(false);
        //                    break;
        //                case 3:
        //                    LFH.TRACKOVL3.SetActive(false);
        //                    break;
        //                case 4:
        //                    LFH.TRACKOVL4.SetActive(false);
        //                    break;
        //            }*/
        //            //From = Mathf.MoveTowards(From, 1f, Time.deltaTime * (1f / duriation));                 




        //        }
        //    }
        //}
        public bool isjudging = false;


        public Vector3 boxoob = new Vector3(500, 500, 500);

        IEnumerator DestroyBOX(GameObject other)
        {
            switch (HOLD.Track)
            {
                case 1:
                    LFH.TRACKOVL1.SetActive(false);
                    break;
                case 2:
                    LFH.TRACKOVL2.SetActive(false);
                    break;
                case 3:
                    LFH.TRACKOVL3.SetActive(false);
                    break;
                case 4:
                    LFH.TRACKOVL4.SetActive(false);
                    break;
            }
            if (!(ReferenceEquals(other, null)))
            {
                other.transform.position = boxoob;
                yield return new WaitForFixedUpdate();

                if (!(ReferenceEquals(otherbox.gameObject, null)))
                    otherbox.gameObject.SetActive(false);
                otherbox = null;


            }

        }

        public bool colentered = false;
        public bool shrinkrange = false;
        /* private void OnTriggerEnter(Collider other)
         {
             if (HOLD.Timing + AUMAN.AudioOffset - 5500 <= ArcGameplayManager.Instance.Timing)
             {


                 /*if (other.name == ("LINE"))
                 {



                     if (COMBO != 1)
                     {
                         yield return new WaitForSeconds(0.0f);
                         StartCoroutine(judgeholds());
                     }
                     else
                     {
                         StartCoroutine(givecombo());
                     }

                     //print(duriation / (60 / ATMAN.DEFAULTBPM));

                 }*/

        /*if (other.name=="LINE")
        {
            shralling = true;


            if (COMBO != 1)
            {

                StartCoroutine(judgeholds());
            }
            else
            {
                StartCoroutine(givecombo());
            }

            shouldshrink = true;
            shrinkrange = true;

            //print(duriation / (60 / ATMAN.DEFAULTBPM));
        }
        if (other.name.Contains("HILD"))
        {

            fingerscount += 1;

            stay = true;
            otherbox = other;
            otherboxGM = other.gameObject;
            colentered = true;
            if (stay)
            {
                iscolliding = true;
                //stay = false;
            }
            else
            {
                iscolliding = false;
                wasfrom = false;
            }

            if (iscolliding)
            {
                //StopCoroutine(unHit());
                canhit = true;
                shrinkingrun = false;
            }
            else if (shouldshrink)
            {
                //canhit = true;
                canhit = false;

            }

            if (canhit)
            {
                shrinkrender = true;
                if (firsthit)
                {
                    firsthit = false;
                    StartCoroutine(ArcEffectManager.Instance.PlayHOLDSound(starttime - Mathf.Abs(AUMAN.AudioOffset)));
                }
                render.material.SetFloat("_Alpha", 0.862f);
                REND.sprite = ArcHoldNoteManager.Instance.HighlightSprite;
                ArcEffectManager.Instance.SetHoldNoteEffect(Track, true);
            }
            else
            {
                if (shouldshrink && currentCOMBO != COMBO)
                {
                    render.material.SetFloat("_Alpha", 0.43f);


                    REND.sprite = ArcHoldNoteManager.Instance.DefaultSprite;
                    ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);


                }
                else
                {
                    render.material.SetFloat("_Alpha", 0.862f);

                }
                if (currentCOMBO != COMBO)
                    shrinkrender = false;

            }



            if (shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
            {
                if ((HOLD.EndTiming + AUMAN.AudioOffset) <= ArcGameplayManager.Instance.Timing && currentCOMBO == COMBO && COMBO != 0 && HOLD.EndTiming != 0f)
                {
                    if (!(ReferenceEquals(otherbox, null)))
                    {
                        //otherbox.enabled = false;
                        //otherbox.GetComponent<holdset>().disabled = false;
                        if (!(ReferenceEquals(otherbox.gameObject, null)))
                        {
                            holdset sh = otherbox.gameObject.GetComponent<holdset>();
                            if (sh.iscolliding && sh.TRACK == HOLD.Track)
                            {
                                sh.finger.HoldFin = false;
                            }
                            else
                            {
                                //sh.finger.HoldFin = true;
                                //StartCoroutine(DestroyBOX(otherbox.gameObject));
                            }
                        }

                    }

                    //ting here

                    ArcEffectManager.Instance.SetHoldNoteEffect(Track, false);
                    //HOLD.TPOBJ = null;        
                    switch (HOLD.Track)
                    {
                        case 1:
                            LFH.TRACKOVL1.SetActive(false);
                            break;
                        case 2:
                            LFH.TRACKOVL2.SetActive(false);
                            break;
                        case 3:
                            LFH.TRACKOVL3.SetActive(false);
                            break;
                        case 4:
                            LFH.TRACKOVL4.SetActive(false);
                            break;
                    }
                    switch (HOLD.Track)
                    {
                        case 1:
                            ATMAN.holdQuad1.Remove(this.gameObject);
                            break;
                        case 2:
                            ATMAN.holdQuad2.Remove(this.gameObject);
                            break;
                        case 3:
                            ATMAN.holdQuad3.Remove(this.gameObject);
                            break;
                        case 4:
                            ATMAN.holdQuad4.Remove(this.gameObject);
                            break;

                    }
                    gameObject.SetActive(false);

                }



            }

            if (shrinkrender && shouldshrink && !ATMAN.IsBackwarding && !ATMAN.IsStopped && Time.timeScale != 0)
            {




                    /*if (!IsInvoking("SHRINK"))
                    {
                        InvokeRepeating("SHRINK", 0f, 0.00833333f);
                    }

                    switch (HOLD.Track)
                    {
                        case 1:
                            LFH.TRACKOVL1.SetActive(true);
                            break;
                        case 2:
                            LFH.TRACKOVL2.SetActive(true);
                            break;
                        case 3:
                            LFH.TRACKOVL3.SetActive(true);
                            break;
                        case 4:
                            LFH.TRACKOVL4.SetActive(true);
                            break;
                    }
                    //From = Mathf.MoveTowards(From, 1f, Time.deltaTime * (1f / duriation));                 




            }
            //print("ohter!");
        }
        /*else if (other.name == "LINESHRINK")
        {
            shouldshrink = true;
            shrinkrange = true;
        }
    }
}*/
    }
}
