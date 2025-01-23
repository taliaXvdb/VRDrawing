using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolController : MonoBehaviour
{
    [SerializeField] private Transform controller; // Assign your VR controller
    [SerializeField] private string targetTag = "Tool"; // Set your desired tag in the Inspector
    [SerializeField] private Canvas _colorpickerCanvas;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Material _matRed;
    [SerializeField] private Material _matOrange;
    [SerializeField] private Material _matYellow;
    [SerializeField] private Material _matGreen;
    [SerializeField] private Material _matCyan;
    [SerializeField] private Material _matBlue;
    [SerializeField] private Material _matMagenta;
    [SerializeField] private Material _matBlack;
    private InputAction _gripAction;
    private GameObject lastHoveredTool;
    private GameObject lastSelectedTool;
    private bool _toolSelected = false;
    void Start()
    {

    }
    void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Controller");
        _gripAction = actionMap.FindAction("Grip");
        _gripAction.Enable();
    }

    void OnDisable()
    {
        _gripAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_toolSelected)
        {
            SelectingTools();
        }
    }

    private void SelectingTools()
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject hoveredTool = hit.collider.gameObject;

            if (hoveredTool.CompareTag(targetTag))
            {
                // If it's a new object
                if (hoveredTool != lastHoveredTool)
                {
                    lastHoveredTool = hoveredTool;
                    Debug.Log($"Hovering over: {hoveredTool.name}");
                }
                // check for grip action
                if (_gripAction.ReadValue<float>() > 0)
                {
                    if (lastHoveredTool != null)
                    {
                        lastSelectedTool = lastHoveredTool;
                        Debug.Log($"Selected: {lastSelectedTool.name}");
                        if (lastSelectedTool.name != "Eraser")
                        {
                            _colorpickerCanvas.gameObject.SetActive(true);
                        }
                        _toolSelected = true;
                    }
                }
            }
            else
            {
                // Reset when the object doesn't have the correct tag
                if (lastHoveredTool != null)
                {
                    lastHoveredTool = null;
                }
            }
        }
    }

    public void ResetCurrentTool()
    {
        lastSelectedTool = null;
        _toolSelected = false;
        _colorpickerCanvas.gameObject.SetActive(false);
    }

    public void ChangeColor(string color)
    {
        Debug.Log($"Changing color to: {color}");
        if (lastSelectedTool.name == "MarkerBlack")
        {
            Marker marker = lastSelectedTool.GetComponent<Marker>();
            if (color == "Red")
            {
                marker.SetColor(_matRed);
            }
            else if (color == "Orange")
            {
                marker.SetColor(_matOrange);
            }
            else if (color == "Yellow")
            {
                marker.SetColor(_matYellow);
            }
            else if (color == "Green")
            {
                marker.SetColor(_matGreen);
            }
            else if (color == "Cyan")
            {
                marker.SetColor(_matCyan);
            }
            else if (color == "Blue")
            {
                marker.SetColor(_matBlue);
            }
            else if (color == "Magenta")
            {
                marker.SetColor(_matMagenta);
            }
            else if (color == "Black")
            {
                marker.SetColor(_matBlack);
            }
        }
        else if (lastSelectedTool.name == "Brush")
        {
            Paintbrush brush = lastSelectedTool.GetComponent<Paintbrush>();
            if (color == "Red")
            {
                brush.SetColor(_matRed);
            }
            else if (color == "Orange")
            {
                brush.SetColor(_matOrange);
            }
            else if (color == "Yellow")
            {
                brush.SetColor(_matYellow);
            }
            else if (color == "Green")
            {
                brush.SetColor(_matGreen);
            }
            else if (color == "Cyan")
            {
                brush.SetColor(_matCyan);
            }
            else if (color == "Blue")
            {
                brush.SetColor(_matBlue);
            }
            else if (color == "Magenta")
            {
                brush.SetColor(_matMagenta);
            }
            else if (color == "Black")
            {
                brush.SetColor(_matBlack);
            }
        }
        else if (lastSelectedTool.name == "Pencil")
        {
            Pencil pencil = lastSelectedTool.GetComponent<Pencil>();
            if (color == "Red")
            {
                pencil.SetColor(_matRed);
            }
            else if (color == "Orange")
            {
                pencil.SetColor(_matOrange);
            }
            else if (color == "Yellow")
            {
                pencil.SetColor(_matYellow);
            }
            else if (color == "Green")
            {
                pencil.SetColor(_matGreen);
            }
            else if (color == "Cyan")
            {
                pencil.SetColor(_matCyan);
            }
            else if (color == "Blue")
            {
                pencil.SetColor(_matBlue);
            }
            else if (color == "Magenta")
            {
                pencil.SetColor(_matMagenta);
            }
            else if (color == "Black")
            {
                pencil.SetColor(_matBlack);
            }
        }
        else if (lastSelectedTool.name == "Spray Paint")
        {
            SprayPaint spray = lastSelectedTool.GetComponent<SprayPaint>();
            if (color == "Red")
            {
                spray.SetColor(_matRed);
            }
            else if (color == "Orange")
            {
                spray.SetColor(_matOrange);
            }
            else if (color == "Yellow")
            {
                spray.SetColor(_matYellow);
            }
            else if (color == "Green")
            {
                spray.SetColor(_matGreen);
            }
            else if (color == "Cyan")
            {
                spray.SetColor(_matCyan);
            }
            else if (color == "Blue")
            {
                spray.SetColor(_matBlue);
            }
            else if (color == "Magenta")
            {
                spray.SetColor(_matMagenta);
            }
            else if (color == "Black")
            {
                spray.SetColor(_matBlack);
            }
        }
    }
}
