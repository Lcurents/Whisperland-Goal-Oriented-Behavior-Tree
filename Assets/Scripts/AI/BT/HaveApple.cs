using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using WOTR.Game;
public class HaveApple : Conditional
{
    private DataBehaviour data;
    
	public override TaskStatus OnUpdate()
	{
		data = GetComponent<DataBehaviour>();
		if(this.data.appleCount > 0 && this.data.hunger >=50)
		{
			return TaskStatus.Success;
		}
        return TaskStatus.Failure;
    }
}