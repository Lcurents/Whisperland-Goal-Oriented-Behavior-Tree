using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    public TextMeshPro greetingText;
    public string[] greetingsSource = {
        "Halo, petualang!",
        "Cuaca cerah hari ini, ya?",
        "Butuh bantuan?",
        "Hati-hati di jalan!",
        "Selamat datang di desa kami."
    };

    public float showDistance = 2f;
    private Transform player;
    private bool hasGreeted = false;

    private List<string> greetingsQueue = new List<string>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        greetingText.text = "";
        greetingText.gameObject.SetActive(false);
        ShuffleGreetings(); // Isi awal
    }

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= showDistance)
        {
            if (!hasGreeted)
            {
                ShowGreeting();
                hasGreeted = true;
            }
        }
        else
        {
            if (hasGreeted)
            {
                HideGreeting();
                hasGreeted = false;
            }
        }
    }

    void ShowGreeting()
    {
        if (greetingsQueue.Count == 0)
            ShuffleGreetings(); // Reset saat habis

        greetingText.text = greetingsQueue[0];
        greetingsQueue.RemoveAt(0);
        greetingText.gameObject.SetActive(true);
    }

    void HideGreeting()
    {
        greetingText.gameObject.SetActive(false);
    }

    void ShuffleGreetings()
    {
        greetingsQueue = new List<string>(greetingsSource);

        // Algoritma Fisher-Yates Shuffle
        for (int i = 0; i < greetingsQueue.Count; i++)
        {
            int rnd = Random.Range(i, greetingsQueue.Count);
            var temp = greetingsQueue[i];
            greetingsQueue[i] = greetingsQueue[rnd];
            greetingsQueue[rnd] = temp;
        }
    }
}
