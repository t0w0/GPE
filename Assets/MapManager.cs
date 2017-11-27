using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Geocoding;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class MapManager : MonoBehaviour {

	public float lon;
	public float lat;
	private bool start;
	public AbstractMap _map;
	public BasicMap _map1;

	void Awake () {
		DontDestroyOnLoad(this);
	}

	public void SetLon (string s){
		lon = float.Parse(s);
	}
	public void SetLat (string s){
		lat = float.Parse(s);
	}
	public void CreateMap () {
		SceneManager.LoadScene ( 1 );
	}
}
