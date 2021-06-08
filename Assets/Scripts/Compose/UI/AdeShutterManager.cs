using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Arcaoid.Compose
{
    public class AdeShutterManager : MonoBehaviour
    {
        public const float Duration = 0.65f;
        public static AdeShutterManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        public RectTransform Left, Right;
        public AudioClip CloseAudio, OpenAudio; 

        public void Open()
        {
            
        }
        public void Close()
        {
            
        }
        public IEnumerator OpenCoroutine()
        {
            //Open();
            //print("opened!");
            //ArcaoidComposeManager.readytoshift = true;
            ArcaoidComposeManager.Instance.startshifting();
            GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().OPENMENU();
            ArcaoidComposeManager.Instance.OnPauseClicked();
            yield return new WaitForSecondsRealtime(Duration);
        }
        public IEnumerator CloseCoroutine()
        {
            //Close();
            yield return new WaitForSecondsRealtime(Duration);
        }
    }
}