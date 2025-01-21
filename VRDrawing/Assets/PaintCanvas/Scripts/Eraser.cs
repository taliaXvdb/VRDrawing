using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Eraser : MonoBehaviour
{
    [SerializeField] private Transform _eraserTip;
    [SerializeField] private int _tipSize = 10; // Size of the eraser tip
    [SerializeField] private Color _backgroundColor = Color.white; // Default canvas color
    [SerializeField] private InputActionAsset inputActions;
    private InputAction _triggerAction;
    private float _tipHeight;
    private RaycastHit _touch;
    private PaintCanvas _paintCanvas;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private bool _erasing;
    private bool lockZPosition = false;
    private bool hasCollided = false; // Flag to track if collision has already happened

    void Start()
    {
        _tipHeight = _eraserTip.localScale.y;
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
            transform.position = new Vector3(transform.position.x, transform.position.y, -4.9959f);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        // Check if the trigger button is pressed
        if (_triggerAction.ReadValue<float>() > 0 && hasCollided)
        {
            if (!_erasing)
            {
                // Reset states when starting a new drawing
                _touchedLastFrame = false;
            }

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            _erasing = true;
            Erase();
        }
        else if (_triggerAction.ReadValue<float>() == 0 && hasCollided)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            GetComponent<Rigidbody>().useGravity = false;
            _erasing = false;
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

    private void Erase()
    {
        if (Physics.Raycast(_eraserTip.position, transform.right, out _touch, _tipHeight))
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
