#if UNITY_XRI

using UnityEngine;


namespace Flexalon
{
    public class FlexalonXRInputProvider : MonoBehaviour, InputProvider
    {
        public InputMode InputMode => InputMode.External;
        public bool Active => _selected;
        public Ray Ray => default;
        public Vector3 UIPointer => default;
        public GameObject ExternalFocusedObject => (_hovered || _selected) ? gameObject : null;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable _interactable;
        private bool _hovered => _interactable?.isHovered ?? false;
        private bool _selected => _interactable?.isSelected ?? false;

        public void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (_interactable == null)
            {
                Debug.LogWarning("FlexalonXRInputProvider should be placed next to an XR Interactable component.");
            }
        }
    }
}

#endif