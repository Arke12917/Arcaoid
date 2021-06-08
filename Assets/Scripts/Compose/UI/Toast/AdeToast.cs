using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Schwarzer.UnityExtension;
using DG.Tweening;

namespace Arcaoid.Compose
{
    public class AdeToast : MonoBehaviour
    {
        public const float Duration = 0.4f;
        public const float Stay = 1f;
        public static AdeToast Instance { get; private set; }
        public RectTransform ToastRect;
        public RectTransform ContentRect;
        public Text Content;

        private void Awake()
        {
            Instance = this;
        }
        public void Show(string text)
        {
           /* ToastRect.DOKill(true);
            float height = Content.CalculateHeight(text);
            ContentRect.sizeDelta = new Vector2(-100, height + 30);
            Content.text = text;
            ToastRect.DOAnchorPosY(691 - ArcaoidComposeManager.Instance.BarHeight - 30 - height, Duration).OnComplete(() =>
             {
                 ToastRect.DOAnchorPosY(691, Duration).SetDelay(Stay * height / 30);
             });*/
        }
    }
}