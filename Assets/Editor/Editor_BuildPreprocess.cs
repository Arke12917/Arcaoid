#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


class CustomBuildPreProcess : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {

       /*string dataDirectory = Application.streamingAssetsPath + "/Data/";
        string fileToCreate = Application.streamingAssetsPath + "/Data.tgz";

        Utility_SharpZipCommands.CreateTarGZ_FromDirectory(fileToCreate, dataDirectory);*/

    }
}

#endif