using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
	Vector3 mousePosition;
	Vector3 origin;

	public bool isOrbiting;
	public bool isPanning;

	public float xSensitivity = 50;
	public float ySensitivity = 50;
	public float wheelSensitivity = 50;

	Vector2 bottomLeftCorner;
	Vector2 topRightCorner;

	Vector2 initial_bottomLeftCorner;
	Vector2 initial_topRightCorner;

	Vector3 initialPos;

	public float minSize = 256;
	public float maxSize = 4096;
	public float stepSize = 256;
	Vector2 sizeStep;

	int currentSizeStep;
	int maxSizeSteps;
	int minSizeSteps;

	RectTransform rt;
	Image image;

	public Viewer viewer;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

	private void Start()
	{
		SetRectTransformCorners();
		Initialize();
	}

	void Initialize()
	{
		float width = image.sprite.texture.width;
		float height = image.sprite.texture.height;

		float sizeFactor = height / width;

		sizeStep = new Vector2(stepSize, stepSize / sizeFactor);

		if (sizeFactor > 0)
		{
			if (width > 1920)
			{
				height *= 1920 / width;
				width = 1920;
			}

			maxSizeSteps = (int)((maxSize - width) / stepSize);
			minSizeSteps = -(int)((width - minSize) / stepSize);
		}
		else
		{
			if (height > 1080)
			{
				width *= 1080 / height;
				height = 1080;
			}

			maxSizeSteps = (int)((maxSize - height) / stepSize);
			minSizeSteps = -(int)((height - minSize) / stepSize);
		}

		rt.sizeDelta = new Vector2(width, height);
	}

	void Update()
	{
		// Zoom Out
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (currentSizeStep > minSizeSteps)
			{
				currentSizeStep--;
				rt.sizeDelta -= sizeStep;
			}
		}

		// Zoom In
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (currentSizeStep < maxSizeSteps)
			{
				currentSizeStep++;
				rt.sizeDelta += sizeStep;
			}
		} 

		// Start Panning
		if (Input.GetMouseButtonDown(2))
		{	
			origin = Input.mousePosition;
			initialPos = rt.anchoredPosition;
			isPanning = true;
			isOrbiting = false;
		}

		// Start Orbiting
		if (Input.GetMouseButtonDown(0))
		{
			origin = Input.mousePosition;
			isOrbiting = true;
			isPanning = false;
		}

		// Finish Panning
		if (Input.GetMouseButtonUp(2))
			isPanning = false;

		// Finish Orbiting
		if (Input.GetMouseButtonUp(0))
			isOrbiting = false;

		// Panning
		if (isPanning)
		{
			mousePosition = Input.mousePosition;

			rt.anchoredPosition = initialPos + mousePosition - origin;
		}

		// Orbiting
		if (isOrbiting)
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

	void SetPivotToCursor()
	{
		SetRectTransformCorners();
		//Change pivot
		float xPivot = Mathf.Lerp(0, 1, Mathf.InverseLerp(bottomLeftCorner.x, topRightCorner.x, Input.mousePosition.x));
		float yPivot = Mathf.Lerp(0, 1, Mathf.InverseLerp(bottomLeftCorner.y, topRightCorner.y, Input.mousePosition.y));
		rt.pivot = new Vector2(xPivot, yPivot);

		//Counter anchored position
		float width = rt.sizeDelta.x / 2;
		float height = rt.sizeDelta.y / 2;
		float xPos = Mathf.Lerp(-width, width, xPivot);
		float yPos = Mathf.Lerp(-height, height, yPivot);
		rt.anchoredPosition = new Vector2(xPos, yPos);
	}
}
