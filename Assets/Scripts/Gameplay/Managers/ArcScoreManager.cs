using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using E7.Native;
using Arcaoid.Compose;
using System.IO;
using SecPlayerPrefs;

namespace Arcaoid.Gameplay
{
    public class ArcScoreManager : MonoBehaviour
    {
        public int totaal;
        public int NOTETOTAl;
        public bool hascalculated = false;
        public static double BASESCORE;
        public static double Score;
        public double visiblescore;
        public bool tempbool = false;
        public Image CLEARRATE;
        public Image goverlay;
        public Sprite overlayhard;
        public Sprite GuagetypeNormal;
        public Sprite GuagetypeEasy;
        public Sprite GuagetypeHard;

        public static float clearrate;
        public static Image sngnbg;
        public static string resultdiff;
        public static int MAXRECALL;
        public static int MAXPURES;
        public static int PPURES;
        public static int EPURES;
        public static int LPURES;
        public static int EFARS;
        public static int LFARS;
        public static int MAXFARS;
        public static int MAXLOSTS;
        public static int maxpercent;
        public static Vector2 percentpos;
        public static bool PUREMEM = false;
        public static bool FULLREC= false;

        public static bool PUREEORL;

        public static float CURRENTPERCENT;
        public Text PERCENT;
        public RectTransform TXTPOS;
        public static float basepos;
        public static float BASEPERCENT;
        public static float fillamount;
        public static double BASEFILL;
        public Color below70;
        public Color above70;
        public Color above70easy;
        public Image PM;
        public Image FR;
        public GameObject RESULTTHING;
        public TRACKCOMPLETE TRCK;
        public bool canpause = true;
        public static int guagetypeno = 0;
        public int notetotal = 0;

        public Image Guage1;
        public Image Guage2;
        public Image Guage3;
        public Text PRCNT;
        public static bool ismirrored = false;
        public static bool ishard = false;
        public NativeAudioPointer NAGUAGE;

        public GameObject DYING;
        public GameObject DYINGGUAGE;
        public GameObject TRACKCRASH;
        public AudioSource source;

        public AudioClip laugh;
        public AudioClip crack;

        
        public bool first70 = true;

        public static ArcScoreManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            
         BASESCORE=0;
         Score=0;
            ismirrored = false;
            ishard = false;
            CURRENTPERCENT = 0;
            basepos = 0;
            BASEPERCENT = 0;
            fillamount = 0;
            BASEFILL = 0;
            if (LOADMENU.skillvalue == 0)
            {
                CLEARRATE.sprite = GuagetypeNormal;
                guagetypeno = 0;
            }else if (LOADMENU.skillvalue == 1)
            {
                CLEARRATE.sprite = GuagetypeEasy;
                guagetypeno = 1;
            }else if (LOADMENU.skillvalue == 2)
            {
                guagetypeno = 2;
                ishard = true;
                CURRENTPERCENT = 100;
                CLEARRATE.fillAmount = 1;
                PERCENT.text = "100";
                TXTPOS.anchoredPosition = new Vector2(TXTPOS.anchoredPosition.x, (2.88f * CURRENTPERCENT));
                CLEARRATE.sprite = GuagetypeHard;
                goverlay.sprite = overlayhard;
            }
            else if (LOADMENU.skillvalue == 3)
            {
                CLEARRATE.sprite = GuagetypeNormal;
                guagetypeno = 3;
                Guage1.enabled = false;
                Guage2.enabled = false;
                Guage3.enabled = false;
                PRCNT.enabled = false;

            }
            else if (LOADMENU.skillvalue == 4)
            {
                CLEARRATE.sprite = GuagetypeNormal;
                guagetypeno = 4;
                ismirrored = true;
            }

            PUREEORL = SecurePlayerPrefs.GetBool("#PEORL");

            if (!Application.isEditor)
                NAGUAGE = NativeAudio.Load(ArcEffectManager.Instance.RECSOUND);
        }

        public Text ScoreText;

        
        
