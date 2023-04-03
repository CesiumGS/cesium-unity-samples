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
        "These subscene locations can be flown to by pressing the " +
        "1, 2, 3, 4 keys.")]
    public List<CesiumSubScene> subScenes = new List<CesiumSubScene>();

    [Tooltip("The desired yaw and pitch angles that the camera should have upon " +
        "flying to the target CesiumSubScene." +
        "\n\n" +
        "The first element represents yaw, i.e. horizontal rotation or " +
        "rotation around the Y-axis.\n" +
        "The second element represents yaw, i.e. vertical rotation or " +
        "rotation around the Y-axis.\n" +
        "If no value is provided for a sub-scene, Vector2.zero is used by default.")]
    public List<Vector2> subSceneYawAndPitch = new List<Vector2>();

    public CesiumFlyToController flyToController;

    private void OnValidate()
    {
        if(this.subScenes.Count > 4)
        {
            this.subScenes.RemoveRange(4, this.subScenes.Count - 4);
        }

        if (this.subSceneYawAndPitch.Count > this.subScenes.Count)
        {
            this.subSceneYawAndPitch.RemoveRange(
                this.subScenes.Count - 1,
                this.subSceneYawAndPitch.Count - this.subScenes.Count);
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
        this.FlyToSubScene(index);
    }
    static bool GetKey1Down()
    {
        #if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit1Key.isPressed || Keyboard.current.numpad1Key.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
        #endif
    }

    static bool GetKey2Down()
    {
        #if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit2Key.isPressed || Keyboard.current.numpad2Key.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        #endif
    }
    static bool GetKey3Down()
    {
        #if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit3Key.isPressed || Keyboard.current.numpad3Key.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);
        #endif
    }
    static bool GetKey4Down()
    {
        #if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit4Key.isPressed || Keyboard.current.numpad4Key.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4);
        #endif
    }

    static int? GetKeyboardInput()
    {
        if(GetKey1Down())
        {
            return 1;
        }

        if (GetKey2Down())
        {
            return 2;
        }

        if (GetKey3Down())
        {
            return 3;
        }

        if (GetKey4Down())
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

        Vector2 yawAndPitch = Vector2.zero;
        if(index < this.subSceneYawAndPitch.Count)
        {
            yawAndPitch = this.subSceneYawAndPitch[index];
        }

        if(this.flyToController != null)
        {
            this.flyToController.FlyToLocationEarthCenteredEarthFixed(
                coordinatesECEF,
                yawAndPitch.x,
                yawAndPitch.y,
                true);
        }
    }
}
