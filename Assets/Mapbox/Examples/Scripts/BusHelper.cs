namespace Mapbox.Examples
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Mapbox.Unity.MeshGeneration.Interfaces;

	public class BusHelper : MonoBehaviour, IFeaturePropertySettable
	{
		public static RectTransform Parent;
		public static GameObject UiPrefab;

		private GameObject _uiObject;

		public void Set(Dictionary<string, object> props)
		{
			if (Parent == null)
			{
				var canv = GameObject.Find("Canvas");
				var ob = new GameObject("BusPoiContainer");
				ob.transform.SetParent(canv.transform);
				Parent = ob.AddComponent<RectTransform>();
				UiPrefab = Resources.Load<GameObject>("BusUiPrefab");
			}

			_uiObject = Instantiate(UiPrefab);
			_uiObject.transform.SetParent(Parent);
			_uiObject.transform.GetChild(2).GetComponent<Text>().text = props["route_short_name"].ToString();
			if (props.ContainsKey("stop_name"))
			{
				_uiObject.GetComponentInChildren<Text>().text = props["stop_name"].ToString();
			}
		}

		public void LateUpdate()
		{
			if (_uiObject)
				_uiObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);
		}
	}
}