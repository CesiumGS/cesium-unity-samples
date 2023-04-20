using CesiumForUnity;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CesiumSamplesFlyToLocationHandler : MonoBehaviour
{
    [Tooltip("The CesiumFlyToController that will take flight at runtime.")]
    public CesiumFlyToController flyToController;

    [InspectorName("Locations (Longitude Latitude Height")]
    [Tooltip("The locations in Longitude Latitude Height to fly between at runtime." +
             "\n\n" +
             "This script handles up to eight locations. These subscene locations can " +
             "be flown to by pressing the 1-8 keys on the keyboard.")]
    public List<double3> locations = new List<double3>();

    [Tooltip("The desired yaw and pitch angles that the camera should have upon " +
        "flying to the target location." +
        "\n\n" +
        "The first element represents yaw, i.e. horizontal rotation or " +
        "rotation around the Y-axis.\n" +
        "The second element represents yaw, i.e. vertical rotation or " +
        "rotation around the Y-axis.\n" +
        "If no value is provided for a location, Vector2.zero is used by default.")]
    public List<Vector2> yawAndPitchAngles = new List<Vector2>();

    const int locationLimit = 8;

    private void OnValidate()
    {
        if (this.locations.Count > locationLimit)
        {
            this.locations.RemoveRange(locationLimit, this.locations.Count - locationLimit);
        }

        if (this.yawAndPitchAngles.Count > this.locations.Count)
        {
            this.yawAndPitchAngles.RemoveRange(
                this.locations.Count - 1,
                this.yawAndPitchAngles.Count - this.locations.Count);
        }
    }

    void Update()
    {
        if (this.locations.Count == 0)
        {
            return;
        }

        int? keyboardInput = GetKeyboardInput();
        if (keyboardInput == null)
        {
            return;
        }

        int index = (int)keyboardInput - 1;
        this.FlyToLocation(index);
    }

    #region Inputs

    static bool GetKey1Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit1Key.isPressed || Keyboard.current.numpad1Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
#endif
    }

    static bool GetKey2Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit2Key.isPressed || Keyboard.current.numpad2Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
#endif
    }
    static bool GetKey3Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit3Key.isPressed || Keyboard.current.numpad3Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);
#endif
    }
    static bool GetKey4Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit4Key.isPressed || Keyboard.current.numpad4Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4);
#endif
    }
    static bool GetKey5Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit5Key.isPressed || Keyboard.current.numpad5Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5);
#endif
    }

    static bool GetKey6Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit6Key.isPressed || Keyboard.current.numpad6Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6);
#endif
    }

    static bool GetKey7Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit7Key.isPressed || Keyboard.current.numpad7Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7);
#endif
    }
    static bool GetKey8Down()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current.digit8Key.isPressed || Keyboard.current.numpad8Key.isPressed;
#else
        return Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8);
#endif
    }

    static int? GetKeyboardInput()
    {
        if (GetKey1Down())
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

        if (GetKey5Down())
        {
            return 5;
        }

        if (GetKey6Down())
        {
            return 6;
        }

        if (GetKey7Down())
        {
            return 7;
        }

        if (GetKey8Down())
        {
            return 8;
        }

        return null;
    }

    #endregion

    void FlyToLocation(int index)
    {
        double3 coordinatesLLH = this.locations[index];

        Vector2 yawAndPitch = Vector2.zero;
        if (index < this.yawAndPitchAngles.Count)
        {
            yawAndPitch = this.yawAndPitchAngles[index];
        }

        if (this.flyToController != null)
        {
            this.flyToController.FlyToLocationLongitudeLatitudeHeight(
                coordinatesLLH,
                yawAndPitch.x,
                yawAndPitch.y,
                true);
        }
    }
}
