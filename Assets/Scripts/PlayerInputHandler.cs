using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public void OnClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // Ignore UI clicks

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var unit = hit.collider.GetComponentInParent<Unit>();
            if (unit != null)
            {
                unit.HandleClick();
            }
        }
    }
}
