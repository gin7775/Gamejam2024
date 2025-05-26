using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum DeviceType { None, Mouse, Gamepad, Touch }

public class InputDetector : MonoBehaviour
{

    [Header("Controller")]
    [SerializeField] private Sprite controllerDashSprite;
    [SerializeField] private Sprite controllerMoveSprite;
    [SerializeField] private Sprite controllerShootSprite;
    [SerializeField] private Sprite controllerMeleeSprite;

    [Header("Mouse & Keyboard")]
    [SerializeField] private Sprite kbDashSprite;
    [SerializeField] private Sprite kbMoveSprite;
    [SerializeField] private Sprite kbShootSprite;
    [SerializeField] private Sprite kbMeleeSprite;

    [Header("Phone")]
    [SerializeField] private Sprite phoneDashSprite;
    [SerializeField] private Sprite phoneMoveSprite;
    [SerializeField] private Sprite phoneShootSprite;
    [SerializeField] private Sprite phoneMeleeSprite;

    public DeviceType currentDevice { get; private set; } = DeviceType.None;

    void Update()
    {
        // 1) Touch: algún dedo en progreso
        if (Touchscreen.current != null && Touchscreen.current.touches.Any(t => t.isInProgress))
        {
            currentDevice = DeviceType.Touch;
            Debug.Log("Táctil");
            PhoneUI();

            return;
        }

        // 2) Gamepad: stick movido o botón presionado
        if (Gamepad.current != null && Gamepad.current.allControls.Any(c =>
            (c is StickControl stick && stick.ReadValue().sqrMagnitude > 0.1f) ||
            (c is ButtonControl btn && btn.isPressed)))
        {
            currentDevice = DeviceType.Gamepad;
            Debug.Log("Mando");
            ControllerUI();

            return;
        }

        // 3) Mouse: movimiento del ratón
        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f)
        {
            currentDevice = DeviceType.Mouse;
           // Debug.Log("Ratón y Teclado");
            MouseKAndKeyboardUI();

            return;
        }

        // Si llegamos aquí, no hay entrada detectada
        currentDevice = DeviceType.None;
    }


    // 
    public void ControllerUI()
    {

    }

    public void PhoneUI()
    {

    }

    public void MouseKAndKeyboardUI()
    {

    }

}