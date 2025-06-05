using System;
using UnityEngine; // Perlu ini jika Anda ingin menggunakan Debug.Log di masa depan dalam class ini

// Penting: Kelas harus Serializable agar JsonUtility bisa bekerja
[System.Serializable]
public class PlayerData
{
    public string characterName;
    // Anda bisa menambahkan data lain di sini nanti, misalnya:
    // public int level;
    // public float money;
    // public InventoryData inventory; // Jika Anda punya class InventoryData juga Serializable
}