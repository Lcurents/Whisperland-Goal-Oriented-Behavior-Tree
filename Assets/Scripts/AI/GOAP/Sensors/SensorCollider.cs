using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WOTR.Game;

public class SensorCollider : MonoBehaviour
{
    public List<AppleBehaviour> applesInRange = new List<AppleBehaviour>();
    public List<WatermelonBehaviour> watermelonsInRange = new List<WatermelonBehaviour>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        var apple = other.GetComponent<AppleBehaviour>();
        var watermelon = other.GetComponent<WatermelonBehaviour>();
        if (apple != null && !applesInRange.Contains(apple))
        {
            applesInRange.Add(apple);
        }
        if (watermelon != null && !watermelonsInRange.Contains(watermelon))
        {
            watermelonsInRange.Add(watermelon);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var apple = other.GetComponent<AppleBehaviour>();
        var watermelon = other.GetComponent<WatermelonBehaviour>();
        if (apple != null && applesInRange.Contains(apple))
        {
            applesInRange.Remove(apple);
        }
        if (watermelon != null && !watermelonsInRange.Contains(watermelon))
        {
            watermelonsInRange.Remove(watermelon);
        }
    }
}
