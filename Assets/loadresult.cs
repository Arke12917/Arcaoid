using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SecPlayerPrefs;
using TMPro;
using Arcaoid.Compose;
using Arcaoid.Gameplay;
using System.Linq;
using System.IO;
using System.Globalization;
public class loadresult : MonoBehaviour
{
    public Color above70;
    public Color above70easy;
    public Color below70;

    public Sprite P;
    public Sprite F;
    public Sprite EX;
    public Sprite AA;
    public Sprite A;
    public Sprite B;
    public Sprite C;
    public Sprite D;

    public Sprite PM;
    public Sprite FR;
    public Sprite CLEAR;
    public Sprite LOST;

    public TextMeshProUGUI PLRNAME;
    public TextMeshProUGUI SONG;
    public TextMeshProUGUI Cmposer;
    public TextMeshProUGUI SCORE;
    public TextMeshProUGUI MAXSCORE;
    public TextMeshProUGUI SCRDIFF;
    public Image CLEARTEXT;
    public Image CLEARGRADE;
    public Image SNGBG;
    public TextMeshProUGUI DIFF;
    public TextMeshProUGUI MAXRECALL;
    public TextMeshProUGUI PURES;
    public TextMeshProUGUI FARS;
    public TextMeshProUGUI LOSTS;
    public Image CLEARGUAGE;
    public Image GOVERLAY;
    public Sprite ClearGuageNormal;
    public Sprite ClearGuageEasy;
    public Sprite ClearGuageHard;
    public Sprite overlayhard;
    public Text PERVAL;
    public AudioSource ASOURCE;
    public AudioSource GSOURCE;
    public AudioClip CLEARED;
    public AudioClip CRASHED;

    public Image BGG;
    public GameObject CRACK;
    public GameObject LOBJ;
    public Image lostgrade;
    public Color norm;
    public Color crack;
    public GameObject PARTNER;
    public GameObject PARTNER_LOST;

    public Image partner;
    public Image partnerGL1;
    public Image partnerGL2;
    public Image partnerGL3;
    
    public Animator TRCK;
    public Animator GRD;

    public GameObject PEOLOBJ;
    public TextMeshProUGUI PPURES;
    public TextMeshProUGUI EarlyLateInfo;
    public Animator EORLANIM;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Time.timeScale = 1;
        var usernm = SecurePlayerPrefs.GetString("USERNM");

        CLEARGUAGE.fillAmount = ArcScoreManager.clearrate;
        //print(usernm);
        if (usernm != "")
        {
            PLRNAME.text = usernm;        
        }


