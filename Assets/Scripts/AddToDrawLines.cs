using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToDrawLines : MonoBehaviour {

	public Edge[] edges;

	// Use this for initialization
	void Start () {
		edges = DrawEdges.BuildManifoldEdges (this.GetComponent<MeshFilter> ().sharedMesh);
		Camera.main.GetComponent<DrawEdges> ().gos.Add (gameObject);	
	}
}
