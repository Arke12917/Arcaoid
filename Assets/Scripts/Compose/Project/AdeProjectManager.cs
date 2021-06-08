using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Arcaoid.Compose.Dialog;
//using Arcaoid.Compose.Feature;
using Arcaoid.Compose.UI;
using Arcaoid.Gameplay;
using Arcaoid.Gameplay.Chart;
using Newtonsoft.Json;
using Schwarzer.Mp3Converter;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Arcaoid.Compose
{
    [Serializable]
    public class AdeChartDifficulty
    {
        public string Rating;
        public string ChartPath;
        public string Designer;
    }
   
    public class ArcadeProject
    {
        public string Title;
        public string Artist;
        public float BaseBpm;
        public string AudioPath;
        public AdeChartDifficulty[] Difficulties = new AdeChartDifficulty[3];

        public int LastWorkingDifficulty = 2;
        public int LastWorkingTiming;

        [JsonIgnore]
        public AudioClip AudioClip;
        [JsonIgnore]
        public Texture2D Cover;
        [JsonIgnore]
        public Sprite CoverSprite;
    }

    public class AdeProjectManager : MonoBehaviour
    {
        public static AdeProjectManager Instance { get; private set; }

        public string CurrentProjectFolder { get; set; }
        public ArcadeProject CurrentProject { get; set; }
        public int CurrentDifficulty { get; set; } = 2;

        public Sprite DefaultCover;
        public Image CoverImage;
        public Image[] DifficultyImages;
        public InputField Name, Composer, Diff, BaseBpm, AudioOffset;
        public Text OpenLabel;
        public Text SaveMode;
        public Text SONGTEXT;

        public Color EnableColor, DisableColor;
        public Image FileWatchEnableImage;
        public ArcadeProject ACPRJT;

        //private FileSystemWatcher watcher = new FileSystemWatcher();
        private bool shouldReload = false;

        public string ProjectFilePath
        {
            get
            {
                return $"{CurrentProjectFolder}/Arcade/Project.arcade";
            }
        }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
           // watcher.Changed += OnWatcherChanged;

            //watcher.EnableRaisingEvents = true;
            //StartCoroutine(AutosaveCoroutine());
        }
        private void Update()
        {
            /*if (shouldReload)
            {
                ReloadChart(CurrentDifficulty);
                ArcGameplayManager.Instance.Timing = ArcGameplayManager.Instance.Chart.LastEventTiming - 500;
                shouldReload = false;
            }
           /* if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SaveProject();
                }
            }*/
        }

        private void InitializeProject(string folder)
        {
            ACPRJT = new ArcadeProject();
            //File.WriteAllText(ProjectFilePath, JsonConvert.SerializeObject(p));
        }
        private void CreateDirectories(string folder)
        {
            string[] directories = new string[] { $"{folder}/Arcade", $"{folder}/Arcade/Autosave", $"{folder}/Arcade/Converting", $"{folder}/Arcade/Backup" };
            foreach (var s in directories) if (!Directory.Exists(s)) Directory.CreateDirectory(s);
        }

        public void CleanProject()
        {
            if (CurrentProject == null) return;
            if (CurrentProject.AudioClip != null) Destroy(CurrentProject.AudioClip);
            if (CurrentProject.Cover != null) Destroy(CurrentProject.Cover);
            if (CurrentProject.CoverSprite != null) Destroy(CurrentProject.CoverSprite);
            foreach (Image i in DifficultyImages) i.color = new Color(1f, 1f, 1f, 0.6f);
            CoverImage.sprite = DefaultCover;
            Name.text = "";
            Composer.text = "";
            Diff.text = "";
            Name.interactable = false;
            Composer.interactable = false;
            Diff.interactable = false;
            AdeTimingSlider.Instance.Enable = false;
            OpenLabel.color = new Color(1, 1, 1, 1);
            BaseBpm.interactable = false;
            AudioOffset.interactable = false;
           // watcher.EnableRaisingEvents = false;
           // FileWatchEnableImage.color = DisableColor;
            ArcGameplayManager.Instance.Clean();
        }
        public void OpenProject()
        {
            try
            {
                string folder = null;
                if(Application.platform==RuntimePlatform.WindowsEditor|| Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    //folder = Schwarzer.Windows.Dialog.OpenFolderDialog("选择您的 Arcaoid 自制谱文件夹 (包含 0/1/2.aff, base.mp3/ogg/wav, base.jpg)");
                    folder = Application.persistentDataPath + "/" + LOADMENU.FOLDERNAME;
                    //print(folder);
                }else if (Application.platform == RuntimePlatform.IPhonePlayer||Application.platform== RuntimePlatform.Android)
                {
                    folder = Application.persistentDataPath+"/"+ LOADMENU.FOLDERNAME;
                }
                
                if (folder == null) return;
                CleanProject();
               // CreateDirectories(folder);
                CurrentProjectFolder = folder;
                if (ACPRJT==null) InitializeProject(folder);
                try
                {
                    CurrentProject = ACPRJT;

                }
                catch (Exception)
                {
                    CurrentProject = new ArcadeProject();
                }

                StartCoroutine(LoadingCoroutine());
            }
            catch (Exception)
            {
                CurrentProject = null;
                CurrentProjectFolder = null;
            }
        }
        public void SaveProject()
        {
            /*if (CurrentProject == null || CurrentProjectFolder == null) return;
            CurrentProject.LastWorkingDifficulty = CurrentDifficulty;
            CurrentProject.LastWorkingTiming = 0;
            File.WriteAllText(ProjectFilePath, JsonConvert.SerializeObject(CurrentProject));
            string path = CurrentProjectFolder + $"/{CurrentDifficulty}.aff";
            string backupPath = CurrentProjectFolder + $"/Arcade/Backup/{CurrentDifficulty}_{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}.aff";
            File.Copy(path, backupPath);
            FileStream fs = new FileStream(path, FileMode.Create);
            ArcGameplayManager.Instance.Chart.Serialize(fs, ArcaoidComposeManager.Instance.ArcadePreference.ChartSortMode);
           
            fs.Close();*/
        }

        private IEnumerator AutosaveCoroutine()
        {
            yield return new WaitForSecondsRealtime(5000f);
            /*while (true)
            {
                yield return new WaitForSeconds(30f);
                if (CurrentProject == null || CurrentProjectFolder == null) continue;
                string backupPath = CurrentProjectFolder + $"/Arcade/Autosave/{CurrentDifficulty}_{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}.aff";
                FileStream fs = new FileStream(backupPath, FileMode.Create);
                ArcGameplayManager.Instance.Chart.Serialize(fs, ArcaoidComposeManager.Instance.ArcadePreference.ChartSortMode);
                fs.Close();
            }*/
        }

        private IEnumerator LoadChartCoroutine(int index, bool shutter)
        {
            if (CurrentProject == null || CurrentProjectFolder == null || CurrentProject.AudioClip == null)
            {
                yield break;
            }

            ArcaoidComposeManager.firstloaded = true;
            ArcaoidComposeManager.readytoshift = false;

            if (!File.Exists($"{CurrentProjectFolder}/{index}.aff"))
            {
                //File.WriteAllText($"{CurrentProjectFolder}/{index}.aff", "AudioOffset:0\n-\ntiming(0,100.00,4.00);");
                yield return null;
            }

            if (shutter) yield return AdeShutterManager.Instance.CloseCoroutine();
            ArcaoidComposeManager.Instance.Pause();
           // AdeObsManager.Instance.ForceClose();

            Aff.ArcaoidAffReader reader = null;
            try
            {
                reader = new Aff.ArcaoidAffReader($"{CurrentProjectFolder}/{index}.aff");
            }
            catch (Aff.ArcaoidAffFormatException Ex)
            {
              
                reader = null;
            }
            catch (Exception Ex)
            {
                
                reader = null;
            }
            if (reader == null)
            {
                if (shutter) yield return AdeShutterManager.Instance.OpenCoroutine();
                yield break;
            }
            ArcGameplayManager.Instance.Load(new Gameplay.Chart.ArcChart(reader), CurrentProject.AudioClip);
            CurrentDifficulty = index;

            //Diff.text = CurrentProject.Difficulties[CurrentDifficulty] == null ? "" : CurrentProject.Difficulties[CurrentDifficulty].Rating;
            //foreach (Image i in DifficultyImages) i.color = new Color(1f, 1f, 1f, 0.6f);
            //DifficultyImages[index].color = new Color(1, 1, 1, 1);

            //AudioOffset.interactable = true;
            //AudioOffset.text = ArcAudioManager.Instance.AudioOffset.ToString();

            //watcher.Path = CurrentProjectFolder;
            //watcher.Filter = $"{index}.aff";

            if (shutter) yield return AdeShutterManager.Instance.OpenCoroutine();
        }
        private IEnumerator LoadCoverCoroutine()
        {
            if (!File.Exists($"{CurrentProjectFolder}/base.jpg"))
            {
                CoverImage.sprite = DefaultCover;
                yield break;
            }
            string path = $"{CurrentProjectFolder}/base.jpg";
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(Uri.EscapeUriString("file:///" + path.Replace("\\", "/")));
            yield return req.SendWebRequest();
            if (!string.IsNullOrWhiteSpace(req.error))
            {
                CoverImage.sprite = DefaultCover;
                yield break;
            }
            CurrentProject.Cover = DownloadHandlerTexture.GetContent(req);
            CurrentProject.CoverSprite = Sprite.Create(CurrentProject.Cover, new Rect(0, 0, CurrentProject.Cover.width, CurrentProject.Cover.height), new Vector2(0.5f, 0.5f));
            CoverImage.sprite = CurrentProject.CoverSprite;
        }
        private IEnumerator LoadMusicCoroutine()
        {
            string[] searchPaths = new string[] {$"{CurrentProjectFolder}/base.wav", $"{CurrentProjectFolder}/base.ogg"};
            string path = null;
            foreach (var s in searchPaths)
            {
                if (File.Exists(s)) path = s;
            }
            if (path == null)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (File.Exists($"{CurrentProjectFolder}/base.mp3"))
                    {
                        Task converting = Task.Run(() => Mp3Converter.Mp3ToWav($"{CurrentProjectFolder}/base.mp3", $"{CurrentProjectFolder}/base.wav"));
                        while (!converting.IsCompleted) yield return null;
                        if (converting.Status == TaskStatus.RanToCompletion)
                        {
                            path = $"{CurrentProjectFolder}/base.wav";
                        }
                    }
                }
                else
                {
                    path = $"{CurrentProjectFolder}/base.wav";
                }
            }
            if (path == null)
            {
                
                yield break;
            }
            UnityWebRequest req=null;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                 req = UnityWebRequestMultimedia.GetAudioClip(Uri.EscapeUriString("file:///" + path.Replace("\\", "/")), path.EndsWith("wav") ? AudioType.WAV : AudioType.OGGVORBIS);
            }
            else
            {
                 req = UnityWebRequestMultimedia.GetAudioClip(Uri.EscapeUriString("file:///" + path.Replace("\\", "/")), path.EndsWith("wav") ? AudioType.WAV : AudioType.OGGVORBIS);
            }

            yield return req.SendWebRequest();
            if (!string.IsNullOrWhiteSpace(req.error))
            {
                yield break;
            }
            CurrentProject.AudioClip = DownloadHandlerAudioClip.GetContent(req);
            ArcAudioManager.Instance.SourceACTUAL.clip= DownloadHandlerAudioClip.GetContent(req);
            AdeTimingSlider.Instance.Enable = true;
            AdeTimingSlider.Instance.Length = (int)(CurrentProject.AudioClip.length * 1000);
        }
        private IEnumerator LoadingCoroutine()
        {
            //yield return AdeShutterManager.Instance.CloseCoroutine();
            ArcaoidComposeManager.Instance.Songname.text = LOADMENU.finalname;
            ArcaoidComposeManager.Instance.Composer.text = LOADMENU.composer;
            if (LOADMENU.currentdiff == 2)
            {
                ArcaoidComposeManager.Instance.Difficulty.text = "Future "+LOADMENU.finaldiff;
                CurrentProject.LastWorkingDifficulty = 2;
            }
            else if(LOADMENU.currentdiff == 1)
            {
                ArcaoidComposeManager.Instance.Difficulty.text = "Present " + LOADMENU.finaldiff;
                CurrentProject.LastWorkingDifficulty = 1;
            }
            else if (LOADMENU.currentdiff == 0)
            {
                ArcaoidComposeManager.Instance.Difficulty.text = "Past " + LOADMENU.finaldiff;
                CurrentProject.LastWorkingDifficulty = 0;
            }

            yield return LoadCoverCoroutine();
            yield return LoadMusicCoroutine();
            yield return LoadChartCoroutine(CurrentProject.LastWorkingDifficulty, false);

            //Name.text = CurrentProject.Title;
            //Composer.text = CurrentProject.Artist;
            //Diff.text = "";
           // Name.interactable = true;
            //Composer.interactable = true;
            //Diff.interactable = true;
            //OpenLabel.color = new Color(0, 0, 0, 0);

            ArcTimingManager.Instance.BaseBpm = CurrentProject.BaseBpm == 0 ? 100 : CurrentProject.BaseBpm;
            //BaseBpm.interactable = true;
           // BaseBpm.text = ArcTimingManager.Instance.BaseBpm.ToString();

            //watcher.EnableRaisingEvents = false;
            //FileWatchEnableImage.color = DisableColor;
            ArcGameplayManager.Instance.Timing = CurrentProject.LastWorkingTiming;

            yield return AdeShutterManager.Instance.OpenCoroutine();
           // GameObject.FindGameObjectWithTag("SHUTTERMNG").GetComponent<SwitchScene>().OPEN();
        }
        public void ReloadChart(int index)
        {
            StartCoroutine(LoadChartCoroutine(index, true));
        }

        public void OnComposerEdited()
        {
            if (CurrentProject == null) return;
            CurrentProject.Artist = Composer.text;
        }
        public void OnNameEdited()
        {
            if (CurrentProject == null) return;
            CurrentProject.Title = Name.text;
        }
        public void OnDiffEdited()
        {
            if (CurrentProject == null) return;
            if (CurrentDifficulty < 0 || CurrentDifficulty > 2) return;
            if (CurrentProject.Difficulties[CurrentDifficulty] == null)
                CurrentProject.Difficulties[CurrentDifficulty] = new AdeChartDifficulty();
            CurrentProject.Difficulties[CurrentDifficulty].Rating = Diff.text;
        }
        public void OnBaseBpmEdited()
        {
            float value;
            bool result = float.TryParse(BaseBpm.text, out value);
            if (result)
            {
                if (value <= 0) value = 100;
                if (CurrentProject != null) CurrentProject.BaseBpm = value;
                ArcTimingManager.Instance.BaseBpm = value;
                File.WriteAllText(ProjectFilePath, JsonConvert.SerializeObject(CurrentProject));
                ArcArcManager.Instance.Rebuild();
                BaseBpm.text = value.ToString();
            }
        }
        public void OnAudioOffsetEdited()
        {
            int value;
            bool result = int.TryParse(AudioOffset.text, out value);
            if (result)
            {
                ArcAudioManager.Instance.AudioOffset = value;
                AudioOffset.text = value.ToString();
            }
        }
       /* public void OnFileWatchClicked()
        {
            if (CurrentProject != null && CurrentDifficulty != -1)
            {
                //watcher.EnableRaisingEvents = !watcher.EnableRaisingEvents;
                //FileWatchEnableImage.color = watcher.EnableRaisingEvents ? EnableColor : DisableColor;
            }
        }*/

       /* private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            DateTime begin = DateTime.Now;
            FileStream fs = null;

            retry:
            try
            {
                fs = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                if (DateTime.Now - begin < TimeSpan.FromSeconds(3))
                    goto retry;
                else return;
            }

            fs?.Close();

            shouldReload = true;
        }

        public void OnSaveModeClicked()
        {
            ArcaoidComposeManager.Instance.ArcadePreference.ChartSortMode = ArcaoidComposeManager.Instance.ArcadePreference.ChartSortMode == ChartSortMode.Timing ? ChartSortMode.Type : ChartSortMode.Timing;
            SaveMode.text = ArcaoidComposeManager.Instance.ArcadePreference.ChartSortMode == ChartSortMode.Timing ? "Time" : "Type";
        }
        public void OnOpenFolder()
        {
            if (CurrentProject == null || string.IsNullOrWhiteSpace(CurrentProjectFolder)) return;
            Schwarzer.Windows.Dialog.OpenExplorer(CurrentProjectFolder);
        }*/

        public void OnApplicationQuit()
        {
            //SaveProject();
        }
    }
}