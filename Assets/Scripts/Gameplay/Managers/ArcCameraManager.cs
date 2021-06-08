using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;

namespace Arcaoid.Gameplay
{
    public class ArcCameraManager : MonoBehaviour
    {
        public Camera GameplayCamera;
        public Transform GAMEplayCamera;
        public Camera Stationary;
        public Transform SkyInputLabel;
        public static ArcCameraManager Instance { get; private set; }

        [HideInInspector]
        public float CurrentTilt = 0;

        [HideInInspector]
        public List<ArcCamera> Cameras = new List<ArcCamera>();

        [HideInInspector]
        public bool IsReset = true;

        public Vector3 ResetPosition
        {
            get
            {
                return new Vector3(0, 9, Is16By9 || Is18By9 ? 9 : 8);
            }
        }
        public Vector3 ResetRotation
        {
            get
            {
                return new Vector3(Is16By9 || Is18By9 ? 26.565f : 27.378f, 180, 0);
            }
        }

        public bool Is16By9
        {
            get
            {
                return Mathf.Abs((1f * GameplayCamera.pixelWidth / GameplayCamera.pixelHeight) - (16f / 9f)) < 0.1f;
            }
        }

        public bool Is18By9
        {
            get
            {
                return Mathf.Abs((1f * GameplayCamera.pixelWidth / GameplayCamera.pixelHeight) - (18f / 9f)) < 0.2f;
            }
        }


        private void Awake()
        {
            Instance = this;
            GAMEplayCamera = GameplayCamera.transform;
        }
        private void Start()
        {
            ResetCamera();
            ResetotherCamera();
            
        }

        public void Clean()
        {
            Cameras.Clear();
        }
        public void Load(List<ArcCamera> cameras)
        {
            if(Mathf.Sign(ArcAudioManager.Instance.AudioOffset) == -1)
            {
                foreach(var cam in cameras)
                {
                    cam.Timing += ArcAudioManager.Instance.AudioOffset;
                }
            }
            Cameras = cameras;
        }

        public void ResetCamera()
        {
            GameplayCamera.fieldOfView = Is16By9||Is18By9 ? 50 : 65;
            GameplayCamera.nearClipPlane = 0.3f;
            GameplayCamera.farClipPlane = 100;
            SkyInputLabel.localPosition = new Vector3(Is16By9 || Is18By9 ? -7.1f : -6.5f, 0.1f, 0);
            GAMEplayCamera.position = new Vector3(0, 9, Is16By9 || Is18By9 ? 9 : 8);
            GAMEplayCamera.LookAt(new Vector3(0, -5.5f, -20), new Vector3(0, 1, 0));
            IsReset = true;
        }

        public void ResetotherCamera()
        {
            Stationary.fieldOfView = Is16By9||Is18By9 ? 50 : 65;
            Stationary.nearClipPlane = 0.3f;
            Stationary.farClipPlane = 100;
            SkyInputLabel.localPosition = new Vector3(Is16By9 || Is18By9 ? -7.1f : -6.5f, 0.1f, 0);
            Stationary.transform.position = new Vector3(0, 9, Is16By9 || Is18By9 ? 9 : 8);
            Stationary.transform.LookAt(new Vector3(0, -5.5f, -20), new Vector3(0, 1, 0));
            IsReset = true;

        }

        private void Update()
        {
            //if (UpdateEditorCamera()) return;
            UpdateCameraPosition();
            //UpdateCameraTilt();
        }

        public bool EditorCamera { get; set; }
        public Vector3 EditorCameraPosition { get; set; }
        public Vector3 EditorCameraRotation { get; set; }
        private bool UpdateEditorCamera()
        {
            if(EditorCamera)
            {
                GameplayCamera.transform.localPosition = EditorCameraPosition;
                GameplayCamera.transform.localRotation = Quaternion.Euler(EditorCameraRotation);
                return true;
            }
            return false;
        }

        private void UpdateCameraPosition()
        {
            int currentTiming = ArcGameplayManager.Instance.Timing;
            for (int i = 0; i < Cameras.Count; ++i)
            {
                ArcCamera c = Cameras[i];
                if (c.Timing > currentTiming) break;
                c.Update(currentTiming);
                if (c.CameraType == Chart.CameraType.Reset)
                {
                    for (int r = 0; r < i; ++r)
                    {
                        ArcCamera cr = Cameras[r];
                        cr.Update(c.Timing);
                    }
                }
            }
            Vector3 position = ResetPosition;
            Vector3 rotation = ResetRotation;
            foreach (var c in Cameras)
            {
                if (c.Timing > currentTiming) break;
                IsReset = c.CameraType == Chart.CameraType.Reset;
                if (IsReset)
                {
                    position = ResetPosition;
                    rotation = ResetRotation;
                }
                position += new Vector3(-c.Move.x, c.Move.y, c.Move.z) * c.Percent / 100;
                rotation += new Vector3(-c.Rotate.y, -c.Rotate.x, c.Rotate.z) * c.Percent;
            }
            if(!float.IsNaN(position.x)&& !float.IsNaN(position.y)&& !float.IsNaN(position.z))
            GAMEplayCamera.localPosition = position;

            if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z))
                GAMEplayCamera.localRotation = Quaternion.Euler(0, 0, rotation.z) * Quaternion.Euler(rotation.x, rotation.y, 0);
        }
        private void UpdateCameraTilt()
        {
            if (!IsReset) return;
            float currentArcPos = ArcGameplayManager.Instance.Auto ? -ArcArcManager.Instance.ArcJudgePos : 0;
            float pos = Mathf.Clamp(currentArcPos / 4.25f, -1, 1) * 0.05f;
            float delta = pos - CurrentTilt;
            float speed = ArcGameplayManager.Instance.IsPlaying ? (currentArcPos == 0 ? 0.02f : 0.04f) : 0;
            CurrentTilt = CurrentTilt + speed * delta;
            GameplayCamera.transform.LookAt(new Vector3(0, -5.5f, -20), new Vector3(CurrentTilt, 1 - CurrentTilt, 0));
        }
    }
}
