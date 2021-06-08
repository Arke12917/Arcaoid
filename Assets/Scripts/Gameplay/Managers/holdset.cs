using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Lean.Touch;

public class holdset : MonoBehaviour {
    public BoxCollider COL;
    public bool disabled=false;
    public bool ishitting = false;
    public bool iscolliding=false;
    public bool isrunning=false;
    public LeanFinger finger;
    public int TRACK=-1;
    public LeanFingerHeld LFH;
    
	// Use this for initialization



    private void OnTriggerExit(Collider other)
    {
       
        if (other.tag == "HoldNote")
        {
           // print(":0");
            ishitting = false;
            
            if (ishitting == true)
            {
                iscolliding = true;
                //StopCoroutine(longdisable());
                ishitting = false;
            }
            else
            {
                iscolliding = false;
            }
           
        }
    }

    private void OnDisable()
    {
      
     disabled = false;
    ishitting = false;
     iscolliding = false;
     isrunning = false;
     finger=null;
     TRACK = -1;
}

    private void OnEnable()
    {
        StartCoroutine(disable());
        //print("startroutine");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HoldNote")
        {

            StopAllCoroutines();
            shrinkhold sh = other.GetComponent<shrinkhold>();
            sh.didhit = true;
            
                if (!(ReferenceEquals(sh, null)))
                {
                    if (!(ReferenceEquals(sh.HOLD, null)))
                        TRACK = sh.HOLD.Track;
                }
            
                 
             ishitting = true;

            switch (TRACK)
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

            if (ishitting == true)
            {
                iscolliding = true;
                //StopCoroutine(longdisable());
                ishitting = false;
            }
            else
            {
                iscolliding = false;
            }
            
        }
    }

    IEnumerator disable()
    {
        disabled = true;
       yield return new WaitForSecondsRealtime(0.2f);
        //yield return new WaitForSecondsRealtime(144.0000576f*Time.fixedDeltaTime);
        if (iscolliding)
        {

        }
        else
        {
            
            disabled = false;
            
            ishitting = false;
        }
        disabled = false;
        gameObject.SetActive(false);
        //StopCoroutine(longdisable());
        
        //toolong = false;

    }

    

}
