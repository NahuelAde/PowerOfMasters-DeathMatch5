using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class HUDButtons : MonoBehaviour
{
    public Button muteButton;          // Botón para silenciar el volumen
    public Sprite buttonMute;          // Sprite para el botón de silencio
    public Sprite buttonUnmute;        // Sprite para el botón de volumen activado

    public GameObject pauseMenu;       // Referencia al menú de pausa

    private bool isPaused = false;     // Estado del juego (pausado o no)
    private bool isMuted = false;      // Estado del sonido (silenciado o no)
    public AudioMixer audioMixer;      // Mezclador de audio


    // MÉTODO QUE SE EJECUTA AL ARRANCAR EL JUEGO
    void Start()
    {
        // Asegurarse de que el menú de pausa esté desactivado al inicio
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        // Recuperar el estado de silencio guardado
        if (PlayerPrefs.HasKey("Muted"))
        {
            isMuted = PlayerPrefs.GetInt("Muted") == 1;
            ApplyMuteState();
        }
    }


    // REINICIAR EL JUEGO
    public void RestartGame()
    {
        Time.timeScale = 1; // Asegurarse de que el tiempo esté normalizado
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }


    // PAUSAR EL JUEGO
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1; // Pausa o reanuda el juego

        // Mostrar u ocultar el menú de pausa
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }
    }


    // SILENCIAR EL VOLUMEN
    public void ToggleMute()
    {
        isMuted = !isMuted;
        
        ApplyMuteState(); // Aplicar el estado de silencio

        // Guardar el estado de silencio en PlayerPrefs para que persista
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }


    // APLICAR ESTADO DE SILENCIO GUARDADO
    private void ApplyMuteState()
    {
        float volume = isMuted ? -80f : 0f; // Si está silenciado, establecer el volumen a -80dB (silencio), sino a 0dB (volumen normal)

        // Ajustar el volumen del mezclador maestro y sus subgrupos
        audioMixer.SetFloat("MasterVolume", volume); // Controlar el volumen del grupo principal
        audioMixer.SetFloat("AmbientSoundVolume", volume); // Controlar el volumen de los sonidos ambientales
        audioMixer.SetFloat("SFXVolume", volume); // Controlar el volumen de los efectos de sonido


        // Cambiar el sprite del botón de silencio
        if (muteButton != null)
        {
            muteButton.image.sprite = isMuted ? buttonUnmute : buttonMute;
        }
    }
}
