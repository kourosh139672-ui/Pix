#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// Editor tools for validating the Tom & Jerry demo and exporting an Android APK.
/// Requires Unity Android Build Support (SDK, NDK and OpenJDK) to be installed.
/// </summary>
public static class TomJerryAndroidBuild
{
    private const string DemoScenePath = "Assets/TomJerry3D/Scenes/TomJerryDemo.unity";
    private const string OutputPath = "Builds/Android/TomJerry3D.apk";

    [MenuItem("Tools/Tom & Jerry 3D/Validate Project")]
    public static void ValidateProject()
    {
        bool sceneExists = File.Exists(DemoScenePath);
        bool scriptsFolderExists = Directory.Exists("Assets/TomJerry3D/Scripts");
        bool hasEnabledScene = EditorBuildSettings.scenes.Any(s => s.enabled && !string.IsNullOrEmpty(s.path));

        Debug.Log("=== Tom & Jerry 3D validation ===");
        Debug.Log((scriptsFolderExists ? "[OK] " : "[MISSING] ") + "Assets/TomJerry3D/Scripts folder");
        Debug.Log((sceneExists ? "[OK] " : "[MISSING] ") + DemoScenePath);
        Debug.Log((hasEnabledScene ? "[OK] " : "[MISSING] ") + "At least one enabled scene in Build Settings");

        if (!sceneExists)
            Debug.LogWarning("Create the demo scene first: Tools > Tom & Jerry 3D > Create Playable Demo Scene.");
        if (!hasEnabledScene)
            Debug.LogWarning("Add the generated demo scene to File > Build Settings > Scenes In Build.");
        if (sceneExists && hasEnabledScene && scriptsFolderExists)
            Debug.Log("Project structure looks ready for an Android build. This check does not compile or run the game.");
    }

    [MenuItem("Tools/Tom & Jerry 3D/Build Android APK")]
    public static void BuildAndroidApk()
    {
        if (!File.Exists(DemoScenePath))
        {
            EditorUtility.DisplayDialog("Tom & Jerry 3D", "Demo scene not found. First run Tools > Tom & Jerry 3D > Create Playable Demo Scene.", "OK");
            return;
        }

        EditorBuildSettingsScene[] enabledScenes = EditorBuildSettings.scenes
            .Where(s => s.enabled && !string.IsNullOrEmpty(s.path))
            .ToArray();

        if (enabledScenes.Length == 0)
        {
            EditorUtility.DisplayDialog("Tom & Jerry 3D", "No enabled scenes found. Add TomJerryDemo.unity to Build Settings.", "OK");
            return;
        }

        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
        {
            EditorUtility.DisplayDialog("Tom & Jerry 3D", "Could not switch to Android. Install Android Build Support, SDK, NDK and OpenJDK in Unity Hub.", "OK");
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.runInBackground = true;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = enabledScenes.Select(s => s.path).ToArray(),
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Tom & Jerry 3D APK created: " + Path.GetFullPath(OutputPath));
            EditorUtility.RevealInFinder(OutputPath);
            EditorUtility.DisplayDialog("Build complete", "APK created at:\n" + OutputPath, "OK");
        }
        else
        {
            Debug.LogError("Android build failed: " + report.summary.result + " — " + report.summary.totalErrors + " error(s). Check the Console for details.");
            EditorUtility.DisplayDialog("Build failed", "Unity could not create the APK. Check the Console and confirm Android Build Support is installed.", "OK");
        }
    }
}
#endif
