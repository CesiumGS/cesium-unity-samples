using CesiumForUnity;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;

#if CESIUM_MAGIC_LEAP
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;
#endif

public class CesiumSamplesLocationBrowser : MonoBehaviour
{
    /// <summary>
    /// The radius around the origin that the icons are visible in.
    /// </summary>
    [SerializeField]
    private float _originRadius = 500.0f;

    [SerializeField]
    private float _iconHoverYOffset = 2.0f;

    [SerializeField]
    private float _iconHoverEffectTime = 1.0f;

    [SerializeField]
    private float _iconDistanceFromCamera = 10.0f;

    [SerializeField]
    private CesiumSamplesLocationsData _locationsData;

    [SerializeField]
    private GameObject _locationIconCanvasPrefab;

    [SerializeField]
    private Transform _locationIconsParent;

    [SerializeField]
    private CesiumGlobeAnchor _globeAnchor;

    [SerializeField]
    private CesiumFlyToController _flyToController;

    [SerializeField]
    private CanvasGroup _backButtonGroup;

    [SerializeField]
    private CanvasGroup _sliderGroup;

    [SerializeField]
    private Slider _dimmerSlider;

    private CesiumGeoreference _georeference;
    private double3 _originEcef;
    private CanvasGroup[] _createdCanvasGroups;
    private GameObject[] _createdGameObjects;

    // Used for tweening when mousing over icons.
    private float[] _iconOffsets;
    private float[] _iconOffsetTargets;
    private float[] _iconOffsetVelocities;

#if CESIUM_MAGIC_LEAP
    private MagicLeapRenderingExtensionsFeature _renderFeature;
#endif

    public void ToggleSliderGroup()
    {
        _sliderGroup.interactable = _sliderGroup.blocksRaycasts = !_sliderGroup.interactable;
        _sliderGroup.alpha = _sliderGroup.interactable ? 1 : 0;
    }

    public void OnBackButtonPressed()
    {
        MoveToLocation(_locationsData.Locations[0]);
    }

    private void Awake()
    {
#if CESIUM_MAGIC_LEAP
        _renderFeature = OpenXRSettings.Instance.GetFeature<MagicLeapRenderingExtensionsFeature>();
        _renderFeature.GlobalDimmerEnabled = true;
        _dimmerSlider.value = _renderFeature.GlobalDimmerValue;
        _dimmerSlider.onValueChanged.AddListener(val =>
        {
            _renderFeature.GlobalDimmerValue = val;
        });
#endif

        _sliderGroup.alpha = 0;
        _sliderGroup.interactable = _sliderGroup.blocksRaycasts = false;
        _backButtonGroup.alpha = 0;
        _backButtonGroup.interactable = _backButtonGroup.blocksRaycasts = false;

        _georeference = this._globeAnchor.GetComponentInParent<CesiumGeoreference>();

        CesiumSamplesLocationsData.Location originLocation = this._locationsData.Locations.First();
        this._originEcef = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(originLocation.Longitude, originLocation.Latitude, originLocation.Height));
        this._globeAnchor.positionGlobeFixed = this._originEcef;

        this._iconOffsets = new float[this._locationsData.Locations.Length];
        this._iconOffsetTargets = new float[this._locationsData.Locations.Length];
        this._iconOffsetVelocities = new float[this._locationsData.Locations.Length];
        this._createdCanvasGroups = new CanvasGroup[this._locationsData.Locations.Length];
        this._createdGameObjects = new GameObject[this._locationsData.Locations.Length];

        // Create icons for each location.
        for (int i = 0; i < this._locationsData.Locations.Length; i++)
        {
            int locationIndex = i;

            CesiumSamplesLocationsData.Location loc = this._locationsData.Locations[i];
            this._iconOffsets[i] = 0;
            this._iconOffsetTargets[i] = 0;
            this._iconOffsetVelocities[i] = 0;

            GameObject newObject = Instantiate(_locationIconCanvasPrefab, _locationIconsParent);
            TMP_Text newText = newObject.GetComponentInChildren<TMP_Text>();
            newText.text = loc.Name;

            // Add a canvas group so we can fade this icon as needed
            CanvasGroup newCanvasGroup = newObject.AddComponent<CanvasGroup>();
            this._createdCanvasGroups[i] = newCanvasGroup;
            this._createdGameObjects[i] = newObject;

            // Hook up events
            EventTrigger ev = newObject.AddComponent<EventTrigger>();

            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener(ev =>
                {
                    this._iconOffsetTargets[locationIndex] = 1.0f;
                });
                ev.triggers.Add(entry);
            }

            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(ev =>
                {
                    MoveToLocation(loc);
                });
                ev.triggers.Add(entry);
            }

            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener(ev =>
                {
                    this._iconOffsetTargets[locationIndex] = 0.0f;
                });
                ev.triggers.Add(entry);
            }
        }
    }

    private void MoveToLocation(CesiumSamplesLocationsData.Location location)
    {
        _flyToController.FlyToLocationLongitudeLatitudeHeight(new double3(location.Longitude, location.Latitude, location.Height), 0, 0, false);
    }

    private void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;

        float distanceToOrigin = Vector3.Distance(
            (float3)this._georeference.TransformEarthCenteredEarthFixedPositionToUnity(this._originEcef),
            cameraPosition);
        float normalizedDistanceToOrigin = Mathf.Clamp01(distanceToOrigin / this._originRadius);

        for (int i = 0; i < _createdGameObjects.Length; i++)
        {
            CesiumSamplesLocationsData.Location loc = _locationsData.Locations[i];

            // Base alpha on distance from origin, making for a smooth fade out as we move away from the origin
            _createdCanvasGroups[i].alpha = 1.0f - normalizedDistanceToOrigin;
            _createdCanvasGroups[i].blocksRaycasts = _createdCanvasGroups[i].interactable = normalizedDistanceToOrigin < 0.1f;

            // We want to place the icon close to the camera but in the direction of the actual position it's representing
            Vector3 realLocationPosition = LocationToUnityCoordinates(loc);
            Vector3 cameraDir = (realLocationPosition - cameraPosition).normalized;

            _createdGameObjects[i].transform.position = cameraPosition + cameraDir * _iconDistanceFromCamera + _iconOffsets[i] * Camera.main.transform.up * this._iconHoverYOffset;
            _createdGameObjects[i].transform.rotation = Quaternion.LookRotation(cameraDir);

            _iconOffsets[i] = Mathf.SmoothDamp(this._iconOffsets[i], this._iconOffsetTargets[i], ref this._iconOffsetVelocities[i], Time.deltaTime * this._iconHoverEffectTime);
        }

        _backButtonGroup.alpha = normalizedDistanceToOrigin;
        _backButtonGroup.interactable = _backButtonGroup.blocksRaycasts = normalizedDistanceToOrigin > 0.9f;
    }

    private Vector3 LocationToUnityCoordinates(CesiumSamplesLocationsData.Location loc)
    {
        return (float3)_georeference.TransformEarthCenteredEarthFixedPositionToUnity(CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(loc.Longitude, loc.Latitude, loc.Height)));
    }
}