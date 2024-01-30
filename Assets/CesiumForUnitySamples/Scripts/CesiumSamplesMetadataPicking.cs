using CesiumForUnity;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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

    // Cached Dictionary of metadata values. This prevents reallocation every
    // time metadata is sampled from the tileset.
    private Dictionary<String, CesiumMetadataValue> _metadataValues;

    void Start()
    {
        // Fix the cursor to the center of the screen and hide it.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (metadataPanel != null)
        {
            metadataPanel.SetActive(false);
        }

        this._metadataValues = new Dictionary<String, CesiumMetadataValue>();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        bool receivedInput = false;

        if (Mouse.current != null)
        {
            receivedInput = Mouse.current.leftButton.isPressed;
        }
        else if (Gamepad.current != null)
        {
            receivedInput = Gamepad.current.rightShoulder.isPressed;
        }
#else
        bool receivedInput = Input.GetMouseButtonDown(0);
#endif

        if (receivedInput && metadataText != null)
        {
            metadataText.text = String.Empty;

            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.TransformDirection(Vector3.forward),
                    out hit,
                    Mathf.Infinity))
            {
                CesiumPrimitiveFeatures features = hit.transform.GetComponent<CesiumPrimitiveFeatures>();
                CesiumModelMetadata metadata = hit.transform.GetComponentInParent<CesiumModelMetadata>();

                if (features != null && features.featureIdSets.Length > 0)
                {
                    CesiumFeatureIdSet featureIdSet = features.featureIdSets[0];
                    Int64 propertyTableIndex = featureIdSet.propertyTableIndex;
                    if (metadata != null && propertyTableIndex >= 0 && propertyTableIndex < metadata.propertyTables.Length)
                    {
                        CesiumPropertyTable propertyTable = metadata.propertyTables[propertyTableIndex];
                        Int64 featureID = featureIdSet.GetFeatureIdFromRaycastHit(hit);
                        propertyTable.GetMetadataValuesForFeature(this._metadataValues, featureID);
                    }

                    foreach (var valuePair in this._metadataValues)
                    {
                        string valueAsString = valuePair.Value.GetString();
                        if (!String.IsNullOrEmpty(valueAsString) && valueAsString != "null")
                        {
                            metadataText.text += "<b>" + valuePair.Key + "</b>" + ": " + valueAsString + "\n";
                        }
                    }
                }
            }

            if (metadataPanel != null)
            {
                metadataPanel.SetActive(metadataText.text.Length > 0);
            }
        }
    }
}