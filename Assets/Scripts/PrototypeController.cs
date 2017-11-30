using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public class PrototypeController : MonoBehaviour {

	private GameObject map;
	private GameObject mapText;
	private GameObject can;
	private Light sun;
	private Dropdown shadowDrop;
	private Dropdown skinDrop;

	public float maxScaleWidth;
	public RectTransform scaleImage;
	public Text scaleText;
	public RectTransform compass;

	private List<MeshRenderer> buildings = new List<MeshRenderer>();


	// Use this for initialization
	void Start () {
		map = GameObject.Find ("Map");
		mapText = GameObject.Find ("MapText");
		can = GameObject.Find ("Canvas");
		GetScale();
		// sun = GameObject.Find ("Sun").GetComponent<Light>();
		// shadowDrop = GameObject.Find ("Ombres").GetComponent<Dropdown> ();
		// skinDrop = GameObject.Find ("Skins").GetComponent<Dropdown> ();
		// TogglePoi();
		// ToggleLayer ("roads");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
		Quaternion r = Camera.main.transform.parent.rotation;
		Vector3 v = r.eulerAngles;
		compass.rotation = Quaternion.Euler( new Vector3( 0,  0, v.y ) );
	}

	public void SetRotation () {
		Quaternion r = Camera.main.transform.parent.rotation;
		Vector3 v = r.eulerAngles;
		Camera.main.transform.parent.rotation = Quaternion.Euler( new Vector3( v.x,  0, v.z ) );
		Camera.main.transform.parent.position = Vector3.zero;
	}

	public void GetScale () {

		// Calcule des longueusr en unités Unity
		float sideUnits; // longueur du coté de la tile en unité Unity
		float diagUnits;	// longueur de la diagonale de la tile en unité Unity

		sideUnits = map.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.extents.x * 2; // world units
		//diagUnits = sideUnits * Mathf.Sqrt(2);
		
		
		// Calcule des longueurs en metres
		float sideMeters; // longueur du coté de la tile en metres
		float diagMeters; // longueur de la diagonale de la tile en metres
		float lat = (float) map.GetComponent<AbstractMap>().coord.x;
		int zoom = map.GetComponent<AbstractMap>().Zoom;
		sideMeters = 256 * Conversions.GetTileScaleInMeters( lat, 16 ); // meters * 256px
		diagMeters = sideMeters * Mathf.Sqrt(2);

		
		// Calcule des longueurs en screenUnits
		float diagScreenUnit;
		diagScreenUnit = Vector3.Distance( Camera.main.WorldToScreenPoint( new Vector3( sideUnits, 0, sideUnits) ), Camera.main.WorldToScreenPoint( Vector3.zero ) ); // Screen Unit

		// 1 m = x screenUnit  
		float meterScreen = Mathf.Abs( diagScreenUnit / diagMeters );

		float scale = meterScreen * 500;


		if (scale > maxScaleWidth)
			scale = meterScreen * 300; 
		if (scale > maxScaleWidth)
			scale = meterScreen * 100; 
		if (scale > maxScaleWidth)
			scale = meterScreen * 50; 
		if (scale > maxScaleWidth)
			scale = meterScreen * 30;
		if (scale > maxScaleWidth)
			scale = meterScreen * 10;
		scaleImage.sizeDelta = new Vector2 (scale, scaleImage.sizeDelta.y);
		scaleText.text = scale/meterScreen + " m";
	}
	public void ChangeLocation () {
		GameObject.Destroy(GameObject.FindGameObjectWithTag("PersistantObject"));
		SceneManager.LoadScene(0);
	}
	public void ToggleMap() {
		if (map.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled ) {
			foreach ( Transform tr in map.transform ) {
				tr.GetComponent<MeshRenderer>().enabled = false;
			}
		}
		else {
			foreach ( Transform tr in map.transform ) {
				tr.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}

	public void ToggleTextMap() {
		if (mapText.activeSelf)
			mapText.SetActive(false);
		else
			mapText.SetActive(true);
	}

	public void ToggleLayer( string layer) {
		if ( map.transform.GetChild (0).Find(layer).gameObject.activeSelf ) {
 			foreach ( Transform tr in map.transform ) {
 				foreach ( Transform t in tr )
 					if (t.gameObject.name == layer) {
 						t.gameObject.SetActive (false);
 					}
			}
		}
		else {
			foreach ( Transform tr in map.transform ) {
 				foreach ( Transform t in tr )
 					if (t.gameObject.name == layer) {
 						t.gameObject.SetActive (true);
 					}
			}
		}
	}

	public void ToggleRoads() {
		if (map.transform.GetChild (0).GetChild (2).gameObject.activeSelf)
			map.transform.GetChild (0).GetChild (2).gameObject.SetActive(false);
		else
			map.transform.GetChild (0).GetChild (2).gameObject.SetActive(true);
	}

	public void ToggleLanduse() {
		if (map.transform.GetChild (0).GetChild (0).gameObject.activeSelf)
			map.transform.GetChild (0).GetChild (0).gameObject.SetActive(false);
		else
			map.transform.GetChild (0).GetChild (0).gameObject.SetActive(true);
	}

	public void TogglePoi() {
		if (can.activeSelf)
			can.SetActive(false);
		else
			can.SetActive(true);
	}

	public void ChangeShadows (int light) {
		switch (shadowDrop.value) {
		case 0:
			sun.shadows = LightShadows.Soft;
			break;
		case 1:
			sun.shadows = LightShadows.Hard;
			break;
		case 2:
			sun.shadows = LightShadows.None;
			break;
		}
	}

	public void SlideCameraAngle(float angle) {
		Camera.main.transform.rotation = Quaternion.Euler (new Vector3 (angle * 90, 0, 0)); 
	}

	public void ChangeSkin (int skin) {
		switch (skinDrop.value) {
		case 0:
			foreach (MeshRenderer m in buildings) {
				
			}
			break;
		case 1 :
			foreach (MeshRenderer m in buildings) {

			}
			break;
		case 2 :
			foreach (MeshRenderer m in buildings) {

			}
			break;
		}
	}
}
