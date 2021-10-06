using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 origin;

    public bool isOrbiting;

    public float xSensitivity = 50;
    public float ySensitivity = 50;
    public float wheelSensitivity = 50;

    public float minSize = 256;
    public float maxSize = 4096;
    public float stepSize = 256;
    Vector2 sizeStep;

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
        Initialize();
    }

    void Initialize()
    {
        float width = image.sprite.texture.width;
        float height = image.sprite.texture.height;

        float sizeFactor = height / width;

        if (sizeFactor > 1)
        {
            sizeStep = new Vector2(stepSize, stepSize / sizeFactor);
        }
        else
        {
            sizeStep = new Vector2(stepSize / sizeFactor, stepSize);
        }

        if (sizeFactor > 0)
        {
            if (width > 1920)
            {
                height *= 1920 / width;
                width = 1920;
            }
        }
        else
        {
            if (height > 1080)
            {
                width *= 1080 / height;
                height = 1080;
            }
        }

        rt.sizeDelta = new Vector2(width, height);
    }

    void Update()
    {
        // Zoom Out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            rt.sizeDelta -= sizeStep;
        }

        // Zoom In
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            rt.sizeDelta += sizeStep;
        }

        // Start Orbiting
        if (Input.GetMouseButtonDown(0))
        {
            origin = Input.mousePosition;
            isOrbiting = true;
        }

        // Orbiting
        if (isOrbiting)
        {
            Orbit();
        }

        // Finish Orbiting
        if (Input.GetMouseButtonUp(0))
            isOrbiting = false;
    }

    void Orbit()
    {
        mousePosition = Input.mousePosition;

        if (Mathf.Abs(mousePosition.x - origin.x) > xSensitivity)
        {
            if (mousePosition.x > origin.x)
            {
                //Rotate to left
                viewer.Rotate(-1, 0);
            }
            else
            {
                //Rotate to right
                viewer.Rotate(1, 0);
            }

            origin.x = mousePosition.x;
        }


        if (Mathf.Abs(mousePosition.y - origin.y) > ySensitivity)
        {
            if (mousePosition.y > origin.y)
            {
                //Rotate down
                viewer.Rotate(0, -1);
            }
            else
            {
                //Rotate up
                viewer.Rotate(0, 1);
            }

            origin.y = mousePosition.y;
        }
    }
}
