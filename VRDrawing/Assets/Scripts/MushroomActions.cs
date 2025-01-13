using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DollyPathEndEvent : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _dollyCart;
    [SerializeField] private CinemachinePathBase _dollyPath1;
    [SerializeField] private CinemachinePathBase _dollyPath2;
    [SerializeField] private CinemachinePathBase _dollyPath3;
    [SerializeField] private GameObject _mushroom;
    public bool _continuePath = false;
    private bool hasReachedEnd = false;
    private Animator _animator;
    private int _pathIndex = 1;

    void Start()
    {
        _animator = _mushroom.GetComponent<Animator>();
        _animator.SetBool("Start", false);
    }

    void Update()
    {
        if (_dollyCart == null || _dollyPath1 == null) return;

        if (!hasReachedEnd && Mathf.Approximately(_dollyCart.m_Position, _dollyPath1.PathLength))
        {
            hasReachedEnd = true;
            _pathIndex++;
            OnPathEndReached();
        }
    }

    void OnPathEndReached()
    {
        Debug.Log("End of path reached! Triggering event...");
        _animator.SetBool("IsIdle", true);
        if (_continuePath)
        {
            if (_pathIndex == 2)
            {
                _dollyCart.m_Path = _dollyPath2;
                _dollyCart.m_Position = 0;
                _animator.SetBool("IsWalking", true);
                Debug.Log("Switched to Path 2");
            }
            else if (_pathIndex == 3)
            {
                _dollyCart.m_Path = _dollyPath3;
                _dollyCart.m_Position = 0;
                _animator.SetBool("IsWalking", true);
                Debug.Log("Switched to Path 2");
            }
            else
            {
                Debug.Log("End of paths reached!");
            }
        }
    }
}
