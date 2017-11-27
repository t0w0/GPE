using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.MeshGeneration.Interfaces;

public class MakiHelper : MonoBehaviour, IFeaturePropertySettable
{
	public static Transform Parent;
	public static GameObject TreePrefab;

	private GameObject _treeObject;

	public void Set(Dictionary<string, object> props)
	{
		if (Parent == null)
		{
			//var canv = GameObject.Find("Trees");
			//var ob = new GameObject("TreeContainer");
			//ob.transform.SetParent(canv.transform);
			//Parent = ob.AddComponent<RectTransform>();
			Parent = GameObject.Find("Trees").transform;
			TreePrefab = Resources.Load<GameObject>("tree1");
		}

		if (props.ContainsKey("maki"))
		{
			_treeObject = Instantiate(TreePrefab);
			_treeObject.transform.SetParent(Parent);
			//_treeObject.transform.GetComponent<MeshFilter>().mesh = Resources.Load<Sprite>("maki/" + props["maki"].ToString() + "-15");
			if (props.ContainsKey("name"))
			{
				_treeObject.GetComponentInChildren<Text>().text = props["name"].ToString();
			}
		}
	}

	public void LateUpdate()
	{
		if (_treeObject)
			_treeObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);
	}
}
