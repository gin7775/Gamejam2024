using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class NameInputManager : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;
    public Button submitButton;
    public GameObject fallbackButton; // Primer botón a seleccionar al salir del input
    public GameObject RetryButton;
    public GameObject highscore;
    public GameObject firstGameObjectRanking;

    public HighscoreTable highscoreTable;
    private GameManager gameManager;
    private bool panelActivo = false;

    private void Start()
    {
        nameInputPanel.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
        submitButton.onClick.AddListener(SubmitName);
    }

    public void ShowNameInput()
    {
        panelActivo = true;
        nameInputPanel.SetActive(true);
        nameInputField.text = "";

        // Activar input directamente
        nameInputField.ActivateInputField();
        EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
    }

    private void Update()
    {
        if (!panelActivo) return;

        // Confirmar nombre con Enter o botón A
        if ((Keyboard.current?.enterKey.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false)) // botón A
        {
            SubmitName();
        }

        // Si el jugador está escribiendo pero pulsa una flecha/joystick para navegar, salir del campo
        if (nameInputField.isFocused)
        {
            if (
                (Keyboard.current?.downArrowKey.wasPressedThisFrame ?? false) ||
                (Keyboard.current?.upArrowKey.wasPressedThisFrame ?? false) ||
                (Keyboard.current?.leftArrowKey.wasPressedThisFrame ?? false) ||
                (Keyboard.current?.rightArrowKey.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.dpad.down.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.dpad.up.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.dpad.left.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.dpad.right.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.leftStick.down.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.leftStick.up.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.leftStick.left.wasPressedThisFrame ?? false) ||
                (Gamepad.current?.leftStick.right.wasPressedThisFrame ?? false)
               )
            {
                nameInputField.DeactivateInputField(); // salir del input
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(fallbackButton); // pasar a navegación
            }
        }
    }

    private void SubmitName()
    {
        string playerName = nameInputField.text;

        if (playerName.Length > 0)
        {
            highscore.SetActive(true);
            highscoreTable.CheckAndAddHighscore(gameManager.score, playerName);
            highscoreTable.RefreshHighscoreTable();

            RetryButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstGameObjectRanking);

            nameInputPanel.SetActive(false);
            panelActivo = false;
        }
    }
}
