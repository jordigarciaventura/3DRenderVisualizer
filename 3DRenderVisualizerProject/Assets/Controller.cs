using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	Vector3 mousePosition;
	Vector3 origin;

	public bool isDragging;

	public float xSensitivity = 50;
	public float ySensitivity = 50;
	public float wheelSensitivity = 50;

	Vector2 bottomLeftCorner;
	Vector2 topRightCorner;

	RectTransform rt;

	public Viewer viewer;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	private void Start()
	{
		SetRectTransformCorners();
	}

	void Update()
	{
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			Debug.Log("Scroll down");
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			Debug.Log("Scroll up");
		}

		if (Input.GetMouseButtonDown(2))
		{
			Debug.Log("Panning");
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (VectorUtils.IsInsideRectangle(Input.mousePosition, bottomLeftCorner, topRightCorner))
			{
				origin = Input.mousePosition;
				isDragging = true;
			}
		}

		if (Input.GetMouseButtonUp(0))
			isDragging = false;

		if (isDragging)
		{
			mousePosition = Input.mousePosition;

			if (Mathf.Abs(mousePosition.x - origin.x) > xSensitivity)
			{
				if (mousePosition.x > origin.x)
				{
					//Rotate to left
					viewer.Rotate(0, -1);
					
				}
				else
				{
					//Rotate to right
					viewer.Rotate(0, 1);
				}

				origin.x = mousePosition.x;
			}


			if (Mathf.Abs(mousePosition.y - origin.y) > ySensitivity)
			{
				if (mousePosition.y > origin.y)
				{
					//Rotate down
					viewer.Rotate(-1, 0);
				}
				else
				{
					//Rotate up
					viewer.Rotate(1, 0);
				}

				origin.y = mousePosition.y;
			}
		}
	}

	void SetRectTransformCorners()
	{
		Vector3[] worldCorners = new Vector3[4];
		rt.GetWorldCorners(worldCorners);

		bottomLeftCorner = worldCorners[0];
		topRightCorner = worldCorners[2];
	}
}
