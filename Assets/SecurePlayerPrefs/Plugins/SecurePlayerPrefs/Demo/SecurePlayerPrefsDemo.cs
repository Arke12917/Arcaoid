using UnityEngine;
using System.Collections;

using SecPlayerPrefs; //Import namespace!

public class SecurePlayerPrefsDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
      
        //Write
        SecurePlayerPrefs.SetFloat("float", 0.1f);
        SecurePlayerPrefs.SetBool("bool", true);
        SecurePlayerPrefs.SetInt("int", 100);
        SecurePlayerPrefs.SetString("string", "amazing!");

        //Read
        Debug.Log(SecurePlayerPrefs.GetFloat("float"));
        Debug.Log(SecurePlayerPrefs.GetBool("bool"));
        Debug.Log(SecurePlayerPrefs.GetInt("int"));
        Debug.Log(SecurePlayerPrefs.GetString("string"));


        //Serialized Classes
        //Write
        SecureplayerPrefsDemoClass c = new SecureplayerPrefsDemoClass();
        SecureDataManager<SecureplayerPrefsDemoClass> dataManager = new SecureDataManager<SecureplayerPrefsDemoClass>("name");
        c.incremental = true;
        c.playID = "tester";
        c.type = 10;
        dataManager.Save(c);

        //Read
        SecureDataManager<SecureplayerPrefsDemoClass> dataManagerReader = new SecureDataManager<SecureplayerPrefsDemoClass>("name");
        c = dataManagerReader.Get();
        Debug.Log(c.incremental);
        Debug.Log(c.type);
        Debug.Log(c.playID);  
	}
}
