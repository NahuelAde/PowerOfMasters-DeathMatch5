using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Arrays que almacenan los íconos de vida de los jugadores
    public Image[] player1LifeIcons; // Íconos de vida del jugador 1
    public Image[] player2LifeIcons; // Íconos de vida del jugador 2

    public Sprite player1ActiveLifeSprite; // Sprite activo para el jugador 1
    public Sprite player1InactiveLifeSprite; // Sprite inactivo para el jugador 1

    public Sprite player2ActiveLifeSprite; // Sprite activo para el jugador 2
    public Sprite player2InactiveLifeSprite; // Sprite inactivo para el jugador 2


    // MÉTODO PARA ACTUALIZAR ICONOS DE VIDAS DE JUGADORES
    public void UpdateLives(int playerID, int currentLife)
    {
        // Seleccionar el array de iconos de vida y los sprites correspondientes dependiendo del jugador
        Image[] lifeIcons = playerID == 1 ? player1LifeIcons : player2LifeIcons;
        Sprite activeSprite = playerID == 1 ? player1ActiveLifeSprite : player2ActiveLifeSprite;
        Sprite inactiveSprite = playerID == 1 ? player1InactiveLifeSprite : player2InactiveLifeSprite;

        // Recorrer todos los iconos de vida del jugador y actualizar su sprite según las vidas restantes
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Si el índice es menor que el número de vidas actuales, asignar el sprite activo, de lo contrario, asignar el sprite inactivo
            lifeIcons[i].sprite = i < currentLife ? activeSprite : inactiveSprite;
        }
    }
}