        public void checkforscore()
        {
            //yield return new WaitForSecondsRealtime(0.0f);
            canpause = false;
            if (PM.enabled == true)
            {           
                Score = 10000000 + PPURES;
                ScoreText.text = "" + Score.ToString("00000000");
                TRCK.Currentsprite.sprite = TRCK.PURE;
                RESULTTHING.SetActive(true);
                TRCK.TRACKANIM.Play("TRACKEND");
                TRCK.TRACKSOUND.clip = TRCK.pure;
                TRCK.TRACKSOUND.Play();
                PUREMEM = true;
            }else if (FR.enabled == true)
            {
                TRCK.Currentsprite.sprite = TRCK.FULL;
                RESULTTHING.SetActive(true);
                TRCK.TRACKANIM.Play("TRACKEND");
                TRCK.TRACKSOUND.clip = TRCK.full;
                TRCK.TRACKSOUND.Play();
                FULLREC = true;
            }
            else if (CLEARRATE.fillAmount >= 0.7||ishard)
            {
                TRCK.Currentsprite.sprite = TRCK.COMPLETE;
                RESULTTHING.SetActive(true);
                TRCK.TRACKANIM.Play("TRACKEND");
                TRCK.TRACKSOUND.clip = TRCK.complete;
                TRCK.TRACKSOUND.Play();
            }
            else if (CLEARRATE.fillAmount < 0.7)
            {
                TRCK.Currentsprite.sprite = TRCK.LOST;
                RESULTTHING.SetActive(true);
                TRCK.TRACKANIM.Play("TRACKEND");
                TRCK.TRACKSOUND.clip = TRCK.lost;
                TRCK.TRACKSOUND.Play();
            }
            clearrate = CLEARRATE.fillAmount;

            //sngnbg.sprite = AdeProjectManager.Instance.CoverImage.sprite;
            

            resultdiff = ArcaoidComposeManager.Instance.Difficulty.text;
            maxpercent = int.Parse(PERCENT.text);
            percentpos = TXTPOS.anchoredPosition;
            
            
            StartCoroutine(RESULTS());

        }

        IEnumerator RESULTS()
        {
            yield return new WaitForSecondsRealtime(4f);
            
            GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().CLOSEresult();
        }
        
        private void Update()
        {
            //if (!ArcGameplayManager.Instance.IsLoaded) return;
            //if (ArcGameplayManager.Instance.Chart == null) return;
            //ScoreText.text = CalculateScore(ArcGameplayManager.Instance.Timing - ArcAudioManager.Instance.AudioOffset).ToString("D8");
            if (ArcTimingManager.readytorun == true && hascalculated == false)
            {
                hascalculated = true;
                BASESCORE = CalculateSingleScore();
                BASEPERCENT = (float)BASEFILL;
                //print(BASESCORE);
                this.Invoke("checkforscore",ArcAudioManager.Instance.Source.clip.length);


                
            }
            if (Time.timeScale != 0)
            {
                visiblescore = Score;
                var tempscr =(double)Score;
                ScoreText.text= "" + tempscr.ToString("00000000");
                CURRENTPERCENT = CLEARRATE.fillAmount*100;
                if (CURRENTPERCENT < 0)
                    CURRENTPERCENT = 0;
                else if (CURRENTPERCENT > 100)
                    CURRENTPERCENT = 100;
                PERCENT.text = Mathf.Round(CURRENTPERCENT).ToString();
                //if(CURRENTPERCENT!=0)
                TXTPOS.anchoredPosition= new Vector2(TXTPOS.anchoredPosition.x,(2.88f*CURRENTPERCENT));

                if (!ishard)
                {
                    if (CLEARRATE.fillAmount >= 0.7)
                    {
                        if (ArcScoreManager.guagetypeno == 0 || ArcScoreManager.guagetypeno == 3 || ArcScoreManager.guagetypeno == 4)
                        {
                            CLEARRATE.color = above70;
                        }
                        else if (guagetypeno == 1) { CLEARRATE.color = above70easy; }
                        if (first70)
                        {
                            first70 = false;
                            if (!Application.isEditor)
                                NAGUAGE.Play(NativeAudio.PlayOptions.guageOptions);
                            else
                                ArcEffectManager.Instance.Source.PlayOneShot(ArcEffectManager.Instance.RECSOUND);
                        }
                    }
                    else
                    {
                        CLEARRATE.color = below70;
                    }
                }

                if (ishard && Time.timeScale != 0)
                {
                    if (CLEARRATE.fillAmount <= 0.3)
                    {
                        if (DYING.activeSelf == false)
                        {
                            DYING.SetActive(true);
                            DYINGGUAGE.SetActive(true);
                        }
                    }
                    else
                    {
                        if (DYING.activeSelf == true)
                        {
                            DYING.SetActive(false);
                            DYINGGUAGE.SetActive(false);
                        }
                    }
                }

                if (ishard && Time.timeScale != 0 && CLEARRATE.fillAmount == 0&&ArcaoidComposeManager.readytoshift&&!didhard)
                {
                    didhard = true;
                    canpause = false;
                    StartCoroutine(TRACKLOST());

                }
                    
            }
        }

