using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Arcaoid.Gameplay;
using Arcaoid.Aff;
using Arcaoid.Gameplay.Chart;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Remoting.Channels;

namespace Lean.Touch
{
    // This script calls the OnFingerDown event when a finger touches the screen
    public class LeanFingerDown : MonoBehaviour
    {
        public Camera choose;
        public GameObject SLA;
        public GameObject Lane1Hit;
        public GameObject Lane2Hit;
        public GameObject Lane3Hit;
        public GameObject Lane4Hit;
        public GameObject HOLD1;
        public GameObject HOLD2;
        public GameObject HOLD3;
        public GameObject HOLD4;

        public GameObject OBJ1PFAB;
        public GameObject OBJ2PFAB;
        public GameObject OBJ3PFAB;
        public GameObject OBJ4PFAB;
        public Transform Track;
        public Transform hitPlane;

        public GameObject JBAR;
        public GameObject FCOLLIDER;
        public ArcTimingManager ATMAN;
        public TouchManager TManager;
        public GameObject testPosCube;

        public static float xBound = 0f;
        public static float yBound = 3.2f;
        // Event signature
        [System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> { }

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreStartedOverGui = true;

        [Tooltip("Do nothing if this LeanSelectable isn't selected?")]
        public LeanSelectable RequiredSelectable;

        public LeanFingerEvent OnDown;


#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Start();
        }
#endif

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
            LeanTouch.OnFingerDown += FingerDown;
            TManager.OnStartTouch += Tapped;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerDown -= FingerDown;
            TManager.OnEndTouch -= Tapped;
        }

        public void Tapped(Vector2 screenPosition, float time)
        {
            int touchQuad = 0;
            float sky_posX = 0;
            float sky_posY = 0;
            float sky_posZ = 0;



            float skyDist = Vector3.Distance(choose.transform.position, hitPlane.position);
            // Vector3 touchInPlane = finger.GetWorldPosition(skyDist, choose);
            Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, choose.nearClipPlane);
            Vector3 worldCoordinates = choose.ScreenToWorldPoint(screenCoordinates);
            worldCoordinates.z = skyDist;
            Vector3 touchInPlane = worldCoordinates;
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
                //finger.fingerVec = new Vector3(hitPoint.x, temp, hitPoint.z);

