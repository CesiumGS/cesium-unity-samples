using System;
using System.Collections.Generic;
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
	}
}