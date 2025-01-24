using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FreeDrawingManager : MonoBehaviour
{
    [SerializeField] private Canvas _stopDrawingCanvas;
    [SerializeField] private Canvas _saveDrawingCanvas;
    [SerializeField] private PaintCanvas _drawingCanvas;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction _menuAction;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Controller");
        _menuAction = actionMap.FindAction("Primary Button");
        _menuAction.Enable();
    }

    void OnDisable()
    {
        _menuAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_menuAction.triggered)
        {
            Debug.Log("Menu button pressed!");
            _saveDrawingCanvas.gameObject.SetActive(true);
        }
    }

    public void SaveDrawing()
    {
        Debug.Log("Drawing saved!");
        string filename = "MyDrawing.png";
        _drawingCanvas.ExportPaintedLines("SavedDrawings", filename);
        _saveDrawingCanvas.gameObject.SetActive(false);
        _stopDrawingCanvas.gameObject.SetActive(true);
    }

    public void DeleteDrawing()
    {
        Debug.Log("Drawing deleted!");
        _saveDrawingCanvas.gameObject.SetActive(false);
        _stopDrawingCanvas.gameObject.SetActive(true);
    }

    public void CancelMenu()
    {
        _saveDrawingCanvas.gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going back to main menu!");
        SceneManager.LoadScene("Main");
    }

    public void StartNewDrawing()
    {
        Debug.Log("Starting new drawing!");
        SceneManager.LoadScene("DrawingScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game!");
    }
}
