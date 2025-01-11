using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DollyPathEndEvent : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _dollyCart;
    [SerializeField] private CinemachinePathBase _dollyPath;
    [SerializeField] private GameObject _mushroom;
    private bool hasReachedEnd = false;
    private Animator _animator;

    void Update()
    {
        if (_dollyCart == null || _dollyPath == null) return;

        if (!hasReachedEnd && Mathf.Approximately(_dollyCart.m_Position, _dollyPath.PathLength))
        {
            hasReachedEnd = true;
            OnPathEndReached();
        }
    }

    void OnPathEndReached()
    {
        Debug.Log("End of path reached! Triggering event...");
        _animator = _mushroom.GetComponent<Animator>();
        _animator.SetBool("IsIdle", true);
    }
}
