using UnityEngine;
using System;
#if !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
using System.Threading;
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
using File = UnityEngine.Windows.File;
#else
using File = System.IO.File;
#endif

#if NETFX_CORE
#if UNITY_WSA_10_0
using System.Threading.Tasks;
using static System.IO.Directory;
using static System.IO.File;
using static System.IO.FileStream;
#endif
#endif


using ICSharpCode.SharpZipLib.Zip;
public class unzip : MonoBehaviour {
	#if (!UNITY_WEBPLAYER && !UNITY_WEBGL)  || UNITY_EDITOR

	//we use some integer to get error codes from the lzma library (look at lzma.cs for the meaning of these error codes)
	private int zres=0;

	private string myFile;
	private WWW www;

	private string log;

	private string ppath;

	private bool compressionStarted, pass;
	private bool downloadDone;

	//reusable buffers
	private byte[] reusableBuffer, reusableBuffer2, reusableBuffer3;

	//fixed size buffers, that don't get resized, to perform compression/decompression of buffers in them and avoid memory allocations.
	private byte[] fixedInBuffer = new byte[1024 * 256];
	private byte[] fixedOutBuffer = new byte[1024 * 768];
	private byte[] fixedBuffer = new byte[1024 * 1024];

	//A single item integer array that changes to the current number of file that get uncompressed of a zip archive.
	//When running the decompress_File function, compare this int to the total number of files returned by the getTotalFiles function
	//to get the progress of the extraction if the zip contains multiple files.
	//If you use multiple threads, remember to use other progress integers for the other threads, otherwise there will be a sharing violation.
	//
	private int[] progress = new int[1];

	//individual file progress (in bytes)
	private int[] progress2 = new int[1];

	//log for output of results
	void plog(string t)
	{
		log += t + "\n"; ;
	}
	public GameObject loading;
	public GameObject Rcanvas;
	private bool isrunning=false;
	private bool LevelsInstalling;
	private bool AlreadyInstalled=false;
	[SerializeField]
	string baseDirectryPath = "ZipPath";
	[SerializeField]
	string zipName="Summer Night.arcaoidlevel";
	string zipPath {
		get {
			Directory.CreateDirectory (Application.persistentDataPath);
			return Path.Combine (Application.persistentDataPath, zipName);
		}
	}
	public void Unzip ()
	{
		if( File.Exists( zipPath ) == false ){
			Debug.LogError(zipPath + "is not found!");
			System.Diagnostics.Process.Start(Path.GetDirectoryName(zipPath));
			return;
		}

		/*ZipUtil.Unzip (zipPath, baseDirectryPath);
		System.Diagnostics.Process.Start(Path.GetDirectoryName(zipPath));*/
	}

	// Use this for initialization
	void Start () {
	#if (UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
	ppath = UnityEngine.Windows.Directory.localFolder;
	#else
		ppath = Application.persistentDataPath;
	#endif

	#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
	ppath=".";
	#endif

		//Debug.Log(ppath);

		//various byte buffers for testing
		reusableBuffer = new byte[4096];
		reusableBuffer2 = new byte[0];
		reusableBuffer3 = new byte[0];

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	#if UNITY_EDITOR
		//StartCoroutine (InstallLevels ());
	#endif
	}
	
	// Update is called once per frame
	void Update () {
		if(AlreadyInstalled==false){
		DirectoryInfo directoryInfo = new DirectoryInfo (Application.persistentDataPath + "/");
		//print ("Streaming Assets Path: " + directoryInfo);
		FileInfo[] allFiles = directoryInfo.GetFiles ("*.arcaoidlevel");
		foreach (FileInfo file in allFiles) {
			if (isrunning == false) {
				StartCoroutine (DetectNotInstalledLevels ());
			}
		}
		if (Directory.Exists (Application.persistentDataPath + "/Inbox/")) {
			foreach (var file in Directory.GetFiles(Application.persistentDataPath + "/Inbox/", "*.arcaoidlevel")) {
				var toPath = Application.persistentDataPath + "/Inbox/" + Path.GetFileName (file);
				print (toPath);
				if (File.Exists (toPath)) {
					if (isrunning == false) {
						StartCoroutine (DetectNotInstalledLevels ());
					}
					
				}

			}
		}
   }
}
	

