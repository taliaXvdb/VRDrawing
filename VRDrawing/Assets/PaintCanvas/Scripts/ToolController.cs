using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolController : MonoBehaviour
{
    [SerializeField] private Transform controller; // Assign your VR controller
    [SerializeField] private string targetTag = "Tool"; // Set your desired tag in the Inspector
    [SerializeField] private InputActionAsset inputActions;
    private GameObject lastHoveredTool;
    private InputAction gripAction;
    private InputAction cancelAction;
    private bool isHoldingTool;
    void Start()
    {

    }

    void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Controller");
        gripAction = actionMap.FindAction("Grip");
        cancelAction = actionMap.FindAction("Secondary Button");
        gripAction.Enable();
        cancelAction.Enable();
    }

    void OnDisable()
    {
        gripAction.Disable();
        cancelAction.Disable();
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
            if (gripAction.triggered)
            {
                isHoldingTool = true;
                Debug.Log("Grip pressed");
                LockTool(lastHoveredTool);
            }
        }
    }

    void LockTool(GameObject currentTool)
    {
        if (currentTool != null && isHoldingTool)
        {
            Debug.Log($"Locking tool: {currentTool.name}");
            Rigidbody rb = currentTool.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            currentTool.transform.SetParent(controller);
            currentTool.transform.localPosition = Vector3.zero;
            currentTool.transform.localRotation = Quaternion.identity;
        }

        if (cancelAction.triggered)
        {
            isHoldingTool = false;
            Rigidbody rb = currentTool.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            Debug.Log("Secondary button pressed");
            UnlockTool(currentTool);
        }
    }

    void UnlockTool(GameObject currentTool)
    {
        if (currentTool != null && !isHoldingTool)
        {
            Debug.Log($"Unlocking tool: {currentTool.name}");
            currentTool.transform.SetParent(null);
        }
    }
}
