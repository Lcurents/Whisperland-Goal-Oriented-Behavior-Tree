using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using WOTR.Game; // Pastikan ini namespace di mana SensorCollider berada

public class AppleSensorBT : Conditional
{
    private SensorCollider sensor;

    public override void OnStart()
    {
        // Cache komponen saat mulai
        sensor = GetComponent<SensorCollider>();
    }

    public override TaskStatus OnUpdate()
    {
        if (sensor == null)
        {
            Debug.LogWarning("[AppleSensorBT] SensorCollider tidak ditemukan.");
            return TaskStatus.Failure;
        }

        if (sensor.applesInRange.Count > 0)
        {
            // Ada apel dalam radius sensor collider
            return TaskStatus.Success;
        }

        // Tidak ada apel di sekitar
        return TaskStatus.Failure;
    }
}
