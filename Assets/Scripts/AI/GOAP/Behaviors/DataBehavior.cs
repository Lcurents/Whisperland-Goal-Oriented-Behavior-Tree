// Di dalam DataBehaviour.cs Anda
using System;
using UnityEngine;

namespace WOTR.Game
{
    public class DataBehaviour : MonoBehaviour
    {
        public int appleCount = 0;
        public float hunger = 0f;
        // --- Pastikan Anda memiliki ini ---
        public bool goalIdleCompleted = false;
        public bool goalPickupAppleCompleted = false; // Jika ada di PickupAppleGoalBT
        public bool goalEatAppleCompleted = false; // Jika ada di EatGoalBT
                                                   // --- Akhir pengecekan ---

        private void Update()
        {
            this.hunger += Time.deltaTime * 5f;
        }
    }
}