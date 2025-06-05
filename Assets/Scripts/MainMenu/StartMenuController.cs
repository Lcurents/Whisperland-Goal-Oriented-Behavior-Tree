using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Tambahkan ini jika Anda ingin referensi langsung ke Button atau CanvasGroup

public class StartMenuController : MonoBehaviour
{
    // Referensi ke GameObject yang menampung tombol (panel atau tombol itu sendiri)
    public GameObject buttonContainer;
    public GameObject Nama;

    // Referensi ke AudioSource untuk sound effect
    public AudioSource buttonAppearAudioSource;

    // Audio Clip untuk sound effect saat tombol muncul
    public AudioClip appearSoundClip;

    public float delayBeforeButtonsAppear = 5.0f; // Jeda waktu sebelum tombol muncul

    void Start()
    {
        // Pastikan buttonContainer dinonaktifkan di awal jika belum dilakukan di Editor
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Button Container is not assigned in StartMenuController!");
        }

        // Memulai coroutine untuk menunda kemunculan tombol
        StartCoroutine(ShowButtonsWithDelay());
    }

    IEnumerator ShowButtonsWithDelay()
    {
        // Menunggu selama waktu yang ditentukan
        yield return new WaitForSeconds(delayBeforeButtonsAppear);

        // Setelah jeda, aktifkan container tombol
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(true);
        }

        // Putar sound effect jika AudioSource dan AudioClip tersedia
        if (buttonAppearAudioSource != null && appearSoundClip != null)
        {
            buttonAppearAudioSource.PlayOneShot(appearSoundClip);
        }
        else
        {
            if (buttonAppearAudioSource == null)
            {
                Debug.LogWarning("Button Appear Audio Source is not assigned in StartMenuController!");
            }
            if (appearSoundClip == null)
            {
                Debug.LogWarning("Appear Sound Clip is not assigned in StartMenuController!");
            }
        }
    }

    public void OnContinueClick()
    {
        // Pastikan ada logic untuk memuat scene jika tombol diklik
        SceneManager.LoadScene("SampleScene");
    }

    public void OnStartClick()
    {
        buttonContainer.SetActive(false);
        Nama.SetActive(true);
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}