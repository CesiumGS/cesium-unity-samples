using CesiumForUnity;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CesiumSamplesDynamicCameraProvider : ActionBasedContinuousMoveProvider
{
    private readonly float _dynamicClippingPlanesMinHeight = 10000.0f;
    private readonly float _initialNearClipPlane = 0.1f;
    private readonly float _initialFarClipPlane = 10000000.0f;
    private readonly float _maximumNearClipPlane = 1000.0f;
    private readonly float _maximumFarClipPlane = 500000000.0f;
    private readonly float _maximumNearToFarRatio = 100000.0f;
    private readonly float _earthDiameter = (float)(2.0 * CesiumWgs84Ellipsoid.GetMaximumRadius());
    private CesiumGeoreference _georeference;

    private bool GetHeight(Transform transform, out float height)
    {
        double3 center =
            this._georeference.TransformEarthCenteredEarthFixedPositionToUnity(double3.zero);

        RaycastHit hitInfo;
        if (Physics.Linecast(transform.position, (float3)center, out hitInfo))
        {
            height = Vector3.Distance(transform.position, hitInfo.point);
            return true;
        }
        height = 0.0f;
        return false;
    }

    private void UpdateClippingPlanes(float height, Camera camera)
    {
        float nearClipPlane = this._initialNearClipPlane;
        float farClipPlane = this._initialFarClipPlane;

        if (height > this._dynamicClippingPlanesMinHeight)
        {
            farClipPlane = height + _earthDiameter;
            farClipPlane = Mathf.Min(farClipPlane, this._maximumFarClipPlane);

            float farClipRatio = farClipPlane / this._maximumNearToFarRatio;

            if (farClipRatio > nearClipPlane)
            {
                nearClipPlane = Mathf.Min(farClipRatio, this._maximumNearClipPlane);
            }
        }
        camera.nearClipPlane = nearClipPlane;
        camera.farClipPlane = farClipPlane;
    }

    protected override void Awake()
    {
        base.Awake();
        this._georeference = GetComponentInParent<CesiumGeoreference>();
        endLocomotion += DynamicCameraProvider_endLocomotion;
    }

    private void DynamicCameraProvider_endLocomotion(LocomotionSystem obj)
    {
        float height;
        if(GetHeight(system.xrOrigin.transform, out height))
        {
            this.moveSpeed = height;
            this.UpdateClippingPlanes(height, system.xrOrigin.Camera);
        }
    }
}
