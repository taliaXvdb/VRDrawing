using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _mushroom;
    [SerializeField] private Canvas _house01Canvas;
    [SerializeField] private Canvas _house02Canvas;
    [SerializeField] private Canvas _house03Canvas;
    [SerializeField] private List<Button> _buttons;
    public bool DoingTutorial = false;
    private Animator _animator;
    private GameManager _gameManager;
    private TextWriter _textWriter;
    private MushroomActions _mushroomActions;
    private TMP_Text _tutorialText;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _animator = _mushroom.GetComponent<Animator>();
        _textWriter = GameObject.FindObjectOfType<TextWriter>();
        _mushroomActions = GameObject.FindObjectOfType<MushroomActions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_mushroomActions._pathIndex == 2)
        {
            _house01Canvas.gameObject.SetActive(false);
        }
        if (_mushroomActions._pathIndex == 3)
        {
            _house02Canvas.gameObject.SetActive(false);
        }
        if (_mushroomActions._pathIndex == 4)
        {
            _house03Canvas.gameObject.SetActive(false);
        }
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Started!");
        DoingTutorial = true;
        _gameManager.isTutorial = false;
        StartMushroomPath();
    }

    public void SkipTutorial()
    {
        Debug.Log("Tutorial Skipped!");
        _gameManager.isTutorial = false;
        DoingTutorial = false;
        _animator.SetBool("IsDone", true);
        StartCoroutine(Despawn());
        GameModeSelector gameModeSelector = GameObject.FindObjectOfType<GameModeSelector>();
        gameModeSelector.enabled = true;
    }

    public void StartMushroomPath()
    {
        Cinemachine.CinemachineDollyCart dollyCart = GameObject.FindObjectOfType<Cinemachine.CinemachineDollyCart>();
        dollyCart.enabled = true;
        _animator.SetBool("IsWalking", true);
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(_mushroom);
    }

    public void ShowHouse01()
    {
        _house01Canvas.gameObject.SetActive(true);
        _textWriter.enabled = true;
        _textWriter.canvas = _house01Canvas;
        _textWriter.buttons = _buttons;
        if (_house01Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
        {
            _tutorialText = _house01Canvas.GetComponentInChildren<TMP_Text>();
            _textWriter.uiText = _tutorialText;
        }
        _textWriter.Start();
    }

    public void ShowHouse02()
    {
        _house02Canvas.gameObject.SetActive(true);
        _textWriter.enabled = true;
        _textWriter.canvas = _house02Canvas;
        _textWriter.buttons = _buttons;
        if (_house02Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
        {
            _tutorialText = _house02Canvas.GetComponentInChildren<TMP_Text>();
            _textWriter.uiText = _tutorialText;
        }
        _textWriter.Start();
    }

    public void ShowHouse03()
    {
        _house03Canvas.gameObject.SetActive(true);
        _textWriter.enabled = true;
        _textWriter.canvas = _house03Canvas;
        _textWriter.buttons = _buttons;
        if (_house03Canvas.GetComponentInChildren<TMP_Text>().CompareTag("FullText"))
        {
            _tutorialText = _house03Canvas.GetComponentInChildren<TMP_Text>();
            _textWriter.uiText = _tutorialText;
        }
        _textWriter.Start();
    }


}
