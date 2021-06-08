using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;
using SecPlayerPrefs;
using System.Globalization;

public class SONGCARD : MonoBehaviour
{
    public LOADMENU LDMNU;

    public Image BASE;
    public GameObject TOPDIFFFTR;
    public GameObject TOPDIFFPRS;
    public GameObject TOPDIFFPST;
    public TextMeshProUGUI DIFFICULTY;
    public Image CLEARTYPE;
    public Image GRADE;
    public TextMeshProUGUI NAME;
    public GameObject SLCTDBG;
    public string CHARTER;
    public string ILLUSTRATOR;
    public string BPM;
    public string FTR;
    public string PRS;
    public string PST;
    public string FOLDNAME;
    public AudioClip SONG;
    public string SIDE="LIGHT";
    public bool CUSTOMBG = false;

    public string composer;

    public bool didset = false;

    private void OnEnable()
    {
        if (NAME.text == LOADMENU.finalname)
        {
            OnClickReturn();
        }
    }

    void ChangeMainScore()
    {
        if (LOADMENU.SLCSONG == NAME.text)
        {
            LDMNU.MAXGRADE.sprite = GRADE.sprite;
            var toemp = (int)SecurePlayerPrefs.GetFloat("MAXSCORE" + NAME.text + LOADMENU.currentdiff);
            var teemp = toemp;
            var tomp = teemp.ToString();
            if (string.IsNullOrEmpty(tomp))
            {
                LDMNU.MAXSCORE.text = "00,000,000";
            }
            else
            {

                tomp = string.Format("{00000000:#,#}", teemp);
                LDMNU.MAXSCORE.text = tomp;
                if (LDMNU.MAXSCORE.text == "0")
                {
                    LDMNU.MAXSCORE.text = "00,000,000";
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.name = NAME.text;
        if (LOADMENU.SLCSONG == NAME.text)
        {
            if (SLCTDBG.activeSelf == false)
            {
                SLCTDBG.SetActive(true);
                LOADMENU.FOLDERNAME = FOLDNAME;
                if (!didset)
                {
                    didset = true;
                    ChangeMainScore();
                }
            }
        }
        else
        {
            if (SLCTDBG.activeSelf == true)
            {
                SLCTDBG.SetActive(false);
                didset = false;
            }
        }
        if (LOADMENU.currentdiff == 0)
        {
            if (TOPDIFFPST.activeSelf == false)
            {
                TOPDIFFPST.SetActive(true);
                TOPDIFFPRS.SetActive(false);
                TOPDIFFFTR.SetActive(false);
                DIFFICULTY.text = PST;
                var tempp = SecurePlayerPrefs.GetString("GRADE" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    GRADE.sprite =LDMNU.nothing;
                }
                else
                {
                    if (tempp == "EX" || tempp == "P" || tempp == "F")
                    {
                        GRADE.sprite = LDMNU.EX;
                    }
                    else if (tempp == "AA")
                    {
                        GRADE.sprite = LDMNU.AA;
                    }
                    else if (tempp == "A")
                    {
                        GRADE.sprite = LDMNU.A;
                    }
                    else if (tempp == "B")
                    {
                        GRADE.sprite = LDMNU.B;
                    }
                    else if (tempp == "C")
                    {
                        GRADE.sprite = LDMNU.C;
                    }
                    else if (tempp == "D")
                    {
                        GRADE.sprite = LDMNU.D;
                    }
                }
                tempp = null;
                tempp = SecurePlayerPrefs.GetString("CLEAR" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    CLEARTYPE.sprite = LDMNU.nothing;
                }
                else
                {
                    if (tempp == "PM")
                    {
                        CLEARTYPE.sprite = LDMNU.P;
                    }
                    else if (tempp == "FR")
                    {
                        CLEARTYPE.sprite = LDMNU.F;
                    }
                    else if (tempp == "CLEARHARD")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEARHARD;
                    }
                    else if (tempp == "CLEAR")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEAR;
                    }
                    else if (tempp == "CLEAREASY")
                    {
                        CLEARTYPE.sprite = LDMNU.C;
                    }
                    else if (tempp == "LOST")
                    {
                        CLEARTYPE.sprite = LDMNU.LOST;
                    }
                }
                if (LOADMENU.SLCSONG == NAME.text)
                {
                    LDMNU.MAXGRADE.sprite = GRADE.sprite;
                    var toemp = (int)SecurePlayerPrefs.GetFloat("MAXSCORE" + NAME.text + LOADMENU.currentdiff);
                    var teemp = (double)toemp;
                    var tomp = teemp.ToString();
                    if (string.IsNullOrEmpty(tomp))
                    {
                        LDMNU.MAXSCORE.text = "00,000,000";
                    }
                    else
                    {

                        tomp = string.Format("{00000000:#,#}", teemp);
                        LDMNU.MAXSCORE.text = tomp;
                        if (LDMNU.MAXSCORE.text == "0")
                        {
                            LDMNU.MAXSCORE.text = "00,000,000";
                        }
                    }
                }
            }
           
        }else if (LOADMENU.currentdiff == 1)
        {
            if (TOPDIFFPRS.activeSelf == false)
            {
                TOPDIFFPST.SetActive(false);
                TOPDIFFPRS.SetActive(true);
                TOPDIFFFTR.SetActive(false);
                DIFFICULTY.text = PRS;
                var tempp = SecurePlayerPrefs.GetString("GRADE" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    GRADE.sprite = LDMNU.nothing;
                }
                else
                {
                    if (tempp == "EX" || tempp == "P" || tempp == "F")
                    {
                        GRADE.sprite = LDMNU.EX;
                    }
                    else if (tempp == "AA")
                    {
                        GRADE.sprite = LDMNU.AA;
                    }
                    else if (tempp == "A")
                    {
                        GRADE.sprite = LDMNU.A;
                    }
                    else if (tempp == "B")
                    {
                        GRADE.sprite = LDMNU.B;
                    }
                    else if (tempp == "C")
                    {
                        GRADE.sprite = LDMNU.C;
                    }
                    else if (tempp == "D")
                    {
                        GRADE.sprite = LDMNU.D;
                    }
                }
                
                tempp = null;
                tempp = SecurePlayerPrefs.GetString("CLEAR" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    CLEARTYPE.sprite = LDMNU.nothing;
                }
                else
                {
                    if (tempp == "PM")
                    {
                        CLEARTYPE.sprite = LDMNU.P;
                    }
                    else if (tempp == "FR")
                    {
                        CLEARTYPE.sprite = LDMNU.F;
                    }
                    else if (tempp == "CLEARHARD")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEARHARD;
                    }
                    else if (tempp == "CLEAR")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEAR;
                    }
                    else if (tempp == "CLEAREASY")
                    {
                        CLEARTYPE.sprite = LDMNU.C;
                    }
                    else if (tempp == "LOST")
                    {
                        CLEARTYPE.sprite = LDMNU.LOST;
                    }
                }
                if (LOADMENU.SLCSONG == NAME.text)
                {
                    LDMNU.MAXGRADE.sprite = GRADE.sprite;
                    var toemp = (int)SecurePlayerPrefs.GetFloat("MAXSCORE" + NAME.text + LOADMENU.currentdiff);
                    var teemp = (double)toemp;
                    var tomp = teemp.ToString();
                    if (string.IsNullOrEmpty(tomp))
                    {
                        LDMNU.MAXSCORE.text = "00,000,000";
                    }
                    else
                    {

                        tomp = string.Format("{00000000:#,#}", teemp);
                        LDMNU.MAXSCORE.text = tomp;
                        if (LDMNU.MAXSCORE.text == "0")
                        {
                            LDMNU.MAXSCORE.text = "00,000,000";
                        }
                    }
                }
            }
        }
        else if (LOADMENU.currentdiff == 2)
        {
            if (TOPDIFFFTR.activeSelf == false)
            {
                TOPDIFFPST.SetActive(false);
                TOPDIFFPRS.SetActive(false);
                TOPDIFFFTR.SetActive(true);
                DIFFICULTY.text = FTR;
                var tempp = SecurePlayerPrefs.GetString("GRADE" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    GRADE.sprite = LDMNU.nothing;
                }
                else
                {
                    if (tempp == "EX" || tempp == "P" || tempp == "F")
                    {
                        GRADE.sprite = LDMNU.EX;
                    }
                    else if (tempp == "AA")
                    {
                        GRADE.sprite = LDMNU.AA;
                    }
                    else if (tempp == "A")
                    {
                        GRADE.sprite = LDMNU.A;
                    }
                    else if (tempp == "B")
                    {
                        GRADE.sprite = LDMNU.B;
                    }
                    else if (tempp == "C")
                    {
                        GRADE.sprite = LDMNU.C;
                    }
                    else if (tempp == "D")
                    {
                        GRADE.sprite = LDMNU.D;
                    }
                }
                tempp = null;
                tempp = SecurePlayerPrefs.GetString("CLEAR" + NAME.text + LOADMENU.currentdiff);
                if (string.IsNullOrEmpty(tempp))
                {
                    CLEARTYPE.sprite = LDMNU.nothing;
                }
                else
                {
                    if (tempp == "PM")
                    {
                        CLEARTYPE.sprite = LDMNU.P;
                    }
                    else if (tempp == "FR")
                    {
                        CLEARTYPE.sprite = LDMNU.F;
                    }
                    else if (tempp == "CLEARHARD")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEARHARD;
                    }
                    else if (tempp == "CLEAR")
                    {
                        CLEARTYPE.sprite = LDMNU.CLEAR;
                    }
                    else if (tempp == "CLEAREASY")
                    {
                        CLEARTYPE.sprite = LDMNU.C;
                    }
                    else if (tempp == "LOST")
                    {
                        CLEARTYPE.sprite = LDMNU.LOST;
                    }
                }
                if (LOADMENU.SLCSONG == NAME.text)
                {
                    LDMNU.MAXGRADE.sprite = GRADE.sprite;
                    var toemp = (int)SecurePlayerPrefs.GetFloat("MAXSCORE" + NAME.text + LOADMENU.currentdiff);
                    var teemp = (double)toemp;
                    var tomp = teemp.ToString();
                    if (string.IsNullOrEmpty(tomp))
                    {
                        LDMNU.MAXSCORE.text = "00,000,000";
                    }
                    else
                    {

                        tomp = string.Format("{00000000:#,#}", teemp);
                        LDMNU.MAXSCORE.text = tomp;
                        if (LDMNU.MAXSCORE.text == "0")
                        {
                            LDMNU.MAXSCORE.text = "00,000,000";
                        }
                    }
                }
            }
        }
    }

    public void OnClickReturn()
    {
        DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] allFiless = directoryInfoo.GetDirectories(FOLDNAME);
        foreach (DirectoryInfo directory in allFiless)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directory + "/");
            var EXT = "*base";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (File.Exists(directory + "/" + "base.ogg"))
                {
                    EXT = "*.ogg";
                }
                else
                {
                    EXT = "*.wav";
                }

            }
            else
            {
                if (File.Exists(directory + "/" + "base.ogg"))
                {
                    EXT = "*.ogg";
                }
                else
                {
                    EXT = "*.wav";
                }
            }

            FileInfo[] allFiles = directoryInfo.GetFiles(EXT);
            foreach (FileInfo file in allFiles)
            {
                StartCoroutine(LoadSoundAsync(file));
            }

        }
        LOADMENU.SLCSONG = NAME.text;
        LDMNU.LOADCURRENTS(this);
    }

    public void OnClick()
    {
        if (SLCTDBG.activeSelf == true)
        {
            if ((LOADMENU.currentdiff == 2 && LDMNU.CURRENTFTR.text == "?") || (LOADMENU.currentdiff == 1 && LDMNU.CURRENTPRS.text == "?") || (LOADMENU.currentdiff == 0 && LDMNU.CURRENTPST.text == "?"))
            {

            }
            else
            {
                LOADMENU.finalname = NAME.text;
                LOADMENU.finaldiff = DIFFICULTY.text;
                LOADMENU.composer = composer;
                LOADMENU.FOLDERNAME = FOLDNAME;
                LOADMENU.songside = SIDE;
                LOADMENU.CUSTOMBG = CUSTOMBG;
                if(LDMNU.USERNAME.text!="")
                SecurePlayerPrefs.SetString("USERNM", LDMNU.USERNAME.text);
               
                Time.fixedDeltaTime = float.Parse(LDMNU.TIMESTEPTXT.text, new CultureInfo("en-US").NumberFormat);
                SecurePlayerPrefs.SetFloat("TIMESTEP", Time.fixedDeltaTime);
                GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().CLOSEMENU();
            }
        }
        else
        {
            DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
            //print("Streaming Assets Path: " + Application.persistentDataPath);
            DirectoryInfo[] allFiless = directoryInfoo.GetDirectories(FOLDNAME);
            foreach (DirectoryInfo directory in allFiless)
            {
                
                DirectoryInfo directoryInfo = new DirectoryInfo(directory + "/");
                //print("Streaming Assets Path: " + directoryInfo);
                var EXT = "*base";
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (File.Exists(directory + "/" + "base.ogg"))
                    {
                        EXT = "*.ogg";
                    }
                    else
                    {
                        EXT = "*.wav";
                    }
                    
                }
                else
                {
                    if (File.Exists(directory + "/" + "base.ogg"))
                    {
                        EXT = "*.ogg";
                    }
                    else
                    {
                        EXT = "*.wav";
                    }
                }
             
                FileInfo[] allFiles = directoryInfo.GetFiles(EXT);
                foreach (FileInfo file in allFiles)
                {
                    StartCoroutine(LoadSoundAsync(file));
                }

            }                           
            LOADMENU.SLCSONG = NAME.text;

            if (LOADMENU.SLCSONG == NAME.text)
            {
                LDMNU.MAXGRADE.sprite = GRADE.sprite;
                var toemp = (int)SecurePlayerPrefs.GetFloat("MAXSCORE" + NAME.text + LOADMENU.currentdiff);
                var teemp = (double)toemp;
                var tomp = teemp.ToString();
                if (string.IsNullOrEmpty(tomp))
                {
                    LDMNU.MAXSCORE.text = "00,000,000";
                }
                else
                {

                    tomp = string.Format("{00000000:#,#}", teemp);
                    LDMNU.MAXSCORE.text = tomp;
                    if (LDMNU.MAXSCORE.text == "0")
                    {
                        LDMNU.MAXSCORE.text = "00,000,000";
                    }
                }
            }

            LDMNU.LOADCURRENTS(this);        
        }
    }

    public IEnumerator LoadSoundAsync(FileInfo file)
    {
      using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + file.FullName, AudioType.OGGVORBIS))
      {
         yield return www.SendWebRequest();
         LDMNU.CURRENTAUDIO.clip = DownloadHandlerAudioClip.GetContent(www);
         LDMNU.CURRENTAUDIO.Play();
      }
    }
}
