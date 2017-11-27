using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectScriptFix : MonoBehaviour {

	public Material mat;

	// Use this for initialization
	void Start () {
		if ( !SystemInfo.supportsImageEffects ) {
			print( "System doesn't support image effects" );
			enabled = false;
			return;
		}
		if ( mat == null || !mat.shader.isSupported ) {
			print ( "Shader" + mat.shader.name + "is not supported" );
			enabled = false;
			return;
		}

		//turn on depth rendering for the camera so that the shader can access it via _CameraDepthTexture
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}
	
	void OnRenderImage ( RenderTexture src, RenderTexture dest ) {
		// src is the fulle rendered scene that you would normally
		// send directly to the monitor. We are intercapting
		// this so we can do a bit more work, before passing it on.

		if (mat != null) {
			Graphics.Blit ( src, dest, mat );
		}
		else {
			Graphics.Blit ( src, dest );
		}
	}
}
