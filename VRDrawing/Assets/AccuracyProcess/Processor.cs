using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Processor : MonoBehaviour
{
    Texture2D image2;
    public float similarity;
    public void ProcessImages(Texture2D originalImage)
    {
        Texture2D image1 = originalImage;
        string filePath = Application.dataPath + "/AccuracyProcess/SavedDrawings/MyDrawing.png";

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            image2 = new Texture2D(2, 2); // Placeholder dimensions
            image2.LoadImage(fileData);
            image2 = MakeTextureReadable(image2); // Ensure it's readable
        }
        else
        {
            Debug.LogError("File not found at path: " + filePath);
            return; // Exit early to avoid further errors
        }

        // Ensure SiameseModelScript is found
        SiameseModelScript siameseModel = FindObjectOfType<SiameseModelScript>();
        if (siameseModel == null)
        {
            Debug.LogError("SiameseModelScript not found in the scene.");
            return;
        }

        // Compare images and log similarity
        similarity = siameseModel.CompareImages(image1, image2);
        Debug.Log("Similarity Score: " + similarity);
    }


    Texture2D MakeTextureReadable(Texture2D source)
    {
        // Create a new Texture2D with the same dimensions and format
        Texture2D readableTexture = new Texture2D(source.width, source.height, source.format, false);

        // Copy pixel data
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTexture;
    }
}
