using UnityEngine;
using CesiumForUnity;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MetadataPicking : MonoBehaviour
{
    public GameObject Panel;
    public Text Text;
    void Update()
    {
        if (Keyboard.current.spaceKey.IsPressed())
        {
            if (Text != null)
            {
                Text.text = "";
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    var metadataScript = hit.transform.GetComponentInParent<CesiumMetadata>();
                    if (metadataScript != null)
                    {
                        metadataScript.loadMetadata(hit.transform, hit.triangleIndex);
                        foreach (MetadataProperty property in metadataScript.Properties())
                        {
                            string propertyName = property.GetPropertyName();
                            string propertyValue = property.GetString("null");
                            if(propertyValue != "null" && propertyValue != "")
                            {
                                Text.text += "<b>" + propertyName + "</b>" + ": " + propertyValue + "\n";
                            }
                        }
                    }
                }
                if (Text.text.Length != 0)
                {
                    Panel.SetActive(true);
                }
                else
                {
                    Panel.SetActive(false);
                }
            }
        }
    }
}