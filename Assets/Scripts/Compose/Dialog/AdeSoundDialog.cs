using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace Arcaoid.Compose.Dialog
{
    [Serializable]
    public class SoundPreferences
    {
        public float Chart = 0.7f;
        public float Effect = 0f;
    }
    public class AdeSoundDialog : MonoBehaviour
    {
        public static AdeSoundDialog Instance { get; private set; }

        public GameObject View;
        public InputField ChartAudio, EffectAudio;
        public AudioSource ChartSource, EffectSource;
        private SoundPreferences preferences;
        public string PreferencesSavePath
        {
            get
            {
                return ArcaoidComposeManager.ArcadePersistentFolder + "/Sound.json";
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Load();
        }
        private void OnDestroy()
        {
            //Save();
        }
        private void Load()
        {
            try
            {
                preferences = JsonConvert.DeserializeObject<SoundPreferences>(File.ReadAllText(PreferencesSavePath));
            }
            catch (Exception)
            {
                preferences = new SoundPreferences();
            }
            finally
            {
                ChartAudio.text = preferences.Chart.ToString();
                EffectAudio.text = preferences.Effect.ToString();
                //ChartSource.volume = preferences.Chart;
                //EffectSource.volume = preferences.Effect;
            }
        }
        private void Save()
        {
            File.WriteAllText(PreferencesSavePath, JsonConvert.SerializeObject(preferences));
        }

        public void OnChartAudioChange()
        {
            try
            {
                float val = float.Parse(ChartAudio.text);
                val = Mathf.Clamp(val, 0, 1);
                preferences.Chart = val;
                //ChartSource.volume = val;
                //Save();
            }
            catch (Exception)
            {
                ChartAudio.text = preferences.Chart.ToString();
            }
        }
        public void OnEffectAudioChange()
        {
            try
            {
                float val = float.Parse(EffectAudio.text);
                val = Mathf.Clamp(val, 0, 1);
                preferences.Effect = val;
                //EffectSource.volume = val;
                //Save();
            }
            catch (Exception)
            {
                EffectAudio.text = preferences.Effect.ToString();
            }
        }

        public void SwitchStatus()
        {
            View.SetActive(!View.activeSelf);
        }
    }
}