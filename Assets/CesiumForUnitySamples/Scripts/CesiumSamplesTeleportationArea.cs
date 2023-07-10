using UnityEngine;
using CesiumForUnity;
using UnityEngine.XR.Interaction.Toolkit;

public class CesiumSamplesTeleportationArea : MonoBehaviour
{
    void Start()
    {
        Cesium3DTileset tileset = GetComponent<Cesium3DTileset>();
        if (tileset != null)
        {
            tileset.OnTileGameObjectCreated += go =>
                go.AddComponent<TeleportationArea>();
        }
    }
}