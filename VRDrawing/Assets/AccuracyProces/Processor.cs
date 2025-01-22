using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : MonoBehaviour
{
    public void ProcessImages()
    {
        Texture2D image1 = Resources.Load<Texture2D>("OriginalImages/");
        Texture2D image2 = Resources.Load<Texture2D>("CreatedImages/");

        SiameseModelScript siameseModel = FindObjectOfType<SiameseModelScript>();

        float similarity = siameseModel.CompareImages(image1, image2);
        Debug.Log("Similarity Score: " + similarity);

    }
}
