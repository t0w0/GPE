using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileTexture : MonoBehaviour {

    // Give us the texture so that we can scale proportianally the width according to the height variable below
    // We will grab it from the meshRenderer
    public Texture texture;
    public Vector3 extents;
    public float textureToMeshZ = 1f; // Use this to contrain texture to a certain size

    Vector3 prevScale = Vector3.one;

    float prevTextureToMeshZ = -1f;

    // Use this for initialization
    void Start () {
        //texture = gameObject.GetComponent<MeshRenderer>().material.texture;
        this.prevScale = gameObject.transform.lossyScale;
        this.prevTextureToMeshZ = this.textureToMeshZ;

        this.UpdateTiling();
    }

    // Update is called once per frame
    void Update () {
        // If something has changed
        texture = gameObject.GetComponent<Renderer>().material.mainTexture;
        if(gameObject.transform.lossyScale != prevScale || !Mathf.Approximately(this.textureToMeshZ, prevTextureToMeshZ) || extents == null)
            this.UpdateTiling();

        // Maintain previous state variables
        this.prevScale = gameObject.transform.lossyScale;
        this.prevTextureToMeshZ = this.textureToMeshZ;
    }

    [ContextMenu("UpdateTiling")]
    void UpdateTiling()
    {
        // A Unity plane is 10 units x 10 units
        //float planeSizeX = 10f;
        //float planeSizeZ = 10f;

        extents = gameObject.GetComponent<Renderer>().bounds.extents; 
        foreach (Material mat in gameObject.GetComponent<Renderer>().sharedMaterials) {
        	// Figure out texture-to-mesh width based on user set texture-to-mesh height
        	this.texture = mat.mainTexture;
        	//float textureToMeshX = ((float)this.texture.width/this.texture.height)*this.textureToMeshZ;
        	 mat.mainTextureScale = new Vector2(extents.x * textureToMeshZ, extents.y * textureToMeshZ);
        }     
       
    }
}