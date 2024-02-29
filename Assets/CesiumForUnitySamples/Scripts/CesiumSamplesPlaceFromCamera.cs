using UnityEngine;

public class CesiumSamplesPlaceFromCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 _cameraOffset = Vector3.zero;

    private void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        transform.position = cameraPos + Camera.main.transform.rotation * _cameraOffset;
        transform.rotation = Quaternion.LookRotation(transform.position - cameraPos);
    }
}
