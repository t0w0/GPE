using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshRenderer : MonoBehaviour {
	
	// Update is called once per frame
	void Start () {
		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer> ();
		renderer.material = Resources.Load<Material>("ParkMaterial");
	}
}
