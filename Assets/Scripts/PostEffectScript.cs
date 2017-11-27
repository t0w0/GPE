using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PostEffectScript : MonoBehaviour {

	//[Range(0f, 3f)]
	//public float depthLevel = 0.5f;
	//[Range(0f, 1f)]
	//public float edgePresence = 0.5f;


	public Material mat;

	private void Start ()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			print("System doesn't support image effects");
			enabled = false;
			return;
		}
		if (mat == null || !mat.shader.isSupported)
		{
			enabled = false;
			print("Shader " + mat.shader.name + " is not supported");
			return;
		}

		// turn on depth rendering for the camera so that the shader can access it via _CameraDepthTexture
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage ( RenderTexture src, RenderTexture dest ) {
		// rc is the full rendered scene taht you would normally
		// send directly to the monitor. We are intercepting
		// this so we can do a bit mor work, before passing it on.

		Graphics.Blit (src, dest, mat);

		if (mat != null)
		{
			//mat.SetFloat("_DepthLevel", depthLevel);
			//mat.SetFloat("_EdgePresence", edgePresence);
			Graphics.Blit(src, dest, mat);
		}
		else
		{
			Graphics.Blit(src, dest);
		}

	}
}
