using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Arcaoid.Gameplay;

namespace Arcaoid.Compose.Dialog
{
    public class SkinPreference
    {
        public Dictionary<string, int> SkinValues = new Dictionary<string, int>()
        {
            {"Tap",0 },
            {"Hold",0 },
            {"ArcTap",0 },
            {"Track",0 },
            {"CriticalLine",0},
            {"Background",2 }
        };
    }

    [Serializable]
    public class DropdownDictionary
    {
        public Dropdown[] dropdowns;
        public Dropdown this[string name]
        {
            get
            {
                foreach (var d in dropdowns) if (d.name == name) return d;
                return null;
            }
        }
    }

    public class AdeSkinDialog : MonoBehaviour
    {
        public static AdeSkinDialog Instance { get; private set; }
        public string PreferencesSavePath
        {
            get
            {
                return ArcaoidComposeManager.ArcadePersistentFolder + "/Skin.json";
            }
        }
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            //LoadPreferences();
        }

        private SkinPreference p = new SkinPreference();
        private bool shouldSave = true;
        public DropdownDictionary Dropdowns = new DropdownDictionary();

        public void OnItemSelect(Dropdown dropdown)
        {
            string name = dropdown.name;
            if (shouldSave) p.SkinValues[name] = dropdown.value;
            switch (name)
            {
                case "Tap":
                    ArcSkinManager.Instance.SetTapNoteSkin(dropdown.value);
                    break;
                case "Hold":
                    ArcSkinManager.Instance.SetHoldNoteSkin(dropdown.value);
                    break;
                case "ArcTap":
                    ArcSkinManager.Instance.SetArcTapSkin(dropdown.value);
                    break;
                case "Track":
                    ArcSkinManager.Instance.SetTrackSkin(dropdown.value);
                    break;
                case "CriticalLine":
                    ArcSkinManager.Instance.SetCriticalLineSkin(dropdown.value);
                    break;
                case "Background":
                    ArcSkinManager.Instance.SetBackgroundSkin(dropdown.options[dropdown.value].image);
                    break;
            }
        }

        public void LoadPreferences()
        {
            try
            {
                p = JsonConvert.DeserializeObject<SkinPreference>(File.ReadAllText(PreferencesSavePath));
            }
            catch (Exception)
            {
                p = new SkinPreference();
            }
            finally
            {
                shouldSave = false;
                foreach (var s in p.SkinValues) Dropdowns[s.Key].value = s.Value;
                shouldSave = true;
            }
        }
        public void SavePreferences()
        {
            File.WriteAllText(PreferencesSavePath, JsonConvert.SerializeObject(p));
        }
        private void OnApplicationQuit()
        {
            SavePreferences();
        }
    }
}