using UnityEngine;
using CrashKonijn.Agent.Runtime; // Untuk IMonoAgent
using CrashKonijn.Agent.Core; // Untuk IMonoAgent
using CrashKonijn.Goap.Runtime; // Untuk IMovementTarget

public class AgentAnimationController_Final : MonoBehaviour
{
    private Animator animator;
    private IMonoAgent monoAgent; // Referensi ke MonoAgent
    private Vector3 previousPosition; // Untuk menyimpan posisi frame sebelumnya

    // Variabel untuk menyimpan arah terakhir agent menghadap (untuk idle)
    private Vector2 lastMoveDirection = Vector2.down; // Default: menghadap ke bawah

    void Start()
    {
        animator = GetComponent<Animator>();
        monoAgent = GetComponent<IMonoAgent>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
            enabled = false;
            return;
        }
        if (monoAgent == null)
        {
            Debug.LogError("IMonoAgent component not found on " + gameObject.name + ". This script requires a GOAP Agent.");
            enabled = false;
            return;
        }

        // Inisialisasi previousPosition di awal
        previousPosition = transform.position;
    }

    void Update()
    {
        // Hitung kecepatan aktual berdasarkan perubahan posisi
        Vector3 currentPosition = transform.position;
        Vector3 displacement = currentPosition - previousPosition;
        float deltaTime = Time.deltaTime;

        // Hindari pembagian dengan nol jika deltaTime sangat kecil atau nol
        Vector2 currentVelocity = Vector2.zero;
        if (deltaTime > Mathf.Epsilon) // Mathf.Epsilon adalah nilai float terkecil yang bisa diwakili
        {
            currentVelocity = new Vector2(displacement.x, displacement.y) / deltaTime;
        }

        // Simpan posisi saat ini untuk frame berikutnya
        previousPosition = currentPosition;

        // Tentukan apakah agent sedang bergerak
        // Gunakan threshold kecil untuk menghindari masalah floating point atau gerakan yang tidak signifikan
        bool isMoving = currentVelocity.magnitude > 0.05f;

        // Set parameter IsMoving di Animator
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Dapatkan arah pergerakan (normalisasi agar panjang vektor = 1)
            Vector2 moveDirection = currentVelocity.normalized;

            // Set parameter MoveX dan MoveY untuk animasi berjalan
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);

            // Simpan arah pergerakan terakhir yang dominan
            // Ini penting untuk menjaga arah idle yang benar
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
            {
                lastMoveDirection.x = Mathf.Sign(moveDirection.x);
                lastMoveDirection.y = 0;
            }
            else if (Mathf.Abs(moveDirection.y) > Mathf.Abs(moveDirection.x))
            {
                lastMoveDirection.y = Mathf.Sign(moveDirection.y);
                lastMoveDirection.x = 0;
            }
            // Tambahan: Jika murni diagonal atau kedua-duanya nol, biarkan lastMoveDirection sesuai sebelumnya
            else if (moveDirection.magnitude > 0.01f) // Hanya update jika ada gerakan signifikan
            {
                lastMoveDirection = moveDirection.normalized;
            }
        }
        else // Jika agent tidak bergerak (idle)
        {
            // Set parameter LastMoveX dan LastMoveY untuk animasi idle
            animator.SetFloat("LastMoveX", lastMoveDirection.x);
            animator.SetFloat("LastMoveY", lastMoveDirection.y);
        }
    }

    // Untuk debugging visualisasi arah (optional)
    void OnDrawGizmos()
    {
        if (Application.isPlaying && animator != null)
        {
            Gizmos.color = Color.blue;
            Vector3 currentPos = transform.position;
            // Gambar vektor arah terakhir
            Vector3 lastDir = new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0) * 1f; // Skala agar terlihat
            Gizmos.DrawLine(currentPos, currentPos + lastDir);

            // Gambar vektor arah bergerak saat ini (jika bergerak)
            if (animator.GetBool("IsMoving"))
            {
                Gizmos.color = Color.green;
                Vector3 moveDir = new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"), 0) * 1f;
                Gizmos.DrawLine(currentPos, currentPos + moveDir);
            }
        }
    }
}