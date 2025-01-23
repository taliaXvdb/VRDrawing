using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Marker : MonoBehaviour
{
    [SerializeField] private Transform _markerTip;
    [SerializeField] private int _tipSize = 15;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private ToolController _toolController;
    private float smoothingFactor = 0.8f; // Adjust to control smoothness
    private float jitterThreshold = 0.08f; // Minimum movement to consider
    private InputAction _triggerAction;
    private InputAction _cancelAction;
    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector3 _smoothedPosition;
    private PaintCanvasLine _paintCanvasLine;
    private Vector2 _touchpos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    public bool _drawing;
    private bool lockZPosition = false;
    private bool hasCollided = false; // Flag to track if collision has already happened
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = _markerTip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _tipSize * _tipSize).ToArray();
        _tipHeight = _markerTip.localScale.y;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Controller");
        _triggerAction = actionMap.FindAction("Trigger");
        _cancelAction = actionMap.FindAction("Secondary Button");
        _triggerAction.Enable();
        _cancelAction.Enable();
    }

    void OnDisable()
    {
        _triggerAction.Disable();
        _cancelAction.Disable();
    }

    void Update()
    {
        if (_cancelAction.triggered)
        {
            ResetTool();
            return;
        }

        // Lock Z position in Update() to ensure marker stays in place
        if (lockZPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.9385f);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (_drawing)
        {
            Vector3 rawPosition = transform.position;
            _smoothedPosition = Vector3.Lerp(_smoothedPosition, rawPosition, smoothingFactor);

            // Check for jitter
            if (Vector3.Distance(rawPosition, _smoothedPosition) < jitterThreshold)
            {
                _smoothedPosition = rawPosition; // Ignore jitter by snapping to the actual position
            }

            // Update transform to use the smoothed position
            transform.position = _smoothedPosition;
        }


        // Check if the trigger button is pressed
        if (_triggerAction.ReadValue<float>() > 0 && hasCollided)
        {
            if (!_drawing)
            {
                // Reset states when starting a new drawing
                _touchedLastFrame = false;
            }

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            _drawing = true;
            Paint();
        }
        else if (_triggerAction.ReadValue<float>() == 0 && hasCollided)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            GetComponent<Rigidbody>().useGravity = false;
            _drawing = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PaintCanvas") && !hasCollided)
        {
            // Lock Z position without Rigidbody physics
            lockZPosition = true;

            hasCollided = true; // Set the flag to true to prevent further detection
            return;
        }
    }


    private void Paint()
    {
        if (Physics.Raycast(_markerTip.position, transform.forward, out _touch, _tipHeight))
        {
            if (_touch.transform.CompareTag("PaintCanvas"))
            {
                if (_paintCanvas == null)
                {
                    _paintCanvas = _touch.transform.GetComponent<PaintCanvas>();
                }

                _touchpos = new Vector2(_touch.textureCoord.x * _paintCanvas.texture.width, _touch.textureCoord.y * _paintCanvas.texture.height);

                var x = (int)(_touchpos.x - _tipSize / 2);
                var y = (int)(_touchpos.y - _tipSize / 2);

                if (y < 0 || y >= _paintCanvas.texture.height || x < 0 || x >= _paintCanvas.texture.width)
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
        }
        _paintCanvas = null;
        _paintCanvasLine = null;
        _touchedLastFrame = false;
    }

    public void SetColor(Material material)
    {
        _renderer.material = material;
        _colors = Enumerable.Repeat(material.color, _tipSize * _tipSize).ToArray();
    }

    private void ResetTool()
    {
        transform.position = _originalPosition;
        transform.rotation = _originalRotation;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        _drawing = false;
        lockZPosition = false;
        hasCollided = false;
        _toolController.ResetCurrentTool();
    }
}
