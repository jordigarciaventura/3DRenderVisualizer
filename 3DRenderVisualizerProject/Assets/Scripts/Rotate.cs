using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{


	public float xAngle;
	public float yAngle;

    // Update is called once per frame
    void Update()
    {


    }

	public static Vector2 VectorFromAngle(Vector2 dir, float angle)
	{
		angle *= Mathf.Deg2Rad;

		float cosAngle = Mathf.Cos(angle);
		float sinAngle = Mathf.Sin(angle);

		return new Vector2(dir.x * cosAngle - dir.y * sinAngle, dir.x * sinAngle + dir.y * cosAngle);
	}

	public static bool IsInsideRectangle(Vector2 point, Vector2 blCorner, Vector2 trCorner)
	{
		float xMin = blCorner.x;
		float xMax = trCorner.x;
		float yMax = trCorner.y;
		float yMin = blCorner.y;

		if (xMin < point.x && point.x < xMax && yMin < point.y && point.y < yMax)
		{
			return true;
		}

		return false;
	}
}
