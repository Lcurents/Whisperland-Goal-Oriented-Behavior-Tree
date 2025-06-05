using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using WOTR.Game;
using CrashKonijn.Goap.Runtime;
using UnityEngine; // Tambahkan ini jika belum ada untuk Debug.Log

namespace WOTR.Game
{
    [GoapId("PickupApple-2f05cc01-35c2-4367-8db9-fbb561b13cfc")]
    public class PickupAppleAction : GoapActionBase<PickupAppleAction.Data>
    {
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            return ActionRunState.WaitThenComplete(0.5f);
        }

        public override void Complete(IMonoAgent agent, Data data)
        {
            if (data.Target is not TransformTarget transformTarget)
            {
                Debug.LogWarning("[PickupAppleAction] Target bukan TransformTarget atau null.");
                // Ini bisa jadi penyebab kegagalan jika target diperlukan untuk menyelesaikan goal
                // Jika target tidak valid, mungkin goal tidak bisa terpenuhi
                return;
            }

            data.DataBehaviour.appleCount++;
            GameObject.Destroy(transformTarget.Transform.gameObject);

            // --- PENTING: Setel flag completion goal di DataBehaviour ---
            // Ini akan memberi tahu PickupAppleGoalBT bahwa goal sudah selesai.
            if (data.DataBehaviour != null)
            {
                data.DataBehaviour.goalPickupAppleCompleted = true;
                Debug.Log("[PickupAppleAction] Completed! Apple picked up and goalPickupAppleCompleted set to true.");
            }
            else
            {
                Debug.LogError("[PickupAppleAction] DataBehaviour is null in action data!");
            }
        }

        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            [GetComponent]
            public DataBehaviour DataBehaviour { get; set; } // Pastikan ini ada
        }
    }
}