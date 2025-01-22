using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprayPaint : MonoBehaviour
{
    [SerializeField] private Transform _markerTip;
    [SerializeField] private int _tipSize = 25;
    [SerializeField] private InputActionAsset inputActions;
    private float smoothingFactor = 0.8f; // Adjust to control smoothness
    private float jitterThreshold = 0.08f; // Minimum movement to consider
    private InputAction _triggerAction;
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
    private bool _drawing;
    private bool lockZPosition = false;
    private bool hasCollided = false; // Flag to track if collision has already happened
    // Start is called before the first frame update
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
    void Update()
    {
        if (lockZPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.9355f);
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
        if (Physics.Raycast(_markerTip.position, transform.right, out _touch, _tipHeight))
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

    public void SetColor(Material material)
    {
        _renderer.material = material;
        _colors = Enumerable.Repeat(material.color, _tipSize * _tipSize).ToArray();
    }
}

