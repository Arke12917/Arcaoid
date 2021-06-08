using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Collections;
using Arcaoid.Gameplay;
using Arcaoid.Compose;

namespace Lean.Touch
{
    // This script fires events if a finger has been held for a certain amount of time without moving
    public class LeanFingerHeld : MonoBehaviour
    {
        // Event signature
        [System.Serializable] public class FingerEvent : UnityEvent<LeanFinger> { }

        // This class will store extra Finger data
        [System.Serializable]
        public class Link
        {
            public LeanFinger Finger; // The finger associated with this link
            public bool LastSet; // Was this finger held?
            public Vector2 TotalScaledDelta; // The total movement so we can ignore it if it gets too high
        }

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreStartedOverGui = true;

        [Tooltip("Ignore fingers with IsOverGui?")]
        public bool IgnoreIsOverGui;

        [Tooltip("Do nothing if this LeanSelectable isn't selected?")]
        public LeanSelectable RequiredSelectable;

        [Tooltip("The finger must be held for this many seconds")]
        public float MinimumAge = 1.0f;

        [Tooltip("The finger cannot move more than this many pixels relative to the reference DPI")]
        public float MaximumMovement = 5.0f;

        public Camera choose;
        public GameObject ARCHIT;
        public GameObject CONSTANTBOX;
        public GameObject Lane1Hit;
        public GameObject Lane2Hit;
        public GameObject Lane3Hit;
        public GameObject Lane4Hit;
        public GameObject Hit1;
        public GameObject Hit2;
        public GameObject Hit3;
        public GameObject Hit4;
        public deactivate deac1;
        public deactivate deac2;
        public deactivate deac3;
        public deactivate deac4;
        public holdset HOL1;
        public holdset HOL2;
        public holdset HOL3;
        public holdset HOL4;

        public bool LN1active = false;
        public bool LN2active = false;
        public bool LN3active = false;
        public bool LN4active = false;

        public List<LeanFinger> LN1fingers;
        public List<LeanFinger> LN2fingers;
        public List<LeanFinger> LN3fingers;
        public List<LeanFinger> LN4fingers;

        public ArcTimingManager ATMAN;
        public ArcAudioManager AUMAN;
        public ArcGameplayManager AMAN;
        public Transform hitPlane;

        public GameObject temphit;
        // Called on the first frame the conditions are met
        public FingerEvent OnHeldDown;

        // Called on every frame the conditions are met
        public FingerEvent OnHeldSet;

        // Called on the last frame the conditions are met
        public FingerEvent OnHeldUp;

        // This stores all finger links
        private List<Link> links = new List<Link>();

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Start();
        }
#endif
        private void Awake()
        {
            deac1 = Lane1Hit.GetComponent<deactivate>();
            deac2 = Lane2Hit.GetComponent<deactivate>();
            deac3 = Lane3Hit.GetComponent<deactivate>();
            deac4 = Lane4Hit.GetComponent<deactivate>();
            layer_masklane = LayerMask.GetMask("ArcaoidGameplay");
            layer_maskarea = LayerMask.GetMask("HITAREA");
            touchPlane = ATMAN.touchPlane;
        }
        protected virtual void Start()
        {
            if (RequiredSelectable == null)
            {
                RequiredSelectable = GetComponent<LeanSelectable>();
            }
        }

