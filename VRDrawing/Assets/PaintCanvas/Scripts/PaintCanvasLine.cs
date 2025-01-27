using UnityEngine;
using System.Linq;

public class PaintCanvasLine : MonoBehaviour
{
    public Texture2D texture; // Paintable texture
    public Texture2D guideTexture; // Guide layer (lines)
    public Vector2 textureSize = new Vector2(2048, 2048);

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
}
