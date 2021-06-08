using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class ARCHIT : MonoBehaviour {
    
    
    public BoxCollider thisobj;
    public bool wrongArc = false;
    public bool onArc=false;

    public LeanFinger finger;
    public bool arcfollowing = false;
   
    public int Color=-1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	
    

    private void OnDisable()
    {
        
    wrongArc = false;
     onArc = false;

      finger=null;
        Color = -1;
        arcfollowing = false;

      
}
}
