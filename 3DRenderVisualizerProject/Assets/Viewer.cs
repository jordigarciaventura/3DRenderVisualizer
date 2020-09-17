using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class Viewer : MonoBehaviour
{
	public Image image;
	public RectTransform rt;

	[SerializeField]
	public Sprite[] sprites;

	public string folderPath;

	public int vMin;
	public int vMax;
	public int hMin;
	public int hMax;

	int hCount;
	int vCount;

	public int H;
	public int V;

	public bool hTurnAround;
	public bool vTurnAround;

	private void Start()
	{
		
	}

	public void GetImages()
	{
		string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);

		//Operation cancelled
		if (folderPaths.Length == 0) return;
		
		if(folderPaths[0] == null)
		{
			Debug.Log("The folder doesn't exist!");
			return;
		}

		//Get file paths
		folderPath = folderPaths[0];

		DirectoryInfo dInfo = new DirectoryInfo(folderPaths[0]);

		FileInfo[] filePaths = dInfo.GetFilesByExtensions(".png", ".jpg", ".jpeg", ".tiff");

		if (filePaths == null)
		{
			Debug.Log("The folder doesn't contain a correct format!");
			return;
		}

		int filesCount = filePaths.Length;

		//Get file names
		string[] fileNames = new string[filesCount];

		for (int i = 0; i < filesCount; i++)
		{
			fileNames[i] = Path.GetFileNameWithoutExtension(filePaths[i].Name);
		}

		if (!fileNames.Contains("V0H0"))
		{
			Debug.Log("Change the files names!");
			return;
		}

		//Sort file names
		string[] sortedFileNames = SortElements(fileNames);

		//Create sorted file paths
		for (int i = 0; i < filesCount; i++)
		{
			sortedFileNames[i] = string.Concat(folderPath, '\\', sortedFileNames[i]);
		}

		//Get textures
		Texture2D[] textures = new Texture2D[filesCount];

		for (int i = 0; i < filePaths.Length; i++)
		{
			textures[i] = LoadPNG(filePaths[i].FullName);
		}

		//Create sprites
		sprites = new Sprite[filesCount];

		for (int i = 0; i < filesCount; i++)
		{
			sprites[i] = Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f), textures[i].width);
		}

		//Show default view
		V = 0;
		H = 0;

		image.sprite = sprites[GetImageIndex(V, H)];
		image.gameObject.SetActive(true);
	}

	public void Rotate(int vAmount, int hAmount)
	{
		H += hAmount;
		V += vAmount;

		if (hTurnAround)
		{
			if (H > hMax)
			{
				H = hMin;
			}
			else if (H < hMin)
			{
				H = hMax;
			}
		}
		else
		{
			H = Mathf.Clamp(H, hMin, hMax);
		}

		if (vTurnAround)
		{
			if (V > vMax)
			{
				V = vMin;
			}
			else if(V < vMin)
			{
				V = vMax;
			}
		}
		else
		{
			V = Mathf.Clamp(V, vMin, vMax);
		}

		image.sprite = sprites[GetImageIndex(V, H)];
	}

	int GetImageIndex(int vValue, int hValue)
	{
		int v = (int)Mathf.Lerp(0, vCount - 1, Mathf.InverseLerp(vMin, vMax, vValue));
		int h = (int)Mathf.Lerp(0, hCount - 1, Mathf.InverseLerp(hMin, hMax, hValue));

		int index = h + v * hCount;

		return index;
	}

	public static Texture2D LoadPNG(string filePath)
	{
		byte[] fileData = File.ReadAllBytes(filePath);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(fileData);

		return tex;
	}

	void Test()
	{
		string[] names = new string[9];

		names[0] = "V-1H-1";
		names[1] = "V0H1";
		names[2] = "V-1H0";
		names[3] = "V0H-1";
		names[4] = "V-1H1";
		names[5] = "V0H0";
		names[6] = "V1H-1";
		names[7] = "V1H0";
		names[8] = "V1H1";

		string[] sortedElements = SortElements(names);

		for (int i = 0; i < sortedElements.Length; i++)
		{
			Debug.Log(sortedElements[i]);
		}
	}

	/*
	
	SORTING METHOD
	 
	V/H		-1		0		1		<-->	V/H		0		1		2
	-1		V-1H-1	V-1H0	V-1H1	<-->	0		0		1		2
	0		V0H-1	V0H0	V0H1	<-->	1		3		4		5
	1		V1H-1	V1H0	V1H1	<-->	2		6		7		8
	
	*/

	string[] SortElements(string[] names)
	{
		int count = names.Length;

		//Get V and H values
		List<int> vValues = new List<int>();
		List<int> hValues = new List<int>();
  
		for (int i = 0; i < count; i++)
		{
			int[] values = GetValues(names[i]);

			if (!vValues.Contains(values[0]))
				vValues.Add(values[0]);
			if (!hValues.Contains(values[1]))
				hValues.Add(values[1]);
		}

		vCount = vValues.Count;
		hCount = hValues.Count;

		//Get V and H bounds
		vMin = vValues.Min();
		vMax = vValues.Max();
		hMin = hValues.Min();
		hMax = hValues.Max();

		//Sort names
		string[] sortedNames = new string[count];

		for (int v = 0; v < vValues.Count; v++)
		{
			for (int h = 0; h < hValues.Count; h++)
			{
				int vValue = (int)Mathf.Lerp(vMin, vMax, v / (vCount - 1f));
				int hValue = (int)Mathf.Lerp(hMin, hMax, h / (hCount - 1f));

				string sortedName = string.Concat("V", vValue.ToString(), "H", hValue.ToString());

				sortedNames[h + v * hCount] = sortedName;
			}
		}

		return sortedNames;
	}

	//Return V and H values from name ("V-1H0" --> {-1, 0})
	int[] GetValues(string name)
	{
		char[] separator = { 'V', 'H' };

		string[] str_values = name.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

		int[] int_values = { int.Parse(str_values[0]), int.Parse(str_values[1]) };

		return int_values;
	}
}
