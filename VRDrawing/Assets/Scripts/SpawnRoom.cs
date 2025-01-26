using System.Collections;
using UnityEngine;
using TMPro;

public class SpawnRoom : MonoBehaviour
{
    [SerializeField] private Transform controller;
    [SerializeField] private Canvas _introCanvas;
    [SerializeField] private TMP_Text _introText;

    [SerializeField] private Canvas _rightHandCanvas;
    [SerializeField] private TMP_Text _rightHandText;

    [SerializeField] private Canvas _leftHandCanvas;
    [SerializeField] private TMP_Text _leftHandText;

    public float typingSpeed = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource _audioSource;

    private int _currentStep = 0;
    private bool isTyping = false; // Flag to check if typing is in progress
    private bool isSequenceRunning = false; // Prevent Update from triggering multiple coroutines

    private string[][] allTexts; // Stores all text sequences for all canvases
    private Canvas[] allCanvases; // Stores references to all canvases
    private TMP_Text[] allTextFields; // Stores references to all text components

    void Start()
    {
        // Initialize the text arrays
        allTexts = new string[][]
        {
            new string[] { "Whoa, what is this?", "It looks like some kind of portal.", "Let me check it out!" },
            new string[] { "Use the right joystick to look around" },
            new string[] { "Use the left joystick to move around" }
        };

        // Initialize the canvas and text references
        allCanvases = new Canvas[] { _introCanvas, _rightHandCanvas, _leftHandCanvas };
        allTextFields = new TMP_Text[] { _introText, _rightHandText, _leftHandText };

        // Disable all canvases except the first one
        for (int i = 1; i < allCanvases.Length; i++)
        {
            allCanvases[i].gameObject.SetActive(false);
        }

        // Ensure the first canvas is active and clear its text
        _introCanvas.gameObject.SetActive(true);
        _introText.text = "";
    }

    void Update()
    {
        if (!isSequenceRunning && _currentStep < allTexts.Length)
        {
            isSequenceRunning = true;
            StartCoroutine(DisplayCanvasSequence(_currentStep));
        }
    }

    private IEnumerator DisplayCanvasSequence(int stepIndex)
    {
        Canvas currentCanvas = allCanvases[stepIndex];
        TMP_Text currentTextField = allTextFields[stepIndex];
        string[] currentTexts = allTexts[stepIndex];

        currentCanvas.gameObject.SetActive(true);

        // Iterate through each text in the current canvas
        foreach (string text in currentTexts)
        {
            currentTextField.text = ""; // Clear text before typing
            isTyping = true;

            for (int i = 0; i < text.Length; i++)
            {
                currentTextField.text += text[i];
                if (typingSound != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(typingSound);
                }
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
            yield return new WaitForSeconds(2); // Wait before showing the next text
        }

        // After finishing all texts for this canvas, disable it
        currentCanvas.gameObject.SetActive(false);
        _currentStep++;
        isSequenceRunning = false; // Allow Update to proceed to the next sequence
    }
}
