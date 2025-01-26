using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{
    public TMP_Text uiText; // Text element to type into
    public List<Button> buttons; // Buttons to hide/show during typing
    private string fullText; // Full text to type
    public float typingSpeed = 0.05f; // Delay between each character
    [SerializeField] private AudioClip typingSound; // Typing sound effect
    [SerializeField] private AudioSource _audioSource; // Audio source for sound
    private bool isSkipping = false; // Flag to skip typing
    public bool isTyping = false; // Flag to track if typing is in progress

    public void StartTyping(TMP_Text textToType)
    {
        uiText = textToType;
        fullText = uiText.text; // Store the full text
        uiText.text = ""; // Clear the UI text
        isTyping = true;
        isSkipping = false;

        // Start typing coroutine
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            if (isSkipping)
            {
                uiText.text = fullText; // Show the full text immediately
                break;
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

        // Re-enable buttons here
        if (buttons != null)
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }


    public void SkipTyping()
    {
        // Set flag to skip typing
        isSkipping = true;
    }
}
