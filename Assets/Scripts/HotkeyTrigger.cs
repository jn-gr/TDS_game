using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotkeyTrigger : MonoBehaviour
{
    public KeyCode hotkey = KeyCode.Space;
    [Tooltip("Set to 'None' to not have a secondary hotkey attached to this button.")]
    public KeyCode optionalHotkey = KeyCode.None;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hotkey) || (optionalHotkey != KeyCode.None && Input.GetKeyDown(optionalHotkey)))
        {
            button.onClick.Invoke();
        }
    }
}
