using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivate : MonoBehaviour {
    public float time;
    public bool isrunning;
	// Use this for initialization
	void Start () {
        
	}
    void Update()
    {
        if (!isrunning && name.Contains("HILD"))
        StartCoroutine(deac());
    }

    private void OnEnable()
    {
        if (name.Contains("HILD"))
        {
            time = 0.2f;
            //time = 12.0000048f;
        }
        else
        {
            time = 0.05f;
            //time = 6.0000024f;
            StartCoroutine(deac());
        }
        

    }

    IEnumerator deac()
    {
        isrunning = true;
        yield return new WaitForSecondsRealtime(time);
        //yield return new WaitForSecondsRealtime(time*Time.fixedDeltaTime);
        isrunning = false;
        gameObject.SetActive(false);

    }
    
}
