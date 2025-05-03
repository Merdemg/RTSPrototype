using UnityEngine;
using UnityEngine.InputSystem; // Make sure to include this!

public class SimpleInputTester : MonoBehaviour
{
    // This method name MUST match your Action name, prefixed with "On"
    // if using the "Send Messages" behavior.
    public void OnTestAction(InputAction.CallbackContext context)
    {
        // context.performed is true when the button is pressed down
        if (context.performed)
        {
            Debug.Log("TestAction PERFORMED! (Key Pressed)");
        }

        // context.started is true the frame the button is initially pressed
        if (context.started)
        {
            Debug.Log("TestAction STARTED!");
        }

        // context.canceled is true when the button is released
        if (context.canceled)
        {
            Debug.Log("TestAction CANCELED! (Key Released)");
        }
    }

    // You can add other methods for other actions later, e.g.:
    // public void OnMove(InputAction.CallbackContext context)
    // {
    //     Vector2 moveInput = context.ReadValue<Vector2>();
    //     Debug.Log($"Move Input: {moveInput}");
    // }
}