        SONG.text = LOADMENU.finalname;
        Cmposer.text = LOADMENU.composer;
        SCORE.text = string.Format("{00000000:#,#}", ArcScoreManager.Score);
        if (SCORE.text == "")
        {
            SCORE.text = "00,000,000";
        }
        var tempgrade= SecurePlayerPrefs.GetString("GRADE"+LOADMENU.finalname+LOADMENU.currentdiff);
        var tempclear= SecurePlayerPrefs.GetString("CLEAR"+LOADMENU.finalname+LOADMENU.currentdiff);
        string currentgrad="D";
        if (LOADMENU.skillvalue == 0||LOADMENU.skillvalue==3||LOADMENU.skillvalue==4)
        {
            CLEARGUAGE.sprite = ClearGuageNormal;
        }
        else if(LOADMENU.skillvalue==1) { CLEARGUAGE.sprite = ClearGuageEasy; }else if(LOADMENU.skillvalue==2)
        {
            CLEARGUAGE.sprite = ClearGuageHard;
            GOVERLAY.sprite = overlayhard;
        }
        if (ArcScoreManager.PUREMEM)
        {
            currentgrad = "P";
            CLEARTEXT.sprite = PM;
            CLEARGRADE.sprite = P;
            if (tempgrade != "P")
            {
                SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "P");
                SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "PM");
            }
            CLEARGUAGE.color = above70;
            ASOURCE.clip = CLEARED;
            if (LOADMENU.HASPARTNER&&(ArcScoreManager.MAXFARS+ArcScoreManager.MAXLOSTS+ArcScoreManager.MAXPURES)>150)
            {
                SecurePlayerPrefs.SetBool("DEADPARTNER", false);
                LOADMENU.DEADPARTNER = false;
            }
            else
            {
                //LOADMENU.DEADPARTNER = true;
            }
        }
        else if (ArcScoreManager.FULLREC)
        {
            currentgrad = "F";
            CLEARTEXT.sprite = FR;
            CLEARGRADE.sprite = F;
            if (tempgrade != "P"&&tempgrade!="F")
            {
                SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "F");
                SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "FR");
            }
            CLEARGUAGE.color = above70;
            ASOURCE.clip = CLEARED;
            if (LOADMENU.HASPARTNER && (ArcScoreManager.MAXFARS + ArcScoreManager.MAXLOSTS + ArcScoreManager.MAXPURES) > 150)
            {
                SecurePlayerPrefs.SetBool("DEADPARTNER", false);
                LOADMENU.DEADPARTNER = false;
            }
            else
            {
                //LOADMENU.DEADPARTNER = true;
            }
        }
        else
        {
            //CLEARTEXT.sprite = CLEAR;
            
            if (ArcScoreManager.Score >= 9800000)
            {
                currentgrad = "EX";
                CLEARGRADE.sprite = EX;
                if (tempgrade != "P" && tempgrade != "F"&&tempgrade!="EX")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "EX");
                     
                }
                if (LOADMENU.HASPARTNER && (ArcScoreManager.MAXFARS + ArcScoreManager.MAXLOSTS + ArcScoreManager.MAXPURES) > 150)
                {
                    SecurePlayerPrefs.SetBool("DEADPARTNER", false);
                    LOADMENU.DEADPARTNER = false;
                }
                else
                {
                    //LOADMENU.DEADPARTNER = true;
                }
            }
            else if (ArcScoreManager.Score >= 9500000)
            {
                currentgrad = "AA";
                CLEARGRADE.sprite = AA;
                if (tempgrade != "P" && tempgrade != "F" && tempgrade != "EX"&&tempgrade!="AA")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "AA");
                     
                }
            }
            else if (ArcScoreManager.Score >= 9200000)
            {
                currentgrad = "A";
                CLEARGRADE.sprite = A;
                if (tempgrade != "P" && tempgrade != "F" && tempgrade != "EX" && tempgrade != "AA"&&tempgrade!="A")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "A");
                     
                }
            }
            else if (ArcScoreManager.Score >= 8900000)
            {
                currentgrad = "B";
                CLEARGRADE.sprite = B;
                if (tempgrade != "P" && tempgrade != "F" && tempgrade != "EX" && tempgrade != "AA" && tempgrade != "A"&&tempgrade!="B")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "B");
                     
                }
            }
            else if (ArcScoreManager.Score >= 8600000)
            {
                currentgrad = "C";
                CLEARGRADE.sprite = C;
                if (tempgrade != "P" && tempgrade != "F" && tempgrade != "EX" && tempgrade != "AA" && tempgrade != "A" && tempgrade != "B"&&tempgrade!="C")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "C");
                     
                }
            }
            else
            {
                //Lmao what a noob, there's a real surprise for you!
                currentgrad = "D";
                CLEARGRADE.sprite = D;             
                if (tempgrade != "P" && tempgrade != "F" && tempgrade != "EX" && tempgrade != "AA" && tempgrade != "A" && tempgrade != "B" && tempgrade != "C"&&tempgrade!="D")
                {
                    SecurePlayerPrefs.SetString("GRADE" + LOADMENU.finalname + LOADMENU.currentdiff, "D");
                    
                }
            }
            

            if (CLEARGUAGE.fillAmount >= 0.7||(LOADMENU.skillvalue==2&&CLEARGUAGE.fillAmount!=0))
            {
                //print("clear?");
                if (LOADMENU.skillvalue == 2 == false)
                {
                    if (tempclear != "PM" && tempclear != "FR")
                    {
                        if (LOADMENU.skillvalue == 0 || LOADMENU.skillvalue == 3 || LOADMENU.skillvalue == 4 && tempclear != "CLEARHARD")
                        {
                            SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "CLEAR");
                        }
                        else if (LOADMENU.skillvalue == 1 && tempclear != "CLEAR" && tempclear != "CLEARHARD")
                        {
                            SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "CLEAREASY");
                        }
                    }
                }
                else {
                    if (tempclear != "PM" && tempclear != "FR")
                    {
                        SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "CLEARHARD");
                    }
                }
                if(LOADMENU.skillvalue==2==false)
                CLEARGUAGE.color = above70;
                ASOURCE.clip = CLEARED;
            }
            else
            {
                if (tempclear != "PM" && tempclear != "FR"&&tempclear!="CLEAR" && tempclear != "CLEAREASY" && tempclear != "CLEARHARD")
                    SecurePlayerPrefs.SetString("CLEAR" + LOADMENU.finalname + LOADMENU.currentdiff, "LOST");
                CLEARGUAGE.color = below70;
                BGG.color = crack;
                CRACK.SetActive(true);
                LOBJ.SetActive(true);
                lostgrade.sprite = CLEARGRADE.sprite;
                CLEARGRADE.gameObject.SetActive(false);
                CLEARTEXT.gameObject.SetActive(false);
                PARTNER.SetActive(false);
                PARTNER_LOST.SetActive(true);
                ASOURCE.clip = CRASHED;
                if(LOADMENU.HASPARTNER)
                SecurePlayerPrefs.SetBool("DEADPARTNER",true);
            }

        }

        if (LOADMENU.HASPARTNER)
        {
            PARTNER.SetActive(true);
            PARTNER_LOST.SetActive(true);
            DirectoryInfo directoryInfoa = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] allFilea = directoryInfoa.GetFiles("*.png");
            foreach (FileInfo file in allFilea)
            {
                if (file.Name == "partner.png")
                {
                    //print("ok?");
                    StartCoroutine("LoadPlayerUI", file);
                }
            }
        }
        if (LOADMENU.HASPARTNER)
        {


        }
        else
        {
            PARTNER_LOST.SetActive(false);
        }

        var temp= SecurePlayerPrefs.GetFloat("MAXSCORE"+LOADMENU.finalname + LOADMENU.currentdiff);

        if (temp == 0)
        {
            var tempint = ArcScoreManager.Score;
            SecurePlayerPrefs.SetFloat("MAXSCORE" + LOADMENU.finalname + LOADMENU.currentdiff, (float)tempint);
            MAXSCORE.text = "0";
            SCRDIFF.text = "+"+string.Format("{00000000:#,#}", ArcScoreManager.Score);
        }
        else
        {
            if (temp < ArcScoreManager.Score)
            {
                //var tempint = int.Parse(SCORE.text, System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                SecurePlayerPrefs.SetFloat("MAXSCORE" + LOADMENU.finalname + LOADMENU.currentdiff, (float)ArcScoreManager.Score);
                MAXSCORE.text= string.Format("{00000000:#,#}", temp);
                SCRDIFF.text= "+" + string.Format("{00000000:#,#}", ArcScoreManager.Score-temp);
            }
            else if (temp > ArcScoreManager.Score)
            {
                MAXSCORE.text = string.Format("{00000000:#,#}", temp);
                SCRDIFF.text = "-" + string.Format("{00000000:#,#}", temp-ArcScoreManager.Score);
            }
        }

        //SNGBG.sprite = ArcScoreManager.sngnbg.sprite;
        DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] allFiless = directoryInfoo.GetDirectories(LOADMENU.FOLDERNAME);
        foreach (DirectoryInfo directory in allFiless)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directory + "/");
            //print("Streaming Assets Path: " + directoryInfo);
            FileInfo[] PFILE = directoryInfo.GetFiles("base.jpg");
            foreach (FileInfo playerFile in PFILE)
            {


                //moneyo = Int.Parse(line);
                if (playerFile.Name.Contains("meta"))
                {

                }
                //2
                else
                {
                    string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(playerFile.ToString());
                    string[] playerNameData = playerFileWithoutExtension.Split(" "[0]);
                    //3
                    string tempSongName = "";
                    int i = 0;
                    foreach (string stringFromFileName in playerNameData)
                    {
                        if (i != 0)
                        {
                            tempSongName = tempSongName + stringFromFileName + " ";
                        }
                        i++;
                    }
                    //4
                    string wwwPlayerFilePath = "file://" + playerFile.FullName.ToString();
                    WWW www = new WWW(wwwPlayerFilePath);
                    yield return www;
                    Texture2D texxx;
                    texxx = new Texture2D(512, 512, TextureFormat.RGB24, false);
                    www.LoadImageIntoTexture(texxx);
                    //5
                    SNGBG.sprite = Sprite.Create(texxx, new Rect(0.0f, 0.0f, texxx.width, texxx.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
                    //5

                    //BG0.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                }
            }

        }
        DIFF.text = ArcScoreManager.resultdiff;
        MAXRECALL.text = ArcScoreManager.MAXRECALL.ToString();
        PURES.text = ArcScoreManager.MAXPURES.ToString();
        PPURES.text = "+" + ArcScoreManager.PPURES.ToString();
        EarlyLateInfo.text = "L" + ArcScoreManager.LFARS + "(P" + ArcScoreManager.LPURES + ") E" + ArcScoreManager.EFARS + "(P" + ArcScoreManager.EPURES + ")";
        FARS.text = ArcScoreManager.MAXFARS.ToString();
        LOSTS.text = ArcScoreManager.MAXLOSTS.ToString();
       
        PERVAL.text = ArcScoreManager.maxpercent.ToString();
        PERVAL.rectTransform.anchoredPosition =  new Vector2(PERVAL.rectTransform.anchoredPosition.x, (2.37f * CLEARGUAGE.fillAmount*100));
        ASOURCE.loop = true;
        ASOURCE.Play();
        GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().OPEN();
        TRCK.Play("TRACKCLEAR");
        GRD.Play("CLEARG");
        yield return new WaitForSecondsRealtime(1.9f);
        if(CLEARGUAGE.fillAmount>=0.7)
        GSOURCE.PlayOneShot(Gradsound);


    }

    public AudioClip Gradsound;

    public void enableEORLinfo()
    {
        if (PEOLOBJ.activeSelf == false)
        {
            PEOLOBJ.SetActive(true);
            EORLANIM.Play("EORL");
        }
    }

    public void TOMENU()
    {
        ArcScoreManager.clearrate = 0;
        ArcScoreManager.sngnbg = null;
        ArcScoreManager.resultdiff = null;
        ArcScoreManager.MAXRECALL = 0;
        ArcScoreManager.MAXPURES = 0;
        ArcScoreManager. PPURES=0;
    ArcScoreManager. EPURES=0;
    ArcScoreManager. LPURES=0;
    ArcScoreManager. EFARS=0;
    ArcScoreManager. LFARS=0;
    ArcScoreManager.MAXFARS = 0;
        ArcScoreManager.MAXLOSTS = 0;
        ArcScoreManager.maxpercent = 0;
        ArcScoreManager.percentpos = Vector2.zero;
        ArcScoreManager.PUREMEM = false;
        ArcScoreManager.FULLREC = false;
        GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().Nextresult("Menu");
    }

    public void RTRY()
    {
        ArcScoreManager.clearrate = 0;
        ArcScoreManager.sngnbg = null;
        ArcScoreManager.resultdiff = null;
        ArcScoreManager.MAXRECALL = 0;
        ArcScoreManager.MAXPURES = 0;
        ArcScoreManager.PPURES = 0;
        ArcScoreManager.EPURES = 0;
        ArcScoreManager.LPURES = 0;
        ArcScoreManager.EFARS = 0;
        ArcScoreManager.LFARS = 0;
        ArcScoreManager.MAXFARS = 0;
        ArcScoreManager.MAXLOSTS = 0;
        ArcScoreManager.maxpercent = 0;
        ArcScoreManager.percentpos = Vector2.zero;
        ArcScoreManager.PUREMEM = false;
        ArcScoreManager.FULLREC = false;
        GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().Nextresult("ArcPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        
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
            partner.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
            partnerGL1.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
            partnerGL2.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
            partnerGL3.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
            //print("ok???");
            bool ded = SecurePlayerPrefs.GetBool("DEADPARTNER");
            if (ded||CLEARGUAGE.fillAmount<0.7f)
            {
                PARTNER_LOST.SetActive(true);
                PARTNER.SetActive(false);
            }
            else
            {
                PARTNER.SetActive(true);
                PARTNER_LOST.SetActive(false);
            }
            //Sprite.Create(www.texture, new Rect(0,0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        }
    }

}
