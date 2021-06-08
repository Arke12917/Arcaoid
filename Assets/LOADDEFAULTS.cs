using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SecPlayerPrefs;

public class LOADDEFAULTS : MonoBehaviour
{
    public bool Is16By9
    {
        get
        {
            return Mathf.Abs((1f * Screen.width / Screen.height) - (16f / 9f)) < 0.1f;
        }
    }

    public bool Is18By9
    {
        get
        {
            return Mathf.Abs((1f * Screen.width / Screen.height) - (18f / 9f)) < 0.2f;
        }
    }

    public GameObject songobjs;
    public Animator BG;
    // Start is called before the first frame update
    public static int NATIVEWIDTH = Screen.width;
    public static int NATIVEHEIGHT = Screen.height;
    private void Awake()
    {
        
    }

    void Start()
    {
        

        if (SecurePlayerPrefs.HasKey("#RESOLUTION"))
        {
            
        }
        else
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer||Application.platform==RuntimePlatform.WindowsEditor)
            {
                SecurePlayerPrefs.SetInt("#RESOLUTION", 0);
                Screen.SetResolution(Screen.width, Screen.height, true);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                SecurePlayerPrefs.SetInt("#RESOLUTION", 0);


            }
        }

        if (SecurePlayerPrefs.HasKey("#JUDGEOFF")) { }
        else
        {
            SecurePlayerPrefs.SetFloat("#JUDGEOFF", 0);
        }

        if (SecurePlayerPrefs.HasKey("#PEORL")) { }
        else
        {
            SecurePlayerPrefs.SetBool("#PEORL", false);
        }

        if (SecurePlayerPrefs.HasKey("#ALTREND")) 
        {
            SecurePlayerPrefs.SetBool("#ALTREND", false);
        }
        else
        {
            SecurePlayerPrefs.SetBool("#ALTREND", false);
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //print ("/private" + Application.streamingAssetsPath + "/" + "Summer Night/");
            //print ("/private" + Application.persistentDataPath + "/Summer Night/");
            DirectoryInfo directoryI = new DirectoryInfo(Application.streamingAssetsPath);
            //print ("Streaming Assets Path: " + Application.streamingAssetsPath);
            DirectoryInfo[] Filess = directoryI.GetDirectories("*");
            foreach (DirectoryInfo directory in Filess)
            {
                var toPath = directory + "/" + Path.GetFileName("ARCAOID.txt");
                //print (toPath);
                if (File.Exists(toPath))
                {
                    //print ("SN 1 here!");
                    //print (directory.FullName);
                    if (directory.FullName.Contains("In Your Mind"))
                    {
                        if (Directory.Exists(Application.persistentDataPath + "/" + "In Your Mind"))
                        {

                        }
                        else
                        {
                            DirectoryCopy(directory.FullName, Application.persistentDataPath + "/" + "In Your Mind", true);
                        }
                    }
                    else if (directory.FullName.Contains("Unfitting Facade"))
                    {
                        if (Directory.Exists(Application.persistentDataPath + "/" + "Unfitting Facade"))
                        {
                        }
                        else
                        {
                            DirectoryCopy(directory.FullName, Application.persistentDataPath + "/" + "Unfitting Facade", true);
                        }
                    }
                    else if (directory.FullName.Contains("Overload"))
                    {
                        if (Directory.Exists(Application.persistentDataPath + "/" + "Overload"))
                        {
                        }
                        else
                        {
                            DirectoryCopy(directory.FullName, Application.persistentDataPath + "/" + "Overload", true);
                        }
                    }


                }
                //System.IO.Directory.Move("/private"+Application.streamingAssetsPath+"/"+"Summer Night/","/private"+Application.persistentDataPath+"/Summer Night/");
                //File.Delete ("/private"+file.ToString());
            }

        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            //if mg_data doesn't exist, extract default data...
            if (File.Exists(Application.persistentDataPath + "/" + "MG_Data.data") == false)
            {
                
                //copy tgz to directory where we can extract it
                WWW www = new WWW(Application.streamingAssetsPath + "/Data.tgz");
                while (!www.isDone) { }
                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + "Data.tgz", www.bytes);
                //extract it
                Utility_SharpZipCommands.ExtractTGZ(Application.persistentDataPath + "/" + "Data.tgz", Application.persistentDataPath);
                //delete tgz
                File.Delete(Application.persistentDataPath + "/" + "Data.tgz");
            }
            else
            {
               
            }
#endif
            /* print("ok got this far");
             //dbPath = realPath;
             string ppath = Application.persistentDataPath;
             // print(realPath);
             string zipPath = Application.persistentDataPath + "/In Your Mind.zip";
             string exportPath = Application.persistentDataPath + "/";

             AndroidStreamingAssets.Extract();*/
        }
        //print("done?");
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
