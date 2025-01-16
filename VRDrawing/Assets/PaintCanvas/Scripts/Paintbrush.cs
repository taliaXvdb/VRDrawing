using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Paintbrush : MonoBehaviour
{
    [SerializeField] private Transform _brushTip;
    [SerializeField] private int _brushSize = 30; // Larger size for brush strokes
    [SerializeField] private float _opacity = 0.5f; // Semi-transparency for blending
    [SerializeField] private Texture2D _brushTexture; // Optional texture for brush shape

    private Renderer _renderer;
    private float _brushHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;

    void Start()
    {
        _renderer = _brushTip.GetComponent<Renderer>();
        _brushHeight = _brushTip.localScale.y;
    }

    void Update()
    {
        Paint();
    }

    private void Paint()
    {
        if (Physics.Raycast(_brushTip.position, transform.up, out _touch, _brushHeight))
        {
            if (_touch.transform.CompareTag("PaintCanvas"))
            {
                if (_paintCanvas == null)
                {
                    _paintCanvas = _touch.transform.GetComponent<PaintCanvas>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _paintCanvas.textureSize.x - _brushSize / 2);
                var y = (int)(_touchPos.y * _paintCanvas.textureSize.y - _brushSize / 2);

                if (y < 0 || y >= _paintCanvas.textureSize.y || x < 0 || x >= _paintCanvas.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    for (float f = 0.01f; f < 1.0f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);

                        ApplyBrush(lerpX, lerpY);
                    }
                }

                ApplyBrush(x, y);

                _paintCanvas.texture.Apply();

                _lastTouchPos = new Vector2(x, y);
                _touchedLastFrame = true;
                return;
            }
        }

        _paintCanvas = null;
        _touchedLastFrame = false;
    }

    private void ApplyBrush(int x, int y)
    {
        Color[] baseColors = _paintCanvas.texture.GetPixels(x, y, _brushSize, _brushSize);
        Color[] brushColors = GenerateBrushColors();

        for (int i = 0; i < baseColors.Length; i++)
        {
            baseColors[i] = Color.Lerp(baseColors[i], brushColors[i], brushColors[i].a);
        }

        _paintCanvas.texture.SetPixels(x, y, _brushSize, _brushSize, baseColors);
    }

    private Color[] GenerateBrushColors()
    {
        Color brushColor = _renderer.material.color;
        Color[] brushColors = new Color[_brushSize * _brushSize];

        for (int i = 0; i < brushColors.Length; i++)
        {
            float alpha = _opacity * (Random.value + 0.5f); // Randomized opacity for brush texture
            brushColors[i] = new Color(brushColor.r, brushColor.g, brushColor.b, alpha);
        }

        if (_brushTexture != null)
        {
            ApplyBrushTexture(brushColors);
        }

        return brushColors;
    }

    private void ApplyBrushTexture(Color[] brushColors)
    {
        for (int y = 0; y < _brushSize; y++)
        {
            for (int x = 0; x < _brushSize; x++)
            {
                float u = (float)x / _brushSize;
                float v = (float)y / _brushSize;
                Color textureColor = _brushTexture.GetPixelBilinear(u, v);
                int index = y * _brushSize + x;

                brushColors[index] *= textureColor.a; // Modulate alpha with texture
            }
        }
    }
}
