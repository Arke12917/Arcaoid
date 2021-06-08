using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using System;
using UnityEngine.UI;


namespace Arcaoid.Compose
{
    [Serializable]
    public class AdeCameraProperty
    {
        public bool Orthographic;
        public float OrthographicSize;
        public Vector3 Position;
        public Vector3 Rotation;
    }
    public class AdeCameraManager : MonoBehaviour
    {
        public AdeCameraProperty[] Properties;

        private int currentCameraStatus = 0;

        public void SwitchCameraStatus()
        {
            currentCameraStatus = currentCameraStatus + 1;
            if (currentCameraStatus >= Properties.Length) currentCameraStatus = 0;
            if (currentCameraStatus != 0)
            {
                ArcCameraManager.Instance.EditorCamera = true;
                ArcCameraManager.Instance.EditorCameraPosition = Properties[currentCameraStatus].Position;
                ArcCameraManager.Instance.EditorCameraRotation = Properties[currentCameraStatus].Rotation;
                ArcCameraManager.Instance.GameplayCamera.orthographic = Properties[currentCameraStatus].Orthographic;
                ArcCameraManager.Instance.GameplayCamera.orthographicSize = Properties[currentCameraStatus].OrthographicSize;
            }
            else
            {
                ArcCameraManager.Instance.EditorCamera = false;
                ArcCameraManager.Instance.GameplayCamera.orthographic = false;
            }
        }
    }
}