using System.Collections.Generic;
using WOTR.Game;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace WOTR.Game
{
    [GoapId("WatermelonSensor-83b21de9-f3a2-4a41-aeb7-0a2c52b3a9fd")]
    public class WatermelonSensor : MultiSensorBase
    {
        // Cache dari semua Watermelon di dunia (tidak dipakai langsung karena pakai SensorCollider)
        //private WatermelonBehaviour[] watermelons;

        public WatermelonSensor()
        {
            // Bisa tambahkan sensor lain jika diperlukan seperti jumlah inventory, dll.

            this.AddLocalTargetSensor<ClosestWatermelon>((agent, references, target) =>
            {
                var sensor = references.GetCachedComponent<SensorCollider>();
                if (sensor == null || sensor.watermelonsInRange.Count == 0)
                    return null;

                var closestWatermelon = this.Closest(sensor.watermelonsInRange, agent.Transform.position);
                if (closestWatermelon == null)
                    return null;

                if (target is TransformTarget transformTarget)
                    return transformTarget.SetTransform(closestWatermelon.transform);

                return new TransformTarget(closestWatermelon.transform);
            });
        }

        public override void Created() { }

        public override void Update()
        {
            // SensorCollider sudah bekerja via trigger, jadi tidak perlu update manual
        }

        private T Closest<T>(IEnumerable<T> list, Vector3 position)
        where T : MonoBehaviour
        {
            T closest = null;
            float closestDistance = float.MaxValue;

            Vector2 origin = new Vector2(position.x, position.y);

            foreach (var item in list)
            {
                Vector2 itemPos = new Vector2(item.transform.position.x, item.transform.position.y);
                float distance = Vector2.Distance(itemPos, origin);

                if (distance < closestDistance)
                {
                    closest = item;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
