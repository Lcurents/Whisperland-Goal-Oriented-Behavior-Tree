using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using WOTR.Game;
using CrashKonijn.Goap.Core;

// Di dalam IdleGoalBT.cs Anda
public class IdleGoalBT : Action
{
    private DataBehaviour data;
    private GoapActionProvider provider;
    // Hapus flag lokal goalCompleted dan goalRequested dari sini, karena kita akan pakai DataBehaviour

    public override void OnStart()
    {
        provider = GetComponent<GoapActionProvider>();
        data = GetComponent<DataBehaviour>(); // Pastikan DataBehaviour ada di GameObject yang sama

        if (provider == null)
        {
            Debug.LogError("[IdleGoalBT] GoapActionProvider not found!");
            return;
        }
        if (data == null)
        {
            Debug.LogError("[IdleGoalBT] DataBehaviour not found on this GameObject!");
            return;
        }

        // Reset flag di DataBehaviour saat goal BT ini dimulai
        data.goalIdleCompleted = false;

        provider.Events.OnGoalCompleted += OnGoalCompleted; // Tetap berlangganan event
        provider.RequestGoal<IdleGoal>();
        Debug.Log("[IdleGoalBT] RequestGoal<IdleGoal>() dipanggil.");
    }

    public override TaskStatus OnUpdate()
    {
        if (data == null)
        {
            Debug.LogError("[IdleGoalBT] DataBehaviour null di OnUpdate!");
            return TaskStatus.Failure;
        }

        // Cek status completion langsung dari DataBehaviour
        if (data.goalIdleCompleted)
        {
            Debug.Log("[IdleGoalBT] DataBehaviour.goalIdleCompleted is TRUE. Returning Success.");
            return TaskStatus.Success;
        }

        return TaskStatus.Running; // Jika belum selesai, tetap Running
    }

    private void OnGoalCompleted(IGoal goal)
    {
        // Event ini dipanggil ketika sebuah GOAP Goal selesai.
        // Ini adalah layer konfirmasi, pastikan juga set flag di DataBehaviour
        if (goal is IdleGoal)
        {
            Debug.Log("[IdleGoalBT] Event OnGoalCompleted untuk IdleGoal diterima dari provider.");
            if (data != null) data.goalIdleCompleted = true;
        }
    }

    public override void OnEnd()
    {
        if (provider != null)
        {
            provider.Events.OnGoalCompleted -= OnGoalCompleted;
        }
        // Reset DataBehaviour.goalIdleCompleted saat node berakhir, agar bisa digunakan lagi di masa mendatang.
        if (data != null) data.goalIdleCompleted = false;
    }
}