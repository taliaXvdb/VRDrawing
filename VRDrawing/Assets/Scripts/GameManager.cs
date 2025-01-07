using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mushroomPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartMushroomPath());
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
