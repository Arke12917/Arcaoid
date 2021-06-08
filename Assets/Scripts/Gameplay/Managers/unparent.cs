using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using Arcaoid.Gameplay.Chart;

public class unparent : MonoBehaviour {

    public int timing;
    public ArcArcTap AATAP;
    public Vector3 inittransform;

    // Use this for initialization
    private void Awake()
    {
        //inittransform = transform.localPosition;
    }
    void Start () {
        //transform.SetParent(GameObject.FindGameObjectWithTag("NOTEBODY").transform);
	}
	


	// Update is called once per frame
	void Update () {
		
	}
}
