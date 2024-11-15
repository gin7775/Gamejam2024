using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NameInputManager : MonoBehaviour
{
    public GameObject nameInputPanel;        // Panel que pide el nombre
    public TMP_InputField nameInputField;    // Campo de texto para el nombre
    public Button submitButton;              // Botón de confirmar
    public HighscoreTable highscoreTable;
    public GameObject highscore;
    public GameObject RetryButton;

    private GameManager gameManager;


    private void Start()
    {
        // Asegurarse de que el panel esté oculto al inicio
        nameInputPanel.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
        // Añadir listener al botón de confirmar
        submitButton.onClick.AddListener(SubmitName);
    }

    public void ShowNameInput()
    {
        nameInputPanel.SetActive(true);
        nameInputField.text = ""; // Limpiar el campo de texto
        nameInputField.ActivateInputField(); // Activar el campo de texto
    }

    private void Update()
    {
        // Detectar la tecla Enter para confirmar la entrada
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SubmitName();
        }
    }

    private void SubmitName()
    {
        string playerName = nameInputField.text;

        // Validar el nombre (mínimo 1 carácter)
        if (playerName.Length > 0)
        {

            highscore.SetActive(true);
            // Añadir la puntuación a la tabla de puntuaciones
            highscoreTable.CheckAndAddHighscore(gameManager.score, playerName);
            highscoreTable.RefreshHighscoreTable();
            RetryButton.gameObject.SetActive(true);

            // Ocultar el panel de entrada de nombre
            nameInputPanel.SetActive(false);
        }
    }
}
