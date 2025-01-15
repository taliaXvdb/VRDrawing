using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SprayPaint : MonoBehaviour
{
    [SerializeField] private Transform _markerTip;
    [SerializeField] private int _tipSize = 25;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private PaintCanvasLine _paintCanvasLine;
    private Vector2 _touchpos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = _markerTip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _tipSize * _tipSize).ToArray();
        _tipHeight = _markerTip.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Paint();
    }

    private void Paint()
    {
        if (Physics.Raycast(_markerTip.position, transform.up, out _touch, _tipHeight))
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

                // Spray effect: scatter random points within a circular area
                int sprayDensity = 2000; // Number of particles per frame
                float sprayRadius = _tipSize * 5; // Radius of the spray
                Color[] sprayColors = _colors;

                for (int i = 0; i < sprayDensity; i++)
                {
                    float angle = Random.Range(0, Mathf.PI * 2);
                    float radius = Random.Range(0, sprayRadius);
                    int offsetX = (int)(radius * Mathf.Cos(angle));
                    int offsetY = (int)(radius * Mathf.Sin(angle));

                    int sprayX = x + offsetX;
                    int sprayY = y + offsetY;

                    if (sprayX >= 0 && sprayX < _paintCanvas.textureSize.x && sprayY >= 0 && sprayY < _paintCanvas.textureSize.y)
                    {
                        float distance = radius / sprayRadius; // Normalize distance
                        Color sprayColor = _renderer.material.color;
                        sprayColor.a *= 1 - distance; // Reduce opacity based on distance

                        _paintCanvas.texture.SetPixel(sprayX, sprayY, sprayColor);
                    }
                }

                _paintCanvas.texture.Apply();

                _touchedLastFrame = true;
                return;
            }
        }

        _paintCanvas = null;
        _touchedLastFrame = false;
    }
}

