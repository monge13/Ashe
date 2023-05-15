using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Reporting;

public class BuildTools
{
    [MenuItem("Ashe/Build/Android/Debug")]
    public static void AndroidDebugBuild()
    {
        Build(false, false);
    }

    [MenuItem("Ashe/Build/Android/Release")]
    public static void AndroidReleaseBuild()
    {
        Build(false, true);
    }

    [MenuItem("Ashe/Build/iOS/Debug")]
    public static void iOSDebugBuild()
    {
        Build(true, false);
    }

    [MenuItem("Ashe/Build/iOS/Release")]
    public static void iOSReleaseBuild()
    {
        Build(true, true);
    }

    [MenuItem("Ashe/Build/All/Debug")]
    public static void AllDebugBuild()
    {
        AndroidDebugBuild();
        iOSDebugBuild();
    }

    [MenuItem("Ashe/Build/All/Release")]
    public static void AllReleaseBuild()
    {
        AndroidReleaseBuild();
        iOSReleaseBuild();
    }


    private static void Build(bool isIOSBuild, bool isReleseBuild)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // SetUp Scenes
        var scenes = EditorBuildSettings.scenes;
        buildPlayerOptions.scenes = new string[scenes.Length];
        for(int i = 0; i < scenes.Length; ++i){
            if(scenes[i].enabled) {
                buildPlayerOptions.scenes[i] = scenes[i].path;                
            }
        }


        string fileName = Application.productName + ".apk";
        if(isIOSBuild) fileName = "iOS";
        buildPlayerOptions.locationPathName = "Build/Outputs/" + fileName;

        buildPlayerOptions.target = isIOSBuild ? BuildTarget.iOS : BuildTarget.Android;
        buildPlayerOptions.options = isReleseBuild ? BuildOptions.None : BuildOptions.Development;

        if(!isReleseBuild) buildPlayerOptions.extraScriptingDefines = new[] {"_LOG_ENABLED"};

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}