                sky_posX = hitPoint.x;
                sky_posY = temp;
                sky_posZ = hitPoint.z;
            }


            //print(sky_posX + "," + sky_posY);

            if (sky_posX < xBound && sky_posY > yBound)
            {
                touchQuad = 2;
            }
            else if (sky_posX > xBound && sky_posY > yBound)
            {
                touchQuad = 1;
            }
            else if (sky_posX < xBound && sky_posY < yBound)
            {
                touchQuad = 4;
            }
            else if (sky_posX > xBound && sky_posY < yBound)
            {
                touchQuad = 3;
            }


            if (Time.timeScale != 0)
            {


                Transform THENOTE = null;
                bool canhit = true;


                Transform temparc = ARCPOSreturn(new Vector3(sky_posX, sky_posY, sky_posZ), THENOTE, touchQuad);
                bool hitAATAP = false;

                if (temparc != null)
                {

                    THENOTE = temparc;

                    if (THENOTE.CompareTag("ArcTap"))
                    {
                        /*X > XMin && X < XMax &&
                        Y > YMin && Y < YMax*/
                        float X_pos = THENOTE.transform.position.x;
                        float Y_pos = THENOTE.transform.position.y;
                        float fingerYpos = sky_posY;

                        if (!ArcGameplayManager.hascameramove)
                        {
                            if (fingerYpos > 5.557644f)
                            {
                                fingerYpos = (5.557644f);
                            }
                            else if (fingerYpos < 0.6f)
                            {
                                fingerYpos = 0.6f;
                            }
                            else
                            {
                            }
                        }

                        /*float Xmin = X_pos - 2.3f;
                        float Xmax = X_pos + 2.3f;
                        float Ymin = Y_pos - 1.8f;
                        float Ymax = Y_pos + 1.8f;*/

                        float Xmin = X_pos - 3.3f;
                        float Xmax = X_pos + 3.3f;
                        float Ymin = Y_pos - 6.8f;
                        float Ymax = Y_pos + 6.8f;

                        if ((sky_posX > Xmin && sky_posX < Xmax) && (fingerYpos > Ymin && fingerYpos < Ymax))
                        {
                            hitAATAP = true;
                            //print("yay?");
                            DOARC(THENOTE);
                        }

                    }
                    else if (THENOTE.CompareTag("TapNote"))
                    {

                        DOTAP(THENOTE, sky_posX);
                    }
                }

                if ((touchQuad == 3 || touchQuad == 4) && !hitAATAP)
                {
                    //print("kk");
                    if (sky_posX <= -4.25f && sky_posX >= -8.5f)
                    {
                        if (!HOLD4.activeSelf)
                        {
                            Lane4Hit.SetActive(true);

                        }
                    }
                    else if (sky_posX <= 0.0f && sky_posX >= -4.25f)
                    {
                        if (!HOLD3.activeSelf)
                        {
                            Lane3Hit.SetActive(true);
                        }
                    }
                    else if (sky_posX <= 4.25f && sky_posX >= 0.0f)
                    {
                        if (!HOLD2.activeSelf)
                        {
                            Lane2Hit.SetActive(true);
                        }
                    }
                    else if (sky_posX <= 8.5f && sky_posX >= 4.25f)
                    {
                        if (!HOLD1.activeSelf)
                        {
                            Lane1Hit.SetActive(true);
                        }
                    }
                }

            }
        }

        bool ArrayContainsLane(RaycastHit[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].transform.name.Contains("Lane")) return true;
            }
            return false;
        }

        bool ArrayContainsarea(RaycastHit[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].transform.name.Equals("hitarea")) return true;
            }
            return false;
        }

        Transform ARCPOSreturn(Vector3 HITPOINT, Transform THEARC, int touchQuad)
        {
            float minDistance = Mathf.Infinity;
            float minTIME = Mathf.Infinity;
            switch (touchQuad)
            {
                case 1:
                    if (ATMAN.noteQuad1.Count == 0)
                    {
                        return null;
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad1)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        if (AATAP.activeSelf == true)
                        {
                            /*int NOTETIME = 0;
                            if (CLICK.notetype == "judgebar")
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.TAP.Timing);
                            }
                            else
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.ATAP.Timing);
                            }*/
                            float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);

                            if (tempDist < minDistance)
                            {
                                //if (!(AATAP.CompareTag("TapNote") && HITPOINT.y < 3.5f))
                                //{
                                    minDistance = tempDist;
                                   // minTIME = NOTETIME;
                                //}
                            }
                        }
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad1)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);
                        if (Mathf.Approximately(minDistance, tempDist))
                        {
                            return AATAP.transform;
                        }
                    }
                    break;
                case 2:
                    if (ATMAN.noteQuad2.Count == 0)
                    {
                        return null;
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad2)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        if (AATAP.activeSelf == true)
                        {
                            /*int NOTETIME = 0;
                            if (CLICK.notetype == "judgebar")
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.TAP.Timing);
                            }
                            else
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.ATAP.Timing);
                            }*/
                            float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);

                            if (tempDist < minDistance)
                            {
                                //if (!(AATAP.CompareTag("TapNote") && HITPOINT.y < 3.5f))
                               // {
                                    minDistance = tempDist;
                                   // minTIME = NOTETIME;
                               // }
                            }
                        }
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad2)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);
                        if (Mathf.Approximately(minDistance, tempDist))
                        {
                            return AATAP.transform;
                        }
                    }
                    break;
                case 3:
                    if (ATMAN.noteQuad3.Count == 0)
                    {
                        return null;
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad3)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        if (AATAP.activeSelf == true)
                        {
                           /* int NOTETIME = 0;
                            if (CLICK.notetype == "judgebar")
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.TAP.Timing);
                            }
                            else
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.ATAP.Timing);
                            }*/
                            float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);

                            if (tempDist < minDistance)
                            {

                                minDistance = tempDist;
                                //minTIME = NOTETIME;

                            }
                        }
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad3)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);
                        if (Mathf.Approximately(minDistance, tempDist))
                        {
                            return AATAP.transform;
                        }
                    }
                    break;
                case 4:
                    if (ATMAN.noteQuad4.Count == 0)
                    {
                        return null;
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad4)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        if (AATAP.activeSelf == true)
                        {
                            /*int NOTETIME = 0;
                            if (CLICK.notetype == "judgebar")
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.TAP.Timing);
                            }
                            else
                            {
                                NOTETIME = Mathf.Abs(AMAN.Timing - CLICK.ATAP.Timing);
                            }*/
                            float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);

                            if (tempDist < minDistance)
                            {

                                minDistance = tempDist;
                                //minTIME = NOTETIME;
                            }
                        }
                    }
                    foreach (CanClick CLICK in ATMAN.noteQuad4)
                    {
                        GameObject AATAP = CLICK.gameObject;
                        float tempDist = Vector3.Distance(HITPOINT, AATAP.transform.position);
                        if (Mathf.Approximately(minDistance, tempDist))
                        {
                            return AATAP.transform;
                        }
                    }
                    break;
            }

            /*RaycastHit hit;
            //Debug.DrawRay(new Vector3(HITPOINT.x, HITPOINT.y, HITPOINT.z+50), new Vector3(0,0,-999f),Color.red,100f);
            if (Physics.Raycast(new Vector3(HITPOINT.x,HITPOINT.y,HITPOINT.z+50), Vector3.back, out hit, 100f, 1 << LayerMask.NameToLayer("arctaps")))
            {


                if (hit.collider.gameObject.tag.Equals("ArcTap"))
                {
                    //print("YEET");
                    if (ReferenceEquals(THEARC, null))
                    {
                        return hit.collider.transform;
                    }
                    else
                    {
                        if(hit.collider.gameObject.GetComponent<CanClick>().ATAP.Timing> THEARC.gameObject.GetComponent<CanClick>().ATAP.Timing)
                        {
                            return null;
                        }
                        else
                        {
                            return hit.collider.transform;
                        }
                    }
                   
                }
            }*/
            return null;
        }

        Transform TAPPOSreturn(Vector3 HITPOINT, Transform THETAP)
        {

            RaycastHit hit;
            var Dicstance = Vector3.Distance(FCOLLIDER.transform.position, JBAR.transform.position);


            //Debug.DrawRay(HITPOINT, new Vector3(0, 0, 20f), Color.white, 100f);
            if (Physics.Raycast(new Vector3(HITPOINT.x, HITPOINT.y, FCOLLIDER.transform.position.z), Vector3.back, out hit, Dicstance, 1 << LayerMask.NameToLayer("TAPS")))
            {
                if (hit.collider.gameObject.tag.Equals("TapNote"))
                {
                    if (THETAP == null)
                        return hit.collider.transform;
                    else
                    {
                        if (hit.collider.gameObject.GetComponent<CanClick>().TAP.Timing > THETAP.gameObject.GetComponent<CanClick>().TAP.Timing)
                        {
                            return null;
                        }
                        else
                        {
                            return hit.collider.transform;
                        }
                    }
                }
            }
            return null;
        }
        public int cubeLayerIndex;
        public int sphereLayerIndex;
        public LayerMask masque;
        //public LayerMask masqarea;

        public ArcGameplayManager AMAN;
        public ArcScoreManager ASMAN;
        public ArcEffectManager AFMAN;
        public int layer_maskarea;
        public Plane touchPlane;

        /*Vector3 GetIntersection(GameObject obj, LeanFinger finger)
        {
            Ray ray = choose.ScreenPointToRay(finger.p);
            float delta = ray.origin.y - plane.transform.position.y;
            Vector3 dirNorm = ray.direction / ray.direction.y;
            Vector3 IntersectionPos = ray.origin - dirNorm * delta;
            return IntersectionPos;
        }*/

        private void Awake()
        {
            cubeLayerIndex = LayerMask.NameToLayer("no-interact");
            sphereLayerIndex = LayerMask.NameToLayer("Ignore Raycast");
            layer_maskarea = LayerMask.GetMask("HITAREA");
            AMAN = ArcGameplayManager.Instance;
            AFMAN = ArcEffectManager.Instance;
            JudgeOffset = LOADMENU.judgevalue;
            touchPlane = ATMAN.touchPlane;

        }

        public float JudgeOffset;
        public LayerMask beflayer;

        private void FingerDown(LeanFinger finger)
        {
            // Ignore?
            if (IgnoreStartedOverGui == true && finger.IsOverGui == true)
            {
                return;
            }

            if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
            {
                return;
            }
            /*Vector3 touchposfar = new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, choose.farClipPlane);
            Vector3 touchposnear = new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, choose.nearClipPlane);
            Vector3 touchposF = choose.ScreenToWorldPoint(touchposfar);
            Vector3 touchposN = choose.ScreenToWorldPoint(touchposnear);



            Debug.DrawLine(touchposN, touchposF - touchposN, Color.green);
            Ray TR = new Ray(touchposN, touchposF - touchposN);
            //RaycastHit allHits;
            RaycastHit skyHits;*/

            //allHits = Physics.RaycastAll(TR, Mathf.Infinity,layerMask);
            //skyHits = Physics.RaycastAll(TR, Mathf.Infinity);
            int touchQuad = 0;
            float sky_posX = 0;
            float sky_posY = 0;
            float sky_posZ = 0;



            float skyDist = Vector3.Distance(choose.transform.position, hitPlane.position);
            Vector3 touchInPlane = finger.GetWorldPosition(skyDist, choose);
            float touchDist = Vector3.Distance(choose.transform.position, touchInPlane);
            Vector3 finalTouch = touchInPlane - choose.transform.position;


            //Debug.DrawLine(choose.transform.position, finalTouch,Color.green);
            Ray ray = new Ray(choose.transform.position,finalTouch);
           
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

            
            //print(sky_posX + "," + sky_posY);

            if (sky_posX < xBound && sky_posY > yBound)
            {
                touchQuad = 2;
            }
            else if (sky_posX > xBound && sky_posY > yBound)
            {
                touchQuad = 1;
            }
            else if (sky_posX < xBound && sky_posY < yBound)
            {
                touchQuad = 4;
            }
            else if (sky_posX > xBound && sky_posY < yBound)
            {
                touchQuad = 3;
            }

           // GameObject TPCUBE=Instantiate(testPosCube);
            //testPosCube.transform.position = finger.fingerVec;
            //print(finger.fingerVec);

            /* if ((Physics.Raycast(TR.origin, TR.direction, out skyHits, 999f, layer_maskarea)))
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

                         //float skyDist = Vector3.Distance(choose.transform.position, hitPlane.position);
                         Vector3 touchInPlane = skyHits.point;


                          sky_posX = touchInPlane.x;
                          sky_posY = touchInPlane.y;
                          sky_posZ = touchInPlane.z;

                         if (sky_posX < xBound && sky_posY > yBound)
                         {
                             touchQuad = 2;
                         }
                         else if (sky_posX > xBound && sky_posY > yBound)
                         {
                             touchQuad = 1;
                         }
                         else if (sky_posX < xBound && sky_posY < yBound)
                         {
                             touchQuad = 4;
                         }
                         else if (sky_posX > xBound && sky_posY < yBound)
                         {
                             touchQuad = 3;
                         }

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

            // float touch_posX = touchInWorld.x;
            // float touch_posY = touchInWorld.y;




            //RaycastHit hit;
            if (Time.timeScale != 0)
            {

                /*RaycastHit beforehit;

               if(Physics.Raycast(TR.origin, TR.direction, out beforehit, Mathf.Infinity,beflayer))
                {
                    if (beforehit.collider.name.Contains("Lane")||beforehit.collider.tag.Equals("ArcTap"))
                    {
                        finger.downable = true;
                    }else if (beforehit.collider.tag.Equals("HoldNote"))
                    {
                        finger.setable = true;
                    }
                    else if (beforehit.collider.tag.Equals("CAP")|| beforehit.collider.tag.Equals("CAPHOLD"))
                    {
                        finger.setable = true;
                    }
                    else
                    {
                        finger.setable = true;
                    }
                }*/
                finger.setable = true;
                finger.downable = true;

                if (!finger.downable) { return; }

                Transform THENOTE = null;
                bool canhit = true;

                //RaycastHit[] allHits;
                //allHits = Physics.RaycastAll(TR, Mathf.Infinity);
                //foreach (var hit in allHits)
                // {


                /*if (hit.collider.tag.Equals("CAP"))
                {
                    movecap cup = null;

                    cup = hit.collider.GetComponent<movecap>();

                    if (cup.parentArc.washead == true&!cup.iscolliding)
                    {
                        print("nope!");
                        canhit = false;
                    }
                }*/
                // now filter by tag or name
                /* if (hit.transform.name.Equals("hitarea"))
                 {
                     if (Time.timeScale != 0f)
                     {
                         //GameObject SLAQ;
                         //0.2
                         float temp = hit.point.y;
                     if (!ArcGameplayManager.hascameramove)
                     {
                         if (temp > 5.557644f)
                         {
                             temp = (5.557644f);
                         }
                         else if (temp < 0.6f)
                         {
                             temp = 0.6f;
                         }
                         else
                         {
                             temp = hit.point.y;
                         }
                     }
                     else { temp = hit.point.y; }
                         Transform temparc= ARCPOSreturn(new Vector3(hit.point.x, temp, 10), THEARC);

                     if(temparc!=null)
                     THEARC = temparc;

                     }
                 }*/


                Transform temparc = ARCPOSreturn(new Vector3(sky_posX, sky_posY, sky_posZ), THENOTE, touchQuad);
                bool hitAATAP = false;

                if (temparc != null)
                {

                    THENOTE = temparc;

                    if (THENOTE.CompareTag("ArcTap"))
                    {
                        /*X > XMin && X < XMax &&
                        Y > YMin && Y < YMax*/
                        float X_pos = THENOTE.transform.position.x;
                        float Y_pos = THENOTE.transform.position.y;
                        float fingerYpos = sky_posY;

                        if (!ArcGameplayManager.hascameramove)
                        {
                            if (fingerYpos > 5.557644f)
                            {
                                fingerYpos = (5.557644f);
                            }
                            else if (fingerYpos < 0.6f)
                            {
                                fingerYpos = 0.6f;
                            }
                            else
                            {
                            }
                        }

                        /*float Xmin = X_pos - 2.3f;
                        float Xmax = X_pos + 2.3f;
                        float Ymin = Y_pos - 1.8f;
                        float Ymax = Y_pos + 1.8f;*/

                        float Xmin = X_pos - 3.3f;
                        float Xmax = X_pos + 3.3f;
                        float Ymin = Y_pos - 6.8f;
                        float Ymax = Y_pos + 6.8f;

                        if ((sky_posX > Xmin && sky_posX < Xmax) && (fingerYpos > Ymin && fingerYpos < Ymax))
                        {
                            hitAATAP = true;
                            //print("yay?");
                            DOARC(THENOTE);
                        }

                    }
                    else if (THENOTE.CompareTag("TapNote"))
                    {

                        DOTAP(THENOTE, sky_posX);
                    }
                }

                if ((touchQuad == 3 || touchQuad == 4) && !hitAATAP)
                {
                    //print("kk");
                    if (sky_posX <= -4.25f && sky_posX >= -8.5f)
                    {
                        if (!HOLD4.activeSelf)
                        {
                            Lane4Hit.SetActive(true);

                        }
                    }
                    else if (sky_posX <= 0.0f && sky_posX >= -4.25f)
                    {
                        if (!HOLD3.activeSelf)
                        {
                            Lane3Hit.SetActive(true);
                        }
                    }
                    else if (sky_posX <= 4.25f && sky_posX >= 0.0f)
                    {
                        if (!HOLD2.activeSelf)
                        {
                            Lane2Hit.SetActive(true);
                        }
                    }
                    else if (sky_posX <= 8.5f && sky_posX >= 4.25f)
                    {
                        if (!HOLD1.activeSelf)
                        {
                            Lane1Hit.SetActive(true);
                        }
                    }
                }



                /*  if (hit.transform.name.Equals("Lane1"))
                  {
                      if (!HOLD1.activeSelf)
                      {
                          Lane1Hit.SetActive(true);
                      }
                      Transform temptap= TAPPOSreturn(new Vector3(OBJ1PFAB.transform.position.x, 0, OBJ1PFAB.transform.position.z), THETAP);
                  if (!(ReferenceEquals(temptap, null)))
                  THETAP = temptap;


                  }
                  else if (hit.transform.name.Equals("Lane2") && !finger.hitsomething)
                  {
                      if (!HOLD2.activeSelf)
                      {
                          Lane2Hit.SetActive(true);
                      }
                      Transform temptap= TAPPOSreturn(new Vector3(OBJ2PFAB.transform.position.x, 0, OBJ2PFAB.transform.position.z), THETAP);
                  if (!(ReferenceEquals(temptap, null)))
                      THETAP = temptap;


              }
                  else if (hit.transform.name.Equals("Lane3") && !finger.hitsomething)
                  {
                      if (!HOLD3.activeSelf)
                      {
                          Lane3Hit.SetActive(true);
                      }
                      Transform temptap = TAPPOSreturn(new Vector3(OBJ3PFAB.transform.position.x, 0, OBJ3PFAB.transform.position.z), THETAP);
                  if (!(ReferenceEquals(temptap, null)))
                      THETAP = temptap;

              }
                  else if (hit.transform.name.Equals("Lane4") && !finger.hitsomething)
                  {
                      if (!HOLD4.activeSelf)
                      {
                          Lane4Hit.SetActive(true);
                      }
                      Transform temptap= TAPPOSreturn(new Vector3(OBJ4PFAB.transform.position.x, 0, OBJ4PFAB.transform.position.z), THETAP);
                  if (!(ReferenceEquals(temptap, null)))
                      THETAP = temptap;

                  }*/
                //}

                /*if (canhit == true)
                {
                    if (ReferenceEquals(THETAP, null) && ReferenceEquals(THENOTE, null))
                    {

                    }                   
                    else if (ReferenceEquals(THETAP, null) && !(ReferenceEquals(THENOTE, null)))
                    {
                        DOARC(THENOTE);
                    }
                    else if (ReferenceEquals(THENOTE, null) && !(ReferenceEquals(THETAP, null)))
                    {
                        DOTAP(THETAP);
                    }
                    else
                    {
                        var DISTANCETAP = Vector3.Distance(TR.origin,THETAP.position);
                        var DISTANCEARC = Vector3.Distance(TR.origin, THENOTE.position);
                        if (DISTANCETAP > DISTANCEARC)
                        {
                            DOARC(THENOTE);
                        }
                        else if (DISTANCEARC > DISTANCETAP)
                        {
                            DOTAP(THETAP);
                        }
                    }
                }*/



                /* else
                 {

                     foreach (var hit in allHits)
                     {

                         // now filter by tag or name
                         if (hit.transform.name.Equals("hitarea"))
                         {
                             if (Time.timeScale != 0f)
                             {
                                 //GameObject SLAQ;
                                 //0.2
                                 float temp = hit.point.y;
                                 if (temp > 5.557644f)
                                 {
                                     temp = (5.557644f);
                                     ARCPOS(new Vector3(hit.point.x, temp, 10), finger);
                                     // SLAQ = Instantiate(SLA, new Vector3(hit.point.x, temp, 2), Quaternion.identity);
                                     /* if (SLAQ.GetComponent<HitATAP>().hitarc)
                                      {
                                          hitsomething = true;
                                      }
                                 }
                                 else if (temp < 0.2f)
                                 {
                                     temp = 0.2f;
                                     ARCPOS(new Vector3(hit.point.x, temp, 10), finger);
                                     //SLAQ = Instantiate(SLA, new Vector3(hit.point.x, temp, 2), Quaternion.identity);
                                     /*if (SLAQ.GetComponent<HitATAP>().hitarc)
                                     {
                                         hitsomething = true;
                                     }
                                 }
                                 else
                                 {
                                     temp = hit.point.y;
                                     ARCPOS(new Vector3(hit.point.x, temp, 10), finger);
                                     //SLAQ = Instantiate(SLA, new Vector3(hit.point.x, temp, 2), Quaternion.identity);
                                     /*if (SLAQ.GetComponent<HitATAP>().hitarc)
                                     {
                                         hitsomething = true;
                                     }
                                 }
                             }
                         }

                         if (hit.transform.name.Equals("Lane1"))
                         {
                             if (!HOLD1.activeSelf)
                             {
                                 Lane1Hit.SetActive(true);
                             }
                             TAPPOS(new Vector3(OBJ1PFAB.transform.position.x, 0, OBJ1PFAB.transform.position.z));
                             // var LA = GameObject.Instantiate(OBJ1PFAB, hit.transform);
                             //LA.GetComponent<BoxCollider>().enabled = true;
                             //var LA = hit.transform.GetChild(0).GetComponent<BoxCollider>();
                             //LA.enabled=true;

                         }
                         else if (hit.transform.name.Equals("Lane2"))
                         {
                             if (!HOLD2.activeSelf)
                             {
                                 Lane2Hit.SetActive(true);
                             }
                             TAPPOS(new Vector3(OBJ2PFAB.transform.position.x, 0, OBJ2PFAB.transform.position.z));
                             //var LA = GameObject.Instantiate(OBJ2PFAB, hit.transform);
                             // LA.GetComponent<BoxCollider>().enabled = true;

                         }
                         else if (hit.transform.name.Equals("Lane3"))
                         {
                             if (!HOLD3.activeSelf)
                             {
                                 Lane3Hit.SetActive(true);
                             }
                             TAPPOS(new Vector3(OBJ3PFAB.transform.position.x, 0, OBJ3PFAB.transform.position.z));
                             //var LA = GameObject.Instantiate(OBJ3PFAB, hit.transform);
                             //LA.GetComponent<BoxCollider>().enabled = true;

                         }
                         else if (hit.transform.name.Equals("Lane4"))
                         {
                             if (!HOLD4.activeSelf)
                             {
                                 Lane4Hit.SetActive(true);
                             }
                             TAPPOS(new Vector3(OBJ4PFAB.transform.position.x, 0, OBJ4PFAB.transform.position.z));
                             //var LA = GameObject.Instantiate(OBJ4PFAB, hit.transform);
                             //LA.GetComponent<BoxCollider>().enabled = true;

                         }
                     }
                 }*/
            }
            /* if (Physics.Raycast(TR.origin, TR.direction, out hit, Mathf.Infinity))
             {

                 //getFinger = finger.GetWorldPosition (4.3f).x;
                 if(hit.collider.gameObject.name=="hitarea"&&Time.timeScale!=0f)
                 Instantiate(SLA, new Vector3(hit.point.x, hit.point.y, 0), Quaternion.identity);

             }*/


            // Call event
            finger.hitsomething = false;
            OnDown.Invoke(finger);
        }

        void DOARC(Transform THEARC)
        {
            bool CCK;
            ArcArcTap ATAPP;
            CanClick Ck = THEARC.gameObject.GetComponent<CanClick>();
            CCK = Ck.canclick;
            bool canrun = true;
            //time = hit.collider.gameObject.GetComponent<CanClick>().startTime;
            ATAPP = Ck.ATAP;
            if (Ck.ATAP.shouldwait)
            {
                if (AMAN.Timing - ArcAudioManager.Instance.AudioOffset <= Ck.ATAP.TIME)
                {
                    //print("NOPEE");
                    //print(Ck.ATAP.TIME);
                    //print(AMAN.Timing);
                    canrun = false;
                }
            }
            if (Ck.ATAP.holdwait)
            {
                if (AMAN.Timing - ArcAudioManager.Instance.AudioOffset <= Ck.ATAP.TIME)
                {
                    //print("NOPEE");
                    //print(Ck.ATAP.TIME);
                    //print(AMAN.Timing);
                    canrun = false;
                }
            }

            if (CCK == true && canrun == true)
            {
                Transform othertransform = THEARC;
                GameObject TRUEARC = THEARC.gameObject;
                GameObject theModel = othertransform.parent.gameObject;

                if (ATAPP.Timing <= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) + 20 + JudgeOffset && ATAPP.Timing >= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) - 50 + JudgeOffset)
                {
                    //print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), false, ATAPP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), true);
                    //AFMAN.PlayArcSound();
                    AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f);
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ArcScoreManager.PPURES += 1;
                    ASMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theModel.transform.parent.gameObject.SetActive(false);

                }
                else if (ATAPP.Timing <= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) + 50 + JudgeOffset && ATAPP.Timing >= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) - 80 + JudgeOffset)
                {
                    //print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), false, ATAPP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), true);
                    //AFMAN.PlayArcSound();
                    if (ATAPP.Timing < AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        if (ArcScoreManager.PUREEORL == true)
                        {
                            AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "LATE");
                        }
                        else { AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "NULL"); }
                        ArcScoreManager.LPURES += 1;
                    }
                    else if (ATAPP.Timing > AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        if (ArcScoreManager.PUREEORL == true)
                        {
                            AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "EARLY");
                        }
                        else { AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "NULL"); }
                        ArcScoreManager.EPURES += 1;
                    }
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ASMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theModel.transform.parent.gameObject.SetActive(false);

                }
                else
                {
                    // print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), true, ATAPP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), true);
                    //AFMAN.PlayArcSound();
                    if (ATAPP.Timing < AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "FAR", 0.15f, "LATE");
                        ArcScoreManager.LFARS += 1;
                    }
                    else if (ATAPP.Timing > AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "FAR", 0.15f, "EARLY");
                        ArcScoreManager.EFARS += 1;
                    }

                    ArcScoreManager.Score += ArcScoreManager.BASESCORE / 2d;
                    ASMAN.PM.enabled = false;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXFARS += 1;
                    ASMAN.CLEARRATE.fillAmount += (ArcScoreManager.fillamount / 2);
                    /*if ((ArcScoreManager.CURRENTPERCENT + (ArcScoreManager.BASEPERCENT / 2f)) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT / 2f;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/
                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theModel.transform.parent.gameObject.SetActive(false);
                }

            }
        }

        void DOTAP(Transform THETAP, float sky_posX)
        {
            bool CCK;
            ArcTap TAP;
            CanClick Ck = THETAP.gameObject.GetComponent<CanClick>();
            CCK = Ck.canclick;
            bool canrun = true;
            bool inLane = false;
            //time = hit.collider.gameObject.GetComponent<CanClick>().startTime;
            TAP = Ck.TAP;

            if ((sky_posX <= -4.25f && sky_posX >= -8.5f) && TAP.Track == 4)
            {
                inLane = true;
            }
            else if ((sky_posX <= 0.0f && sky_posX >= -4.25f) && TAP.Track == 3)
            {
                inLane = true;
            }
            else if ((sky_posX <= 4.25f && sky_posX >= 0.0f) && TAP.Track == 2)
            {
                inLane = true;
            }
            else if ((sky_posX <= 8.5f && sky_posX >= 4.25f) && TAP.Track == 1)
            {
                inLane = true;
            }


            if (Ck.TAP.shouldwait)
            {
                if (AMAN.Timing - ArcAudioManager.Instance.AudioOffset <= Ck.TAP.TIME)
                {
                    //print("NOPEE");
                    //print(Ck.ATAP.TIME);
                    //print(AMAN.Timing);
                    canrun = false;
                }
            }

            if (Ck.TAP.holdwait)
            {
                if (AMAN.Timing - ArcAudioManager.Instance.AudioOffset <= Ck.TAP.TIME)
                {
                    //print("NOPEE");
                    //print(Ck.ATAP.TIME);
                    //print(AMAN.Timing);
                    canrun = false;
                }
            }

            if (CCK == true && canrun == true && inLane)
            {
                Transform othertransform = THETAP;
                GameObject theNote = othertransform.gameObject;
                if (TAP.Timing <= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) + 20 + JudgeOffset && TAP.Timing >= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) - 50 + JudgeOffset)
                {
                    //print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), false, TAP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), false);
                    //AFMAN.PlayArcSound();
                    AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f);
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE + 1;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ArcScoreManager.PPURES += 1;
                    ASMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theNote.SetActive(false);

                }
                else if (TAP.Timing <= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) + 50 + JudgeOffset && TAP.Timing >= (AMAN.Timing - ArcAudioManager.Instance.AudioOffset) - 80 + JudgeOffset)
                {
                    //print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), false, TAP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), false);
                    //AFMAN.PlayArcSound();
                    if (TAP.Timing < AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        if (ArcScoreManager.PUREEORL == true)
                        {
                            AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "LATE");
                        }
                        else { AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "NULL"); }
                        ArcScoreManager.LPURES += 1;
                    }
                    else if (TAP.Timing > AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset)
                    {
                        if (ArcScoreManager.PUREEORL == true)
                        {
                            AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "EARLY");
                        }
                        else { AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "PURE", 0.15f, "NULL"); }
                        ArcScoreManager.EPURES += 1;
                    }
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXPURES += 1;
                    ASMAN.CLEARRATE.fillAmount += ArcScoreManager.fillamount;
                    /*if ((ArcScoreManager.CURRENTPERCENT + ArcScoreManager.BASEPERCENT) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theNote.SetActive(false);

                }
                else
                {
                    // print(time);
                    AFMAN.PlayTapNoteEffectAt(new Vector2(othertransform.position.x, othertransform.position.y), true, TAP.Timing - Mathf.Abs(ArcAudioManager.Instance.AudioOffset), false);
                    // AFMAN.PlayArcSound();
                    if (TAP.Timing < (AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset))
                    {
                        //print("farearl");
                        AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "FAR", 0.15f, "LATE");
                        ArcScoreManager.LFARS += 1;
                    }
                    else if (TAP.Timing > (AMAN.Timing - ArcAudioManager.Instance.AudioOffset + JudgeOffset))
                    {
                        // print("farlate");
                        AFMAN.PlayTIMEEffectAt(new Vector2(othertransform.position.x, othertransform.position.y + 1.2f), "FAR", 0.15f, "EARLY");
                        ArcScoreManager.EFARS += 1;
                    }
                    ArcScoreManager.Score += ArcScoreManager.BASESCORE / 2d;
                    ArcTimingManager.COMBO += 1;
                    ArcScoreManager.MAXFARS += 1;
                    ASMAN.PM.enabled = false;
                    ASMAN.CLEARRATE.fillAmount += (ArcScoreManager.fillamount / 2);
                    /*if ((ArcScoreManager.CURRENTPERCENT + (ArcScoreManager.BASEPERCENT / 2f)) < 100)
                        ArcScoreManager.CURRENTPERCENT += ArcScoreManager.BASEPERCENT / 2f;
                    else
                        ArcScoreManager.CURRENTPERCENT = 100;*/

                    ATMAN.noteQuad1.Remove(Ck);
                    ATMAN.noteQuad2.Remove(Ck);
                    ATMAN.noteQuad3.Remove(Ck);
                    ATMAN.noteQuad4.Remove(Ck);
                    theNote.SetActive(false);
                }
                //finger.hitsomething = true;

                if (!(ReferenceEquals(TAP.LINE, null)))
                {
                    TAP.LINE.transform.parent = null;
                    TAP.LINE.gameObject.SetActive(false);
                }

            }
        }

    }
}