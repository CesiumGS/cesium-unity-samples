using CesiumForUnity;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;



#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CesiumSamplesMetadataPickingAEC : MonoBehaviour
{
    // The GameObject with the UI to enable / disable depending on
    // whether metadata has been picked.
    public GameObject metadataPanel;

    // The text to display the metadata properties.
    public Text metadataText;

    // A game object used to mark the click location
    public GameObject metadataMarker;

    // Cached Dictionary of metadata values. This prevents reallocation every
    // time metadata is sampled from the tileset.
    private Dictionary<String, CesiumMetadataValue> _metadataValues;

    void Start()
    {
        if (metadataPanel != null)
        {
            metadataPanel.SetActive(true);
        }

        if (metadataMarker != null) 
        {
            metadataMarker.SetActive(true);
        }

        this._metadataValues = new Dictionary<String, CesiumMetadataValue>();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        bool receivedInput = false;

        if (Mouse.current != null)
        {
            receivedInput = Mouse.current.leftButton.wasPressedThisFrame;
        }
       
#else
        bool receivedInput = Input.GetMouseButtonDown(0);
#endif

        if (receivedInput && metadataText != null && metadataMarker != null && !EventSystem.current.IsPointerOverGameObject())
        {
            metadataText.text = "Select a building element to view metadata \r\nproperties";
            metadataMarker.SetActive(false);

            RaycastHit hit;

#if ENABLE_INPUT_SYSTEM
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
            if (Physics.Raycast(
                    ray,
                    out hit,
                    Mathf.Infinity))
            {
                CesiumPrimitiveFeatures features = hit.transform.GetComponent<CesiumPrimitiveFeatures>();
                CesiumModelMetadata metadata = hit.transform.GetComponentInParent<CesiumModelMetadata>();

                if (features != null && features.featureIdSets.Length > 0)
                {
                    metadataText.text = String.Empty;

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
                    metadataText.text = metadataText.text.TrimEnd("\n");

                    metadataMarker.SetActive(true);
                    metadataMarker.transform.position = hit.point;
                }
            }
        }

        if (metadataMarker != null && metadataMarker.activeInHierarchy)
        {
            metadataMarker.transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, metadataMarker.transform.position);
        }
    }
}