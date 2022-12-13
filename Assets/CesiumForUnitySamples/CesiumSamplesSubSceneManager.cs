using CesiumForUnity;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CesiumSamplesSubSceneManager : MonoBehaviour
{
    [Tooltip("The CesiumSubScenes to fly between at runtime." +
        "\n\n" +
        "Note that this script only handles four CesiumSubScenes. " +
        "These can be flown to with the keyboard by pressing the " +
        "1, 2, 3, 4 keys.")]
    public List<CesiumSubScene> subScenes = new List<CesiumSubScene>();

    [Tooltip("The desired pitch and yaw angles that the camera should have upon " +
        "flying to the target CesiumSubScene." +
        "\n\n" +
        "The first element represents pitch, i.e. rotation around the X-axis. " +
        "The second element represents yaw, i.e. rotation around the Y-axis. " +
        "If no value is provided for a sub-scene, Vector2.zero is used by default.")]
    public List<Vector2> subScenePitchAndYaw = new List<Vector2>();

    public CesiumCameraController cameraController;

    private void OnValidate()
    {
        if(this.subScenes.Count > 4)
        {
            this.subScenes.RemoveRange(4, this.subScenes.Count - 4);
        }

        if (this.subScenePitchAndYaw.Count > this.subScenes.Count)
        {
            this.subScenePitchAndYaw.RemoveRange(
                this.subScenes.Count - 1,
                this.subScenePitchAndYaw.Count - this.subScenes.Count);
        }
    }

    void Update()
    {
        if (this.subScenes.Count == 0)
        {
            return;
        }

        int? keyboardInput = GetKeyboardInput();
        if(keyboardInput == null)
        {
            return;
        }

        int index = (int)keyboardInput - 1;
        FlyToSubScene(index);
    }

    int? GetKeyboardInput()
    {
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.digit1Key.isPressed || Keyboard.current.numpad1Key.isPressed)
        #elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        #endif
        {
            return 1;
        }

        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.digit2Key.isPressed || Keyboard.current.numpad2Key.isPressed)
        #elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        #endif
        {
            return 2;
        }

        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.digit3Key.isPressed || Keyboard.current.numpad3Key.isPressed)
        #elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        #endif
        {
            return 3;
        }

        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.digit4Key.isPressed || Keyboard.current.numpad4Key.isPressed)
        #elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        #endif
        {
            return 4;
        }

        return null;
    }

    void FlyToSubScene(int index)
    {
       CesiumSubScene subScene = this.subScenes[index];
        double3 coordinatesECEF = new double3(
            subScene.ecefX,
            subScene.ecefY,
            subScene.ecefZ);

        Vector2 pitchAndYaw = Vector2.zero;
        if(index < this.subScenePitchAndYaw.Count)
        {
            pitchAndYaw = this.subScenePitchAndYaw[index];
        }

        if(this.cameraController != null)
        {
            this.cameraController.FlyToLocationEarthCenteredEarthFixed(
                coordinatesECEF,
                pitchAndYaw.y,
                pitchAndYaw.x,
                true);
        }
    }
}
