using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid;
using Arcaoid.Gameplay;

public class disablehead : MonoBehaviour
{
    public enablecap ENCAP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("SEGMENT"))
        {
            var AAS = other.GetComponent<ArcArcSegmentComponent>();
            if (AAS.OFFSET == 0.9f&&(ENCAP.parentArc.Arc.ArcGroup==AAS.AAR.arc.ArcGroup))
            {
                //print("neat!");
                ENCAP.parentArc.HeadRenderer.enabled = false;
                ENCAP.parentArc.HeightIndicatorRenderer.enabled = false;
            }
            (this).enabled = false;
        }
    }
}
