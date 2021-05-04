using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

public static class GoogleSignInPostBuild
{

    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        var targetGuid = proj.GetUnityMainTargetGuid();
        proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/GoogleService-Info.plist", "GoogleService-Info.plist"));
        proj.WriteToFile(projPath);
#endif

    }

}