using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MushroomActions : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _dollyCart;
    [SerializeField] private CinemachinePathBase _dollyPath1;
    [SerializeField] private CinemachinePathBase _dollyPath2;
    [SerializeField] private CinemachinePathBase _dollyPath3;
    [SerializeField] private GameObject _mushroom;
    public bool _continuePath = false;
    private bool hasReachedEnd = false;
    private Animator _animator;
    private TutorialManager _tutorialManager;
    public int _pathIndex = 1;

    void Start()
    {
        _animator = _mushroom.GetComponent<Animator>();
        _tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
    }

    void Update()
    {
        if (_dollyCart == null || _dollyPath1 == null) return;

        if (_pathIndex == 1)
        {
            if (!hasReachedEnd && Mathf.Approximately(_dollyCart.m_Position, _dollyPath1.PathLength))
            {
                hasReachedEnd = true;
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsIdle", true);
                _tutorialManager.ShowHouse01();
            }
        }
        else if (_pathIndex == 2)
        {
            if (!hasReachedEnd && Mathf.Approximately(_dollyCart.m_Position, _dollyPath2.PathLength))
            {
                hasReachedEnd = true;
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsIdle", true);
                _tutorialManager.ShowHouse02();
            }
        }
        else if (_pathIndex == 3)
        {
            if (!hasReachedEnd && Mathf.Approximately(_dollyCart.m_Position, _dollyPath3.PathLength))
            {
                hasReachedEnd = true;
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsIdle", true);
                _tutorialManager.ShowHouse03();
            }
        }
        else
        {
            _tutorialManager.ShowEnd();
        }
    }

    void OnPathEndReached()
    {
        Debug.Log("End of path reached! Triggering event...");
        if (_continuePath)
        {
            if (_pathIndex == 2)
            {
                _dollyCart.m_Path = _dollyPath2;
                _dollyCart.m_Position = 0;
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsWalking", true);
                Debug.Log("Switched to Path 2");
                _continuePath = false;
                hasReachedEnd = false;
            }
            else if (_pathIndex == 3)
            {
                _dollyCart.m_Path = _dollyPath3;
                _dollyCart.m_Position = 0;
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsWalking", true);
                Debug.Log("Switched to Path 2");
                _continuePath = false;
                hasReachedEnd = false;
            }
            else
            {
                Debug.Log("End of paths reached!");
                _animator.SetBool("IsIdle", true);
                _animator.SetBool("IsWalking", false);
                hasReachedEnd = false;
                GameModeSelector gameModeSelector = GameObject.FindObjectOfType<GameModeSelector>();
                gameModeSelector.enabled = true;
            }
        }
    }

    public void ContinuePath()
    {
        Debug.Log("Continue Path!");
        _pathIndex++;
        _continuePath = true;
        OnPathEndReached();
    }
}
