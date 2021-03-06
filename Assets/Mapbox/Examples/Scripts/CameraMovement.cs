namespace Mapbox.Examples
{
	using UnityEngine;

	public class CameraMovement : MonoBehaviour
	{

		private PrototypeController protoScript;
		[SerializeField]
		float _panSpeed = 20f;

		[SerializeField]
		float _zoomSpeed = 50f;

		[SerializeField]
		float _rotationSpeed = 50f;

		[SerializeField]
		float _pitchSpeed = 50f;

		[SerializeField]
		Camera _referenceCamera;

		Quaternion _originalRotation;
		Vector3 _origin;
		Vector3 _delta;
		bool _shouldDrag;

		void Awake()
		{
			_originalRotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);

			if (_referenceCamera == null)
			{
				_referenceCamera = GetComponent<Camera>();
				if (_referenceCamera == null)
				{
					throw new System.Exception("You must have a reference camera assigned!");
				}
			}
			protoScript = GameObject.Find("PrototypeControl").GetComponent<PrototypeController>();
		}

		void LateUpdate()
		{
			var x = 0f;
			var y = 0f;
			var z = 0f;

			if (Input.GetMouseButton(0))
			{
				var mousePosition = Input.mousePosition;
				mousePosition.z = _referenceCamera.transform.parent.localPosition.y;
				_delta = _referenceCamera.ScreenToWorldPoint(mousePosition) - _referenceCamera.transform.parent.localPosition;
				_delta.y = 0f;
				if (_shouldDrag == false)
				{
					_shouldDrag = true;
					_origin = _referenceCamera.ScreenToWorldPoint(mousePosition);
				}
			}
			else
			{
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{
				var offset = _origin - _delta;
				offset.y = transform.parent.localPosition.y;
				transform.parent.localPosition = offset;
			}
			else
			{
				x = Input.GetAxis("Horizontal");
				z = Input.GetAxis("Vertical");
				y = -Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
				if (Camera.main.orthographic) {
					if (y < 0 && Camera.main.orthographicSize > 5)
						Camera.main.orthographicSize += y;
					else if (y > 0 && Camera.main.orthographicSize < 70)
						Camera.main.orthographicSize += y;
					//_referenceCamera.orthographicSize = Camera.main.orthographicSize += y;
				}
				else
					transform.parent.localPosition += transform.parent.forward * y + (_originalRotation * new Vector3(x * _panSpeed, 0, z * _panSpeed));
			protoScript.GetScale();
			}
		}

		public void RotateCamera (bool clockwise) {
			if (clockwise)
				transform.parent.Rotate(Vector3.up * _rotationSpeed);
			else
				transform.parent.Rotate(Vector3.up * -_rotationSpeed);
		}
	}
}