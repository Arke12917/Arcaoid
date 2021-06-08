using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Schwarzer.UnityExtension;

namespace Arcaoid.Gameplay
{
    public class ArcSkinManager : MonoBehaviour
    {
        public static ArcSkinManager Instance { get; private set; }

        [Header("Tap")]
        public Sprite[] TapSkins = new Sprite[2];
        public Color[] ConnectionLineColors = new Color[2];

        [Header("Hold")]
        public Sprite[] HoldSkins = new Sprite[2];
        public Sprite[] HoldHighlightSkins = new Sprite[2];

        [Header("ArcTap")]
        public Sprite[] ArcTapSkins = new Sprite[2];

        [Header("Track")]
        public SpriteRenderer Track;
        public Sprite[] TrackSkins = new Sprite[5];

        [Header("Critical Line")]
        public SpriteRenderer[] CriticalLines = new SpriteRenderer[4];
        public Sprite[] CriticalLineSkins = new Sprite[2];

        [Header("Background")]
        public Image Background;
        private void Awake()
        {
            Instance = this;
        }

        public void SetTapNoteSkin(int index)
        {
            //if (TapSkins.OutOfRange(index)) return;

            ArcTapNoteManager.Instance.TapNotePrefab.GetComponent<SpriteRenderer>().sprite = TapSkins[index];
            foreach (var t in ArcTapNoteManager.Instance.Taps) t.spriteRenderer.sprite = TapSkins[index];

            if (index < 0 || index > ConnectionLineColors.Length - 1) return;
            //ArcArcManager.Instance.ConnectionColor = ConnectionLineColors[index];
            //foreach (var t in ArcTapNoteManager.Instance.Taps)
               // foreach (var l in t.ConnectionLines)
                    //l.startColor = l.endColor = ConnectionLineColors[index];
        }
        public void SetHoldNoteSkin(int index)
        {
            //if (HoldSkins.OutOfRange(index)) return;

            ArcHoldNoteManager.Instance.HoldNotePrefab.GetComponent<SpriteRenderer>().sprite = HoldSkins[index];
            ArcHoldNoteManager.Instance.DefaultSprite = HoldSkins[index];
            ArcHoldNoteManager.Instance.HighlightSprite = HoldHighlightSkins[index];
            foreach (var h in ArcHoldNoteManager.Instance.Holds) h.ReloadSkin();
        }
        public void SetArcTapSkin(int index)
        {
            //if (ArcTapSkins.OutOfRange(index)) return;

            ArcArcManager.Instance.ArcTapSkin = ArcTapSkins[index].texture;
            foreach (var a in ArcArcManager.Instance.Arcs)
                foreach (var at in a.ArcTaps)
                {

                }
                    //at.ReloadSkin();
        }
        public void SetTrackSkin(int index)
        {
            if (TrackSkins.OutOfRange(index)) return;

            Track.sprite = TrackSkins[index];
        }
        public void SetCriticalLineSkin(int index)
        {
            if (CriticalLineSkins.OutOfRange(index)) return;

            foreach (var c in CriticalLines) c.sprite = CriticalLineSkins[index];
        }
        public void SetBackgroundSkin(Sprite sprite)
        {
            Background.sprite = sprite;
        }
    }
}
