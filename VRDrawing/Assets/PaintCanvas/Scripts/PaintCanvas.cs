using System.Collections;
using UnityEngine;

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
}
