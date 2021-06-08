using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcaoid.Gameplay
{
    public class ArcTapNoteEffectComponent : MonoBehaviour
    {
        public bool Available { get; set; } = true;
        public ParticleSystem Effect;
        public Material current;
        public Material PURE;
        public Material FAR;
        public Material LOST;
        public Material EARLY;
        public Material LATE;
        public ParticleSystemRenderer PSRENDER;

        public void Awake()
        {
            PSRENDER = gameObject.GetComponent<ParticleSystemRenderer>();
        }

        public Vector3 eorl = new Vector3(0, 4.86f, 0);

        public void PlayAt(Vector2 pos)
        {
            Available = false;
            transform.position = pos;
            Effect.Play();

            StartCoroutine(WaitForEnd());
        }

        public IEnumerator PlayTIME(Vector2 pos, string TYPE, float time, string EorL)
        {
            Available = false;
            yield return new WaitForSeconds(time);
            if (EorL == "NULL")
            {
                transform.position = pos;
                //current=PSRENDER.material;
                if (TYPE == "PURE")
                {
                    PSRENDER.material = PURE;
                }
                else if (TYPE == "FAR")
                {
                    PSRENDER.material = FAR;
                }
                else if (TYPE == "LOST")
                {
                    PSRENDER.material = LOST;
                }
                else if (TYPE == "LATE")
                {
                    PSRENDER.material = LATE;
                }
                else if (TYPE == "EARLY")
                {
                    PSRENDER.material = EARLY;
                }
            }
            else
            {
                transform.position = eorl;
                if (EorL == "EARLY")
                {
                    PSRENDER.material = EARLY;
                }
                else { PSRENDER.material = LATE; }
            }

            Effect.Play();

            StartCoroutine(WaitForEnd());
        }


        IEnumerator WaitForEnd()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Effect.Stop();
            Effect.Clear();
            Available = true;
            yield break;
            
        }
    }
}