using UnityEngine;
using CesiumForUnity;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CesiumSamplesMetadataPicking : MonoBehaviour
{
    // The GameObject with the UI to enable / disable depending on
    // whether metadata has been picked.
    public GameObject metadataPanel;

    // The text to display the metadata properties.
    public Text metadataText;

    void Start()
    {
        // Fix the cursor to the center of the screen and hide it.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (metadataPanel != null)
        {
            metadataPanel.SetActive(false);
        }
    }

    void Update()
    {
        #if ENABLE_INPUT_SYSTEM
        bool getMetadata = Mouse.current.leftButton.isPressed;
        #elif ENABLE_LEGACY_INPUT_MANAGER
        bool getMetadata = Input.GetMouseButtonDown(0);
        #endif

        if (getMetadata && metadataText != null)
        {
            metadataText.text = "";

            RaycastHit hit;
            if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.TransformDirection(Vector3.forward),
                out hit,
                Mathf.Infinity))
            {
                CesiumMetadata metadata = hit.transform.GetComponentInParent<CesiumMetadata>();
                if (metadata != null)
                {
                    MetadataProperty[] properties =
                        metadata.GetProperties(hit.transform, hit.triangleIndex);

                    // List out each metadata property in the target UI.
                    foreach (MetadataProperty property in properties)
                    {
                        string propertyName = property.GetPropertyName();
                        string propertyValue = property.GetString("null");
                        if (propertyValue != "null" && propertyValue != "")
                        {
                            metadataText.text += "<b>" + propertyName + "</b>" + ": "
                                + propertyValue + "\n";
                        }
                    }
                }
            }

            if(metadataPanel != null)
            {
                metadataPanel.SetActive(metadataText.text.Length > 0);
            }
        }
    }
}