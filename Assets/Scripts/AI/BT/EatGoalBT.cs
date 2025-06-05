using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using System.Linq;
using WOTR.Game;


public class EatGoalBT : Action
{
    private DataBehaviour data;
    private GoapActionProvider provider;

    public override void OnStart()
    {
        provider = GetComponent<GoapActionProvider>();
        data = GetComponent<DataBehaviour>();

        if (provider == null)
        {
            Debug.LogError("GoapActionProvider not found!");
            return;
        }

        if (data != null)
            data.goalEatAppleCompleted = false;

        provider.Events.OnGoalCompleted += OnGoalCompleted;

        provider.RequestGoal<EatGoal>();
    }

    public override TaskStatus OnUpdate()
    {
        if (data == null)
            return TaskStatus.Failure;

        if (data.goalEatAppleCompleted)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    private void OnGoalCompleted(IGoal goal)
    {
        if (goal is EatGoal)
        {
            data.goalEatAppleCompleted = true;
        }
    }


    public override void OnEnd()
    {
        if (provider != null)
        {
            provider.Events.OnGoalCompleted -= OnGoalCompleted;
        }
    }
}
