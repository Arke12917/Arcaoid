using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcaoid.Gameplay;
using UnityEngine.UI;
using System.IO;
[System.Serializable]
public class ObjectPoolItem {
	public int amountToPool;
	public GameObject objectToPool;
	public bool shouldExpand;

}
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;

    public Sprite TAPNOTELIGHT;
    public Sprite TAPNOTEDARK;

    public Sprite HOLDNORMALL;
    public Sprite HOLDNORMALD;
    public Sprite HOLDHIGHLIGHTL;
    public Sprite HOLDHIGHLIGHTD;

    public Texture2D ARCTAPLIGHT;
    public Texture2D ARCTAPDARK;
    public GameObject ATAPSHADER;

    public Image BG;
    public Sprite BGLIGHT;
    public Sprite BGCONFLICT;

    public SpriteRenderer track;
    public SpriteRenderer track1;
    public SpriteRenderer track2;
    public SpriteRenderer track3;
    public SpriteRenderer track4;
    public Sprite tracklight;
    public Sprite tracklightLANE;
    public Sprite tracklightREVERSE;
    public Sprite trackdark;
    public Sprite trackdarkLANE;
    public Sprite trackdarkREVERSE;

    // Use this for initialization
    void Awake()
    {
        SharedInstance = this;
    }
    IEnumerator Start()
    {
        bool iscustom = false;
        pooledObjects = new List<GameObject>();
        if(LOADMENU.CUSTOMBG==true)
        {
            DirectoryInfo directoryInfoo = new DirectoryInfo(Application.persistentDataPath);
            //print("Streaming Assets Path: " + Application.persistentDataPath);
            DirectoryInfo[] allFiless = directoryInfoo.GetDirectories(LOADMENU.FOLDERNAME);
            foreach (DirectoryInfo directory in allFiless)
            {

                DirectoryInfo directoryInfo = new DirectoryInfo(directory + "/");
                //print("Streaming Assets Path: " + directoryInfo);
                FileInfo[] allFiles = directoryInfo.GetFiles("background.png");
                foreach (FileInfo file in allFiles)
                {
                    //StartCoroutine(songcard(file));
                    
                        //moneyo = Int.Parse(line);
                        if (file.Name.Contains("meta"))
                        {

                        }
                        //2
                        else
                        {
                            string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(file.ToString());
                            string[] playerNameData = playerFileWithoutExtension.Split(" "[0]);
                            //3
                            string tempSongName = "";
                            int i = 0;
                            foreach (string stringFromFileName in playerNameData)
                            {
                                if (i != 0)
                                {
                                    tempSongName = tempSongName + stringFromFileName + " ";
                                }
                                i++;
                            }
                            //4
                            string wwwPlayerFilePath = "file://" + file.FullName.ToString();
                            WWW www = new WWW(wwwPlayerFilePath);
                            yield return www;
                            Texture2D texxx;
                            texxx = new Texture2D(1286, 965, TextureFormat.RGB24, false);
                            www.LoadImageIntoTexture(texxx);
                            //5
                            BG.sprite = Sprite.Create(texxx, new Rect(0.0f, 0.0f, texxx.width, texxx.height), new Vector2(0.5f, 0.5f), 100.0f, 1);                           
                        }
                    
                }

            }
            iscustom = true;
        }
        if(LOADMENU.songside == "CONFLICT")
        {
            if (!iscustom)
            {
                BG.sprite = BGCONFLICT;
            }
            track.sprite = trackdark;
            track4.sprite = trackdark;
            track1.sprite = trackdarkREVERSE;
            track2.sprite = trackdarkLANE;
            track3.sprite = trackdarkLANE;
        }
        else
        {
            if (!iscustom)
            {
                BG.sprite = BGLIGHT;
            }
            track.sprite = tracklight;
            track4.sprite = tracklight;
            track1.sprite = tracklightREVERSE;
            track2.sprite = tracklightLANE;
            track3.sprite = tracklightLANE;
        }

        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.name.Contains("TapNote"))
            {
                if (LOADMENU.songside == "CONFLICT")
                    item.objectToPool.GetComponent<SpriteRenderer>().sprite = TAPNOTEDARK;
                else
                    item.objectToPool.GetComponent<SpriteRenderer>().sprite = TAPNOTELIGHT;
            }
            else if (item.objectToPool.name.Contains("ArcTap"))
            {
                if (LOADMENU.songside == "CONFLICT")
                    ATAPSHADER.GetComponent<Renderer>().material.mainTexture = ARCTAPDARK;
                else
                    ATAPSHADER.GetComponent<Renderer>().material.mainTexture = ARCTAPLIGHT;
            }
            else if (item.objectToPool.name.Contains("HoldNote"))
            {
                if (LOADMENU.songside == "CONFLICT")
                {
                    item.objectToPool.GetComponent<SpriteRenderer>().sprite = HOLDNORMALD;
                    ArcHoldNoteManager.Instance.HighlightSprite = HOLDHIGHLIGHTD;
                    ArcHoldNoteManager.Instance.DefaultSprite = HOLDNORMALD;
                }
                else
                {
                    item.objectToPool.GetComponent<SpriteRenderer>().sprite = HOLDNORMALL;
                    ArcHoldNoteManager.Instance.HighlightSprite = HOLDHIGHLIGHTL;
                    ArcHoldNoteManager.Instance.DefaultSprite = HOLDNORMALL;
                }
            }
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                


                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
                
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FindObjectwithTag(string _tag)
    {
        
        Transform parent = transform;
        GetChildObject(parent, _tag);
    }

    public GameObject GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                return child.gameObject;
            }
            if (child.childCount > 0)
            {
                GetChildObject(child, _tag);
            }
        }
        return null;
    }
}
   



