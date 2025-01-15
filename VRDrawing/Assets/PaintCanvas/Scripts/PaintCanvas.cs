using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        texture.filterMode = FilterMode.Bilinear; // Smooth blending
        texture.wrapMode = TextureWrapMode.Clamp; // Prevents wrapping at the edges
        r.material.mainTexture = texture;
    }
}