        IEnumerator TRACKLOST()
        {
            CancelInvoke();
            Time.timeScale = 0;
            ArcAudioManager.Instance.Pause();
            ArcAudioManager.Instance.SourceACTUAL.Pause();
            TRCK.Currentsprite.sprite = TRCK.LOST;
            yield return new WaitForSecondsRealtime(0.7f);
            source.clip = crack;
            source.Play();
            TRACKCRASH.SetActive(true);
            yield return new WaitForSecondsRealtime(0.2f);
            source.clip = laugh;
            source.Play();
            yield return new WaitForSecondsRealtime(0.5f);
            RESULTTHING.SetActive(true);
            TRCK.TRACKANIM.Play("TRACKEND");
            TRCK.TRACKSOUND.clip = TRCK.lost;
            TRCK.TRACKSOUND.Play();
            clearrate = CLEARRATE.fillAmount;

            resultdiff = ArcaoidComposeManager.Instance.Difficulty.text;
            maxpercent = 0;
            percentpos = TXTPOS.anchoredPosition;
            StartCoroutine(RESULTS());
        }

        bool didhard= false;

        void Calculatejudgehold(Chart.ArcHold hold)
        {
            int total = 0;
            int testotal = 0;
            total += ArcTapNoteManager.Instance.Taps.Count;

            int u = 0;
            double bpm = ArcTimingManager.Instance.CalculateBpmByTiming((int)hold.Timing);
            if (bpm <= 0) return;
            if (hold.Timing == hold.EndTiming) return;
            double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
            int totall = (int)((hold.EndTiming - hold.Timing) / interval);
            if ((u ^ 1) >= totall)
            {
                NOTETOTAl += 1;
                //hold.JudgeTimings.Add((int)(hold.Timing + (hold.EndTiming - hold.Timing) * 0.5f));
                return;
            }
            int n = u ^ 1;
            int t = (int)hold.Timing;
            while (true)
            {
                t = (int)(hold.Timing + n * interval);
                if (t < hold.EndTiming)
                {
                    NOTETOTAl += 1;
                   // hold.JudgeTimings.Add(t);
                }
                if (totall == ++n)
                {
                    break;
                }
            }
        }

        void Calculatejudgearc(Chart.ArcArc arc)
        {
            arc.JudgeTimings.Clear();
            if (arc.IsVoid) return;
            int u = arc.RenderHead ? 0 : 1;
            double bpm = ArcTimingManager.Instance.CalculateBpmByTiming(arc.Timing);
            if (bpm <= 0) return;
            if (arc.Timing == arc.EndTiming ) return;
            double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
            int total = (int)((arc.EndTiming - arc.Timing) / interval);
            if ((u ^ 1) >= total)
            {
                //arc.JudgeTimings.Add((int)(arc.Timing + (arc.EndTiming - arc.Timing) * 0.5f));
                NOTETOTAl += 1;
                return;
            }
            int n = u ^ 1;
            int t = arc.Timing;
            while (true)
            {
                t = (int)(arc.Timing + n * interval);
                if (t < arc.EndTiming)
                {
                    // arc.JudgeTimings.Add(t);
                    NOTETOTAl += 1;
                }
                if (total == ++n) break;
            }
        }

