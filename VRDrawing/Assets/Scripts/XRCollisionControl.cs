using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XRCollisionControl : MonoBehaviour
{

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Portal")
    //     {
    //         Debug.Log("Collided with wall");
    //     }
    // }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Portal")
        {
            Debug.Log("Collided with portal");
            SceneManager.LoadScene("Main");
        }
    }
}
