using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NameInputManager : MonoBehaviour
{
    public GameObject nameInputPanel;        // Panel que pide el nombre
    public TMP_InputField nameInputField;    // Campo de texto para el nombre
    public Button submitButton;              // Botón de confirmar
    public HighscoreTable highscoreTable;    // Referencia a la tabla de puntuaciones

    private int finalScore;

    private void Start()
    {
        // Asegurarse de que el panel esté oculto al inicio
        nameInputPanel.SetActive(false);

        // Añadir listener al botón de confirmar
        submitButton.onClick.AddListener(SubmitName);
    }

    public void ShowNameInput(int score)
    {
        finalScore = score;
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
            // Añadir la puntuación a la tabla de puntuaciones
            highscoreTable.CheckAndAddHighscore(finalScore, playerName);

            // Ocultar el panel de entrada de nombre
            nameInputPanel.SetActive(false);
        }
    }
}
