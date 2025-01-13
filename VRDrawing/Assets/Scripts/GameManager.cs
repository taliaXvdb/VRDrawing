using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Flexalon;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private Canvas _startTutorialCanvas;
    [SerializeField] private List<Button> _buttons;
    private TMP_Text _tutorialText;
    // Start is called before the first frame update
    void Start()
    {
        TextWriter textWriter = GameObject.FindObjectOfType<TextWriter>();
        textWriter.enabled = true;
        textWriter.canvas = _startTutorialCanvas;
        textWriter.buttons = _buttons;
        if (_startTutorialCanvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
        {
            _tutorialText = _startTutorialCanvas.GetComponentInChildren<TMP_Text>();
            textWriter.uiText = _tutorialText;
        }
        // StartCoroutine(StartMushroomPath());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartMushroomPath()
    {
        yield return new WaitForSeconds(5);
        //get the cinemachine dolly cart script
        Cinemachine.CinemachineDollyCart dollyCart = GameObject.FindObjectOfType<Cinemachine.CinemachineDollyCart>();
        dollyCart.enabled = true;
        Debug.Log("5 seconds have passed");
    }
}
