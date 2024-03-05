using UnityEngine;
using CesiumForUnity;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using System.Linq;

public class CesiumSamplesMetadataPickingVR : MonoBehaviour
{
    public Cesium3DTileset tileset;
    public CharacterController characterController;
    public TextMeshProUGUI metadataText;
    public GameObject canvas;
    public InputActionProperty activateButton;
    public XRRayInteractor rayInteractor;
    public LayerMask layerMask;
    public bool useXrInteractionSystem = true;

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


    private void Start()
    {
        if (activateButton.action != null)
        {
            activateButton.action.Enable();
            activateButton.action.performed += Action_performed;
        }
    }

    private string GetInterestingProperties(Dictionary<string, CesiumMetadataValue> metadataValues)
    {
        string ret = "";
        foreach ((string propertyName, CesiumMetadataValue property) in metadataValues.OrderBy(kv => kv.Key))
        {
            if (!ignoreProperties.Contains(propertyName))
            {
                string propertyValue = property.GetString("null");
                if (propertyValue != "null")
                {
                    ret += $"{propertyName}: {propertyValue}\n";
                }
            }
        }
        return ret;
    }

    private string GetBuildingName(Dictionary<string, CesiumMetadataValue> metadataValues)
    {
        if (metadataValues.ContainsKey("name:en") && metadataValues["name:en"].GetString("null") != null)
        {
            return metadataValues["name:en"].GetString();
        }

        if (metadataValues.ContainsKey("name") && metadataValues["name"].GetString("null") != null)
        {
            return metadataValues["name"].GetString();
        }

        if (metadataValues.ContainsKey("addr:housename") && metadataValues["addr:housename"].GetString("null") != null)
        {
            return metadataValues["addr:housename"].GetString();
        }

        if (metadataValues.ContainsKey("addr:street") && metadataValues.ContainsKey("addr:housenumber"))
        {
            return metadataValues["addr:housenumber"] + " " + metadataValues["addr:street"];
        }

        return "Name N/A";
    }

    private Vector3 GetTopOfBuilding(Vector3 hitLocation, Dictionary<string, CesiumMetadataValue> metadataValues)
    {
        var georeference = GetComponentInParent<CesiumGeoreference>();
        if (georeference != null)
        {
            float buildingHeight =
                metadataValues.ContainsKey("cesium#estimatedHeight") ?
                    metadataValues["cesium#estimatedHeight"].GetFloat(0.0f) :
                    0.0f;

            foreach (RaycastHit hit in Physics.RaycastAll(hitLocation, Vector3.down, 100.0f, layerMask.value))
            {
                if (!hit.transform.IsChildOf(tileset.transform))
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

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (!rayInteractor.IsOverUIGameObject())
        {
            canvas.SetActive(false);
        }

        if (!useXrInteractionSystem && !rayInteractor.IsOverUIGameObject())
        {
            if (Physics.Raycast(
                rayInteractor.transform.position,
                rayInteractor.transform.TransformDirection(Vector3.forward),
                out RaycastHit hit,
                Mathf.Infinity,
                layerMask.value))
            {
                CesiumPrimitiveFeatures features = hit.transform.GetComponent<CesiumPrimitiveFeatures>();
                if (features != null && features.featureIdSets.Length > 0)
                {
                    CesiumFeatureIdSet featureIdSet = features.featureIdSets[0];
                    long propertyTableIndex = featureIdSet.propertyTableIndex;

                    CesiumModelMetadata metadata = hit.transform.GetComponentInParent<CesiumModelMetadata>();

                    if (metadata != null && propertyTableIndex >= 0 && propertyTableIndex < metadata.propertyTables.Length)
                    {
                        CesiumPropertyTable propertyTable = metadata.propertyTables[propertyTableIndex];
                        long featureId = featureIdSet.GetFeatureIdFromRaycastHit(hit);
                        Dictionary<string, CesiumMetadataValue> metadataValues = new Dictionary<string, CesiumMetadataValue>();
                        propertyTable.GetMetadataValuesForFeature(metadataValues, featureId);

                        Vector3 camPos = Camera.main.transform.position;
                        Vector3 dir = Vector3.Normalize(hit.point - camPos);
                        canvas.transform.parent.position = camPos + dir * 20;

                        //canvas.transform.parent.rotation = Quaternion.LookRotation(new Vector3(topOfBuilding.x - camPos.x, 0, topOfBuilding.z - camPos.z), Vector3.up);
                        canvas.transform.parent.rotation = Quaternion.LookRotation(dir);

                        metadataText.text =
                            $"<size=150%><b>{GetBuildingName(metadataValues)}</b></size>\n<size=75%>{GetInterestingProperties(metadataValues)}</size>";
                        Debug.Log(metadataText.text);

                        canvas.SetActive(true);
                    }
                }
            }
        }
    }
}
