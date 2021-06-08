using System.Collections;
using System.IO;
using SecPlayerPrefs;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LOADMENU : MonoBehaviour
{
   public static int currentdiff = 2;
   public static string finalname;
   public static string FOLDERNAME;
   public static string finaldiff;
   public static string composer;
   public static string SLCSONG = " ";
   public AudioSource CURRENTAUDIO;

   public GameObject PARTNERCAN;
   public Image Partner;
   public GameObject ALONE;
   public GameObject PARTNERFKD;
   public Image PARTNERGL1;
   public Image PARTNERGL2;
   public Image PARTNERGL3;
   public GameObject SWITCHPART;



   public Image CURRENTBG;
   public Animator BGCLIP;
   public TextMeshProUGUI CURRENTFTR;
   public TextMeshProUGUI CURRENTPRS;
   public TextMeshProUGUI CURRENTPST;
   public TextMeshProUGUI MAXSCORE;
   public Image MAXGRADE;
   public TextMeshProUGUI CURRENTNAME;
   public TextMeshProUGUI CURRENTCHARTER;
   public TextMeshProUGUI CURRENTILLUS;
   public TextMeshProUGUI CURRENTBPM;
   public Image CURRENTSIDE;
   public static string songside;
   public Sprite CONFLICT;
   public Sprite LIGHT;

   public GameObject FTRSLC;
   public GameObject PRSSLC;
   public GameObject PSTSLC;

   public static float SCROLLSPEED = 1.0f;
   public static float AOFFSET = 0f;
   public TMP_InputField OFFVALUE;

   public static float judgevalue = 0f;
   public TMP_InputField judgeoffset;

   public static bool CLICKENABLED = true;
   public static bool CUSTOMBG = false;

   public TextMeshProUGUI Scrollspeed;
   public TextMeshProUGUI clickenabled;

   public GameObject tutorial;


   public Sprite P;
   public Sprite F;
   public Sprite CLEAR;
   public Sprite CLEARHARD;
   public Sprite LOST;
   public Sprite EX;
   public Sprite AA;
   public Sprite A;
   public Sprite B;
   public Sprite C;
   public Sprite D;
   public Sprite nothing;

   public GameObject songcardPF;
   public Transform LIST;

   public static bool HASPARTNER = false;
   public static bool DEADPARTNER = false;

   public bool HIGHRES;
   public TMP_Dropdown RESDROP;
   public TMP_Dropdown SKILLDROP;

   public GameObject SettingsCanvas;
   public TextMeshProUGUI USERNAME;
   public TextMeshProUGUI placholder;
   public static string usernm = "Guest";
   private bool seentut = false;
   public Text timeplaceholder;
   public InputField INFIELD;
   public Toggle VOIDARCS;
   public Toggle PEORL;
   public Toggle ALTREND;
   public GameObject MORESETTINGS;
   public static int skillvalue;



   public void Tutorial()
   {
      tutorial.SetActive(false);
   }

   public void SwitchRes()
   {
      //print("switching");
      switch (RESDROP.value)
      {
         case 0:
            Screen.SetResolution(LOADDEFAULTS.NATIVEWIDTH, LOADDEFAULTS.NATIVEHEIGHT, true);
            break;
         case 1:
            if (Is16By9)
            {
               Screen.SetResolution(1280, 720, true);
            }
            else if (Is18By9)
            {
               Screen.SetResolution(1440, 720, true);
            }
            else if (Is4By3)
            {
               Screen.SetResolution(960, 720, true);
            }
            else
            {
               Screen.SetResolution(1200, 720, true);
            }
            break;
         case 2:
            if (Is16By9)
            {
               Screen.SetResolution(854, 480, true);
            }
            else if (Is18By9)
            {
               Screen.SetResolution(960, 480, true);
            }
            else if (Is4By3)
            {
               Screen.SetResolution(640, 480, true);
            }
            else
            {
               Screen.SetResolution(800, 480, true);
            }
            break;
         case 3:
            if (Is16By9)
            {
               Screen.SetResolution(640, 360, true);
            }
            else if (Is18By9)
            {
               Screen.SetResolution(720, 360, true);
            }
            else if (Is4By3)
            {
               Screen.SetResolution(480, 360, true);
            }
            else
            {
               Screen.SetResolution(600, 360, true);
            }
            break;
         case 4:
            if (Is16By9)
            {
               Screen.SetResolution(448, 252, true);
            }
            else if (Is18By9)
            {
               Screen.SetResolution(480, 240, true);
            }
            else if (Is4By3)
            {
               Screen.SetResolution(320, 240, true);
            }
            else
            {
               Screen.SetResolution(400, 240, true);
            }
            break;
      }
      SecurePlayerPrefs.SetInt("#RESOLUTION", RESDROP.value);
   }

   public void changeskill()
   {
      switch (SKILLDROP.value)
      {
         case 0:
            skillvalue = 0;
            break;
         case 1:
            skillvalue = 1;
            break;
         case 2:
            skillvalue = 2;
            break;
         case 3:
            skillvalue = 3;
            break;
         case 4:
            skillvalue = 4;
            break;
      }
      SecurePlayerPrefs.SetInt("#SKILL", SKILLDROP.value);

   }

   public bool Is16By9
   {
      get
      {
         return Mathf.Abs((1f * Screen.width / Screen.height) - (16f / 9f)) < 0.1f;
      }
   }

   public bool Is18By9
   {
      get
      {
         return Mathf.Abs((1f * Screen.width / Screen.height) - (18f / 9f)) < 0.2f;
      }
   }

   public bool Is4By3
   {
      get
      {
         return Mathf.Abs((1f * Screen.width / Screen.height) - (4f / 3f)) < 0.1f;
      }
   }

   public void changedpi()
   {
      //HIGHRES = !HIGHRES;
      /*if (TGGLE.isOn)
      {          

          SecurePlayerPrefs.SetBool("#HIGHRES", true);
          Screen.SetResolution(Screen.width, Screen.height, true);

      }
      else if (!TGGLE.isOn)
      {            

          SecurePlayerPrefs.SetBool("#HIGHRES", false);
          if (Is16By9)
          {
              if (Screen.height <= 720)
                  Screen.SetResolution(896, 504, true);
              else
                  Screen.SetResolution(1280, 720, true);
          }
          else if (Is18By9)
          {
              if (Screen.height <= 720)
                  Screen.SetResolution(960, 480, true);
              else
                  Screen.SetResolution(1480, 720, true);
          }
          else
          {
              if (Screen.height <= 720)
                  Screen.SetResolution(640, 480, true);
              else
                  Screen.SetResolution(960, 720, true);
          }
      }*/
   }

   private bool specialcase = false;
   private void Awake()
   {
      var VER = SecurePlayerPrefs.GetString("#VERSION");
      if (VER == "1.0.0")
      {
         SecurePlayerPrefs.SetFloat("TIMESTEP", 0.00833333f);
         SecurePlayerPrefs.SetString("#VERSION", "1.0.7");
      }
      if (VER == "1.0.5")
      {
         SecurePlayerPrefs.SetFloat("AOFFSET", 0);
         specialcase = true;
         SecurePlayerPrefs.SetString("#VERSION", "1.0.7");

      }
      if (VER == "")
      {
         SecurePlayerPrefs.SetFloat("TIMESTEP", 0.00833333f);
         SecurePlayerPrefs.SetString("#VERSION", "1.0.7");
      }
   }

   private IEnumerator Start()
   {

      Time.timeScale = 1;
      currentdiff = 2;
      CLICKENABLED = SecurePlayerPrefs.GetBool("TAPSAC");
      SCROLLSPEED = SecurePlayerPrefs.GetFloat("SCROLLSPEED");
      AOFFSET = SecurePlayerPrefs.GetFloat("AOFFSET");
      judgevalue = SecurePlayerPrefs.GetFloat("#JUDGEOFF");
      HASPARTNER = SecurePlayerPrefs.GetBool("PARTNER");
      DEADPARTNER = SecurePlayerPrefs.GetBool("DEADPARTNER");
      seentut = SecurePlayerPrefs.GetBool("TUTORIAL");

      float TSTEMP = SecurePlayerPrefs.GetFloat("TIMESTEP");
      if (TSTEMP == 0)
      {
         if (Application.platform == RuntimePlatform.Android)
         {
            TSTEMP = 0.015f;
         }
         else
         {
            TSTEMP = 0.00833333f;
         }

         SecurePlayerPrefs.SetFloat("TIMESTEP", TSTEMP);

      }
      Time.fixedDeltaTime = TSTEMP;
      INFIELD.text = Time.fixedDeltaTime.ToString();
      TIMESTEPTXT.text = Time.fixedDeltaTime.ToString();
      timeplaceholder.text = "";



      if (SCROLLSPEED == 0)
      {
         SCROLLSPEED = 1;
         SecurePlayerPrefs.SetFloat("SCROLLSPEED", SCROLLSPEED);
      }
      usernm = SecurePlayerPrefs.GetString("USERNM");
      //print(usernm);
      if (usernm != "")
      {
         USERNAME.text = usernm;
         placholder.text = "";
      }

      if (!seentut)
      {
         tutorial.SetActive(true);
         SecurePlayerPrefs.SetBool("TUTORIAL", true);
      }

      /* HIGHRES = SecurePlayerPrefs.GetBool("#HIGHRES");
       if (HIGHRES == false)
       {
           TGGLE.isOn = false;
       }*/
      if (SecurePlayerPrefs.HasKey("#VOIDARCS")) { } else { SecurePlayerPrefs.SetBool("#VOIDARCS", true); }

      bool shouldvoid = SecurePlayerPrefs.GetBool("#VOIDARCS");
      if (shouldvoid == false)
      {
         VOIDARCS.isOn = false;
      }

      if (VOIDARCS.isOn)
      {
         SecurePlayerPrefs.SetBool("#VOIDARCS", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#VOIDARCS", false);
      }

      bool shouldpeol = SecurePlayerPrefs.GetBool("#PEORL");
      if (shouldpeol == false)
      {
         PEORL.isOn = false;
      }

      if (PEORL.isOn)
      {
         SecurePlayerPrefs.SetBool("#PEORL", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#PEORL", false);
      }

      bool shouldalt = SecurePlayerPrefs.GetBool("#ALTREND");
      if (shouldalt == false)
      {
         ALTREND.isOn = false;
      }

      if (ALTREND.isOn)
      {
         SecurePlayerPrefs.SetBool("#ALTREND", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#ALTREND", false);
      }

      if (SecurePlayerPrefs.HasKey("#SKILL")) { }
      else
      {
         SecurePlayerPrefs.SetInt("#SKILL", 0);
      }
      judgeoffset.text = judgevalue.ToString();

      MORESETTINGS.SetActive(false);
      RESDROP.value = SecurePlayerPrefs.GetInt("#RESOLUTION");
      SKILLDROP.value = SecurePlayerPrefs.GetInt("#SKILL");




      DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
      bool exists = false;
      FileInfo[] allFileees = directoryInfoo.GetFiles("*.png");
      foreach (FileInfo file in allFileees)
      {
         if (file.Name == "partner.png")
         {
            exists = true;
         }
      }
      if (!exists)
      {
         SecurePlayerPrefs.SetBool("PARTNER", false);
         HASPARTNER = false;
      }
      else
      {
         SecurePlayerPrefs.SetBool("PARTNER", true);
         HASPARTNER = true;
      }

      DirectoryInfo[] allFiless = directoryInfoo.GetDirectories("*");
      foreach (DirectoryInfo directory in allFiless)
      {
         var toPath = directory + "/" + Path.GetFileName("ARCAOID.txt");

         if (File.Exists(toPath))
         {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory + "/");
            //print("Streaming Assets Path: " + directoryInfo);
            FileInfo[] allFiles = directoryInfo.GetFiles("ARCAOID.txt");
            foreach (FileInfo file in allFiles)
            {
               //StartCoroutine(songcard(file));
               GameObject SNGCARD = Instantiate(songcardPF, LIST);
               SONGCARD CARD = SNGCARD.GetComponent<SONGCARD>();

               CARD.gameObject.SetActive(false);

               FileInfo[] PFILE = directoryInfo.GetFiles("base.jpg");
               CARD.FOLDNAME = directory.Name;
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

                     UnityWebRequest www = UnityWebRequestTexture.GetTexture(wwwPlayerFilePath, false);
                     www.downloadHandler = new DownloadHandlerBuffer();
                     yield return www.SendWebRequest();


                     /*WWW www = new WWW(wwwPlayerFilePath);
                     yield return www;*/
                     Texture2D texxx;
                     texxx = new Texture2D(512, 512, TextureFormat.RGB24, false);
                     texxx.LoadImage(www.downloadHandler.data);
                     //5
                     CARD.BASE.sprite = Sprite.Create(texxx, new Rect(0.0f, 0.0f, texxx.width, texxx.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
                     //5

                     //BG0.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
                  }
               }
               using (TextReader rdr = file.OpenText())
               {
                  int lineIndex = 0;
                  string line;
                  while ((line = rdr.ReadLine()) != null)
                  {
                     if (lineIndex == 0)
                     {
                        CARD.NAME.text = line;
                     }
                     else if (lineIndex == 1)
                     {
                        CARD.CHARTER = line;
                     }
                     else if (lineIndex == 2)
                     {
                        CARD.ILLUSTRATOR = line;
                     }
                     else if (lineIndex == 3)
                     {
                        CARD.BPM = line;
                     }
                     else if (lineIndex == 4)
                     {
                        CARD.FTR = line;
                     }
                     else if (lineIndex == 5)
                     {
                        CARD.PRS = line;
                     }
                     else if (lineIndex == 6)
                     {
                        CARD.PST = line;
                     }
                     else if (lineIndex == 7)
                     {
                        CARD.SIDE = line;
                     }
                     else if (lineIndex == 8)
                     {
                        CARD.composer = line;
                     }
                     else if (lineIndex == 9)
                     {
                        if (line.Contains("CUSTOMBG=TRUE"))
                        {
                           CARD.CUSTOMBG = true;
                        }
                        else
                        {
                           CARD.CUSTOMBG = false;
                        }
                     }
                     lineIndex++;
                  }
                  rdr.Close();
               }
               CARD.DIFFICULTY.text = CARD.FTR;
               var tempp = SecurePlayerPrefs.GetString("GRADE" + CARD.NAME.text + LOADMENU.currentdiff);
               if (string.IsNullOrEmpty(tempp))
               {
                  CARD.GRADE.sprite = nothing;
               }
               else
               {
                  if (tempp == "EX" || tempp == "P" || tempp == "F")
                  {
                     CARD.GRADE.sprite = EX;
                  }
                  else if (tempp == "AA")
                  {
                     CARD.GRADE.sprite = AA;
                  }
                  else if (tempp == "A")
                  {
                     CARD.GRADE.sprite = A;
                  }
                  else if (tempp == "B")
                  {
                     CARD.GRADE.sprite = B;
                  }
                  else if (tempp == "C")
                  {
                     CARD.GRADE.sprite = C;
                  }
                  else if (tempp == "D")
                  {
                     CARD.GRADE.sprite = D;
                  }
               }
               tempp = null;
               tempp = SecurePlayerPrefs.GetString("CLEAR" + CARD.NAME.text + LOADMENU.currentdiff);
               if (string.IsNullOrEmpty(tempp))
               {
                  CARD.CLEARTYPE.sprite = nothing;
               }
               else
               {
                  if (tempp == "PM")
                  {
                     CARD.CLEARTYPE.sprite = P;
                  }
                  else if (tempp == "FR")
                  {
                     CARD.CLEARTYPE.sprite = F;
                  }
                  else if (tempp == "CLEARHARD")
                  {
                     CARD.CLEARTYPE.sprite = CLEARHARD;
                  }
                  else if (tempp == "CLEAR")
                  {
                     CARD.CLEARTYPE.sprite = CLEAR;
                  }
                  else if (tempp == "CLEAREASY")
                  {
                     CARD.CLEARTYPE.sprite = C;
                  }
                  else if (tempp == "LOST")
                  {
                     CARD.CLEARTYPE.sprite = LOST;
                  }
               }
               if (specialcase == true)
               {
                  var tempgrade = SecurePlayerPrefs.GetString("GRADE" + CARD.NAME.text + 0);
                  if (tempgrade == "P")
                  {
                     SecurePlayerPrefs.SetFloat("MAXSCORE" + CARD.NAME.text + 0, 10000000);
                  }
                  tempgrade = SecurePlayerPrefs.GetString("GRADE" + CARD.NAME.text + 1);
                  if (tempgrade == "P")
                  {
                     SecurePlayerPrefs.SetFloat("MAXSCORE" + CARD.NAME.text + 1, 10000000);
                  }
                  tempgrade = SecurePlayerPrefs.GetString("GRADE" + CARD.NAME.text + 2);
                  if (tempgrade == "P")
                  {
                     SecurePlayerPrefs.SetFloat("MAXSCORE" + CARD.NAME.text + 2, 10000000);
                  }
               }
               /*if (file.Name.Contains("base")&&(file.Extension==(".jpg")))
               {
                   StartCoroutine("LoadPlayerUI", file);

               }
               else if (file.Name.Contains("diff"))
               {
                   StartCoroutine("loaddifficulty", file);
               }
               else if (file.Name.Contains("Illustrator"))
               {
                   StartCoroutine("loadIllustrator", file);
               }*/
               CARD.gameObject.SetActive(true);
            }
         }
      }
      Destroy(songcardPF);

      PARTNERFKD.SetActive(true);
      if (HASPARTNER)
      {
         loadpartner();
         ALONE.SetActive(false);
         if (DEADPARTNER)
         {
            Partner.gameObject.SetActive(false);
            SWITCHPART.SetActive(false);
         }
         else
         {
            PARTNERFKD.SetActive(false);
         }
      }
      else
      {
         PARTNERFKD.SetActive(false);
      }
      PARTNERCAN.SetActive(false);
      Resources.UnloadUnusedAssets();
      System.GC.Collect();

      try
      {
         GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().OPEN();
      }
      catch
      {

      }
      if (finalname != null)
      {
         //GameObject.Find(finalname + finalname).GetComponent<SONGCARD>().ONCLICKRETURN();
      }
        NativeGallery.Permission result = NativeGallery.RequestPermission(true);
        

    }

   public void openpartner()
   {
      SettingsCanvas.SetActive(false);
      PARTNERCAN.SetActive(true);
   }

   public void openMOAH()
   {
      SettingsCanvas.SetActive(false);
      MORESETTINGS.SetActive(true);

   }

   public void BACC()
   {
      MORESETTINGS.SetActive(false);
      SettingsCanvas.SetActive(true);
   }

   public void closepartner()
   {
      PARTNERCAN.SetActive(false);
   }

   public void disablevoids()
   {
      bool shouldvoid = SecurePlayerPrefs.GetBool("#VOIDARCS");
      if (shouldvoid == false)
      {
         SecurePlayerPrefs.SetBool("#VOIDARCS", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#VOIDARCS", false);
      }
   }

   public void disablepeol()
   {
      bool shouldpeol = SecurePlayerPrefs.GetBool("#PEORL");
      if (shouldpeol == false)
      {
         SecurePlayerPrefs.SetBool("#PEORL", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#PEORL", false);
      }
   }

   public void disablealt()
   {
      bool shouldalt = SecurePlayerPrefs.GetBool("#ALTREND");
      if (shouldalt == false)
      {
         SecurePlayerPrefs.SetBool("#ALTREND", true);
      }
      else
      {
         SecurePlayerPrefs.SetBool("#ALTREND", false);
      }
   }

   private IEnumerator LoadPlayerUI(FileInfo playerFile)
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
         Partner.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
         PARTNERGL1.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
         PARTNERGL2.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
         PARTNERGL3.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 1);

         //Sprite.Create(www.texture, new Rect(0,0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
      }
   }

   public Text TIMESTEPTXT;

   private void loadpartner()
   {
      DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
      FileInfo[] allFiles = directoryInfoo.GetFiles("*.png");
      foreach (FileInfo file in allFiles)
      {
         if (file.Name == "partner.png")
         {
            StartCoroutine("LoadPlayerUI", file);
         }
      }
   }

   private void PickImage(int maxSize)
   {
      NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           //Debug.Log("Image path: " + path);
           if (path != null)
          {
              // Create Texture from selected image
              Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
             if (texture == null || texture.width != 540 || texture.height != 720)
             {
                 //Debug.Log("Couldn't load texture from " + path);
                 return;
             }



             Partner.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
             PARTNERGL1.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
             PARTNERGL2.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
             PARTNERGL3.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f, 1);
             SecurePlayerPrefs.SetBool("PARTNER", true);
             HASPARTNER = true;
             ALONE.SetActive(false);
             if (DEADPARTNER)
             {
                Partner.gameObject.SetActive(false);
                SWITCHPART.SetActive(false);
                PARTNERFKD.SetActive(true);
             }
              print("oki!");
              SaveTextureToFile(texture, Application.persistentDataPath + "/" + "partner.png");

          }
       }, "Select a PNG image", "image/png", maxSize);

      Debug.Log("Permission result: " + permission);

   }

   public void chooseimage()
   {


      PickImage(2048);
   }

   private void SaveTextureToFile(Texture2D texture, string filename)
   {
      System.IO.File.WriteAllBytes(filename, texture.EncodeToPNG());
   }

   public void ENABLETAPS()
   {

      //SecurePlayerPrefs.
      if (!CLICKENABLED)
      {
         CLICKENABLED = true;
         clickenabled.text = "OFF";
         SecurePlayerPrefs.SetBool("TAPSAC", true);
      }
      else
      {
         CLICKENABLED = false;
         clickenabled.text = "ON";
         SecurePlayerPrefs.SetBool("TAPSAC", false);
      }
   }

   public void LOADCURRENTS(SONGCARD CARD)
   {
      CURRENTFTR.text = CARD.FTR;
      CURRENTPRS.text = CARD.PRS;
      CURRENTPST.text = CARD.PST;
      CURRENTNAME.text = CARD.NAME.text;
      CURRENTCHARTER.text = "Charter: " + CARD.CHARTER;
      CURRENTILLUS.text = "Illustrator: " + CARD.ILLUSTRATOR;
      CURRENTBPM.text = "BPM: " + CARD.BPM;
      CURRENTBG.sprite = CARD.BASE.sprite;
      if (CARD.SIDE == "CONFLICT")
      {
         CURRENTSIDE.sprite = CONFLICT;
      }
      else
      {
         CURRENTSIDE.sprite = LIGHT;
      }
      BGCLIP.Play("BGSPIN");
      //BGCLIP.Play("LOL");


   }
   public void SWITCHFTR()
   {
      currentdiff = 2;
      FTRSLC.SetActive(true);
      PRSSLC.SetActive(false);
      PSTSLC.SetActive(false);
      BGCLIP.Play("BGSPIN");

   }
   public void SWITCHPRS()
   {
      currentdiff = 1;
      FTRSLC.SetActive(false);
      PRSSLC.SetActive(true);
      PSTSLC.SetActive(false);
      BGCLIP.Play("BGSPIN");
   }
   public void SWITCHPST()
   {
      currentdiff = 0;
      FTRSLC.SetActive(false);
      PRSSLC.SetActive(false);
      PSTSLC.SetActive(true);
      BGCLIP.Play("BGSPIN");
   }

   public void opensettings()
   {
      SettingsCanvas.SetActive(true);
      if (CLICKENABLED == true)
      {
         clickenabled.text = "OFF";
      }
      else
      {
         clickenabled.text = "ON";
      }

      Scrollspeed.text = SCROLLSPEED.ToString("F1");
      OFFVALUE.text = AOFFSET.ToString();

   }
   public string lmao;
   public void OFF_SET()
   {
      lmao = OFFVALUE.text;
      AOFFSET = float.Parse(lmao);
      SecurePlayerPrefs.SetFloat("AOFFSET", AOFFSET);
      //print(AOFFSET);
      //print("awt!");
   }

   public string lol;
   public void JUDGE_SET()
   {
      lol = judgeoffset.text;
      judgevalue = float.Parse(lol);
      SecurePlayerPrefs.SetFloat("#JUDGEOFF", judgevalue);
      // print(judgevalue);
   }

   public void OFFUP()
   {

      AOFFSET += 0.01f;
      SecurePlayerPrefs.SetFloat("AOFFSET", AOFFSET);
      OFFVALUE.text = AOFFSET.ToString("F2");
   }


   public void OFFDOWN()
   {

      AOFFSET -= 0.01f;
      SecurePlayerPrefs.SetFloat("AOFFSET", AOFFSET);
      OFFVALUE.text = AOFFSET.ToString("F2");
   }

   public void Increment()
   {
      if (SCROLLSPEED < 6.5f)
      {
         SCROLLSPEED += 0.1f;
      }

      SecurePlayerPrefs.SetFloat("SCROLLSPEED", SCROLLSPEED);
      Scrollspeed.text = SCROLLSPEED.ToString("F1");
   }

   public void Decrement()
   {
      if (SCROLLSPEED > 1.0f)
      {
         SCROLLSPEED -= 0.1f;
      }

      SecurePlayerPrefs.SetFloat("SCROLLSPEED", SCROLLSPEED);
      Scrollspeed.text = SCROLLSPEED.ToString("F1");
   }

   public void closesettings()
   {
      SettingsCanvas.SetActive(false);
   }

   public void closeMOAH()
   {
      MORESETTINGS.SetActive(false);
   }

   // Update is called once per frame
   private void Update()
   {
      if (MAXSCORE.text == "")
      {
         MAXSCORE.text = "00,000,000";
      }
   }

   public IEnumerator songcard(FileInfo INFOfile)
   {
      yield return new WaitForSeconds(0.0f);
      using (TextReader rdr = INFOfile.OpenText())
      {
         int lineIndex = 0;
         string line;
         while ((line = rdr.ReadLine()) != null)
         {
            if (lineIndex == 0)
            {
               //moneyo = Int.Parse(line);
            }
            else if (lineIndex == 1)
            {
               //workedhrs = Int.Parse(line);
            }
            lineIndex++;
         }
         rdr.Close();
      }
   }



}
