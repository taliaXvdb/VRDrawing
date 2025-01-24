using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private string targetTag = "Door";
    [SerializeField] private Canvas house01Canvas;
    [SerializeField] private Canvas house02Canvas;
    [SerializeField] private Canvas house03Canvas;
    [SerializeField] private Canvas hoverHouse01Canvas;
    [SerializeField] private Canvas hoverHouse02Canvas;
    [SerializeField] private Canvas hoverHouse03Canvas;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private List<Button> _buttons;
    private InputAction buttonAction;
    private InputAction cancelAction;
    private TextWriter _textWriter;
    private bool settingUpGamemode = false;
    private TMP_Text _tutorialText;
    private GameObject lastHoveredObject; // Tracks the last object hovered

    void Start()
    {
        _textWriter = GameObject.FindObjectOfType<TextWriter>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        // Get the action from the input asset
        var actionMap = inputActions.FindActionMap("Controller");
        buttonAction = actionMap.FindAction("Primary Button");
        cancelAction = actionMap.FindAction("Secondary Button");

        buttonAction.Enable();
        cancelAction.Enable();
    }

    void OnDisable()
    {
        buttonAction.Disable();
        cancelAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject.CompareTag(targetTag) && !settingUpGamemode)
            {
                if (hoveredObject != lastHoveredObject)
                {
                    lastHoveredObject = hoveredObject;

                    settingUpGamemode = true;
                    Debug.Log($"Hovering over: {hoveredObject.name}");
                    if (hoveredObject.name == "DoorHouse01")
                    {
                        hoverHouse01Canvas.gameObject.SetActive(true);
                    }
                    else if (hoveredObject.name == "DoorHouse02")
                    {
                        hoverHouse02Canvas.gameObject.SetActive(true);
                    }
                    else if (hoveredObject.name == "DoorHouse03")
                    {
                        hoverHouse03Canvas.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (lastHoveredObject != null)
                {
                    Debug.Log("Not hovering over a door!");
                    lastHoveredObject = null;
                    HideHoverCanvas();
                }
            }
            if (buttonAction.triggered)
            {
                Debug.Log("Activate is pressed");
                HideHoverCanvas();
                ShowSettings(hoveredObject.name);
            }
            if (cancelAction.triggered)
            {
                Debug.Log("Cancel is pressed");
                HideSettings();
            }
            else
            {
                // Reset when no object is hit or the object doesn't have the correct tag
                if (lastHoveredObject != null)
                {
                    Debug.Log("No longer hovering over anything.");
                    lastHoveredObject = null;
                    HideHoverCanvas();
                }
            }
        }
    }

    void ShowSettings(string houseName)
    {
        settingUpGamemode = true;
        if (houseName == "DoorHouse01")
        {
            house01Canvas.gameObject.SetActive(true);
            _textWriter.enabled = true;
            _textWriter.canvas = house01Canvas;
            _textWriter.buttons = _buttons;
            if (house01Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
            {
                _tutorialText = house01Canvas.GetComponentInChildren<TMP_Text>();
                _textWriter.uiText = _tutorialText;
            }
            _textWriter.Start();
        }
        else if (houseName == "DoorHouse02")
        {
            house02Canvas.gameObject.SetActive(true);
            _textWriter.enabled = true;
            _textWriter.canvas = house02Canvas;
            _textWriter.buttons = _buttons;
            if (house02Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
            {
                _tutorialText = house02Canvas.GetComponentInChildren<TMP_Text>();
                _textWriter.uiText = _tutorialText;
            }
            _textWriter.Start();
        }
        else if (houseName == "DoorHouse03")
        {
            house03Canvas.gameObject.SetActive(true);
            _textWriter.enabled = true;
            _textWriter.canvas = house03Canvas;
            _textWriter.buttons = _buttons;
            if (house03Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
            {
                _tutorialText = house03Canvas.GetComponentInChildren<TMP_Text>();
                _textWriter.uiText = _tutorialText;
            }
            _textWriter.Start();
        }
    }

    void HideHoverCanvas()
    {
        hoverHouse01Canvas.gameObject.SetActive(false);
        hoverHouse02Canvas.gameObject.SetActive(false);
        hoverHouse03Canvas.gameObject.SetActive(false);
    }

    void HideSettings()
    {
        house01Canvas.gameObject.SetActive(false);
        house02Canvas.gameObject.SetActive(false);
        house03Canvas.gameObject.SetActive(false);
    }

    public void startNewFreeDrawing()
    {
        Debug.Log("Starting new drawing!");
        SceneManager.LoadScene("DrawingScene");
    }
}
