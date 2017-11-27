using UnityEngine;
using System.Collections;
 
[ExecuteInEditMode]
public class ImageEffectClearAlpha : MonoBehaviour {
 
    //Shader clears alpha channel only
   // public Shader clearAlphaShader;
 
    public Material material;
    /*protected static Material material {
        get {
            if (m_Material != null) {
                m_Material.hideFlags = HideFlags.HideAndDontSave;
                m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }*/
   
   
    // Called by camera to apply image effect
    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Graphics.Blit (source, destination, material);
    }
}