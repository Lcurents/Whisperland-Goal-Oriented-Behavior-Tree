using UnityEngine;
using TMPro; // Untuk TextMeshProUGUI

public class PlayerNameDisplayController : MonoBehaviour
{
    private TextMeshProUGUI playerNameText;

    void Awake()
    {
        playerNameText = GetComponent<TextMeshProUGUI>();
        if (playerNameText == null)
        {
            Debug.LogError("PlayerNameDisplayController: TextMeshProUGUI component not found on " + gameObject.name + "!");
            enabled = false; // Nonaktifkan script jika tidak ada komponen teks
        }
    }

    void Start()
    {
        // Debugging: Pastikan CharacterDataManager.Instance sudah ada
        if (CharacterDataManager.Instance == null)
        {
            Debug.LogError("PlayerNameDisplayController: CharacterDataManager.Instance is null! Make sure _CharacterDataManager GameObject is in a scene that loads BEFORE this one, and has DontDestroyOnLoad.");
            playerNameText.text = "Error: Manager Not Found"; // Tampilkan error di UI
            return;
        }

        // Dapatkan nama karakter dari CharacterDataManager
        // Karena CharacterDataManager punya nameInputField.text yang sudah diisi dari file JSON,
        // kita bisa langsung ambil dari sana.
        string loadedName = CharacterDataManager.Instance.nameInputField.text;

        // Jika nama kosong (misalnya belum pernah disimpan atau file tidak ada),
        // bisa atur nama default atau tampilkan placeholder
        if (string.IsNullOrEmpty(loadedName))
        {
            loadedName = "Player"; // Nama default jika belum ada
            Debug.LogWarning("Player name not found in save data. Using default 'Player'.");
        }

        // Perbarui teks nama
        UpdatePlayerNameDisplay(loadedName);
    }

    // Fungsi untuk memperbarui teks nama
    public void UpdatePlayerNameDisplay(string newName)
    {
        if (playerNameText != null)
        {
            playerNameText.text = newName;
            Debug.Log($"PlayerNameDisplayController: Displaying name: {newName}");
        }
    }

    // Opsional: Jika nama bisa berubah saat game berjalan (misalnya di menu opsi),
    // Anda bisa menambahkan event di CharacterDataManager dan melanggan di sini.
    // Contoh di CharacterDataManager: public event System.Action<string> OnCharacterNameChanged;
    // Kemudian di sini:
    // void OnEnable()
    // {
    //     if (CharacterDataManager.Instance != null)
    //     {
    //         CharacterDataManager.Instance.OnCharacterNameChanged += UpdatePlayerNameDisplay;
    //     }
    // }
    // void OnDisable()
    // {
    //     if (CharacterDataManager.Instance != null)
    //     {
    //         CharacterDataManager.Instance.OnCharacterNameChanged -= UpdatePlayerNameDisplay;
    //     }
    // }
}