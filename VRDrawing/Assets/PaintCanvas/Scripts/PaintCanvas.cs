using System.Collections;
using UnityEngine;
using System.IO;

public class PaintCanvas : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);

    [SerializeField] private Material canvasMaterial; // Optional: assign in Inspector

    void Start()
    {
        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        var r = GetComponent<Renderer>();
        if (r == null)
        {
            Debug.LogError("Renderer component missing on PaintCanvas object.");
            return;
        }

        // Create texture with support for alpha (transparency)
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Fill texture with a default color (white/transparent)
        ClearCanvas(Color.white);

        // Assign the texture to the material
        if (canvasMaterial != null)
        {
            canvasMaterial.mainTexture = texture;
            r.material = canvasMaterial;
        }
        else
        {
            r.material.mainTexture = texture;
        }
    }

    public void ClearCanvas(Color clearColor)
    {
        var clearColors = new Color[(int)(textureSize.x * textureSize.y)];
        for (int i = 0; i < clearColors.Length; i++) clearColors[i] = clearColor;
        texture.SetPixels(clearColors);
        texture.Apply();
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

        // Set up a transparent background
        Color[] exportPixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            // Check if the pixel is painted (not transparent or background color)
            if (pixels[i].a > 0.1f) // Adjust threshold as needed
            {
                exportPixels[i] = pixels[i]; // Keep painted color
            }
            else
            {
                exportPixels[i] = new Color(0, 0, 0, 0); // Transparent
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
