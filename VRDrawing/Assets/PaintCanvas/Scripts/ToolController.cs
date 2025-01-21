using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolController : MonoBehaviour
{
    [SerializeField] private Transform controller; // Assign your VR controller
    [SerializeField] private string targetTag = "Tool"; // Set your desired tag in the Inspector
    private GameObject lastHoveredTool;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(controller.position, controller.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, 10f))
        {
            GameObject hoveredTool = hit.collider.gameObject;

            if (hoveredTool.CompareTag(targetTag))
            {
                // If it's a new object
                if (hoveredTool != lastHoveredTool)
                {
                    lastHoveredTool = hoveredTool;
                    Debug.Log($"Hovering over: {hoveredTool.name}");
                }
            }
            else
            {
                // Reset when the object doesn't have the correct tag
                if (lastHoveredTool != null)
                {
                    lastHoveredTool = null;
                }
            }
        }
    }
}
