using UnityEngine;
using UnityEditor;

/// <summary>
/// Start Scene auto loader.
/// </summary>
/// <description>
/// This class adds a Start Scene Autoload menu containing options to select
/// a "start scene" enable it to be auto-loaded when the user presses play
/// in the editor. When enabled, the selected scene will be loaded on play,
/// then the original scene will be reloaded on stop.
///
/// Based on an idea on this thread:
/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>
[InitializeOnLoad]
static class StartSceneAutoLoader
{
    // Static constructor binds a playmode-changed callback.
    // [InitializeOnLoad] above makes sure this gets executed.
    static StartSceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    // Properties are remembered as editor preferences.
    private const string editorPrefIsLoadStartOnPlay = "StartSceneAutoLoader.IsLoadStartOnPlay";
    private const string editorPrefStartScenePath = "StartSceneAutoLoader.StartScenePath";
    private const string editorPrefPreviousScenePath = "StartSceneAutoLoader.PreviousScenePath";

    private static bool IsLoadStartSceneOnPlay
    {
        get { return EditorPrefs.GetBool(editorPrefIsLoadStartOnPlay, false); }
        set { EditorPrefs.SetBool(editorPrefIsLoadStartOnPlay, value); }
    }

    private static string StartScene
    {
        get { return EditorPrefs.GetString(editorPrefStartScenePath, "Start.unity"); }
        set { EditorPrefs.SetString(editorPrefStartScenePath, value); }
    }

    private static string PreviousScene
    {
        get { return EditorPrefs.GetString(editorPrefPreviousScenePath, UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path); }
        set { EditorPrefs.SetString(editorPrefPreviousScenePath, value); }
    }

    // Menu items to select the "start" scene and control whether or not to load it.
    [MenuItem("StartSceneAutoLoader/Scene Autoload/Select Start Scene...")]
    private static void SelectStartScene()
    {
        string startScene = EditorUtility.OpenFilePanel("Select Start Scene", Application.dataPath, "unity");
        Debug.Log(Application.temporaryCachePath);
        if (!string.IsNullOrEmpty(startScene))
        {
            StartScene = startScene;
            IsLoadStartSceneOnPlay = true;
        }
    }

    [MenuItem("StartSceneAutoLoader/Scene Autoload/Load StartScene On Play", true)]
    private static bool ShowLoadStartSceneOnPlay()
    {
        return !IsLoadStartSceneOnPlay;
    }
    [MenuItem("StartSceneAutoLoader/Scene Autoload/Load StartScene On Play")]
    private static void EnableLoadStartSceneOnPlay()
    {
        IsLoadStartSceneOnPlay = true;
    }

    [MenuItem("StartSceneAutoLoader/Scene Autoload/Don't Load StartScene On Play", true)]
    private static bool ShowDontLoadStartSceneOnPlay()
    {
        return IsLoadStartSceneOnPlay;
    }
    [MenuItem("StartSceneAutoLoader/Scene Autoload/Don't Load StartScene On Play")]
    private static void DisableLoadStartSceneOnPlay()
    {
        IsLoadStartSceneOnPlay = false;
    }

    // Play mode change callback handles the scene load/reload.
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!IsLoadStartSceneOnPlay)
        {
            return;
        }

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // User pressed play -- autoload start scene.
            PreviousScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(StartScene);
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // User pressed stop -- reload previous scene.
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(PreviousScene);
        }
    }
}