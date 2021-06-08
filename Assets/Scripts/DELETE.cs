using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DELETE : MonoBehaviour {
    public GameObject confirmcanvas;
    public GameObject cantdelete;
    public GameObject restartcanvas;
    public GameObject settingscanvas;
    public LOADMENU LDMENU;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Deletesong(){
        settingscanvas.SetActive(false);
        if(LDMENU.CURRENTNAME.text=="In Your Mind"||LDMENU.CURRENTNAME.text=="Unfitting Facade"||LDMENU.CURRENTNAME.text == "Overload")
        {
            settingscanvas.SetActive(false);
            cantdelete.SetActive(true);
        }else{
            settingscanvas.SetActive(false);
        confirmcanvas.SetActive(true);
       }
    }
    public void closecan(){
        cantdelete.SetActive(false);
    }
    public void yes(){
        confirmcanvas.SetActive(false);
        if(Application.platform==RuntimePlatform.IPhonePlayer){
            System.IO.Directory.Delete("/private" + Application.persistentDataPath+"/"+LOADMENU.FOLDERNAME,true);
            restartcanvas.SetActive(true);
        } else if(Application.platform==RuntimePlatform.Android||Application.platform==RuntimePlatform.WindowsEditor){
            System.IO.Directory.Delete(Application.persistentDataPath+"/"+LOADMENU.FOLDERNAME,true);
            restartcanvas.SetActive(true);
        }
        StartCoroutine(TOTITLE());
    }

    public IEnumerator TOTITLE()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        Destroy(GameObject.FindGameObjectWithTag("SHUTTER"));
        SceneManager.LoadScene("Title");
    }

    public void no(){
        confirmcanvas.SetActive(false);
    }
}
