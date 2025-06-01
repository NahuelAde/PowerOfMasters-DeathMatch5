using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager; // Singleton para acceso global a GameManager
    private bool gameEnded = false; // Verifica si el juego ha terminado
    public GameObject victoryScreen; // Canvas para la pantalla de victoria
    public TMP_Text victoryText; // Texto para mostrar el mensaje de victoria


    // MÉTODO QUE SE LLAMA AL INICIO PARA CONFIGURAR EL SINGLETON
    private void Awake()
    {
        // Configuración del Singleton: asegura que haya solo una instancia de GameManager
        if (gameManager == null)
            gameManager = this;
        else
            Destroy(gameObject);
    }

    // MANEJAR LA MUERTE DE UN JUGADOR
    public void PlayerDied(int losingPlayerID)
    {
        if (gameEnded) return; // Si el juego ya terminó, no ejecutar nada

        gameEnded = true;

        // Determinar el jugador ganador basándonos en el jugador que perdió
        int winningPlayerID = losingPlayerID == 1 ? 2 : 1;

        // Encontrar al jugador ganador y activar su animación de victoria
        PlayerController winner = FindPlayerByID(winningPlayerID);
        if (winner != null)
        {
            winner.SetWinningState(); // Activar el estado ganador
        }

        // Mostrar la pantalla de victoria
        ShowVictoryScreen(winningPlayerID);
    }


    // BUSCAR EL JUGADOR SEGÚN SU ID EN LA ESCENA
    private PlayerController FindPlayerByID(int playerID)
    {
        // Buscar al jugador según su ID en la escena
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        // Recorrer todos los jugadores y buscar el que tenga el ID correspondiente
        foreach (PlayerController player in players)
        {
            if (player.playerID == playerID)
                return player; // Retornar el jugador que coincide con el ID
        }
        return null; // Retornar null si no se encuentra el jugador
    }


    // MOSTRAR LA PANTALLA DE VICTORIA CON EL MENSAJE DEL JUGADOR GANADOR
    private void ShowVictoryScreen(int winningPlayerID)
    {
        victoryScreen.SetActive(true);
        victoryText.text = $"Player {winningPlayerID} Wins!";
    }
}