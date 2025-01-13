using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Flexalon;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text uiText;
    public List<Button> buttons;
    private string fullText;
    public float typingSpeed = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource _audioSource;
    private bool isSkipping = false; // Flag to skip typing
    public bool isTyping = false; // Flag to check if typing is in progress
    private MushroomActions _mushroomActions;
    private TutorialManager _tutorialManager;

    public void Start()
    {
        _mushroomActions = GameObject.FindObjectOfType<MushroomActions>();
        _tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
        uiText = canvas.GetComponentInChildren<TMP_Text>();
        if (uiText.CompareTag("FullText"))
        {
            fullText = uiText.text;
            uiText.text = ""; // Clear the text
        }

        isTyping = true;
        // Start the typing effect
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            if (isSkipping)
            {
                uiText.text = fullText; // Show the full text
                yield break;
            }

            uiText.text += fullText[i];
            if (typingSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(typingSound);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        Debug.Log("Text has finished typing!");
        if (_tutorialManager.DoingTutorial)
        {
            if (_mushroomActions._pathIndex == 1)
            {
                buttons[0].gameObject.SetActive(true);
            }
            else if (_mushroomActions._pathIndex == 2)
            {
                buttons[1].gameObject.SetActive(true);
            }
            else if (_mushroomActions._pathIndex == 3)
            {
                buttons[2].gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    // Update Method for Skipping
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) // Replace with your desired skip input
        // {
        //     isSkipping = true;
        // }
    }
}
