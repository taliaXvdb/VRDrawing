using UnityEngine;
using Unity.Barracuda;
using Unity.VisualScripting;

public class SiameseModelScript : MonoBehaviour
{
    public NNModel modelAsset;  // The ONNX model asset
    private IWorker worker;

    void Start()
    {
        // Load the model
        var model = ModelLoader.Load(modelAsset);

        // Create a worker to run the model
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    void OnDestroy()
    {
        // Clean up the worker
        worker.Dispose();
    }

    public float CompareImages(Texture2D image1, Texture2D image2)
    {
        // Resize images to match the model input size
        image1 = ResizeTexture(image1, 224, 224); // Example target size
        image2 = ResizeTexture(image2, 224, 224);

        // Preprocess images into tensors
        Tensor input1 = PreprocessImage(image1);
        Tensor input2 = PreprocessImage(image2);

        // Set the input for the model (the names must match the ONNX model input names)
        worker.SetInput("inputs", input1);  // Image 1 input
        worker.SetInput("inputs_1", input2); // Image 2 input

        // Execute the model
        worker.Execute();

        // Get the output (distance)
        Tensor output = worker.PeekOutput("output_0");

        // Return the similarity score (this is the Euclidean distance)
        return output[0];
    }

    // Resize Texture2D to the specified target width and height
    private Texture2D ResizeTexture(Texture2D original, int targetWidth, int targetHeight)
    {
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight);
        float incX = 1.0f / targetWidth;
        float incY = 1.0f / targetHeight;

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float u = x * incX;
                float v = y * incY;
                resizedTexture.SetPixel(x, y, original.GetPixelBilinear(u, v));
            }
        }

        resizedTexture.Apply();
        return resizedTexture;
    }

    // Preprocess the image into a tensor
    private Tensor PreprocessImage(Texture2D image)
    {
        var texture = image.GetPixels32();
        Tensor tensor = new Tensor(1, image.height, image.width, 3);

        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                var color = texture[x + y * image.width];
                tensor[0, y, x, 0] = color.r / 255f; // Red
                tensor[0, y, x, 1] = color.g / 255f; // Green
                tensor[0, y, x, 2] = color.b / 255f; // Blue
            }
        }

        return tensor;
    }
}
