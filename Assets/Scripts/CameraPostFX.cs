using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraPostFX : MonoBehaviour
{

	public Material effect;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, effect);
	}
}
