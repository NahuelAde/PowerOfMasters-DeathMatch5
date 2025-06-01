using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController audioController;
    private AudioSource audioSource;

    // MÉTODO QUE SE EJECUTA AL INICIALIZAR UN OBJETO EN LA ESCENA
    private void Awake()
    {
        if (audioController == null)
        {
            audioController = this; 
            DontDestroyOnLoad(gameObject); // Evitar que objeto se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Destruir objeto si está duplicado
        }

        audioSource = GetComponent<AudioSource>(); // Obtener AudioSource asociado a GameObject
    }


    // MÉTODO PARA REPRODUCIR SONIDO CON UN CLIP DE AUDIO ESPECÍFICO
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip); // Reproducir sonido sin interrumpir otros que ya están sonando
    }
}
