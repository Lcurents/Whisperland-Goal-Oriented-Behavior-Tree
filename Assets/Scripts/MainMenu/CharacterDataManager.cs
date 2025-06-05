using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterDataManager : MonoBehaviour
{
    // Pastikan ini adalah instance dari CharacterDataManager
    public static CharacterDataManager Instance { get; private set; } // <<== Perbaiki ini

    public TMP_InputField nameInputField;
    public TextMeshProUGUI characterNameDisplay;

    private string saveFileName = "playerdata.json";
    private string saveFilePath;

    void Awake()
    {
        // Implementasi Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Hancurkan duplikat
            return;
        }
        Instance = this; // <<== Set instance-nya ke objek ini
        DontDestroyOnLoad(gameObject); // Opsional: Agar data tetap ada antar scene

        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"Save file path: {saveFilePath}");
    }


void Start()
    {
        // Pastikan referensi UI terisi
        if (nameInputField == null)
        {
            Debug.LogError("Name Input Field not assigned in Inspector!");
        }
        if (characterNameDisplay == null)
        {
            Debug.LogError("Character Name Display Text not assigned in Inspector!");
        }

        // Coba muat nama karakter saat game dimulai (jika sudah ada)
        LoadCharacterName();
    }

    // Dipanggil saat tombol "Simpan Nama" diklik
    public void SaveCharacterName()
    {
        PlayerData data = new PlayerData();
        data.characterName = nameInputField.text; // Ambil teks dari InputField

        // Konversi objek PlayerData menjadi string JSON
        string json = JsonUtility.ToJson(data, true); // true untuk formatting yang mudah dibaca

        // Tulis string JSON ke file
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"Character name '{data.characterName}' saved to: {saveFilePath}");

        // Perbarui tampilan setelah menyimpan
        characterNameDisplay.text = $"Nama: {data.characterName}";
        SceneManager.LoadScene("SampleScene");
    }

    // Dipanggil saat tombol "Muat Nama" diklik (atau dari Start)
    public void LoadCharacterName()
    {
        if (File.Exists(saveFilePath))
        {
            // Baca string JSON dari file
            string json = File.ReadAllText(saveFilePath);

            // Konversi string JSON kembali menjadi objek PlayerData
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            // Tampilkan nama yang dimuat
            nameInputField.text = data.characterName;
            characterNameDisplay.text = $"Nama: {data.characterName}";

            Debug.Log($"Character name '{data.characterName}' loaded from: {saveFilePath}");
        }
        else
        {
            Debug.LogWarning($"Save file not found at: {saveFilePath}. No character name loaded.");
            nameInputField.text = ""; // Kosongkan input field
            characterNameDisplay.text = "Nama: [Belum Dimuat]";
        }
    }

    // Opsional: Untuk debugging, menghapus file save
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted!");
            nameInputField.text = "";
            characterNameDisplay.text = "Nama: [Belum Dimuat]";
        }
        else
        {
            Debug.LogWarning("No save file to delete.");
        }
    }
}