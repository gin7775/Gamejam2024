using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class InputFieldExitOnSelect : MonoBehaviour
{
    public TMP_InputField inputField;     // Campo de texto a monitorear
    public GameObject fallbackButton;     // Botón al que moverse si hay input de navegación

    private bool listening = false;

    // Este método lo llamas desde el OnSelect del InputField
    public void OnInputFieldSelected()
    {
        listening = true;
        inputField.ActivateInputField();
    }

    private void Update()
    {
        if (!listening || !inputField.isFocused) return;

        bool navigate =
           
            (Gamepad.current?.dpad.down.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.dpad.up.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.dpad.left.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.dpad.right.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftStick.down.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftStick.up.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftStick.left.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftStick.right.wasPressedThisFrame ?? false);

        if (navigate)
        {
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(fallbackButton);
            listening = false;
        }
    }
}
