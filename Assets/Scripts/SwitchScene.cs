using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    public RectTransform UP;
    public RectTransform DOWN;
    public RectTransform LEFT;
    public RectTransform RIGHT;
    public AudioSource AU;
    public AudioClip OPENED;
    public AudioClip CLOSED;
    public Image TTS;
    public SHUTTERSNG SNGSHUTTER;
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate=120;
    }

    public void REM()
    {
        TTS.enabled = false;
    }

   public void CLOSE()
    {
        //StartCoroutine(OPENROUTINE());
        UP.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        AU.PlayOneShot(CLOSED);
        StartCoroutine(LevelLoad("Menu"));

    }

    public void CLOSEresult()
    {
        //StartCoroutine(OPENROUTINE());
        UP.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        AU.PlayOneShot(CLOSED);
        StartCoroutine(LevelLoad("Result"));
    }

    public void Nextresult(string LVL)
    {
        //StartCoroutine(OPENROUTINE());
        UP.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        AU.PlayOneShot(CLOSED);
        StartCoroutine(LevelLoad(LVL));
    }

    public void CLOSENormal()
    {
        //StartCoroutine(OPENROUTINE());
        UP.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        AU.PlayOneShot(CLOSED);
      

    }

    public void CLOSEMENU()
    {
        //StartCoroutine(OPENROUTINE());
        UP.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(0, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(0, 0.6f).SetUpdate(true);
        AU.PlayOneShot(CLOSED);
        SNGSHUTTER.SNGOBJS.SetActive(true);
        /*SNGSHUTTER.BG.CrossFade("SONGBGFADE", 0f);
        SNGSHUTTER.BG.Update(0f);
        SNGSHUTTER.BG.Update(0f);*/
        
        LOADMENU ldmenu = GameObject.FindGameObjectWithTag("LDMENU").GetComponent<LOADMENU>();
       SNGSHUTTER.SNGBG.sprite = ldmenu.CURRENTBG.sprite;
        SNGSHUTTER.SNGNAME.text = ldmenu.CURRENTNAME.text;
        SNGSHUTTER.COMPOSER.text = "Music: "+LOADMENU.composer;
        SNGSHUTTER.CHARTER.text = ldmenu.CURRENTCHARTER.text;
        SNGSHUTTER.BG.Play("SONGBGFADE");
        SNGSHUTTER.TXT1.Play("TEXTFADE1");
        SNGSHUTTER.TXT2.Play("TEXTFADE2");
        SNGSHUTTER.TXT3.Play("TEXTFADE3");
       StartCoroutine(LevelLoad("ArcPlayer"));

    }


    public void OPEN()
    {
        UP.DOAnchorPosY(1876, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(-1876, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(2602, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(-2602, 0.6f).SetUpdate(true);     
        AU.PlayOneShot(OPENED);
    }

    public void OPENMENU()
    {
        SNGSHUTTER.SNGOBJS.SetActive(false);
        UP.DOAnchorPosY(1876, 0.6f).SetUpdate(true);
        DOWN.DOAnchorPosY(-1876, 0.6f).SetUpdate(true);
        RIGHT.DOAnchorPosX(2602, 0.6f).SetUpdate(true);
        LEFT.DOAnchorPosX(-2602, 0.6f).SetUpdate(true);
        AU.PlayOneShot(OPENED);
    }

    [SerializeField] private float delay = 2f;

    //function to be called on button click
    public void LoadNextLevel(string name)
    {
        StartCoroutine(LevelLoad(name));
    }

    //load level after one sceond delay
    IEnumerator LevelLoad(string name)
    {
        if (name != "Result")
        {
            Time.timeScale = 1;
        }
        else
        {
            yield return new WaitForSecondsRealtime(2f);
            Time.timeScale = 1;
        }

        float elapsedTime = 0;
        float currentVolume = AudioListener.volume;

        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(currentVolume, 0, elapsedTime / delay);
            yield return null;
        }
        AudioSource[] audios = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach(AudioSource AUU in audios)
        {
            AUU.Stop();
        }
        AudioListener.volume = 1;
        if (name == "ArcPlayer")
        {
            yield return new WaitForSecondsRealtime(2.0f);
        }
        SceneManager.LoadScene(name);
    }

    IEnumerator OPENROUTINE()
    {
        yield return new WaitForSecondsRealtime(0.0f);
        var top = Mathf.Lerp(UP.rect.yMin, -85.95001f, Time.deltaTime);
        var bottom = Mathf.Lerp(UP.rect.yMax, -117.95f, Time.deltaTime);

        UP.rect.Set(top,bottom,UP.rect.width,UP.rect.height);
        if(UP.rect.yMin== -85.95001f&&UP.rect.yMax== -117.95f)
        {

        }
        else
        {
            StartCoroutine(OPENROUTINE());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
