using UnityEngine;

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

        // Initialize the paintable texture
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, false);
        texture.Apply();

        // Set the main texture to the paintable canvas
        _material.SetTexture("_MainTex", texture);

        // Set the guide texture (this should be assigned in the Inspector or dynamically loaded)
        if (guideTexture != null)
        {
            _material.SetTexture("_GuideTex", guideTexture);
        }
    }
}
