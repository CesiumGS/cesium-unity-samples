using CesiumForUnity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CesiumSamplesMetadataPickingMagicLeap : MonoBehaviour
{
    public Cesium3DTileset tileset;
    public CharacterController characterController;
    public TextMeshProUGUI metadataText;
    public GameObject canvas;
    public InputActionProperty activateButton;
    public XRRayInteractor rayInteractor;
    public LayerMask layerMask = ~0;
    public bool placeOnBuilding = true;

    private const string ESTIMATED_HEIGHT_KEY = "cesium#estimatedHeight";

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

    private string GetInterestingProperties(CesiumPropertyTable propertyTable, long featureId)
    {
        string ret = "";
        foreach ((string propertyName, CesiumPropertyTableProperty property) in propertyTable.properties.OrderBy(p => p.Key))
        {
            if (!ignoreProperties.Contains(propertyName))
            {
                string propertyValue = property.GetString(featureId, "null");
                if (!string.IsNullOrWhiteSpace(propertyValue) && propertyValue != "null")
                {
                    ret += $"{propertyName}: {propertyValue}\n";
                }
            }
        }
        return ret;
    }

    private string GetBuildingName(CesiumPropertyTable propertyTable, long featureId)
    {
        string name;
        if (TryGetString(propertyTable, featureId, "name:en", out name))
        {
            return name;
        }

        if (TryGetString(propertyTable, featureId, "name", out name))
        {
            return name;
        }

        if (TryGetString(propertyTable, featureId, "addr:housename", out name))
        {
            return name;
        }

        if (TryGetString(propertyTable, featureId, "addr:housenumber", out string houseNum) && TryGetString(propertyTable, featureId, "addr:street", out string street))
        {
            return $"{houseNum} {street}";
        }

        return "Name N/A";
    }

    private bool TryGetString(CesiumPropertyTable table, long featureId, string key, out string value)
    {
        if (table.properties.ContainsKey(key))
        {
            value = table.properties[key].GetString(featureId, "null");
            return !string.IsNullOrWhiteSpace(value) && value != "null";
        }

        value = null;
        return false;
    }

    private Vector3 GetTopOfBuilding(Vector3 hitLocation, CesiumPropertyTable propertyTable, long featureId)
    {
        var georeference = GetComponentInParent<CesiumGeoreference>();
        if (georeference != null)
        {
            float buildingHeight = 0.0f;
            if (propertyTable.properties.ContainsKey(ESTIMATED_HEIGHT_KEY))
            {
                buildingHeight = propertyTable.properties[ESTIMATED_HEIGHT_KEY].GetFloat(featureId, 0.0f);
            }

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

        if (!rayInteractor.IsOverUIGameObject())
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

                        if (placeOnBuilding)
                        {
                            Vector3 topOfBuilding = GetTopOfBuilding(hit.point, propertyTable, featureId);
                            float distance = Vector3.Distance(camPos, topOfBuilding);
                            if (distance > 1.0f)
                            {
                                canvas.transform.localScale = distance * Vector3.one;
                            }
                            canvas.transform.parent.rotation = Quaternion.LookRotation(new Vector3(topOfBuilding.x - camPos.x, 0, topOfBuilding.z - camPos.z), Vector3.up);
                        }
                        else
                        {
                            Vector3 dir = Vector3.Normalize(hit.point - camPos);
                            canvas.transform.parent.position = camPos + dir * 20;
                            canvas.transform.parent.rotation = Quaternion.LookRotation(dir);
                        }

                        metadataText.text =
                            $"<size=150%><b>{GetBuildingName(propertyTable, featureId)}</b></size>\n<size=75%>{GetInterestingProperties(propertyTable, featureId)}</size>";

                        canvas.SetActive(true);
                    }
                }
            }
        }
    }
}
