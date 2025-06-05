using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMove : MonoBehaviour
{
    public Joystick movementJoystick;
    public float playerSpeed;
    private Rigidbody2D rb;
    private Animator animator;

    private float lastMoveX;
    private float lastMoveY;

    public AudioClip footstepSoundClip; // Slot untuk suara langkah kaki
    private AudioSource audioSource; // Referensi ke komponen Audio Source

    // Tidak perlu isWalkingSoundPlaying atau Coroutine lagi dengan Animation Event
    // private bool isWalkingSoundPlaying = false;
    // private Coroutine footstepCoroutine; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on player! Footstep sounds will not play.");
            // Jika tidak ada AudioSource, script tidak bisa memutar suara, tapi tetap biarkan aktif untuk animasi
        }
        if (footstepSoundClip == null)
        {
            Debug.LogWarning("Footstep Sound Clip is not assigned in the Inspector! Footstep sounds will not play.");
        }

        audioSource.loop = false; // Pastikan ini false karena kita akan memutar suara secara manual
        audioSource.playOnAwake = false; 

        lastMoveX = 0f;
        lastMoveY = -1f;
    }

    private void FixedUpdate()
    {
        float moveX = movementJoystick.Direction.x;
        float moveY = movementJoystick.Direction.y;

        rb.linearVelocity = new Vector2(moveX * playerSpeed, moveY * playerSpeed);

        bool isMoving = (moveX != 0 || moveY != 0);

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);

            lastMoveX = moveX;
            lastMoveY = moveY;
        }
        else // Jika karakter tidak bergerak (idle)
        {
            // Jika agent berhenti, pastikan suara langkah kaki berhenti juga
            // Ini penting jika animasi sedang berjalan dan tiba-tiba berhenti
            if (audioSource != null && audioSource.isPlaying && footstepSoundClip != null)
            {
                // Kita tidak perlu mengecek isWalkingSoundPlaying lagi karena event animasi yang memicunya
                // AudioSource.Stop() akan menghentikan semua PlayOneShot yang sedang berjalan pada AudioSource ini
                audioSource.Stop(); 
            }

            animator.SetFloat("LastMoveX", lastMoveX);
            animator.SetFloat("LastMoveY", lastMoveY);
        }
    }

    // Fungsi ini akan dipanggil oleh Animation Event
    public void PlayFootstepSFX()
    {
        if (audioSource != null && footstepSoundClip != null)
        {
            // Memastikan player sedang dalam status "moving" di Animator
            // Ini mencegah suara langkah kaki diputar saat animasi walk beralih ke idle, misalnya
            if (animator.GetBool("IsMoving")) 
            {
                audioSource.PlayOneShot(footstepSoundClip);
            }
        }
    }
}