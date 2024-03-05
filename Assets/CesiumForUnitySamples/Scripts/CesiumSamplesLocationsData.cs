using CesiumForUnity;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Cesium/Cesium Samples Locations Data")]
public class CesiumSamplesLocationsData : ScriptableObject
{
	[SerializeField]
	private Location[] _locations = new Location[0];

	public Location[] Locations => _locations;

	[Serializable]
	public class Location
	{
		public string Name;
		public double Longitude;
		public double Latitude;
		public double Height;

		public bool IsEnabled = true;

		public double3 CoordinatesEcef => CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(new double3(Longitude, Latitude, Height));
	}
}