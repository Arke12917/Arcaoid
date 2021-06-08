using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Arcaoid;
using Arcaoid.Aff;

public class scalelane2 : MonoBehaviour {
    public bool setonce = false;
    public float basespeed;

    public float basecentre;
    public float curspeed;
    public List<float> Timings;
    public BoxCollider COL;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!ArcTimingManager.Instance.firstclicked)
        {
            if (!setonce)
            {
                if (gameObject.name.Contains("HILD"))
                {
                    basespeed = 0.00270588681f * ArcTimingManager.Instance.DropRate;
                    basecentre = 0.00041174494f * ArcTimingManager.Instance.DropRate;
                }
                else
                {
                    basespeed = 0.01272072527f * ArcTimingManager.Instance.DropRate;
                    basecentre = 0.00091451461f * ArcTimingManager.Instance.DropRate;
                }





                curspeed = basespeed / 1.8f;
                basecentre = basecentre / 1.8f;

                curspeed = curspeed * (ArcTimingManager.Instance.DEFAULTBPM / 100f);
                curspeed = curspeed / (ArcTimingManager.Instance.DEFAULTBPM / 100f);
                basecentre = basecentre * (ArcTimingManager.Instance.DEFAULTBPM / 100f);
                basecentre = basecentre / (ArcTimingManager.Instance.DEFAULTBPM / 100f);

                setonce = true;
            }


            //transform.position = new Vector3(transform.position.x, transform.position.y, curspeed * ArcTimingManager.Instance.CurrentSpeed);
            if (Mathf.Sign(ArcTimingManager.Instance.CurrentSpeed) == 1)
            {
                COL.size = new Vector3(COL.size.x, curspeed * ArcTimingManager.Instance.CurrentSpeed, COL.size.z);
                COL.center = new Vector3(COL.center.x, basecentre * ArcTimingManager.Instance.CurrentSpeed, COL.center.z);
            }
            
        }
    }
}
