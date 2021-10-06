using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Viewer : MonoBehaviour
{
    public Image image;
    public RectTransform rt;

    public string fileNameFormat = "V%VH%H";

    [SerializeField]
    public Sprite[] sprites;

    public int vMin;
    public int vMax;
    public int hMin;
    public int hMax;

    public int H;
    public int V;

    string folderPath;
    string fileExtension; // .png, .jpg, .jpeg, .tiff

    FileInfo[] files;
    string[] fileNames;
    int filesCount;

    Regex hRg;
    Regex vRg;

    public bool hTurnAround;
    public bool vTurnAround;

    private void Start()
    {
        OpenFileBrowser();
        GetFiles();
        GetFileNames();
        SetRegexExps();
        SetBounds();
        SetTurnArounds();
        SortFileNames();
        GetImages();
    }

    public void OpenFileBrowser()
    {
        string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);

        // Cancelled
        if (folderPaths.Length == 0) return;

        folderPath = folderPaths[0];
    }

    public void GetFiles()
    {
        // The folder doesn't exist
        if (folderPath == null)
        {
            Debug.Log("The folder doesn't exist!");
            return;
        }

        DirectoryInfo dInfo = new DirectoryInfo(folderPath);

        files = dInfo.GetFilesByExtensions(".png", ".jpg", ".jpeg", ".tiff");
        filesCount = files.Length;

        if (filesCount == 0) return;

        fileExtension = files[0].Extension;
    }

    public void SortFileNames()
    {
        for (int v = vMin; v <= vMax; v++)
        {
            for (int h = hMin; h <= hMax; h++)
            {
                string hValue = h.ToString();
                if (!hTurnAround && h > 0)
                {
                    hValue = "+" + hValue;
                }

                string vValue = v.ToString();
                if (!vTurnAround && v > 0)
                {
                    vValue = "+" + vValue;
                }

                string fileName = fileNameFormat.Replace("%H", hValue);
                fileName = fileName.Replace("%V", vValue);

                fileNames[GetIndex(h, v)] = fileName;

                Debug.Log(fileName);
            }
        }
    }

    public void SetTurnArounds()
    {
        Regex signedRg = new Regex("^[+-]");

        for (int i = 0; i < filesCount; i++)
        {
            if (!hTurnAround && !vTurnAround) return;
            if (hTurnAround)
            {
                string hValue = hRg.Match(fileNames[i]).Value;
                hTurnAround = !signedRg.IsMatch(hValue);
            }
            if (vTurnAround)
            {
                string vValue = vRg.Match(fileNames[i]).Value;
                vTurnAround = !signedRg.IsMatch(vValue);
            }
        }
    }

    public void SetBounds()
    {
        int[] hValues = new int[filesCount];
        int[] vValues = new int[filesCount];

        for (int i = 0; i < fileNames.Length; i++)
        {
            hValues[i] = GetH(fileNames[i]);
            vValues[i] = GetV(fileNames[i]);
        }

        hMin = hValues.Min();
        hMax = hValues.Max();
        vMin = hValues.Min();
        vMax = vValues.Max();
    }

    public void SetRegexExps()
    {
        string numberRg = @"[-\+]?\d+";
        Regex hFirstRg = new Regex("%H.+%V");
        Regex separatorRg = new Regex("(?<=%.).+(?=%.)");

        string separator = separatorRg.Match(fileNameFormat).Value;

        if (hFirstRg.IsMatch(fileNameFormat))
        {
            hRg = new Regex(numberRg + "(?=" + separator + ")");
            vRg = new Regex("(?<=" + separator + ")" + numberRg);
        }
        else
        {
            vRg = new Regex(numberRg + "(?=" + separator + ")");
            hRg = new Regex("(?<=" + separator + ")" + numberRg);
        }
    }

    public void GetFileNames()
    {
        fileNames = new string[filesCount];
        for (int i = 0; i < filesCount; i++)
        {
            fileNames[i] = Path.GetFileNameWithoutExtension(files[i].Name);
        }
    }

    public void GetImages()
    {
        //Sort file names
        string[] filePaths = new string[filesCount];

        //Create sorted file paths
        for (int i = 0; i < filesCount; i++)
        {
            filePaths[i] = folderPath + '\\' + fileNames[i] + fileExtension;
        }

        //Get textures
        Texture2D[] textures = new Texture2D[filesCount];

        for (int i = 0; i < filesCount; i++)
        {
            textures[i] = LoadTexture(filePaths[i]);
        }

        //Create sprites
        sprites = new Sprite[filesCount];

        for (int i = 0; i < filesCount; i++)
        {
            sprites[i] = Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height), new Vector2(0.5f, 0.5f), textures[i].width);
        }

        //Show default view
        H = 0;
        V = 0;

        image.sprite = sprites[GetIndex(H, V)];
        image.gameObject.SetActive(true);

    }

    public void Rotate(int hAmount, int vAmount)
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
            else if (V < vMin)
            {
                V = vMax;
            }
        }
        else
        {
            V = Mathf.Clamp(V, vMin, vMax);
        }

        image.sprite = sprites[GetIndex(H, V)];
    }


    public int GetH(string fileName)
    {
        return int.Parse(hRg.Match(fileName).Value);
    }

    public int GetV(string fileName)
    {
        return int.Parse(vRg.Match(fileName).Value);
    }

    int GetIndex(int h, int v)
    {
        return (h - hMin) + (v - vMin) * (hMax - hMin + 1);
    }

    public static Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        return tex;
    }
}