	public IEnumerator DetectNotInstalledLevels()
	{
		isrunning=true;
		loading.SetActive (true);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Directory.Exists (Application.persistentDataPath + "/Inbox/")) {
				foreach (var file in Directory.GetFiles(Application.persistentDataPath + "/Inbox/", "*.arcaoidlevel")) {
					var toPath = Application.persistentDataPath + "/" + Path.GetFileName (file);
					if (File.Exists (toPath)) {
						File.Delete (toPath);
					}
					File.Move (file, toPath);
				}
			}
			if (LevelsInstalling == false) {
				StartCoroutine (InstallLevels ());
			}
		} else if (Application.platform == RuntimePlatform.Android) {
			if (LevelsInstalling == false) {
				StartCoroutine (InstallLevels ());
			}
		}

		while (LevelsInstalling == true)
			yield return null;
		loading.SetActive (false);
		isrunning=false;
    }
	public IEnumerator InstallLevels()
	{
		loading.SetActive (true);
		LevelsInstalling = true;
		yield return new WaitForSeconds (0.0f);
		//call the download coroutine to download a test file
		DirectoryInfo directoryInfo = new DirectoryInfo (Application.persistentDataPath);
		print ("Streaming Assets Path: " + Application.persistentDataPath);
		FileInfo[] allFiles = directoryInfo.GetFiles ("*.arcaoidlevel");
		foreach (FileInfo file in allFiles) {
			var fileBuffer = File.ReadAllBytes (Application.persistentDataPath + "/" + file.Name);

			plog ("Validate: " + lzip.validateFile (null, fileBuffer).ToString ());


			//decompress the downloaded file
			zres = lzip.decompress_File (null, ppath + "/", progress, fileBuffer, progress2);
			plog ("decompress: " + zres.ToString ());

			yield return new WaitForSecondsRealtime (1.0f);
			if (Application.platform == RuntimePlatform.Android) {
				System.IO.File.Delete(Application.persistentDataPath + "/" + file.Name);
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				System.IO.File.Delete("/private" + Application.persistentDataPath+"/"+file.Name);
				//File.Delete ("/private"+file.ToString());
			}

		}

		Rcanvas.SetActive (true);
		loading.SetActive (false);
		AlreadyInstalled = true;
		LevelsInstalling = false;
        StartCoroutine(TOTITLE());
	}

    public IEnumerator TOTITLE()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        Destroy(GameObject.FindGameObjectWithTag("SHUTTER"));
        SceneManager.LoadScene("Title");       
    }
	private bool extracting;

	/*public IEnumerator ExtractZipFile(string fileName, byte[] zipFileData, string targetDirectory, int bufferSize = 256 * 1024)
	{
		extracting = true;
		targetDirectory = Application.persistentDataPath + "/";
		if (!extracting) yield break;
		using (var fileStream = new MemoryStream())
		{
			ZipFile zipFile = null;
			try
			{
				fileStream.Write(zipFileData, 0, zipFileData.Length);
				fileStream.Flush();
				fileStream.Seek(0, SeekOrigin.Begin);

				zipFile = new ZipFile(fileStream);

				foreach (ZipEntry entry in zipFile)
				{
					// Loop through to ensure the file is valid
				}

			}
			catch (Exception e)
			{
				Log.e("Cannot read " + fileName + ". Is it a valid .zip archive file?");
				Log.e(e.Message);
				extracting = false;
			}
			if (!extracting || zipFile == null) yield break;

			foreach (ZipEntry entry in zipFile)
			{
				var targetFile = Path.Combine(targetDirectory, entry.Name);
				if (entry.Name.Contains("__MACOSX")) continue; // Fucking macOS...
				print("Extracting " + entry.Name + "...");

				FileStream outputFile = null;

				try
				{
					outputFile = File.Create(targetFile);
				}
				catch (Exception e)
				{
					Log.e("Cannot extract " + entry.Name + ". Is the .zip archive file valid?");
					Log.e(e.Message);
					extracting = false;
				}
				if (!extracting || outputFile == null) yield break;

				using (outputFile)
				{
					if (entry.Size <= 0) continue;
					var zippedStream = zipFile.GetInputStream(entry);
					var dataBuffer = new byte[bufferSize];

					int readBytes;
					while ((readBytes = zippedStream.Read(dataBuffer, 0, bufferSize)) > 0)
					{
						outputFile.Write(dataBuffer, 0, readBytes);
						outputFile.Flush();
						yield return null;
					}
				}
			}
		}
		extracting = false;
	}*/
 }
	#endif
