using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Compose;
using Arcaoid.Gameplay;
using Arcaoid.Aff;

public class movebar : MonoBehaviour {
    public bool setonce=false;
    public float basespeed;
    public float curspeed;
    public List<float> Timings;
    public Transform bartransform;
    // Use this for initialization
    void Start () {
        bartransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (ArcaoidComposeManager.readytoshift == true)
        {
            if (!setonce)
            {
                if (this.name == "judgebar")
                {
                    basespeed = -0.19791208791f * ArcTimingManager.Instance.DropRate;
                }else if (this.name== "judgeplane")
                {
                    basespeed = -0.35978021978f * ArcTimingManager.Instance.DropRate;
                }else if (this.name== "failcollider")
                {
                    basespeed = 0.13747252747f * ArcTimingManager.Instance.DropRate;
                }


                curspeed = basespeed / 1.8f;

                curspeed = curspeed * (ArcTimingManager.Instance.DEFAULTBPM/100f);
                curspeed = curspeed / (ArcTimingManager.Instance.DEFAULTBPM/100f);
                setonce = true;
            }

            if (ArcTimingManager.Instance.CurrentSpeed == 0)
            {
                bartransform.position = new Vector3(bartransform.position.x, bartransform.position.y, curspeed * (ArcTimingManager.Instance.DEFAULTBPM/ArcTimingManager.Instance.BaseBpm));
            }
            else
            {
                bartransform.position = new Vector3(bartransform.position.x, bartransform.position.y, curspeed * ArcTimingManager.Instance.CurrentSpeed);
            }
        }
	}
}
