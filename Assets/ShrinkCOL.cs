using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;

public class ShrinkCOL : MonoBehaviour
{
    public int TIME;
    public CanClick CCK;
    public bool didsend = false;
    public BoxCollider COLLIDER;
    public LayerMask mask;
    Vector3 Normalsize = new Vector3(4.5f, 4f, 1);
   // Vector3 Normalsize = new Vector3(3.8f, 3.4f, 1);
    Vector3 Reducedsize = new Vector3(2.4f, 2.2f, 1);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void sendray()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 150f, mask))
        {
            if (hit.collider.tag == "ArcCaps")
            {
                var CAP = hit.collider.GetComponent<movecap>();
                if (!CAP.parentArc.Arc.IsVoid)
                {
                    if (Mathf.Abs(CCK.ATAP.Timing - CAP.parentArc.Arc.Timing) <= 175f && Mathf.Abs(CCK.ATAP.Timing - CAP.parentArc.Arc.Timing) != 0)
                    {
                        CCK.ATAP.shouldwait = true;
                        CCK.ATAP.TIME = (int)((CCK.ATAP.Timing - CAP.parentArc.Arc.Timing) / 1.2) + CAP.parentArc.Arc.Timing;
                       // print("SHOULD WAIT");
                        //print(ACTAP.TIME);
                    }
                }
            }
            else
            {
                var CAP = hit.collider.GetComponent<shrinkhold>();
                if (CAP != null)
                {
                    if (Mathf.Abs(CCK.ATAP.Timing - CAP.HOLD.EndTiming) <= 175f && Mathf.Abs(CCK.ATAP.Timing - CAP.HOLD.EndTiming) != 0)
                    {
                        CCK.ATAP.holdwait = true;
                        CCK.ATAP.TIME = (int)((CCK.ATAP.Timing - CAP.HOLD.EndTiming) / 1.2) + CAP.HOLD.EndTiming;
                        //print("HOLD WAIT");
                        //print(ACTAP.TIME);
                    }
                }
                
            }
        }
    }
    // Update is called once per frame
    

    IEnumerator prepray()
    {
        yield return new WaitUntil(() => CCK.ATAP != null);
        if (didsend == false)
        {
            didsend = true;
            // yield return new WaitForSeconds(0.1f);
            sendray();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("ArcTap"))
        {
            COLLIDER.size = Reducedsize;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(prepray());
    }

    private void OnDisable()
    {
        COLLIDER.size = Normalsize;
        TIME = 0;
        didsend = false;
    }
}
