using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotkeyTrigger : MonoBehaviour
{   
    [Header("Input Settings")]
    public string actionName = "TriggerHotkey"; // Name of the input action

    private Button button;
    private InputAction hotkeyAction;

    void Start()
    {
        button = GetComponent<Button>();

        // Get the action from the PlayerInput on the Main Camera
        PlayerInput playerInput = Camera.main.GetComponent<PlayerInput>();
        InputActionMap actionMap = playerInput.actions.FindActionMap("RTS Camera");
        hotkeyAction = actionMap.FindAction(actionName);

        if (hotkeyAction == null)
        {
            Debug.LogError($"Action '{actionName}' not found'");
            return;
        }

        // Enable the action and subscribe to its performed event
        hotkeyAction.Enable();
        hotkeyAction.performed += OnHotkeyPressed;
    }

    void OnDestroy()
    {
        // Unsubscribe from the action's event
        hotkeyAction.performed -= OnHotkeyPressed;
    }

    private void OnHotkeyPressed(InputAction.CallbackContext context)
    {
        // Trigger the button when the action is performed
        button.onClick.Invoke();
    }
}