        protected virtual void OnEnable()
        {
            // Hook events
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerSet += OnFingerSet;
            LeanTouch.OnFingerUp += OnFingerUp;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerSet -= OnFingerSet;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        private void OnFingerDown(LeanFinger finger)
        {
            // Ignore?
            if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
            {
                return;
            }
            if (IgnoreIsOverGui == true && finger.IsOverGui == true)
            {
                return;
            }

            if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
            {
                return;
            }

            // Get link for this finger and reset
            var link = FindLink(finger, true);

            link.LastSet = false;
            link.TotalScaledDelta = Vector2.zero;
        }
        public GameObject LANE1PFAB;
        public GameObject LANE2PFAB;
        public GameObject LANE3PFAB;
        public GameObject LANE4PFAB;

        public GameObject TRACKOVL1;
        public GameObject TRACKOVL2;
        public GameObject TRACKOVL3;
        public GameObject TRACKOVL4;

        public int layer_masklane;
        public int layer_maskarea;
        public static float xBound = 0f;
        public static float yBound = 3.2f;


        public GameObject testPosCube;
        public Plane touchPlane;
        private void OnFingerSet(LeanFinger finger)
        {
            if (!finger.setable) { return; }
            // Try and find the link for this finger
            var link = FindLink(finger, false);

            //if (link != null)
            //{
            // Has this finger been held for more than MinimumAge without moving more than MaximumMovement?
            var set = true;

            link.TotalScaledDelta += finger.ScaledDelta;

            if (set == true && link.LastSet == false)
            {
                ATMAN.allFingers.Add(finger);
                int touchLane = 0;
                float skyDist = Vector3.Distance(choose.transform.position, hitPlane.position);
                Vector3 touchInPlane = finger.GetWorldPosition(skyDist, choose);


                float sky_posX = touchInPlane.x;
                float sky_posY = touchInPlane.y;
                float sky_posZ = 0.0f;

                //GameObject TPCUBE=Instantiate(testPosCube);
                //testPosCube.transform.position = touchInPlane;

                if (sky_posX <= 8.5f && sky_posX >= 4.25f && sky_posY < yBound)
                {
                    touchLane = 1;
                }
                else if (sky_posX <= 4.25f && sky_posX >= 0.0f && sky_posY < yBound)
                {
                    touchLane = 2;
                }
                else if (sky_posX <= 0.0f && sky_posX >= -4.25f && sky_posY < yBound)
                {
                    touchLane = 3;
                }
                else if (sky_posX <= -4.25f && sky_posX >= -8.5f && sky_posY < yBound)
                {
                    touchLane = 4;
                }

                float touchDist = Vector3.Distance(choose.transform.position, touchInPlane);
                Vector3 finalTouch = touchInPlane - choose.transform.position;


                //Debug.DrawLine(choose.transform.position, finalTouch,Color.green);
                Ray ray = new Ray(choose.transform.position, finalTouch);

                float enter;
                if (touchPlane.Raycast(ray, out enter))
                {
                    var hitPoint = ray.GetPoint(enter);

                    float temp = 0;
                    if (!ArcGameplayManager.hascameramove)
                    {
                        temp = hitPoint.y > 5.557644f ? 5.557644f : hitPoint.y;
                        temp = hitPoint.y < 0f ? 0 : temp;
                    }
                    else { temp = hitPoint.y; }
                    finger.fingerVec = new Vector3(hitPoint.x, temp, hitPoint.z);

                    sky_posX = hitPoint.x;
                    sky_posY = temp;
                    sky_posZ = hitPoint.z;
                }



                //

                /*
                Vector3 touchposfar = new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, choose.farClipPlane);
                Vector3 touchposnear = new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, choose.nearClipPlane);
                Vector3 touchposF = choose.ScreenToWorldPoint(touchposfar);
                Vector3 touchposN = choose.ScreenToWorldPoint(touchposnear);

                

                Debug.DrawLine(touchposN, touchposF - touchposN, Color.green);
                Ray TR = new Ray(touchposN, touchposF - touchposN);
                //RaycastHit allHits;
               RaycastHit skyHits;

                //allHits = Physics.RaycastAll(TR, Mathf.Infinity,layerMask);
                //skyHits = Physics.RaycastAll(TR, Mathf.Infinity);
                
                if ((Physics.Raycast(TR.origin, TR.direction, out skyHits, 999f, layer_maskarea)))
                {
                    if (skyHits.transform.name.Equals("hitarea"))
                    {
                        if (Time.timeScale != 0f)
                        {

                            float temp = 0;
                            if (!ArcGameplayManager.hascameramove)
                            {
                                temp = skyHits.point.y > 5.557644f ? 5.557644f : skyHits.point.y;
                                temp = skyHits.point.y < 0f ? 0 : temp;
                            }
                            else { temp = skyHits.point.y; }
                            finger.fingerVec = new Vector3(skyHits.point.x, temp, skyHits.point.z);

                            

                            /*if (finger.fingerVec == Vector3.zero)
                            {
                                // finger.tempPREFAB = Instantiate(ARCHIT, new Vector3(skyHits.point.x, skyHits.point.y, 0), Quaternion.identity);
                                finger.tempPREFAB = ObjectPooler.SharedInstance.GetPooledObject("ARCHIT");
                                finger.tempPREFAB.transform.position = new Vector3(skyHits.point.x, skyHits.point.y, 0);
                                //finger.CONSTANTBOX = Instantiate(CONSTANTBOX, new Vector3(skyHits.point.x, skyHits.point.y, 0), Quaternion.identity);
                                finger.CONSTANTBOX = ObjectPooler.SharedInstance.GetPooledObject("CONSTBOX");
                                finger.CONSTANTBOX.transform.position = new Vector3(skyHits.point.x, skyHits.point.y, 0);
                                finger.Arcbox = finger.tempPREFAB.GetComponent<ARCHIT>();
                                finger.Arcbox.finger = finger;
                                finger.tempPREFAB.SetActive(true);
                                finger.CONSTANTBOX.SetActive(true);


                            }
                            else
                            {
                                float temp = 0;
                                if (!ArcGameplayManager.hascameramove)
                                {
                                    temp = skyHits.point.y > 5.557644f ? 5.557644f : skyHits.point.y;
                                    temp = skyHits.point.y < 0f ? 0 : temp;
                                }
                                else { temp = skyHits.point.y; }

                            }

                        }
                    }
                }*/

                if (Time.timeScale != 0)
                {

                    GameObject LA = null;
                    if (touchLane == 1)
                    {
                        if (finger.startTrack == -1)
                        {
                            finger.startTrack = 1;

                        }
                        if (!Hit1.activeSelf)
                        {
                            Lane1Hit.SetActive(true);
                            deac1.StopAllCoroutines();
                            deac1.isrunning = false;
                        }

                        if (!finger.isrunning && !finger.HoldFin && finger.canHit)
                        {
                            

                            if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack == 1)
                            {
                                StopCoroutine(HoldLeeway(finger));
                                StopCoroutine(PreventCollision(finger));
                                finger.isPreventing = false;
                            }
                            else if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack != 1)
                            {
                                finger.HoldFin = true;

                                //StartCoroutine(DestroyBOX(finger));
                            }
                            else
                            {


                                if (ATMAN.holdQuad1.Count == 0)
                                {
                                    finger.HoldFin = true;
                                }
                                else
                                {
                                    List<shrinkhold> holdnotes = new List<shrinkhold>();
                                    float minTime = -1f;
                                    int selectTime = 0;
                                    foreach (GameObject AAHOLD in ATMAN.holdQuad1)
                                    {
                                        shrinkhold theHold = AAHOLD.GetComponent<shrinkhold>();
                                        holdnotes.Add(theHold);
                                        float temptime = Mathf.Abs(AMAN.Timing-theHold.HOLD.Timing);
                                        if (temptime < minTime||minTime==-1)
                                        {
                                            minTime = temptime;
                                            selectTime = theHold.HOLD.Timing;
                                        }
                                    }
                                    foreach (shrinkhold holdNote in holdnotes)
                                    {
                                        if (selectTime == holdNote.HOLD.Timing)
                                        {
                                            if (holdNote.HOLD.Timing + AUMAN.AudioOffset -200 <= AMAN.Timing)
                                            {
                                                
                                                holdNote.hitByLane = true;
                                                //ArcaoidComposeManager.Instance.Pause();
                                                finger.currentbox = holdNote.gameObject;
                                            }
                                            else
                                            {
                                                if (!finger.isPreventing)
                                                {
                                                    finger.isPreventing = true;
                                                    StartCoroutine(PreventCollision(finger));
                                                }
                                            }
                                        }
                                    }
                                }
                              
                            }
                            finger.isrunning = false;
                        }




                    }
                    else if (touchLane == 2)
                    {
                        if (finger.startTrack == -1)
                        {
                            finger.startTrack = 2;

                        }
                        if (!Hit2.activeSelf)
                        {
                            Lane2Hit.SetActive(true);
                            deac2.StopAllCoroutines();
                            deac2.isrunning = false;
                        }

                        if (!finger.isrunning && !finger.HoldFin && finger.canHit)
                        {


                            if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack == 2)
                            {
                                StopCoroutine(HoldLeeway(finger));
                                StopCoroutine(PreventCollision(finger));
                                finger.isPreventing = false;
                            }
                            else if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack != 2)
                            {
                                finger.HoldFin = true;

                                //StartCoroutine(DestroyBOX(finger));
                            }
                            else
                            {


                                if (ATMAN.holdQuad2.Count == 0)
                                {
                                    finger.HoldFin = true;
                                }
                                else
                                {
                                    List<shrinkhold> holdnotes = new List<shrinkhold>();
                                    float minTime = -1f;
                                    int selectTime = 0;
                                    foreach (GameObject AAHOLD in ATMAN.holdQuad2)
                                    {
                                        shrinkhold theHold = AAHOLD.GetComponent<shrinkhold>();
                                        holdnotes.Add(theHold);
                                        float temptime = Mathf.Abs(AMAN.Timing - theHold.HOLD.Timing);
                                        if (temptime < minTime || minTime == -1)
                                        {
                                            minTime = temptime;
                                            selectTime = theHold.HOLD.Timing;
                                        }
                                    }
                                    foreach (shrinkhold holdNote in holdnotes)
                                    {
                                        if (selectTime == holdNote.HOLD.Timing)
                                        {
                                            if (holdNote.HOLD.Timing + AUMAN.AudioOffset - 200 <= AMAN.Timing)
                                            {

                                                holdNote.hitByLane = true;
                                                //ArcaoidComposeManager.Instance.Pause();
                                                finger.currentbox = holdNote.gameObject;
                                            }
                                            else
                                            {
                                                if (!finger.isPreventing)
                                                {
                                                    finger.isPreventing = true;
                                                    StartCoroutine(PreventCollision(finger));
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            finger.isrunning = false;
                        }



                    }
                    else if (touchLane == 3)
                    {
                        if (finger.startTrack == -1)
                        {
                            finger.startTrack = 3;

                        }
                        if (!Hit3.activeSelf)
                        {
                            Lane3Hit.SetActive(true);
                            deac3.StopAllCoroutines();
                            deac3.isrunning = false;
                        }

                        if (!finger.isrunning && !finger.HoldFin && finger.canHit)
                        {


                            if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack == 3)
                            {
                                StopCoroutine(HoldLeeway(finger));
                                StopCoroutine(PreventCollision(finger));
                                finger.isPreventing = false;
                            }
                            else if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack != 3)
                            {
                                finger.HoldFin = true;

                                //StartCoroutine(DestroyBOX(finger));
                            }
                            else
                            {


                                if (ATMAN.holdQuad3.Count == 0)
                                {
                                    finger.HoldFin = true;
                                }
                                else
                                {
                                    List<shrinkhold> holdnotes = new List<shrinkhold>();
                                    float minTime = -1f;
                                    int selectTime = 0;
                                    foreach (GameObject AAHOLD in ATMAN.holdQuad3)
                                    {
                                        shrinkhold theHold = AAHOLD.GetComponent<shrinkhold>();
                                        holdnotes.Add(theHold);
                                        float temptime = Mathf.Abs(AMAN.Timing - theHold.HOLD.Timing);
                                        if (temptime < minTime || minTime == -1)
                                        {
                                            minTime = temptime;
                                            selectTime = theHold.HOLD.Timing;
                                        }
                                    }
                                    foreach (shrinkhold holdNote in holdnotes)
                                    {
                                        if (selectTime == holdNote.HOLD.Timing)
                                        {
                                            if (holdNote.HOLD.Timing + AUMAN.AudioOffset - 200 <= AMAN.Timing)
                                            {

                                                holdNote.hitByLane = true;
                                                //ArcaoidComposeManager.Instance.Pause();
                                                finger.currentbox = holdNote.gameObject;
                                            }
                                            else
                                            {
                                                if (!finger.isPreventing)
                                                {
                                                    finger.isPreventing = true;
                                                    StartCoroutine(PreventCollision(finger));
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            finger.isrunning = false;
                        }




                    }
                    else if (touchLane == 4)
                    {
                        if (finger.startTrack == -1)
                        {
                            finger.startTrack = 4;

                        }
                        if (!Hit4.activeSelf)
                        {
                            Lane4Hit.SetActive(true);
                            deac4.StopAllCoroutines();
                            deac4.isrunning = false;
                        }

                        if (!finger.isrunning && !finger.HoldFin && finger.canHit)
                        {


                            if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack == 4)
                            {
                                StopCoroutine(HoldLeeway(finger));
                                StopCoroutine(PreventCollision(finger));
                                finger.isPreventing = false;
                            }
                            else if (!(ReferenceEquals(finger.currentbox, null)) && finger.startTrack != 4)
                            {
                                finger.HoldFin = true;

                                //StartCoroutine(DestroyBOX(finger));
                            }
                            else
                            {


                                if (ATMAN.holdQuad4.Count == 0)
                                {
                                    finger.HoldFin = true;
                                }
                                else
                                {
                                    List<shrinkhold> holdnotes = new List<shrinkhold>();
                                    float minTime = -1f;
                                    int selectTime = 0;
                                    foreach (GameObject AAHOLD in ATMAN.holdQuad4)
                                    {
                                        shrinkhold theHold = AAHOLD.GetComponent<shrinkhold>();
                                        holdnotes.Add(theHold);
                                        float temptime = Mathf.Abs(AMAN.Timing - theHold.HOLD.Timing);
                                        if (temptime < minTime || minTime == -1)
                                        {
                                            minTime = temptime;
                                            selectTime = theHold.HOLD.Timing;
                                        }
                                    }
                                    foreach (shrinkhold holdNote in holdnotes)
                                    {
                                        if (selectTime == holdNote.HOLD.Timing)
                                        {
                                            if (holdNote.HOLD.Timing + AUMAN.AudioOffset - 200 <= AMAN.Timing)
                                            {

                                                holdNote.hitByLane = true;
                                                //ArcaoidComposeManager.Instance.Pause();
                                                finger.currentbox = holdNote.gameObject;
                                            }
                                            else
                                            {
                                                if (!finger.isPreventing)
                                                {
                                                    finger.isPreventing = true;
                                                    StartCoroutine(PreventCollision(finger));
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            finger.isrunning = false;
                        }


                    }

                }




                if (OnHeldDown != null)
                {
                    OnHeldDown.Invoke(finger);
                }
            }

            if (set == true)
            {
                if (OnHeldSet != null)
                {
                    OnHeldSet.Invoke(finger);
                }
            }

            if (set == false && link.LastSet == true)
            {
                if (OnHeldUp != null)
                {
                    OnHeldUp.Invoke(finger);
                }
            }

            // Store last value
            //link.LastSet = set;
            //}

        }

        /*  IEnumerator DestroyBOX(LeanFinger finger)
          {
              yield return new WaitForSeconds(0.0f);
              GameObject BOX = finger.holdboxes;

                  if (!(ReferenceEquals(BOX, null)))
                  {
                      BOX.transform.position = boxoob;                   

                  }


              yield return new WaitForFixedUpdate();

                  if (!(ReferenceEquals(BOX, null)))
                  {

                      BOX.SetActive(false);
                  }


              finger.holdboxes=null;
              finger.currentbox = null;
          }*/

        public Vector3 boxoob = new Vector3(500, 500, 500);
        public Vector3 ACoob = new Vector3(600, 600, 600);

        IEnumerator PreventCollision(LeanFinger finger)
        {
            yield return new WaitForSeconds(0.1f);
            finger.HoldFin = true;
        }


        IEnumerator HoldLeeway(LeanFinger finger)
        {
            yield return new WaitForSecondsRealtime(0.0f);
            // yield return new WaitForSeconds(24.0000096f*Time.fixedDeltaTime);
            GameObject GO = finger.holdboxes;

            if (!(ReferenceEquals(GO, null)))
            {
                GO.transform.position = boxoob;

            }

            yield return new WaitForFixedUpdate();

            if (!(ReferenceEquals(GO, null)))
            {
                GO.SetActive(false);
            }


            finger.holdboxes = null;
            finger.currentbox = null;
            finger.shouldrun = true;
        }

        public Coroutine DBOX;

        IEnumerator DeleteBOX(LeanFinger finger)
        {
            //yield return new WaitForSeconds(0.0f);
            yield return new WaitUntil(() => finger.shouldrun == true);
            if (!(ReferenceEquals(finger, null)))
            {
                finger.Color = -1;
            }

            //if (finger.tempPREFAB != null)
            //if (finger.tempPREFAB!=null)
            /*if (!(ReferenceEquals(finger, null)))
            {
                if (!(ReferenceEquals(finger.tempPREFAB, null)))
                {
                    finger.tempPREFAB.transform.position = ACoob;
                    finger.CONSTANTBOX.transform.position = ACoob;
                    yield return new WaitForFixedUpdate();
                    finger.tempPREFAB.SetActive(false);
                    finger.tempPREFAB = null;
                    finger.CONSTANTBOX.SetActive(false);
                    finger.CONSTANTBOX = null;
                }
            }*/
            finger.HoldFin = false;
            finger.CRUNNING = false;
        }

        private void OnFingerUp(LeanFinger finger)
        {
            if (Time.timeScale != 0)
            {
                finger.CRUNNING = true;
                StartCoroutine(HoldLeeway(finger));
                StartCoroutine(DeleteBOX(finger));

                // Find link for this finger, and clear it
                var link = FindLink(finger, false);

                if (link != null)
                {
                    links.Remove(link);

                    if (link.LastSet == true)
                    {
                        if (OnHeldUp != null)
                        {
                            StartCoroutine(fingerup(finger));
                        }
                    }
                }
            }
        }

        public IEnumerator fingerup(LeanFinger finger)
        {
            //yield return new WaitForSecondsRealtime(0.0f);
            yield return new WaitUntil(() => finger.CRUNNING == false);
            ATMAN.allFingers.Remove(finger);
            OnHeldUp.Invoke(finger);
        }

        private Link FindLink(LeanFinger finger, bool createIfNull)
        {
            // Find existing link?
            for (var i = 0; i < links.Count; i++)
            {
                var link = links[i];

                if (link.Finger == finger)
                {
                    return link;
                }
            }

            // Make new link?
            if (createIfNull == true)
            {
                var link = new Link();

                link.Finger = finger;

                links.Add(link);

                return link;
            }

            return null;
        }
    }
}