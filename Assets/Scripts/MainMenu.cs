using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // CARGAR ESCENA DEL JUEGO
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // SALIR DEL JUEGO
    public void QuitGame()
    {
        Application.Quit();
    }
}
