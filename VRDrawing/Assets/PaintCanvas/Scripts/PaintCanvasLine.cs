using UnityEngine;
using System.Linq;
using System.IO;

public class PaintCanvasLine : MonoBehaviour
{
    public Texture2D texture; // Paintable texture
    public Texture2D guideTexture; // Guide layer (lines)
    public Vector2 textureSize = new Vector2(1024, 1024);

    private Material _material;

    void Start()
    {
        var r = GetComponent<Renderer>();
        _material = r.material;

        texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, false);

        // Initialize with transparent pixels
        Color[] transparentPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), (int)textureSize.x * (int)textureSize.y).ToArray();
        texture.SetPixels(transparentPixels);
        texture.Apply();

        // Assign the texture to the material
        _material.SetTexture("_MainTex", texture);

        // Assign the guide texture (if available)
        if (guideTexture != null)
        {
            _material.SetTexture("_GuideTex", guideTexture);
        }
    }

    public void ExportPaintedLines(string folderName, string fileName)
    {
        // Define the path to save the PNG in the Assets folder
        string directoryPath = Application.dataPath + "/" + folderName;

        // Ensure the folder exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = directoryPath + "/" + fileName;

        // Create a new texture for exporting
        Texture2D exportTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Get pixels from the main texture
        Color[] pixels = texture.GetPixels();

        // Set up a white background and convert lines to black
        Color[] exportPixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0.1f) // If painted (threshold for transparency)
            {
                exportPixels[i] = new Color(0, 0, 0, 1); // Black for lines
            }
            else
            {
                exportPixels[i] = new Color(1, 1, 1, 1); // White background
            }
        }

        // Apply the filtered pixels to the export texture
        exportTexture.SetPixels(exportPixels);
        exportTexture.Apply();

        // Encode to PNG
        byte[] pngData = exportTexture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(filePath, pngData);
            Debug.Log("Exported painted lines to: " + filePath);
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }

        // Cleanup
        Destroy(exportTexture);
    }

}
