using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LineDrawingManager : MonoBehaviour
{
    [SerializeField] private Canvas _stopDrawingCanvas;
    [SerializeField] private Canvas _saveDrawingCanvas;
    [SerializeField] private PaintCanvasLine _drawingCanvas;
    [SerializeField] private Canvas _showScoreCanvas;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Texture2D _originalImage;
    private InputAction _menuAction;
    private Processor _processor;

    // Start is called before the first frame update
    void Start()
    {
        _processor = FindObjectOfType<Processor>();
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
            _saveDrawingCanvas.gameObject.SetActive(true);
        }
    }

    public void SaveDrawing()
    {
        string filename = "MyDrawing.png";
        _drawingCanvas.ExportPaintedLines("AccuracyProcess/SavedDrawings", filename);
        _saveDrawingCanvas.gameObject.SetActive(false);
        _showScoreCanvas.gameObject.SetActive(true);
        ShowScore();
    }

    public void CancelMenu()
    {
        _saveDrawingCanvas.gameObject.SetActive(false);
    }

    public void ShowScore()
    {
        _processor.ProcessImages(_originalImage);
        float score = _processor.similarity;
        score = Mathf.Round(score * 100);
        Debug.Log("Score: " + score + "%");
        _showScoreCanvas.GetComponentInChildren<TMP_Text>().text = "Your score is: " + score + "%";
    }

    public void ShowStopDrawing()
    {
        _showScoreCanvas.gameObject.SetActive(false);
        _stopDrawingCanvas.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public void TryAgainDrawing()
    {
        SceneManager.LoadScene("FollowTheLine");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game!");
    }
}
