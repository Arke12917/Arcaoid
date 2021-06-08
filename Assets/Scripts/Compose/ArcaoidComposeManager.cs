using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Schwarzer.Windows;
using Arcaoid.Gameplay;
using Arcaoid.Aff;
using DG.Tweening;
using Newtonsoft.Json;
//using Arcaoid.Compose.Dialog;
using Arcaoid.Compose.Editing;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;
using Arcaoid.Gameplay.Chart;
using UnityEngine.SceneManagement;
using SecPlayerPrefs;
using TMPro;

namespace Arcaoid.Compose
{
    [Serializable]
    public class ArcadePreference
    {
        public int AgreedUserAgreementVersion;
        public long ReadWhatsNewVersion;
        public int DropRate= 16;
        public bool Auto;
        public Arcaoid.Gameplay.Chart.ChartSortMode ChartSortMode;
    }

    public class ArcaoidComposeManager : MonoBehaviour
    {
        public static ArcaoidComposeManager Instance { get; private set; }
        public const float EditorModeGameplayCameraScale = 0.9f;
        public const float ModeSwitchDuration = 0.0f;
        public const float BarSizeRatio = 1.381f;
        public const Ease ToEditorModeEase = Ease.OutCubic;
        public const Ease ToPlayerModeEase = Ease.InCubic;
        public GameObject noterb;
        public static bool firstloaded = true;
        public static bool readytoshift = false;
        public GameObject PAUSECANVAS;
        public Text Difficulty;
        public Text Songname;
        public Text Composer;
        public GameObject Getready;
        public Animator GETREADY;
        
        

        public static string ArcadePersistentFolder
        {
            get
            {
                string returndat = null;
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {


                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Arcade"))
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Arcade");
                    returndat= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Arcade";
                }else if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                {
                    returndat = Application.persistentDataPath + "/" + "Arcaoid";
                }
                return returndat;
            }
        }
        public string PreferencesSavePath
        {
            get
            {
                return ArcadePersistentFolder + "/Preferences.json";
            }
        }

        public static bool IsEditorMode { get; set; } = true;
        public float BarHeight
        {
            get
            {
                return IsEditorMode ? TopBar.sizeDelta.y : 0;
            }
        }
        public float BarWidth
        {
            get
            {
                return IsEditorMode ? LeftBar.sizeDelta.x : 0;
            }
        }

        public Camera GameplayCamera, EditorCamera;
        public ArcGameplayManager GameplayManager;
        [Header("Bar")]
        public RectTransform TopBar;
        public RectTransform BottomBar, LeftBar, RightBar, Bars;
        public RectTransform TopBarView, BottomBarView, LeftBarView, RightBarView;
        [Header("Pause")]
        public Button PauseButton;
        public Image PauseButtonImage;
        public Sprite PausePause, PausePlay, PausePausePressed, PausePlayPressed;
        [Header("Info")]
        public CanvasGroup InfoCanvasGroup;
        public Image TimingSliderHandle;
        public Sprite DefaultSliderSprite, GlowSliderSprite;
        [Header("Auto")]
        public Button AutoButton;

        public UnityEvent OnPlay = new UnityEvent();
        public UnityEvent OnPause = new UnityEvent();
        public ArcadePreference ArcadePreference = new ArcadePreference();
        public Text Version;

        private bool switchingMode = false;
        private int playShotTiming = 0;
        public Rigidbody rb;
        

        private void Awake()
        {
            

            Instance = this;
            //Version.text = ArcadeBuildInfo.BuildString;
            Application.targetFrameRate = 120;
            //Time.timeScale = 0;
            firstloaded = true;
        readytoshift = false;
        IsEditorMode = false;

        }
        private void Start()
        {
            //ArcGameplayManager.Instance.OnMusicFinished.AddListener(Pause);
            //LoadPreferences();
            //
            Pause();
           rb = noterb.GetComponent<Rigidbody>();
            float tempsped = (16.6666666667f * LOADMENU.SCROLLSPEED);
            ArcTimingManager.Instance.DropRate =Mathf.RoundToInt(tempsped);
            print(ArcTimingManager.Instance.DropRate);
            AdeProjectManager.Instance.OpenProject();
         
        }

