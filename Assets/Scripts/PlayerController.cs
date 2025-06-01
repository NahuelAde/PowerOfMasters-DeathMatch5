using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ATRIBUTOS
    public float Speed; // Velocidad de movimiento horizontal del jugador
    public float JumpForce; // Fuerza aplicada para que el jugador salte
    public int life = 3; // Inicializar las vidas del jugador en 3
    public int playerID; // Identificador del jugador
    private Vector2 reboundDirection; // Dirección de rebote

    // CONSTANTES
    private const float FlipScaleX = 1.5f; // Escala horizontal del personaje
    private const float GroundCheckOffset = 0.7f; // Offset del Raycast para detectar el suelo
    private const float RayLength = 0.1f; // Longitud del Raycast
    private const float AttackDuration = 0.2f; // Duración del ataque en segundos
    private const int Damage = 1; // Daño de un golpe

    // REFERENCIAS
    private Rigidbody2D playerRigidbody; // Referencia al componente Rigidbody2D del jugador
    private Animator playerAnimator; // Referencia al componente Animator del jugador
    public HUD hud; // Referencia al HUD para mostrar las vidas

    public AudioSource soundRunning; // AudioSource para los pasos
    public AudioClip clipRunning; // Clip de audio para los pasos
    public AudioClip clipJumping; // Clip de audio de salto
    public AudioClip clipSword; // Clip de audio para el golpeo de espada
    public AudioClip clipHit; // Clip de audio para golpe recibido
    public AudioClip clipVictory; // Clip de audio grito victoria
    public AudioClip clipDead; // Clip de audio jugador muerto

    // ESTADOS
    private bool isAttacking; // Indica si el jugador está atacando
    private bool isRunning; // Indica si el jugador está corriendo
    private bool isJumping; // Indica si el jugador está saltando
    private bool isGettingDamage; // Indica si el jugador está recibiendo daño
    private bool isDead; // Indica si el jugador ha muerto
    private bool isWinningPlayer; // Indica el jugador ganador


    // START ES LLAMADO AL INICIO
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>(); // Obtener una referencia al componente Rigidbody 2D asociado al objeto del jugador
        playerAnimator = GetComponent<Animator>(); // Obtener una referencia al Animator asociado al objeto del jugador
    }


    // UPDATE SE LLAMA UNA VEZ POR CADA FRAME
    void FixedUpdate()
    {
        HandleInput(); // Manejar entradas del usuario
        UpdateAnimations(); // Actualizar animaciones basadas en estados
        HandleRunningSound(); // Manejo centralizado del sonido de pasos

        // Verificar si el jugador cae por debajo del límite
        if (transform.position.y < 290f && !isDead)
        {
            Die();
        }
    }


    // MANEJO DE ENTRADAS DE USUARIO
    private void HandleInput()
    {
        if (isDead) return;

        // Movimiento horizontal
        Action<float> processMovement = horizontalInput =>
        {
            if (Mathf.Abs(horizontalInput) > 0.1f)
                Move(horizontalInput * Speed);
            else
                StopMoving();
        };

        // Verificar y ejecutar si se debe saltar
        Action handleJump = () =>
        {
            if (Input.GetButtonDown($"Player{playerID}Vertical") && IsGrounded())
                Jump();
        };

        // Verificar y ejecutar si se debe atacar
        Action handleAttack = () =>
        {
            if (Input.GetButtonDown($"Player{playerID}Attack") && !isAttacking)
                StartCoroutine(PerformAttack());
        };

        // Procesar todas las entradas en una lista de funciones
        new List<Action>
    {
        () => processMovement(Input.GetAxis($"Player{playerID}Horizontal")),
        handleJump,
        handleAttack
    }.ForEach(action => action());
    }


    // MOVIMIENTO DEL PERSONAJE
    public void Move(float speed)
    {
        playerRigidbody.velocity = new Vector2(speed, playerRigidbody.velocity.y);
        transform.localScale = new Vector3(speed > 0 ? FlipScaleX : -FlipScaleX, FlipScaleX, 1.0f);
        isRunning = true;
    }


    // DETENER EL MOVIMIENTO DEL PERSONAJE
    private void StopMoving()
    {
        playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
        isRunning = false;
    }


    // SALTO DEL PERSONAJE
    private void Jump()
    {
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, JumpForce);
        isJumping = true;
        AudioController.audioController.PlaySound(clipJumping);
    }


    // COMPROBACIÓN SI PERSONAJE ESTÁ EN EL SUELO
    private bool IsGrounded()
    {
        // Posición de inicio del Raycast: base del jugador
        Vector2 rayOrigin = new(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y - GroundCheckOffset);

        // Disparar el Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, RayLength, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }


    // ACTIVACIÓN DEL ESTADO DE ATAQUE DEL JUGADOR DURANTE UN TIEMPO DETERMINADO
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        playerAnimator.SetBool("isAttacking", true); // Activar animación de ataque
        yield return new WaitForSeconds(AttackDuration); // Esperar la duración del ataque
        if (!isDead)
            AudioController.audioController.PlaySound(clipSword);
        {
            isAttacking = false;
            playerAnimator.SetBool("isAttacking", false); // Desactivar animación de ataque
        }
    }


    // COMPROBACIÓN SI PERSONAJE RECIBE UN GOLPE
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Sword"))
        {
            Vector2 hitDirection = new(collider.transform.position.x, 0);
            GetDamage(hitDirection, Damage);
            AudioController.audioController.PlaySound(clipHit);
        }
    }


    // ASIGNAR DAÑO AL RECIBIR UN IMPACTO
    public void GetDamage(Vector2 direction, int damage)
    {
        if (!isGettingDamage)
        {
            isGettingDamage = true;
            life -= damage;

            // Actualizar el HUD
            if (hud != null)
            {
                hud.UpdateLives(playerID, life);
            }

            // Comprobar si el jugador muere a raíz del golpe
            if (life <= 0)
            {
                isDead = true;
                GameManager.gameManager.PlayerDied(playerID); // Notificar al GameManager
                AudioController.audioController.PlaySound(clipDead);
            }
            else if (!isDead)
            {
                // Iniciar corutina para desactivar el estado de recibir daño
                StartCoroutine(StopDamage());
            }
        }
    }


    // DESACTIVAR DAÑO TRAS UN TIEMPO DETERMINADO
    private IEnumerator StopDamage()
    {
        yield return new WaitForSeconds(AttackDuration); // Esperar la duración del ataque
        isGettingDamage = false;
    }


    // ACTIVAR EL ESTADO DE VICTORIA DEL JUGADOR
    public void SetWinningState()
    {
        isWinningPlayer = true;
        playerAnimator.SetBool("isWinningPlayer", isWinningPlayer);
        AudioController.audioController.PlaySound(clipVictory);

        // Detener todos los sonidos de pasos
        if (soundRunning.isPlaying)
        {
            soundRunning.Stop();
        }

        // Reiniciar otros estados de animación
        isRunning = false;
        isJumping = false;
        isAttacking = false;
        isGettingDamage = false;

        StopMoving(); // Detener cualquier movimiento

        enabled = false; // Deshabilitar el control del jugador
    }


    // MANEJAR LA MUERTE AL CAER FUERA DE ESCENARIO
    private void Die()
    {
        isDead = true;

        // Notificar al HUD que las vidas del jugador son cero
        if (hud != null)
        {
            hud.UpdateLives(playerID, 0);
        }

        GameManager.gameManager.PlayerDied(playerID); // Notificar al GameManager
    }


    // MÉTODO PARA MANEJAR EL SONIDO DE PASOS
    private void HandleRunningSound()
    {
        if (isDead || isWinningPlayer) // Si el jugador está muerto o ha ganado, detener el sonido y salir del método
        {
            if (soundRunning.isPlaying)
            {
                soundRunning.Stop();
            }
            return;
        }

        bool isMoving = Mathf.Abs(playerRigidbody.velocity.x) > 0.1f; // Verificar si el jugador se está moviendo

        if (isMoving && IsGrounded()) // Reproducir el sonido solo si está en el suelo
        {
            if (!soundRunning.isPlaying) // Evitar reiniciar el sonido si ya está reproduciéndose
            {
                PlayFootsteps();
            }
        }
        else if (!isMoving || !IsGrounded()) // Detener el sonido si no se está moviendo o no está en el suelo
        {
            if (soundRunning.isPlaying)
            {
                soundRunning.Stop();
            }
        }
    }


    // MÉTODO PARA REPRODUCIR EL SONIDO DE PASOS
    private void PlayFootsteps()
    {
        if (!isDead && soundRunning != null && clipRunning != null)
        {
            if (soundRunning.clip != clipRunning) // Asignar el clip si no está configurado
            {
                soundRunning.clip = clipRunning;
            }
            soundRunning.loop = true; // Asegurar que el sonido esté en bucle
            soundRunning.Play();
        }
    }


    // ACTUALIZAR LAS ANIMACIONES
    private void UpdateAnimations()
    {
        // Prioridad a la animación de victoria
        if (isWinningPlayer)
        {
            playerAnimator.SetBool("isWinningPlayer", true);
            return; // Salir para evitar sobrescribir con otras animaciones
        }

        playerAnimator.SetBool("isRunning", isRunning);  // Actualizar el estado de correr
        playerAnimator.SetBool("isJumping", isJumping = !IsGrounded()); // Actualizar el estado de saltar comprobando si el jugador está en el suelo
        playerAnimator.SetBool("isGettingDamage", isGettingDamage); // Actualizar el estado de recibir daño
        playerAnimator.SetBool("isDead", isDead); // Actualizar el estado en caso de morir
    }


    // DESTRUIR LA ANIMACIÓN DE MUERTE AL FINALIZAR
    public void OnDeathAnimationEnd()
    {
        StopAllCoroutines(); // Detener cualquier corutina asociada al objeto
        Destroy(gameObject);
    }
}



