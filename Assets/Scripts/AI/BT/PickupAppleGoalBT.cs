using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using WOTR.Game;

public class PickupAppleGoalBT : Action
{
    private DataBehaviour data;
    private GoapActionProvider provider;
    private bool goalCompleted = false; // Flag lokal untuk status BT
    private bool planFailed = false; // Flag lokal untuk kegagalan plan
    private bool goalRequested = false; // Flag untuk memastikan RequestGoal hanya sekali

    public override void OnStart()
    {
        provider = GetComponent<GoapActionProvider>();
        data = GetComponent<DataBehaviour>();

        // Reset semua flag saat task BT dimulai
        goalCompleted = false;
        planFailed = false;
        goalRequested = false;

        if (provider == null)
        {
            Debug.LogError("[PickupAppleGoalBT] GoapActionProvider not found!");
            return; // Return segera jika provider tidak ditemukan
        }
        if (data == null)
        {
            Debug.LogError("[PickupAppleGoalBT] DataBehaviour not found!");
            return; // Return segera jika data tidak ditemukan
        }

        // Reset flag di DataBehaviour saat goal BT ini dimulai
        data.goalPickupAppleCompleted = false;

        // Berlangganan event CrashKonijn
        provider.Events.OnGoalCompleted += OnGoalCompleted;
        provider.Events.OnNoActionFound += OnNoActionFound;

        // Minta goal hanya sekali
        if (!goalRequested)
        {
            provider.RequestGoal<PickupAppleGoal>();
            goalRequested = true;
            Debug.Log("[PickupAppleGoalBT] RequestGoal<PickupAppleGoal>() dipanggil.");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (data == null)
        {
            Debug.LogError("[PickupAppleGoalBT] DataBehaviour null di OnUpdate!");
            return TaskStatus.Failure;
        }

        if (planFailed) // Jika ada kegagalan plan yang dilaporkan
        {
            Debug.LogWarning("[PickupAppleGoalBT] Plan gagal dibuat, return Failure.");
            return TaskStatus.Failure;
        }

        // Cek status completion langsung dari DataBehaviour (yang disetel oleh PickupAppleAction)
        // ATAU dari flag lokal goalCompleted (yang disetel oleh event OnGoalCompleted)
        if (goalCompleted || data.goalPickupAppleCompleted)
        {
            Debug.Log("[PickupAppleGoalBT] Goal PickupApple selesai, return Success.");
            return TaskStatus.Success;
        }

        // Jika belum selesai, tetap Running
        return TaskStatus.Running;
    }

    private void OnGoalCompleted(IGoal goal)
    {
        // Event ini dipanggil ketika sebuah GOAP Goal selesai.
        if (goal is PickupAppleGoal)
        {
            Debug.Log("[PickupAppleGoalBT] Event OnGoalCompleted untuk PickupAppleGoal diterima.");
            goalCompleted = true; // Set flag lokal
            if (data != null) data.goalPickupAppleCompleted = true; // Set flag di DataBehaviour
        }
    }

    private void OnNoActionFound(IGoalRequest request)
    {
        // Pastikan ini adalah event kegagalan untuk goal yang sedang kita minta
        if (request.Goals.Exists(g => g is PickupAppleGoal))
        {
            Debug.LogWarning("[PickupAppleGoalBT] OnNoActionFound terpicu untuk PickupAppleGoal. Ini bisa berarti goal tidak bisa dicapai lagi.");
            planFailed = true; // Set flag kegagalan
            goalCompleted = true; // Set goalCompleted juga agar OnUpdate bisa keluar dari Running
        }
    }

    public override void OnEnd()
    {
        if (provider != null)
        {
            // Pastikan untuk unsubscribe event
            provider.Events.OnGoalCompleted -= OnGoalCompleted;
            provider.Events.OnNoActionFound -= OnNoActionFound;
        }
        // Reset semua flag di DataBehaviour saat node BT berakhir, agar bisa digunakan lagi
        if (data != null) data.goalPickupAppleCompleted = false;
        goalCompleted = false;
        planFailed = false;
        goalRequested = false;
    }
}