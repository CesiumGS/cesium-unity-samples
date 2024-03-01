using UnityEngine;
using CesiumForUnity;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class MetadataInteractable : XRBaseInteractable
{
    public TextMeshProUGUI metadataText;
    public GameObject canvas;

    private static HashSet<string> ignoreProperties = new HashSet<string>() {
        "addr:city",
        "addr:country",
        "addr:housenumber",
        "addr:postcode",
        "addr:state",
        "addr:street",
        "building:colour",
        "cesium#color",
        "cesium#estimatedHeight",
        "cesium#latitude",
        "cesium#longitude",
        "ele",
        "elementId",
        "elementType",
        "gnis:county_id",
        "gnis:created",
        "gnis:edited",
        "gnis:feature_id",
        "gnis:state_id",
        "layer",
        "name",
        "name:ar",
        "name:de",
        "name:el",
        "name:es",
        "name:etymology",
        "name:fr",
        "name:he",
        "name:hi",
        "name:hr",
        "name:ja",
        "name:ko",
        "name:pl",
        "name:uk",
        "name:zh",
        "name:zh_pinyin",
        "nycdoitt:bin",
        "part#elementId",
        "part#elementType",
        "part#building:colour",
        "part#roof:colour",
        "part#roof:direction",
        "part#addr:city",
        "part#addr:country",
        "part#addr:housenumber",
        "part#addr:postcode",
        "part#addr:state",
        "part#addr:street",
        "part#wikidata",
        "part#nycdoitt:bin",
        "roof:colour",
        "type",
        "wikidata",
      };

    private string GetInterestingProperties(CesiumPropertyTable propertyTable, Int64 featureID)
    {
        string result = "";
        foreach (var propertyName in propertyTable.properties.Keys)
        {
            if (!ignoreProperties.Contains(propertyName))
            {
                string propertyValue = propertyTable.properties[propertyName].GetString(featureID);
                if (!String.IsNullOrEmpty(propertyValue) && propertyValue != "null")
                {
                    result += $"{propertyName}: {propertyValue}\n";
                }
            }
        }
        return result;
    }

    private Vector3 GetTopOfBuilding(Vector3 hitLocation, float buildingHeight)
    {
        var georeference = GetComponentInParent<CesiumGeoreference>();
        if (georeference != null)
        {
            foreach (RaycastHit hit in Physics.RaycastAll(hitLocation, Vector3.down, 100.0f))
            {
                if (!hit.transform.parent.name.Contains("CesiumWorldTerrain"))
                {
                    continue;
                }
                Vector3 topOfBuilding = hit.point;
                topOfBuilding.y += (float)(buildingHeight * georeference.scale);
                return topOfBuilding;
            }
        }
        return Vector3.zero;
    }

    const string estimatedHeightKey = "cesium#estimatedHeight";

    protected override void OnActivated(ActivateEventArgs args)
    {
        RaycastHit hit;
        Transform interactorTransform = args.interactorObject.transform;
        if (Physics.Raycast(interactorTransform.position,
                            interactorTransform.TransformDirection(Vector3.forward),
                            out hit, Mathf.Infinity))
        {
            CesiumPrimitiveFeatures features = hit.transform.GetComponent<CesiumPrimitiveFeatures>();
            CesiumModelMetadata metadata =
                hit.transform.GetComponentInParent<CesiumModelMetadata>();
            if (features != null && metadata != null && metadata.propertyTables.Length > 0)
            {
                CesiumPropertyTable propertyTable = metadata.propertyTables[0];

                Int64 featureID = features.GetFeatureIdFromRaycastHit(hit);

                float estimatedHeight = 0.0f;
                if (propertyTable.properties.ContainsKey(estimatedHeightKey))
                {
                    estimatedHeight = propertyTable.properties[estimatedHeightKey].GetFloat(featureID);
                }

                Vector3 topOfBuilding = GetTopOfBuilding(hit.point, estimatedHeight);
                canvas.transform.position = topOfBuilding;
                Vector3 camPos = Camera.main.transform.position;
                float distance = Vector3.Distance(camPos, topOfBuilding);
                if (distance > 1.0f)
                {
                    canvas.transform.localScale = distance * Vector3.one;
                }
                canvas.transform.rotation = Quaternion.LookRotation(
                    new Vector3(topOfBuilding.x - camPos.x, 0, topOfBuilding.z - camPos.z), Vector3.up);

                string name = String.Empty;
                if (propertyTable.properties.ContainsKey("name"))
                {
                    name = propertyTable.properties["name"].GetString(featureID);
                }

                if (String.IsNullOrEmpty(name) || name == "null")
                {
                    name = "Name N/A";
                }

                metadataText.text =
                    $"<size=150%><b>{name}</b></size>\n<size=75%>{GetInterestingProperties(propertyTable, featureID)}</size>";

                canvas.SetActive(true);
            }
        }
    }
}

public class CesiumSamplesMetadataPickingVR : MonoBehaviour
{
    public Cesium3DTileset tileset;
    public CharacterController characterController;
    public TextMeshProUGUI metadataText;
    public GameObject canvas;
    public InputActionReference activateButton;
    public XRRayInteractor rayInteractor;
    void Start()
    {
        if (activateButton != null)
        {
            activateButton.action.performed += Action_performed;
        }
        if (tileset != null && characterController != null &&
            metadataText != null)
        {
            tileset.OnTileGameObjectCreated += go =>
            {
                foreach (Transform child in go.transform)
                {
                    var mc = child.GetComponent<MeshCollider>();
                    if (mc != null)
                    {
                        Physics.IgnoreCollision(mc, characterController);
                    }
                }
                var script = go.AddComponent<MetadataInteractable>();
                script.metadataText = metadataText;
                script.canvas = canvas;
            };
        }
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (!rayInteractor.IsOverUIGameObject())
        {
            canvas.SetActive(false);
        }
    }
}
