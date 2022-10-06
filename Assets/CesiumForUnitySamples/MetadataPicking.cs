using UnityEngine;
using CesiumForUnity;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MetadataPicking : MonoBehaviour
{
    public GameObject RowTemplate;
    public GameObject Panel;
    public List<GameObject> Rows;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            foreach (GameObject Row in Rows)
            {
                GameObject.DestroyImmediate(Row);
            }
            Rows.Clear();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                var metadataScript = hit.transform.GetComponentInParent<CesiumMetadata>();
                if(metadataScript != null){
                    metadataScript.loadMetadata(hit.transform, hit.triangleIndex);
                    foreach(var kvp in metadataScript.Keys().Zip(metadataScript.Values(), Tuple.Create)){
                        string propertyName = kvp.Item1;
                        GameObject Row = GameObject.Instantiate(RowTemplate);
                        Row.name = propertyName;
                        Row.transform.SetParent(RowTemplate.transform.parent);
                        Row.SetActive(true);
                        Rows.Add(Row); 
                        
                        var Property = Row.transform.GetChild(0).GetComponent<Text>();
                        Property.text = propertyName;

                        var Value = Row.transform.GetChild(1).GetComponent<Text>();
                        MetadataValue metadataValue = kvp.Item2;
                        Value.text =metadataValue.GetString("null");
                    }
                }
            }
            if (Rows.Count != 0)
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