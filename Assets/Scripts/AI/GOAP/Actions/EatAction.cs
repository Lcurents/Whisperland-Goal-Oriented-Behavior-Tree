using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using WOTR.Game;
using UnityEngine;
using CrashKonijn.Agent.Runtime;

namespace WOTR.Game
{
    [GoapId("Eat-1be06ed6-78a9-47b3-81b6-fcfa1ac1219a")]
    public class EatAction : GoapActionBase<EatAction.Data>
    {
        // This method is called every frame while the action is running
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            // Instead of using a timer, we can use the Wait ActionRunState.
            // The system will wait for the specified time before completing the action
            // Whilst waiting, the Perform method won't be called again
            return ActionRunState.WaitThenComplete(5f);
        }

        // This method is called when the action is completed
        public override void Complete(IMonoAgent agent, Data data)
        {
            data.DataBehaviour.appleCount--;
            data.DataBehaviour.hunger = 0f;
        }

        // The action class itself must be stateless!
        // All data should be stored in the data class
        public class Data : IActionData
        {
            public ITarget Target { get; set; }

            // When using the GetComponent attribute, the system will automatically inject the reference
            [GetComponent]
            public DataBehaviour DataBehaviour { get; set; }
        }
    }
}