using UnityEngine;
using Unity.Barracuda;

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
        // Preprocess images into tensors
        Tensor input1 = PreprocessImage(image1);
        Tensor input2 = PreprocessImage(image2);

        // Concatenate the input tensors
        Tensor input = new Tensor(1, input1.height, input1.width, input1.channels + input2.channels);
        for (int y = 0; y < input1.height; y++)
        {
            for (int x = 0; x < input1.width; x++)
            {
                input[0, y, x, 0] = input1[0, y, x, 0];
                input[0, y, x, 1] = input1[0, y, x, 1];
                input[0, y, x, 2] = input1[0, y, x, 2];
                input[0, y, x, 3] = input2[0, y, x, 0];
                input[0, y, x, 4] = input2[0, y, x, 1];
                input[0, y, x, 5] = input2[0, y, x, 2];
            }
        }

        // Run inference
        worker.Execute(input);
        
        // Get the result tensor (similarity score)
        Tensor output = worker.PeekOutput();

        // Return the similarity score
        float similarity = output[0]; // or output[0,0] depending on output shape
        return similarity;
    }

    // Function to preprocess the image (resize, normalize, etc.)
    private Tensor PreprocessImage(Texture2D image)
    {
        // Convert the texture to a 3D array (H x W x C)
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
