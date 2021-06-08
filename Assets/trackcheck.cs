using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackcheck : MonoBehaviour
{
    public GameObject chosentrack;
    public GameObject holdFX;
    public ParticleSystem PART;
    bool isrunning = false;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (holdFX.activeSelf == false && PART.isPlaying)
        {   
            if(!isrunning)
            {
                isrunning = true;
                StartCoroutine(Disable());
            }
        }
        else
        {
            StopAllCoroutines();
            isrunning = false;
        }
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.25f);
        chosentrack.SetActive(false);
        PART.Stop();
        isrunning = false;
    }
}
