using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR 
[InitializeOnLoad]
static class CesiumSamplesSceneManager
{
    static readonly string samplesKey = "OpenedCesiumSamples";

    static CesiumSamplesSceneManager()
    {
        DisableTextMeshProIcons();

        EditorSceneManager.sceneOpened += ResetSceneViewCamera;
        EditorApplication.update += OpenFirstSampleScene;
        EditorApplication.quitting += RemoveSamplesKeyFromPlayerPrefs;
    }

    // There's no public API to disable component icons in the SceneView, which
    // can make it hard to read the UI that appears in the editor. This script
    // disables the TextMeshPro icon using Reflection so that it doesn't get in
    // the way of the UI.
    static void DisableTextMeshProIcons()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));
        Type AnnotationUtility = assembly.GetType("UnityEditor.AnnotationUtility");
        if (AnnotationUtility != null)
        {
            Type Annotation = Type.GetType("UnityEditor.Annotation, UnityEditor");
            FieldInfo ClassId = Annotation.GetField("classID");
            FieldInfo ScriptClass = Annotation.GetField("scriptClass");

            MethodInfo GetAnnotations = AnnotationUtility.GetMethod(
                "GetAnnotations",
                BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo SetIconEnabled = AnnotationUtility.GetMethod(
                "SetIconEnabled",
                BindingFlags.Static | BindingFlags.NonPublic);

            Array annotations = (Array)GetAnnotations.Invoke(null, null);
            foreach (object annotation in annotations)
            {
                int classId = (int)ClassId.GetValue(annotation);
                string scriptClass = (string)ScriptClass.GetValue(annotation);
                if(scriptClass == "TextMeshPro" || scriptClass == "TextMeshProUGUI")
                {
                    SetIconEnabled.Invoke(
                        null,
                        new object[] { classId, scriptClass, 0 });
                }
            }
        }
    }

    // Unity will not open the first sample scene on its own, so this manually opens
    // the sample scene when the Editor is first opened. The PlayerPref prevents
    // the first scene from being re-opened whenever the user's code compiles.
    static void OpenFirstSampleScene()
    {
        EditorApplication.update -= OpenFirstSampleScene;
        if(PlayerPrefs.GetInt(samplesKey, -1) > 0)
        {
            return;
        }

        PlayerPrefs.SetInt(samplesKey, 1);
        EditorSceneManager.OpenScene("Assets/Scenes/01_CesiumWorld.unity");
    }

    static void RemoveSamplesKeyFromPlayerPrefs()
    {
        PlayerPrefs.DeleteKey(samplesKey);
    }

    static void ResetSceneViewCamera(Scene scene, OpenSceneMode mode)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();
        for(int i = 0; i < gameObjects.Length; i++)
        {
            CesiumSamplesScene sampleScene =
                gameObjects[i].GetComponent<CesiumSamplesScene>();

            if(sampleScene != null)
            {
                EditorApplication.update += sampleScene.ResetSceneViewOnLoad;
                return;
            }
        }
    }
}
#endif

[ExecuteInEditMode]
class CesiumSamplesScene : MonoBehaviour
{
    [Header("Default Scene View Settings")]
    [SerializeField]
    [Tooltip("The position for the editor camera to look at when the scene " +
        "view is reset (i.e. when \"1\" is pressed in the editor). This is used " +
        "as an input for SceneView.LookAtDirect.")]
    private Vector3 _lookAtPosition = Vector3.zero;

    [SerializeField]
    [Tooltip("The rotation of the editor camera when the scene view is reset " +
        "(i.e. when \"1\" is pressed in the editor). This is used as an input " +
        "for SceneView.LookAtDirect.")]
    private Vector3 _lookAtRotation = Vector3.zero;

    [SerializeField]
    [Tooltip("The size of the editor camera's view when the scene view is reset " +
        "(i.e. when \"1\" is pressed in the editor). This is used as an input " +
        "for SceneView.LookAtDirect.")]
    private float _lookAtSize = 0;

    [SerializeField]
    [Tooltip("The List of GameObjects that the scene should disable during play mode.")]
    private List<GameObject> _objectsToDisable = new List<GameObject>();

    [SerializeField]
    [Tooltip("The List of GameObjects that the scene should enable during play mode.")]
    private List<GameObject> _objectsToEnable = new List<GameObject>();

    void OnEnable()
    {
        #if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            return;
        }
        #endif

        for (int i = 0; i < this._objectsToDisable.Count; i++)
        {
            this._objectsToDisable[i].SetActive(false);
        }

        for (int i = 0; i < this._objectsToEnable.Count; i++)
        {
            this._objectsToEnable[i].SetActive(true);
        }
    }

    #if UNITY_EDITOR
    void Update()
    {
        #if ENABLE_INPUT_SYSTEM
        bool resetView =
            Keyboard.current.digit1Key.isPressed || Keyboard.current.numpad1Key.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        bool resetView = Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
        #endif
        
        if (resetView && EditorWindow.focusedWindow == SceneView.lastActiveSceneView)
        {
            ResetSceneView();
        }
    }

    void ResetSceneView()
    {
        SceneView.lastActiveSceneView.LookAtDirect(
            this._lookAtPosition,
            Quaternion.Euler(this._lookAtRotation),
            this._lookAtSize);
    }

    public void ResetSceneViewOnLoad()
    {
        ResetSceneView();
        EditorApplication.update -= ResetSceneViewOnLoad;
    }
    #endif
}
