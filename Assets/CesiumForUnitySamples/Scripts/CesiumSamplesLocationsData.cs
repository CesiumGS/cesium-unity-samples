using CesiumForUnity;
using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A series of locations saved as a scriptable object for use at runtime, e.g., to put UI markers
/// at each location on the globe.
/// </summary>
public class CesiumSamplesLocationData : ScriptableObject
{
    /// <summary>
    /// A representation of a location on the globe.
    /// </summary>
    [Serializable]
    public class Location
    {
        public string Name;
        public double Longitude;
        public double Latitude;
        public double Height;

        /// <summary>
        /// Can used to toggle whether or not a location is used in some way at runtime.
        /// </summary>
        public bool IsEnabled = true;

        public double3 CoordinatesEcef => CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(Longitude, Latitude, Height));
    }

    [SerializeField]
	private Location[] _locations = new Location[0];

    /// <summary>
    /// The array of locations saved in this object.
    /// </summary>
	public Location[] Locations => _locations;

}