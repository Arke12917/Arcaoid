using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostProcessBuildScript
{

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {

            // Get plist
            var plistPath = pathToBuiltProject + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            var rootDict = plist.root;

            // Set file sharing enabled
            rootDict.SetBoolean("UIFileSharingEnabled", true);

            // Associate with .arcaoidlevel file
            var array = rootDict.CreateArray("CFBundleDocumentTypes");
            var dict = array.AddDict();
            dict.SetString("CFBundleTypeName", "Arcaoid Level Document");
            dict.SetString("LSHandlerRank", "Alternate");
            dict.SetString("CFBundleTypeRole", "Viewer");
            array = dict.CreateArray("LSItemContentTypes");
            array.AddString("io.arcaoid.arcaoidlevel");

            array = rootDict.CreateArray("UTExportedTypeDeclarations");
            dict = array.AddDict();
            dict.SetString("UTTypeIdentifier", "io.arcaoid.arcaoidlevel");
            dict.SetString("UTTypeDescription", "Arcaoid Level Document");
            var dict2 = dict.CreateDict("UTTypeTagSpecification");
            dict2.SetString("public.filename-extension", "arcaoidlevel");
            dict2.SetString("public.mime-type", "application/arcaoid");
            array = dict.CreateArray("UTTypeConformsTo");
            array.AddString("public.data");

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }

    }

}