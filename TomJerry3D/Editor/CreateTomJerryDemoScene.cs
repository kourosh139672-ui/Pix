using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates a saved Unity scene containing the runtime-built Tom & Jerry house demo.
/// Run from Tools > Tom & Jerry 3D > Create Playable Demo Scene.
/// </summary>
public static class CreateTomJerryDemoScene
{
    [MenuItem("Tools/Tom & Jerry 3D/Create Playable Demo Scene")]
    public static void CreateScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject bootstrap = new GameObject("TomJerryDemoBootstrap");
        bootstrap.AddComponent<PlayableHouseDemo>();

        const string folder = "Assets/TomJerry3D/Scenes";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        const string scenePath = folder + "/TomJerryDemo.unity";

        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Tom & Jerry 3D demo scene created at " + scenePath);
    }
}
