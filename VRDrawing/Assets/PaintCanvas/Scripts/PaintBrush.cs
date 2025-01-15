using System.Collections;
using System.Linq;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    [SerializeField] private Transform _brushTip;
    [SerializeField] private float _blendFactor = 0.5f;
    [SerializeField] private float _pressureFactor = 1.0f;
    [SerializeField] private int _brushSize = 10;
    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;

    void Start()
    {
        _renderer = _brushTip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _brushSize * _brushSize).ToArray();
        _tipHeight = _brushTip.localScale.y;
    }

    void Update()
    {
        Paint();
    }

    private void Paint()
    {
        if (Physics.Raycast(_brushTip.position, transform.up, out _touch, _tipHeight))
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

                // Dynamic brush size based on speed
                if (_touchedLastFrame)
                {
                    float speed = (new Vector2(x, y) - _lastTouchPos).magnitude;
                    _brushSize = Mathf.Clamp((int)(10 + speed * 5), 5, 20);
                    Debug.Log($"Brush Size: {_brushSize}");

                    // Update _colors array for new brush size
                    _colors = Enumerable.Repeat(_renderer.material.color, _brushSize * _brushSize).ToArray();
                }

                if (_touchedLastFrame)
                {
                    ApplyBrushStroke(x, y);

                    for (float f = 0.01f; f < 1.0f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        ApplyBrushStroke(lerpX, lerpY);
                    }

                    _paintCanvas.texture.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _paintCanvas = null;
        _touchedLastFrame = false;
    }

    private void ApplyBrushStroke(int x, int y)
    {
        Color[] existingColors = _paintCanvas.texture.GetPixels(x, y, _brushSize, _brushSize);

        for (int i = 0; i < _colors.Length; i++)
        {
            int pixelX = i % _brushSize;
            int pixelY = i / _brushSize;
            float distance = Vector2.Distance(new Vector2(pixelX, pixelY), new Vector2(_brushSize / 2, _brushSize / 2));

            float strength = Mathf.Clamp01(1 - (distance / (_brushSize / 2)));
            Color currentColor = existingColors[i];
            Color brushColor = _renderer.material.color;
            brushColor.a *= _pressureFactor * strength;
            existingColors[i] = Color.Lerp(currentColor, brushColor, _blendFactor * strength);
        }

        _paintCanvas.texture.SetPixels(x, y, _brushSize, _brushSize, existingColors);
    }
}
