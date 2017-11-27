using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenShot : MonoBehaviour {

	public static int resWidth = 1920;
	public static int resHeight = 1080; 
	public static int scale = 1;
	public Camera myCamera;
	public static bool isTransparent;
	public static string path; 

	// Use this for initialization
	void Start () {
		path = Application.dataPath;
	    if (Application.platform == RuntimePlatform.OSXPlayer) {
	        path += "/../../";
	    }
	    else if (Application.platform == RuntimePlatform.WindowsPlayer) {
	        path += "/../";
	    }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			TakeHiResShot();
		}
		if (takeHiResShot) 
		{
			int resWidthN = resWidth*scale;
			int resHeightN = resHeight*scale;
			RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
			myCamera.targetTexture = rt;

			TextureFormat tFormat;
			if(isTransparent)
				tFormat = TextureFormat.ARGB32;
			else
				tFormat = TextureFormat.RGB24;


			Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat,false);
			myCamera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
			myCamera.targetTexture = null;
			RenderTexture.active = null; 
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidthN, resHeightN);
			
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));
			Application.OpenURL(filename);
			takeHiResShot = false;
		}
	}

	private bool takeHiResShot = false;
	public string lastScreenshot = "";

	public void TakeHiResShot() {
		Debug.Log("Taking Screenshot");
		takeHiResShot = true;
	}

	public string ScreenShotName(int width, int height) {

		string strPath="";

		strPath = string.Format("{0}/screen_{1}x{2}_{3}.png", 
		                     path, 
		                     width, height, 
		                               System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
		lastScreenshot = strPath;
	
		return strPath;
	}

	public void EditWidth (string v) {
		resWidth = int.Parse(v);
	}
	public void EditHeight (string v) {
		resHeight = int.Parse(v);
	}
	public void EditScale (float v) {
		scale = Mathf.FloorToInt(v);
	}
	public void EditPath (string p) {
		path = p;
	}
	public void EditTransparency (bool b) {
		isTransparent = b;
	}
}


