using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CREDITS : MonoBehaviour
{
    public GameObject CREDCANVAS;
    public GameObject privacytxt;
    public GameObject credtxt;
    public GameObject privbutton;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void opencanvas()
    {
        CREDCANVAS.SetActive(true);
    }

    public void closecanvas()
    {
        privacytxt.SetActive(false);
        credtxt.SetActive(true);
        privbutton.SetActive(true);
        CREDCANVAS.SetActive(false);
    }

    public void privacyopen()
    {
        privbutton.SetActive(false);
        credtxt.SetActive(false);
        privacytxt.SetActive(true);
    }
 
    public void GetInvite()
    {
        //Application.OpenURL("https://discord.gg/R7P55f8");
        Application.OpenURL("https://arcaoid.xyz/chart");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
