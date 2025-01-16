using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pencil : MonoBehaviour
{
    [SerializeField] private Transform _pencilTip;
    [SerializeField] private int _tipSize = 15;
    [SerializeField] private float _opacity = 0.2f; // Adjust for semi-transparent strokes
    [SerializeField] private float _grainIntensity = 0.3f; // Intensity of the grain effect

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector2 _touchpos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;

    void Start()
    {
        _renderer = _pencilTip.GetComponent<Renderer>();
        _colors = GenerateGrainyColors();
        _tipHeight = _pencilTip.localScale.y;
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Physics.Raycast(_pencilTip.position, transform.up, out _touch, _tipHeight))
        {
            if (_touch.transform.CompareTag("PaintCanvas"))
            {
                if (_paintCanvas == null)
                {
                    _paintCanvas = _touch.transform.GetComponent<PaintCanvas>();
                }

                _touchpos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchpos.x * _paintCanvas.textureSize.x - _tipSize / 2);
                var y = (int)(_touchpos.y * _paintCanvas.textureSize.y - _tipSize / 2);

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

                        Color[] existingColors = _paintCanvas.texture.GetPixels(lerpX, lerpY, _tipSize, _tipSize);
                        Color[] blendedColors = BlendColors(existingColors, GenerateGrainyColors());
                        _paintCanvas.texture.SetPixels(lerpX, lerpY, _tipSize, _tipSize, blendedColors);
                    }

                    _paintCanvas.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _touchedLastFrame = true;
                return;
            }
        }

        _paintCanvas = null;
        _touchedLastFrame = false;
    }

    private Color[] GenerateGrainyColors()
    {
        Color baseColor = new Color(_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, _opacity);
        Color[] grainyColors = new Color[_tipSize * _tipSize];

        for (int i = 0; i < grainyColors.Length; i++)
        {
            float grainOffset = Random.Range(-_grainIntensity, _grainIntensity);
            grainyColors[i] = new Color(
                Mathf.Clamp01(baseColor.r + grainOffset),
                Mathf.Clamp01(baseColor.g + grainOffset),
                Mathf.Clamp01(baseColor.b + grainOffset),
                baseColor.a
            );
        }

        return grainyColors;
    }

    private Color[] BlendColors(Color[] baseColors, Color[] newColors)
    {
        Color[] result = new Color[baseColors.Length];
        for (int i = 0; i < baseColors.Length; i++)
        {
            result[i] = Color.Lerp(baseColors[i], newColors[i], newColors[i].a);
        }
        return result;
    }
}
