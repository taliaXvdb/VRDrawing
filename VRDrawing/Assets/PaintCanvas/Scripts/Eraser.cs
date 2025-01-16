using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField] private Transform _eraserTip;
    [SerializeField] private int _tipSize = 10; // Size of the eraser tip
    [SerializeField] private Color _backgroundColor = Color.white; // Default canvas color

    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;

    void Start()
    {
        _tipHeight = _eraserTip.localScale.y;
    }

    void Update()
    {
        Erase();
    }

    private void Erase()
    {
        if (Physics.Raycast(_eraserTip.position, transform.up, out _touch, _tipHeight))
        {
            if (_touch.transform.CompareTag("PaintCanvas"))
            {
                if (_paintCanvas == null)
                {
                    _paintCanvas = _touch.transform.GetComponent<PaintCanvas>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _paintCanvas.textureSize.x - _tipSize / 2);
                var y = (int)(_touchPos.y * _paintCanvas.textureSize.y - _tipSize / 2);

                if (y < 0 || y >= _paintCanvas.textureSize.y || x < 0 || x >= _paintCanvas.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    ApplyEraser(x, y);

                    for (float f = 0.01f; f < 1.0f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        ApplyEraser(lerpX, lerpY);
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

    private void ApplyEraser(int x, int y)
    {
        Color[] eraseColors = Enumerable.Repeat(_backgroundColor, _tipSize * _tipSize).ToArray();
        _paintCanvas.texture.SetPixels(x, y, _tipSize, _tipSize, eraseColors);
    }
}
