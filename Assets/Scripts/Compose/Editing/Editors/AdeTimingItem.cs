using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Arcaoid.Gameplay.Chart;
using Arcaoid.Compose.Command;

namespace Arcaoid.Compose.Editing
{
    public class AdeTimingItem : MonoBehaviour
    {
        public InputField ItemInputField;
        public ArcTiming TimingReference;
        public RectTransform RectTransform;
        public Button AddBtn, RemoveBtn;

        public string Text
        {
            get
            {
                return ItemInputField.text;
            }
            set
            {
                ItemInputField.text = value;
            }
        }
        public void Add()
        {
            AdeTimingEditor.Instance.Add(TimingReference);
        }
        public void Delete()
        {
            AdeTimingEditor.Instance.Delete(TimingReference);
        }
        public void OnEndEdit()
        {
            try
            {
                string[] splits = Text.Split(',');
                int timing = int.Parse(splits[0]);
                float bpm = float.Parse(splits[1]);
                float beat = float.Parse(splits[2]);
                ArcTiming n = TimingReference.Clone() as ArcTiming;
                n.Timing = timing;
                n.Bpm = bpm;
                n.BeatsPerLine = beat;
                CommandManager.Instance.Add(new EditArcEventCommand(TimingReference, n));
            }
            catch (Exception)
            {

            }
        }
    }
}