using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Random = UnityEngine.Random;
using WOTR.Game; // Pastikan namespace WOTR.Game ada di sini
using UnityEngine;
using CrashKonijn.Agent.Runtime; // Perlu ini untuk Debug.LogError

namespace WOTR.Game
{
    // The GoapId attribute is used to identify the action, even when you change the name
    // This is used when using the Scriptable Object method of configuring actions
    [GoapId("Idle-ccc6f46c-1626-44aa-b90d-1b2741642166")]
    public class IdleAction : GoapActionBase<IdleAction.Data>
    {
        // This method is called when the action is started
        // This method is optional and can be removed
        public override void Start(IMonoAgent agent, Data data)
        {
            data.Timer = Random.Range(0.5f, 1.5f);
            // Debug.Log("[IdleAction] Started, setting timer."); // Opsional debugging
        }

        // This method is called every frame while the action is running
        // This method is required
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if (data.Timer <= 0f)
            {
                // Debug.Log("[IdleAction] Timer finished, returning Completed."); // Opsional debugging
                return ActionRunState.Completed;
            }

            // Lower the timer for the next frame
            data.Timer -= context.DeltaTime;

            // Return continue to keep the action running
            return ActionRunState.Continue;
        }

        // --- Tambahkan metode Complete() ini untuk memberitahu DataBehaviour ---
        public override void Complete(IMonoAgent agent, Data data)
        {
            // Pastikan data.DataBehaviour tidak null sebelum mengaksesnya
            if (data.DataBehaviour != null)
            {
                data.DataBehaviour.goalIdleCompleted = true; // <--- INI PENTING!
                Debug.Log("[IdleAction] Completed! Setting DataBehaviour.goalIdleCompleted = true."); // Debugging
            }
            else
            {
                Debug.LogError("[IdleAction] DataBehaviour is null in action data!");
            }
        }

        // The action class itself must be stateless!
        // All data should be stored in the data class
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public float Timer { get; set; }

            // --- Tambahkan ini: Untuk mendapatkan referensi ke DataBehaviour ---
            [GetComponent] // Atribut ini akan secara otomatis mengisi DataBehaviour
            public DataBehaviour DataBehaviour { get; set; }
        }
    }
}