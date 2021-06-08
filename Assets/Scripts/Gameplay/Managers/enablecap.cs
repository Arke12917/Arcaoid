using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;

public class enablecap : MonoBehaviour
{
    public ArcArcRenderer parentArc;
    bool isinrange = false;
    public movecap Movecap;
    public ParticleSystem JUDeffect;
    public int current;
    public BoxCollider CAPCOLLIDER;
    public disablehead DNHEAD;

    public SpriteRenderer Actualcap;
    public Rigidbody rb;
    // Use this for initialization
    void Awake()
    {
        //parentArc = transform.parent.gameObject.GetComponent<ArcArcManager>();  
        //JUDeffect = parentArc.ActualCap.GetComponentInChildren<ParticleSystem>();
        
    }

    private void OnEnable()
    {
        StartCoroutine(ishead());
    }

    private void OnDisable()
    {
        isinrange = false;
        current = 0;
        //DNHEAD.enabled = true;

        //CAPCOLLIDER.enabled = true;

    }


    IEnumerator ishead()
    {
        yield return new WaitUntil(() => ArcArcManager.Instance.readytocalc == true);
       /* if (!parentArc.IsHead)
        {
            // print("nothead");
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            //parentArc.HeadRenderer.enabled = false;
            parentArc.HeightIndicatorRenderer.enabled = false;
            
        }*/
        if (parentArc.ArcCap.localScale.x == 0.21f)
        {
           // CAPCOLLIDER.enabled = false;
        }
    }

    // Update is called once per frame


    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("trigger?")&&ArcTimingManager.Instance.IsBackwarding==false)
        {
            //if(Movecap.duriation!=0)
            //Actualcap.enabled=true;

            //Movecap.isinrange = true;
            
            parentArc.HeightIndicatorRenderer.enabled = false;
            parentArc.HeadRenderer.enabled = false;
            if (parentArc.Arc.IsVoid)
            {

            }
            else
            {
                

                //if (Movecap.COMBO != 1&& !(Movecap.duriation==0))
                if ((Movecap.duriation==0))
                {
                    // Movecap.StartCoroutine("judgeholds");
                    Movecap.isinrange = true;
                    //Movecap.StartCoroutine("givecombo");
                }
                else
                {
                    
                }
            }
            

        }
       
    }
}
