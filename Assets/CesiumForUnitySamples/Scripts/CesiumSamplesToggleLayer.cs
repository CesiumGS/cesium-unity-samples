using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CesiumSamplesToggleLayer : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        else
        {
            Debug.LogError("CesiumSamplesToggleLayer requires a Toggle component on the same GameObject.");
        }
    }

    private void Start()
    {
        if (target != null)
        {
            target.SetActive(toggle.isOn);
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (target != null)
        {
            target.SetActive(isOn);
        }
    }
}
