using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DrawTutorialManager : MonoBehaviour
{
    [SerializeField] private Canvas _startTutorialCanvas;
    [SerializeField] private Canvas _toolTutorialCanvas;
    [SerializeField] private Canvas _useToolTutorialCanvas;
    [SerializeField] private Canvas _drawTutorialCanvas;
    [SerializeField] private List<Button> _buttons;
    public bool DoingTutorial = true;
    private int _currentStep = 0;
    private TextTyper _textTyper;
    private ToolController _toolController;
    private TMP_Text _tutorialText;
    // Start is called before the first frame update
    void Start()
    {
        _textTyper = GameObject.FindObjectOfType<TextTyper>();
        _toolController = GameObject.FindObjectOfType<ToolController>();
        _toolController.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentStep == 0)
        {
            ShowStartTutorial();
        }
        if (_currentStep == 1)
        {
            _startTutorialCanvas.gameObject.SetActive(false);
            ShowToolTutorial();
        }
        if (_currentStep == 2)
        {
            _toolTutorialCanvas.gameObject.SetActive(false);
            ShowUseToolTutorial();
        }
        if (_currentStep == 3)
        {
            _useToolTutorialCanvas.gameObject.SetActive(false);
            ShowDrawTutorial();
        }
        if (_currentStep == 4)
        {
            _drawTutorialCanvas.gameObject.SetActive(false);
            EndTutorial();
        }
        else
        {
            return;
        }
    }

    public void SkipTutorial()
    {
        Debug.Log("Tutorial Skipped!");
        _currentStep = -1;
        DoingTutorial = false;
        _textTyper.StopAllCoroutines();
        _textTyper.enabled = false;

        // Directly deactivate canvas for testing
        _startTutorialCanvas.gameObject.SetActive(false);

        _toolController.enabled = true;
    }


    public void EndTutorial()
    {
        Debug.Log("Tutorial Ended!");
        _currentStep = -1;
        DoingTutorial = false;
        _drawTutorialCanvas.gameObject.SetActive(false);
        _toolController.enabled = true;
    }

    public void tutorialCounter()
    {
        _currentStep++;
    }

    private void ShowStartTutorial()
    {
        if (!_startTutorialCanvas.gameObject.activeSelf) // Ensure this runs only once
        {
            _startTutorialCanvas.gameObject.SetActive(true);
            _tutorialText = _startTutorialCanvas.GetComponentInChildren<TMP_Text>();
            _textTyper.buttons = _buttons;

            if (_tutorialText.CompareTag("FullText"))
            {
                Debug.Log("Calling StartTyping for StartTutorial");
                _textTyper.StartTyping(_tutorialText);
            }
        }
    }

    private void ShowToolTutorial()
    {
        if (!_toolTutorialCanvas.gameObject.activeSelf) // Ensure this runs only once
        {
            _toolTutorialCanvas.gameObject.SetActive(true);
            _tutorialText = _toolTutorialCanvas.GetComponentInChildren<TMP_Text>();
            _textTyper.buttons = _buttons;
            if (_tutorialText.CompareTag("FullText"))
            {
                _textTyper.StartTyping(_tutorialText);
            }
        }
    }

    private void ShowUseToolTutorial()
    {
        if (!_useToolTutorialCanvas.gameObject.activeSelf) // Ensure this runs only once
        {
            _useToolTutorialCanvas.gameObject.SetActive(true);
            _tutorialText = _useToolTutorialCanvas.GetComponentInChildren<TMP_Text>();
            _textTyper.buttons = _buttons;
            if (_tutorialText.CompareTag("FullText"))
            {
                _textTyper.StartTyping(_tutorialText);
            }
        }
    }

    private void ShowDrawTutorial()
    {
        if (!_drawTutorialCanvas.gameObject.activeSelf) // Ensure this runs only once
        {
            _drawTutorialCanvas.gameObject.SetActive(true);
            _tutorialText = _drawTutorialCanvas.GetComponentInChildren<TMP_Text>();
            _textTyper.buttons = _buttons;
            if (_tutorialText.CompareTag("FullText"))
            {
                _textTyper.StartTyping(_tutorialText);
            }
        }
    }

}
