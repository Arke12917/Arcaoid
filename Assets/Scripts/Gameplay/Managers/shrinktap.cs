using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;

public class shrinktap : MonoBehaviour {
    bool shrink=false;
    float position;
    public float time;
    public float timetime;
    float basetime;
    float shrinktime;
    public float distance;
    bool hasscaled = false;
    float above40;
    float below40;
    float truescale;

	// Use this for initialization
	void Start () {   
      
    }

    void InitialScale()
    {
        float temp=0;
        float pos = time / 1000f;
        if (ArcTimingManager.Instance.DropRate <= 40f)
        {
            truescale = 2.5f;
            temp = 7.395694f;
        }
        else
        {
            truescale = 4f;
            temp = 8.895694f;
        }
        

        if (time > 100000)
        {
            transform.localScale = new Vector3(1.53f,temp , 1);
            distance= Vector3.Distance(ArcTimingManager.Instance.scalenoteline.transform.position, ArcTimingManager.criticalline.transform.position);
        }  
        else
        {
            transform.localScale = new Vector3(1.53f, truescale + 5.1f * pos / 100f, 1);
            distance = Vector3.Distance(transform.position, ArcTimingManager.criticalline.transform.position);
        }
    }
    
	// Update is called once per frame
	void Update () {
        if (!hasscaled && ArcTimingManager.Shouldrun)
        {
            hasscaled = true;
            InitialScale();
            
            shrinktime = time / 1000f;
        }
        if((time<100000||shrink)&&!ArcTimingManager.Instance.IsBackwarding&&!ArcTimingManager.Instance.IsStopped&&Time.timeScale!=0)
        {
            float temp = Mathf.MoveTowards(transform.localScale.y, truescale, Time.deltaTime * (distance / (20f/ArcTimingManager.Instance.CurrentSpeed)));
            try
            {              
                transform.localScale = new Vector3(1.53f, temp, 1);
            }
            catch
            {

            }

        }
        else if(ArcTimingManager.Instance.IsBackwarding&&shrink){
            //transform.localScale= new Vector3(1.53f, truescale, 1);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "scalenoteline")
        {
            shrink = true;
        }
    }
}
