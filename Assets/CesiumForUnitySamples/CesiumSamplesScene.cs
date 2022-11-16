using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR 
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR 
[InitializeOnLoad]
static class CesiumSamplesSceneManager
{
    static CesiumSamplesSceneManager()
    {
        EditorSceneManager.sceneOpened += ResetSceneViewCamera;
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
    private Vector3 _lookAtPosition = Vector3.zero;

    [SerializeField]
    private Vector3 _lookAtRotation = Vector3.zero;

    [SerializeField]
    private float _lookAtSize = 0;

    private readonly float _sceneViewFarClip = 1000000;

    private GameObject _canvasGameObject;

    void OnEnable()
    {
        this._canvasGameObject = this.transform.Find("Canvas").gameObject;

        #if UNITY_EDITOR
        // Only show the panel outside of play mode.
        this._canvasGameObject.SetActive(!EditorApplication.isPlaying);
        #else
        this._canvasGameObject.SetActive(false);
        #endif
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
        SceneView.lastActiveSceneView.cameraSettings.farClip = this._sceneViewFarClip;

        EditorApplication.update -= ResetSceneViewOnLoad;
    }
    #endif
}
