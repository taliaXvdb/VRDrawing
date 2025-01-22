using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paintbrush : MonoBehaviour
{
    [SerializeField] private Transform _brushTip;
    [SerializeField] private int _brushSize = 30; // Larger size for brush strokes
    [SerializeField] private InputActionAsset inputActions;
    private float _opacity = 0.5f; // Semi-transparency for blending
    private Texture2D _brushTexture; // Optional texture for brush shape
    private float smoothingFactor = 0.8f; // Adjust to control smoothness
    private float jitterThreshold = 0.08f; // Minimum movement to consider
    private InputAction _triggerAction;
    private Renderer _renderer;
    private float _brushHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector3 _smoothedPosition;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private bool _drawing;
    private bool lockZPosition = false;
    private bool hasCollided = false; // Flag to track if collision has already happened
    void Start()
    {
        _renderer = _brushTip.GetComponent<Renderer>();
        _brushHeight = _brushTip.localScale.y;
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
    void Update()
    {
        // Lock Z position in Update() to ensure marker stays in place
        if (lockZPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -5.0445f);
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
        if (Physics.Raycast(_brushTip.position, -transform.up, out _touch, _brushHeight))
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

    public void SetColor(Material material)
    {
        _renderer.material = material;
    }
}
