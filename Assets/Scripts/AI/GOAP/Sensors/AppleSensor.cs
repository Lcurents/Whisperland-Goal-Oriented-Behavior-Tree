using System.Collections.Generic;
using WOTR.Game;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace WOTR.Game
{
    // Defining a GoapId is only necessary when using the ScriptableObject configuration method.
    [GoapId("AppleSensor-d68c875d-29c0-43f3-9d79-054d4cc6505d")]
    public class AppleSensor : MultiSensorBase
    {
        // A cache of all the pears in the world
        private AppleBehaviour[] apple;

        // You must use the constructor to register all the sensors
        // This can also be called outside of the gameplay loop to validate the configuration
        public AppleSensor()
        {
            this.AddLocalWorldSensor<AppleCount>((agent, references) =>
            {
                // Get a cached reference to the DataBehaviour on the agent
                var data = references.GetCachedComponent<DataBehaviour>();
                //Debug.Log($"[Sensor] appleCount = {data.appleCount}");
                return data.appleCount;
            });

            this.AddLocalWorldSensor<Hunger>((agent, references) =>
            {
                // Get a cached reference to the DataBehaviour on the agent
                var data = references.GetCachedComponent<DataBehaviour>();
                //Debug.Log($"[Sensor] Hunger = {data.hunger}");
                // We need to cast the float to an int, because the hunger is an int
                // We will lose the decimal values, but we don't need them for this example
                return (int)data.hunger;
            });

            this.AddLocalTargetSensor<ClosestApple>((agent, references, target) =>
            {
                var sensor = references.GetCachedComponent<SensorCollider>();
                if (sensor == null || sensor.applesInRange.Count == 0)
                    return null;

                var closestApple = this.Closest(sensor.applesInRange, agent.Transform.position);
                if (closestApple == null)
                    return null;

                if (target is TransformTarget transformTarget)
                    return transformTarget.SetTransform(closestApple.transform);

                return new TransformTarget(closestApple.transform);
            });
        }

        // The Created method is called when the sensor is created
        // This can be used to gather references to objects in the scene
        public override void Created() { }

        // This method is equal to the Update method of a local sensor.
        // It can be used to cache data, like gathering a list of all pears in the scene.
        public override void Update()
        {
            // Tidak digunakan karena sensor sekarang menggunakan data dari SensorCollider (trigger-based)
        }


        // Returns the closest item in a list
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