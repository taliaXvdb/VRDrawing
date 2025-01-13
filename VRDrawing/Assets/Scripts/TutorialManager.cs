using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _mushroom;
    private bool DoingTutorial = false;
    private Animator _animator;
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Started!");
        DoingTutorial = true;
        _gameManager.isTutorial = true;
        StartMushroomPath();
    }

    public void SkipTutorial()
    {
        Debug.Log("Tutorial Skipped!");
        _gameManager.isTutorial = false;
        DoingTutorial = false;
        _mushroom.SetActive(false);
    }

    public void StartMushroomPath()
    {
        Cinemachine.CinemachineDollyCart dollyCart = GameObject.FindObjectOfType<Cinemachine.CinemachineDollyCart>();
        dollyCart.enabled = true;
        _animator = _mushroom.GetComponent<Animator>();
        _animator.SetBool("IsWalking", true);
    }
}
