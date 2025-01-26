using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Flexalon;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private Canvas _welcomeCanvas;
    [SerializeField] private Canvas _startTutorialCanvas;
    [SerializeField] private List<Button> _buttons;
    private TMP_Text _tutorialText;
    private TextTyper _textTyper;
    public bool isTutorial = true;

    // Start is called before the first frame update
    void Start()
    {
        _textTyper = GameObject.FindObjectOfType<TextTyper>();

        if (!_welcomeCanvas.gameObject.activeSelf)
        {
            _welcomeCanvas.gameObject.SetActive(true);
            _tutorialText = _welcomeCanvas.GetComponentInChildren<TMP_Text>();

            if (_tutorialText.CompareTag("FullText"))
            {
                _textTyper.StartTyping(_tutorialText);
            }
            StartCoroutine(ShowTutorial());
        }
    }

    private IEnumerator ShowTutorial()
    {
        // Wait until the typing is complete
        while (_textTyper.isTyping)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        // After typing finishes, hide the welcome canvas
        _welcomeCanvas.gameObject.SetActive(false);

        // Now activate the tutorial canvas
        TextWriter textWriter = GameObject.FindObjectOfType<TextWriter>();
        textWriter.enabled = true;
        textWriter.canvas = _startTutorialCanvas;
        textWriter.buttons = _buttons;

        if (_startTutorialCanvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
        {
            _tutorialText = _startTutorialCanvas.GetComponentInChildren<TMP_Text>();
            textWriter.uiText = _tutorialText;
        }

        // Show the start tutorial canvas
        _startTutorialCanvas.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTutorial)
        {
            _startTutorialCanvas.gameObject.SetActive(false);
        }
    }
}
