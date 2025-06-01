using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu; // Panel del menú de pausa
    public GameObject controlsPanel; // Panel del menú de controles
    private bool isPaused = false; // Estado del juego (pausado o no)

    // MÉTODO LLAMADO EN CADA FRAME
    void Update()
    {
        // Detectar la tecla Escape para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu(); // Llamar al método para pausar/reanudar
        }
    }


    // ALTERNAR ENTRE PAUSAR Y REANUDAR EL JUEGO
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1; // Pausar o reanudar el tiempo del juego

        // Activar/desactivar el menú de pausa
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        // Si el juego no está pausado, ocultar el panel de controles
        if (!isPaused && controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }


    // MOSTRAR PANEL DE CONTROLES Y OCULTAR MENÚ DE PAUSA
    public void ShowControlsPanel()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }


    // OCULTAR EL PANEL DE CONTROLES Y VOLVER AL MENÚ DE PAUSA
    public void HideControlsPanel()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }


    // VOLVER AL MENÚ PRINCIPAL
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Reanudar el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu"); // Cargar el menú principal
    }
}
