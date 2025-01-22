using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pencil : MonoBehaviour
{
    [SerializeField] private Transform _pencilTip;
    [SerializeField] private int _tipSize = 15;
    [SerializeField] private InputActionAsset inputActions;
    private float _opacity = 0.2f; // Adjust for semi-transparent strokes
    private float _grainIntensity = 0.3f; // Intensity of the grain effect
    private float smoothingFactor = 0.8f; // Adjust to control smoothness
    private float jitterThreshold = 0.08f; // Minimum movement to consider
    private InputAction _triggerAction;
    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector3 _smoothedPosition;
    private Vector2 _touchpos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private bool _drawing;
    private bool lockZPosition = false;
    private bool hasCollided = false; // Flag to track if collision has already happened
    // Start is called before the first frame update
    void Start()
    {
        _renderer = _pencilTip.GetComponent<Renderer>();
        _colors = GenerateGrainyColors();
        _tipHeight = _pencilTip.localScale.y;
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
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.8553f);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180, transform.rotation.eulerAngles.z);
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
            Draw();
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
            Debug.Log("Collided with PaintCanvas");
            // Lock Z position without Rigidbody physics
            lockZPosition = true;

            hasCollided = true; // Set the flag to true to prevent further detection
            return;
        }
    }

    private void Draw()
    {
        Debug.Log("Drawing");
        if (Physics.Raycast(_pencilTip.position, transform.forward, out _touch, _tipHeight))
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

    public void SetColor(Material material)
    {
        _renderer.material = material;
    }
}
