using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Marker : MonoBehaviour
{
    [SerializeField] private Transform _markerTip;
    [SerializeField] private int _tipSize = 15;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction _triggerAction;
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
    private bool _drawing;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = _markerTip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _tipSize * _tipSize).ToArray();
        _tipHeight = _markerTip.localScale.y;
    }

    void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Controller");
        _triggerAction = actionMap.FindAction("Trigger");
        _triggerAction.Enable();
    }

    void OnDisable()
    {
        _triggerAction.Disable();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (_triggerAction != null)
    //     {
    //         Debug.Log("Trigger value: " + _triggerAction.ReadValue<float>());
    //     }
    //     else
    //     {
    //         Debug.LogError("Trigger action is null");
    //     }
    // }
    void Update()
    {
        // Check if the trigger button is pressed
        if (_triggerAction.ReadValue<float>() > 0)
        {
            if (!_drawing)
            {
                // Reset states when starting a new drawing
                _touchedLastFrame = false;
            }

            _drawing = true;
            Paint();
        }
        else
        {
            _drawing = false;
        }
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

                if (_touchedLastFrame)
                {
                    _paintCanvas.texture.SetPixels(x, y, _tipSize, _tipSize, _colors);

                    for (float f = 0.01f; f < 1.0f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _paintCanvas.texture.SetPixels(lerpX, lerpY, _tipSize, _tipSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;

                    _paintCanvas.texture.Apply();

                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
            else if (_touch.transform.CompareTag("PaintCanvasLine"))
            {
                if (_paintCanvasLine == null)
                {
                    _paintCanvasLine = _touch.transform.GetComponent<PaintCanvasLine>();
                }

                _touchpos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchpos.x * _paintCanvasLine.textureSize.x - _tipSize / 2);
                var y = (int)(_touchpos.y * _paintCanvasLine.textureSize.y - _tipSize / 2);

                if (y < 0 || y >= _paintCanvasLine.textureSize.y || x < 0 || x >= _paintCanvasLine.textureSize.x)
                {
                    return;
                }

                if (_touchedLastFrame)
                {
                    _paintCanvasLine.texture.SetPixels(x, y, _tipSize, _tipSize, _colors);

                    for (float f = 0.01f; f < 1.0f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _paintCanvasLine.texture.SetPixels(lerpX, lerpY, _tipSize, _tipSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;

                    _paintCanvasLine.texture.Apply();

                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _paintCanvas = null;
        _paintCanvasLine = null;
        _touchedLastFrame = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PaintCanvas"))
        {
            Debug.Log("Collided with " + other.gameObject.name);
        }
    }
}