        void Calculatejudgearchold(Chart.ArcArcHold arc)
        {
            arc.JudgeTimings.Clear();
            if (arc.IsVoid) return;
            int u = arc.RenderHead ? 0 : 1;
            double bpm = ArcTimingManager.Instance.CalculateBpmByTiming(arc.Timing);
            if (bpm <= 0) return;
            if (arc.Timing == arc.EndTiming) return;
            double interval = 60000f / bpm / (bpm >= 255 ? 1 : 2);
            int total = (int)((arc.EndTiming - arc.Timing) / interval);
            if ((u ^ 1) >= total)
            {
                //arc.JudgeTimings.Add((int)(arc.Timing + (arc.EndTiming - arc.Timing) * 0.5f));
                NOTETOTAl += 1;
                return;
            }
            int n = u ^ 1;
            int t = arc.Timing;
            while (true)
            {
                t = (int)(arc.Timing + n * interval);
                if (t < arc.EndTiming)
                {
                    // arc.JudgeTimings.Add(t);
                    NOTETOTAl += 1;
                }
                if (total == ++n) break;
            }
        }

        private double CalculateSingleScore()
        {
            NOTETOTAl = 0;
            int total = 0;
            NOTETOTAl += ArcTapNoteManager.Instance.Taps.Count;
            foreach (var hold in ArcHoldNoteManager.Instance.Holds)
            {
                Calculatejudgehold(hold);              
            }
            foreach (var arc in ArcArcManager.Instance.Arcs)
            {
                Calculatejudgearc(arc);
                NOTETOTAl += arc.ArcTaps.Count;
            }
            foreach (var archold in ArcHoldManager.Instance.ArcHolds)
            {
                if (archold.Timing == archold.EndTiming) { }
                else
                {
                    Calculatejudgearchold(archold);
                }
            }
           /* foreach (var hold in ArcHoldNoteManager.Instance.Holds)
            {
                total += hold.JudgeTimings.Count;
            }*/
           /* foreach (var arc in ArcArcManager.Instance.Arcs)
            {
                total += arc.JudgeTimings.Count;
            }*/
            if (NOTETOTAl == 0) return 0;
           // Debug.Log(NOTETOTAl);
            BASEFILL = 80 / (0.457055555556 * (NOTETOTAl));
            fillamount= 0.8f / (0.457055555556f * (NOTETOTAl));
            basepos= 388f / (0.457055555556f * (NOTETOTAl));
            return 10000000d / NOTETOTAl;
        }
        private int CalculateScore(int timing)
        {
            double single = CalculateSingleScore();
            int note = 0;
            foreach (var tap in ArcTapNoteManager.Instance.Taps)
            {
                if (tap.Timing <= timing)
                {
                    note++;
                }
            }
            foreach (var hold in ArcHoldNoteManager.Instance.Holds)
            {
                if (hold.Timing <= timing)
                {
                    foreach (float t in hold.JudgeTimings)
                    {
                        if (t <= timing)
                        {
                            note++;
                        }
                    }
                }
            }
            foreach (var arc in ArcArcManager.Instance.Arcs)
            {
                if (arc.Timing > timing) break;
                foreach (float t in arc.JudgeTimings)
                {
                    if (t <= timing)
                    {
                        note++;
                    }
                }
                foreach (var arctap in arc.ArcTaps)
                {
                    if (arctap.Timing <= timing)
                    {
                        note++;
                    }
                }
            }
            notetotal = note;
            return (int)(note * single + note);
        }
    }
}