        public void RESTART()
        {
            ArcScoreManager.MAXRECALL = 0;
            ArcScoreManager.MAXPURES = 0;
            ArcScoreManager.PPURES = 0;
            ArcScoreManager.EPURES = 0;
            ArcScoreManager.LPURES = 0;
            ArcScoreManager.EFARS = 0;
            ArcScoreManager.LFARS = 0;
            ArcScoreManager.MAXFARS = 0;
            ArcScoreManager.MAXLOSTS = 0;
            GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().CLOSENormal();
            StartCoroutine(Loadarcplayer());
        }
       public void QUIT()
        {
            ArcScoreManager.MAXRECALL = 0;
            ArcScoreManager.MAXPURES = 0;
            ArcScoreManager.PPURES = 0;
            ArcScoreManager.EPURES = 0;
            ArcScoreManager.LPURES = 0;
            ArcScoreManager.EFARS = 0;
            ArcScoreManager.LFARS = 0;
            ArcScoreManager.MAXFARS = 0;
            ArcScoreManager.MAXLOSTS = 0;
            GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().CLOSENormal();
            StartCoroutine(Loadmenu());
        }
       
        IEnumerator Loadarcplayer()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            SceneManager.LoadScene("ArcPlayer");
        }

        IEnumerator Loadmenu()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            SceneManager.LoadScene("Menu");
        }

        private void OnEnable()
        {
            Application.logMessageReceived += OnLog;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= OnLog;
        }
        private void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;
           
        }

        public void Play()
        {
            if (AdeProjectManager.Instance.CurrentProject == null || !ArcGameplayManager.Instance.IsLoaded)
            {
                //AdeToast.Instance.Show("No Chart Loaded!");
                return;
            }

            

            //if (switchingMode) return;
            switchingMode = true;

           
            
            //float border = (1 - EditorModeGameplayCameraScale) / 2;
           //float width = border * 2048 * BarSizeRatio;
            //float height = border * 1536 * BarSizeRatio * EditorCamera.pixelHeight / (EditorCamera.pixelWidth * 0.75f);
            //TopBar.sizeDelta = new Vector2(0, height);
            //BottomBar.sizeDelta = new Vector2(0, height);
            // LeftBar.sizeDelta = new Vector2(width, 0);
            //RightBar.sizeDelta = new Vector2(width, 0);
            //TopBarView.sizeDelta = new Vector2(0, height / BarSizeRatio);
            // BottomBarView.sizeDelta = new Vector2(0, height / BarSizeRatio);
            //LeftBarView.sizeDelta = new Vector2(width / BarSizeRatio, 0);
            // RightBarView.sizeDelta = new Vector2(width / BarSizeRatio, 0);
            // TopBar.DOAnchorPosY(height, ModeSwitchDuration).SetEase(ToPlayerModeEase);
            // BottomBar.DOAnchorPosY(-height, ModeSwitchDuration).SetEase(ToPlayerModeEase);
            //LeftBar.DOAnchorPosX(-width, ModeSwitchDuration).SetEase(ToPlayerModeEase);
            // RightBar.DOAnchorPosX(width, ModeSwitchDuration).SetEase(ToPlayerModeEase).OnComplete(() => { Bars.gameObject.SetActive(false); switchingMode = false; });
            //Bars.gameObject.SetActive(false);
            switchingMode = false;
            
            GameplayCamera.DORect(new Rect(0, 0, 1, 1), ModeSwitchDuration).SetEase(ToPlayerModeEase);

            PauseButtonImage.sprite = PausePause;
            PauseButton.spriteState = new SpriteState() { pressedSprite = PausePausePressed };
            //InfoCanvasGroup.interactable = false;

            TimingSliderHandle.sprite = GlowSliderSprite;

            //AdeClickToCreate.Instance.CancelAddLongNote();
            //AdeClickToCreate.Instance.Mode = ClickToCreateMode.Idle;
            var total = 0;
            foreach (var hold in ArcHoldNoteManager.Instance.Holds)
            {
                total += hold.JudgeTimings.Count;
             
            }
            //print(total);
            if (firstloaded == true)
            {
                firstloaded = false;
                readytoshift = true;      
                startshifting();
            }
            //noterb.GetComponent<Rigidbody>().velocity= new Vector3(0, 0, GameObject.FindGameObjectWithTag("TimingManager").GetComponent<ArcTimingManager>().CurrentSpeed);
            
            IsEditorMode = false;
            if (firststart)
            {
                firststart = false;
                StartCoroutine(STARTO());
            }
            else
            {
                Time.timeScale = 1;
                GameplayManager.Play();
            }
            //ArcTimingManager.Instance.isplaying = false;
            //ArcTimingManager.readytorun = false;
        }

        public bool firststart = true;

        public IEnumerator STARTO()
        {
            yield return new WaitForSecondsRealtime(0.0f);
            ArcScoreManager.Instance.canpause = false;
            if (Mathf.Sign(LOADMENU.AOFFSET) == -1)
            {
                ArcAudioManager.Instance.SourceACTUAL.Play();
                yield return new WaitForSecondsRealtime(Mathf.Abs(LOADMENU.AOFFSET));
                Time.timeScale = 1;
                GameplayManager.Playother();
               
            }                       
            if (Mathf.Sign(LOADMENU.AOFFSET) == 1)
            {
                Time.timeScale = 1;                           
                GameplayManager.PlayDelayed(Mathf.Abs(LOADMENU.AOFFSET));
            }
            StartCoroutine(chartwait());
        }

        public IEnumerator chartwait()
        {
            yield return new WaitForSecondsRealtime(2.0f);
            ArcScoreManager.Instance.canpause = true;
        }
        
        public void startshifting()
        {
            // var Timings = ArcTimingManager.Instance.Timings;     
            /* foreach (ArcTiming t in Timings)
             {

                 var SPED = t.Bpm / ArcTimingManager.Instance.BaseBpm;
                 //print("Timing!");
                 StartCoroutine(BPMSHIFTS(SPED, t.Timing));


             }*/
            //print("can begin!");
            ArcTimingManager.readytorun = true;
            //ArcArcManager.Instance.CalculateArcRel();
            ArcTimingManager.Instance.finishedloading = true;

        }

        public void nextshifting(float shifttime,float thetime)
        {
           // StartCoroutine(BPMSHIFTS(shifttime,thetime));
        }

        public IEnumerator BPMSHIFTS(float shifttime,int thetime)
        {
            //print(thetime);
           // Rigidbody rb = noterb.GetComponent<Rigidbody>();
            yield return new WaitUntil(() =>readytoshift == true);
           //yield return new WaitForSeconds(Mathf.Abs(thetime/1000f));
           // yield return new WaitUntil(() => ArcGameplayManager.Instance.Timing == thetime);
            rb.velocity = new Vector3(0, 0, shifttime*(ArcTimingManager.Instance.DropRate-0.08f));
            //print(rb.velocity);
            
        }

        private void Update()
        {
            if (readytoshift)
            {
                // noterb.transform.mov(new Vector3(0, 0, ArcTimingManager.Instance.CurrentSpeed));
                // noterb.transform.Translate(new Vector3(0,0,(ArcTimingManager.Instance.CurrentSpeed*100f)*ArcTimingManager.Instance.DropRate*Time.deltaTime));
                //Vector3 trans = new Vector3(0, 0, ArcTimingManager.Instance.CurrentSpeed * (ArcTimingManager.Instance.DropRate)) * Time.deltaTime;
                //noterb.transform.Translate(trans);
                //print(trans);
                /*if (Mathf.Approximately(999.99f, ArcTimingManager.Instance.CurrentSpeed))
                {
                    print("aight");
                    //noterb.transform.Translate(new Vector3(0, 0, (99999.00f) * ArcTimingManager.Instance.DropRate));
                    //rb.velocity = new Vector3(0, 0, 35000.00f);
                    rb.velocity = new Vector3(0, 0, (17f*ArcTimingManager.Instance.DropRate));
                    print(rb.velocity);
                    //print(ArcTimingManager.Instance.CurrentSpeed);
                }else if (Mathf.Approximately(8.15f, ArcTimingManager.Instance.CurrentSpeed))
                {
                    print("errm");
                    rb.velocity = new Vector3(0, 0, (17f*ArcTimingManager.Instance.DropRate));
                    print(rb.velocity);
                    //print(ArcTimingManager.Instance.CurrentSpeed);
                }
                else
                {*/
                if (ArcAudioManager.Instance.AudioOffset!=0)
                {
                    if (ArcTimingManager.Instance.Timings == null) return;
                    ///if (ArcTimingManager.Instance.Timings.Count == 0) return;
                    if (!setspeedonce)
                    {
                        setspeedonce = true;
                        lastbpm = ArcTimingManager.Instance.Timings[0].Bpm;
                    }                   
                    if ((ArcTimingManager.Instance.CURRENTBPM == lastbpm)||!ArcTimingManager.Instance.startedupdating||ArcTimingManager.Instance.Timings.Count==1)
                    {
                        rb.velocity = new Vector3(0, 0, ((lastbpm / ArcTimingManager.Instance.BaseBpm) * (ArcTimingManager.Instance.DropRate)));
                    }
                    else
                    {
                        //rb.velocity = new Vector3(0, 0, ((lastbpm / ArcTimingManager.Instance.BaseBpm) * (ArcTimingManager.Instance.DropRate)));
                        rb.velocity = new Vector3(0, 0, (ArcTimingManager.Instance.CurrentSpeed * (ArcTimingManager.Instance.DropRate)));
                    }
                    
                }
                else
                {
                    rb.velocity = new Vector3(0, 0, (ArcTimingManager.Instance.CurrentSpeed * (ArcTimingManager.Instance.DropRate)));
                }
                
                   //print(ArcTimingManager.Instance.CurrentSpeed);
                   //print(rb.velocity);
                   // print(1010 / ArcTimingManager.Instance.DropRate);
                   //}                            
                   //print((ArcTimingManager.Instance.CurrentSpeed * 100f) * ArcTimingManager.Instance.DropRate * Time.deltaTime);
            }
        }
        bool setspeedonce = false;
        public float lastbpm;
        public void Pause()
        {
           
            //if (switchingMode) return;
            switchingMode = true;
            Time.timeScale = 0;
            GameplayManager.Pause();
            //float border = (1 - EditorModeGameplayCameraScale) / 2;
            //Bars.gameObject.SetActive(true);
            //float width = border * 2048 * BarSizeRatio;
            //float height = border * 1536 * BarSizeRatio * EditorCamera.pixelHeight / (EditorCamera.pixelWidth * 0.75f);
           /* TopBar.sizeDelta = new Vector2(0, height);
            BottomBar.sizeDelta = new Vector2(0, height);
            LeftBar.sizeDelta = new Vector2(width, 0);
            RightBar.sizeDelta = new Vector2(width, 0);
            TopBarView.sizeDelta = new Vector2(0, height / BarSizeRatio);
            BottomBarView.sizeDelta = new Vector2(0, height / BarSizeRatio);
            LeftBarView.sizeDelta = new Vector2(width / BarSizeRatio, 0);
            RightBarView.sizeDelta = new Vector2(width / BarSizeRatio, 0);
            TopBar.DOAnchorPosY(0, ModeSwitchDuration).SetEase(ToEditorModeEase);
            BottomBar.DOAnchorPosY(0, ModeSwitchDuration).SetEase(ToEditorModeEase);
            LeftBar.DOAnchorPosX(0, ModeSwitchDuration).SetEase(ToEditorModeEase);
            RightBar.DOAnchorPosX(0, ModeSwitchDuration).SetEase(ToEditorModeEase).OnComplete(() => { switchingMode = false; });*/
            //GameplayCamera.DORect(new Rect(border, border, EditorModeGameplayCameraScale, EditorModeGameplayCameraScale), ModeSwitchDuration).SetEase(ToEditorModeEase);

            PauseButtonImage.sprite = PausePlay;
            PauseButton.spriteState = new SpriteState() { pressedSprite = PausePlayPressed };
            InfoCanvasGroup.interactable = true;

            TimingSliderHandle.sprite = DefaultSliderSprite;

            //noterb.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            IsEditorMode = true;
            
            //ArcTimingManager.readytorun = true;
        }

        public void OnPauseClicked()
        {

            //Time.timeScale=0;
            if(ArcScoreManager.Instance.canpause)
            StartCoroutine(ONPAUSED());
            
        }

        public GameObject skillcanvas;
        public GameObject BALANCE;
        public GameObject SUPPORT;
        public GameObject CHALLENGE;
        public Image partnerim;
        public Sprite partnerdef;
        public Animator canvasfade;
        public Animator partnershift;
        public TextMeshProUGUI skilldesc;
        public AudioClip Skillfx;
        bool shownskil = false;


        public IEnumerator ONPAUSED()
        {
            ArcGameplayManager.Instance.ResetJudge();

            if (IsEditorMode)
            {
                PAUSECANVAS.SetActive(false);
                if (!shownskil)
                {
                    shownskil = true;
                    if (ArcScoreManager.guagetypeno == 0) { }
                    else 
                    {
                        skillcanvas.SetActive(true);
                        bool haspart= SecurePlayerPrefs.GetBool("PARTNER");
                        ArcScoreManager.Instance.source.clip = Skillfx;
                        if (haspart==true)
                        {
                            //print("then wy");
                            DirectoryInfo directoryInfoa = new DirectoryInfo(Application.persistentDataPath);
                            FileInfo[] allFilea = directoryInfoa.GetFiles("*.png");
                            foreach (FileInfo file in allFilea)
                            {
                                if (file.Name == "partner.png")
                                {
                                    //print("ok?");
                                    StartCoroutine(LoadPlayerUI(file));
                                }
                            }
                            yield return new WaitUntil(() => partnerim.sprite!= partnerdef);
                        }
                        
                        if (ArcScoreManager.guagetypeno == 1)
                        {
                            BALANCE.SetActive(false);
                            SUPPORT.SetActive(true);
                            skilldesc.text = "EASY-Gauge loss rate reduced per LOST";
                        }
                        else if (ArcScoreManager.guagetypeno == 2)
                        {
                            BALANCE.SetActive(false);
                            CHALLENGE.SetActive(true);
                            skilldesc.text = "HARD-Track Lost when Gauge reaches 0%";
                        }
                        else if (ArcScoreManager.guagetypeno == 3)
                        {
                            skilldesc.text = "VISUAL-Clear Gauge is hidden";
                        }
                        else if (ArcScoreManager.guagetypeno == 4)
                        {
                            skilldesc.text = "MIRROR-All Notes and Arcs flipped";
                        }
                        canvasfade.Play("skillfade");
                        partnershift.Play("partnershift");
                        ArcScoreManager.Instance.source.Play();
                        yield return new WaitForSecondsRealtime(1.5f);
                        skillcanvas.SetActive(false);
                    }               

                }
                Resources.UnloadUnusedAssets();
                Getready.SetActive(true);
                GETREADY.Play("READY");
                yield return new WaitForSecondsRealtime(2.0f);
                Getready.SetActive(false);
                Play();
                if (AdeProjectManager.Instance.CurrentProject.AudioClip == null)
                {

                }
                else
                {
                    
                    ArcTimingManager.Instance.isplaying = true;
                    ArcTimingManager.Instance.firstclicked = false;
                }

            }
            else { Pause(); ArcTimingManager.Instance.isplaying = false; PAUSECANVAS.SetActive(true); Resources.UnloadUnusedAssets(); }
        }


        IEnumerator LoadPlayerUI(FileInfo playerFile)
        {
            yield return new WaitForSeconds(0.0f);
            //1
            if (playerFile.Name.Contains("meta"))
            {
                yield break;
            }
            //2
            else
            {
                string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(playerFile.ToString());
                //3

                //4
                string wwwPlayerFilePath = "file://" + playerFile.FullName.ToString();
                WWW www = new WWW(wwwPlayerFilePath);
                yield return www;
                Texture2D tex;
                tex = new Texture2D(540, 720, TextureFormat.RGB24, false);
                www.LoadImageIntoTexture(tex);
                //5
                partnerim.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
                //print("nani?");
            }
        }

        public void OnAutoClicked()
        {
            ArcGameplayManager.Instance.Auto = !ArcGameplayManager.Instance.Auto;
            ArcGameplayManager.Instance.ResetJudge();
            AutoButton.image.color = ArcGameplayManager.Instance.Auto ? new Color(0.59f, 0.55f, 0.65f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1);
        }
        public void OnShutdownClicked()
        {
            Application.Quit();
        }

        public void LoadPreferences()
        {
            try
            {
                ArcadePreference = JsonConvert.DeserializeObject<ArcadePreference>(File.ReadAllText(PreferencesSavePath));
            }
            catch (Exception)
            {
                ArcadePreference = new ArcadePreference();
            }
            finally
            {
                ArcTimingManager.Instance.DropRate = ArcadePreference.DropRate;
                ArcGameplayManager.Instance.Auto = ArcadePreference.Auto;
                AdeProjectManager.Instance.SaveMode.text = ArcadePreference.ChartSortMode == Gameplay.Chart.ChartSortMode.Timing ? "Time" : "Type";
                AutoButton.image.color = ArcGameplayManager.Instance.Auto ? new Color(0.59f, 0.55f, 0.65f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1);
                
            }
        }
        public void SavePreferences()
        {
            ArcadePreference.DropRate = ArcTimingManager.Instance.DropRate;
            ArcadePreference.Auto = ArcGameplayManager.Instance.Auto;
            File.WriteAllText(PreferencesSavePath, JsonConvert.SerializeObject(ArcadePreference));
        }

        private void OnApplicationQuit()
        {
            SavePreferences();
        }
    }
}