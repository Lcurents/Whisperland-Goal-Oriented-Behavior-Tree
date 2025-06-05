using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

// Namespaces CrashKonijn GOAP V3 (tetap diperlukan untuk GoapActionProvider, IGoal, dll.)
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

// Namespace WOTR.Game Anda, tempat DataBehaviour dan SensorCollider berada
using WOTR.Game;
using System;

[TaskCategory("GOAP")]
[TaskDescription("A Behavior Designer action that selects the best GOAP goal based on utility " +
                 "by directly accessing agent components, and sets a Shared Variable for BT flow.")]
public class MyPlannerNode : BehaviorDesigner.Runtime.Tasks.Action
{
    // --- Referensi ke GoapActionProvider CrashKonijn ---
    // Variabel ini HARUS diisi di Behavior Designer Inspector
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that contains the CrashKonijn GoapActionProvider component.")]
    public SharedGameObject goapAgentGameObject;
    private GoapActionProvider goapProvider;

    // --- Referensi Langsung ke Komponen Data ---
    // PlannerNode akan mengakses ini secara langsung untuk mendapatkan data utilitas.
    // Ini akan diisi di OnAwake.
    public DataBehaviour dataBehaviour;
    public SensorCollider sensorCollider;


    // --- Shared Variable UNTUK OUTPUT Goal yang Dipilih ---
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The Shared Variable that will store the selected GOAP goal type as a string (e.g., 'PickupAppleGoal', 'IdleGoal').")]
    public SharedString selectedGoapGoalName;

    public override void OnAwake()
    {
        if (goapAgentGameObject != null && goapAgentGameObject.Value != null)
        {
            goapProvider = goapAgentGameObject.Value.GetComponent<GoapActionProvider>();
            // Dapatkan referensi komponen data langsung dari GameObject agen yang ditunjuk
            dataBehaviour = goapAgentGameObject.Value.GetComponent<DataBehaviour>();
            sensorCollider = goapAgentGameObject.Value.GetComponent<SensorCollider>();
        }

        if (goapProvider == null)
        {
            Debug.LogError($"PlannerNode: CrashKonijn GoapActionProvider component not found or not set on the specified GameObject ({goapAgentGameObject?.Value?.name ?? "null"}). " +
                           "Please ensure the 'Goap Agent Game Object' variable is correctly assigned in the Behavior Designer Inspector " +
                           "and the GameObject has a GoapActionProvider script attached.", goapAgentGameObject?.Value);
        }
        else
        {
            Debug.Log("PlannerNode: CrashKonijn GoapActionProvider successfully initialized.");
        }

        // Pastikan komponen data juga ditemukan di OnAwake
        if (dataBehaviour == null)
        {
            Debug.LogError($"PlannerNode: DataBehaviour component not found on the specified GameObject ({goapAgentGameObject?.Value?.name ?? "null"}). " +
                           "Please ensure the GameObject has a DataBehaviour script attached.", goapAgentGameObject?.Value);
        }
        if (sensorCollider == null)
        {
            Debug.LogError($"PlannerNode: SensorCollider component not found on the specified GameObject ({goapAgentGameObject?.Value?.name ?? "null"}). " +
                           "Please ensure the GameObject has a SensorCollider script attached.", goapAgentGameObject?.Value);
        }
    }

    public override TaskStatus OnUpdate()
    {
        // Pastikan semua komponen penting ditemukan
        if (goapProvider == null || dataBehaviour == null || sensorCollider == null)
        {
            Debug.LogError("PlannerNode: Required components (GoapActionProvider, DataBehaviour, SensorCollider) are null. Cannot proceed.", Owner.gameObject);
            return TaskStatus.Failure;
        }

        // --- Hitung Utility dengan membaca langsung dari komponen ---
        float utilityPickupApple = CalculateUtilityPickupApple();
        float utilityIdle = CalculateUtilityIdle();
        float utilityEat = CalculateUtilityEat();

        Debug.Log($"PlannerNode: Current Utilities - PickupApple={utilityPickupApple},Eat={utilityEat}, Idle={utilityIdle}");

        string bestGoalName = "IdleGoal"; // Default
        float maxUtility = utilityIdle; // Default

        if (utilityEat > maxUtility) // Gunakan '>' untuk memprioritaskan EatGoal secara ketat jika utilitasnya lebih tinggi
        {
            maxUtility = utilityEat;
            bestGoalName = "EatGoal";
        }

        // 2. Prioritaskan PickupAppleGoal (jika utilitasnya lebih tinggi dari yang sudah terpilih)
        // Penting: Gunakan '>' untuk memberikan prioritas pada PickupAppleGoal jika utilitasnya LEBIH TINGGI dari maxUtility saat ini.
        // Jika Anda ingin PickupAppleGoal dipilih jika utilitasnya SAMA dengan goal yang sudah terpilih (misal, Idle),
        // maka gunakan '>='. Namun, prioritas ketat (>) lebih umum setelah prioritas tinggi.
        if (utilityPickupApple > maxUtility)
        {
            maxUtility = utilityPickupApple;
            bestGoalName = "PickupAppleGoal";
        }

        selectedGoapGoalName.Value = bestGoalName;
        Debug.Log($"PlannerNode: Selected best goal: {bestGoalName} with utility: {maxUtility}");

        return TaskStatus.Success;
    }

    private float CalculateUtilityEat()
    {
        bool isHungry = dataBehaviour.hunger >= 70;
        bool hasAppleInInventory = dataBehaviour.appleCount > 0;

        // Debugging:
        Debug.Log($"   [Utility Debug] Eat - Hungry:{isHungry}, HasAppleInv:{hasAppleInInventory}");

        

        if (isHungry && hasAppleInInventory)
        {
            return 1.0f;
        }
        else if (!isHungry && !hasAppleInInventory)
        {
            return 0.0f;
        }
        return 0.0f;
    }

    // --- Fungsi Perhitungan Utility (Mengakses Komponen Langsung) ---
    private float CalculateUtilityPickupApple()
    {
        // Langsung akses properti dari DataBehaviour dan SensorCollider yang sudah di-cache
        bool isHungry = dataBehaviour.hunger >= 30;
        bool hasAppleInInventory = dataBehaviour.appleCount > 2;
        bool hasAppleInWorld = sensorCollider.applesInRange.Count > 0; // Menggunakan list dari SensorCollider

        // Debugging:
        Debug.Log($"   [Utility Debug] PickupApple - Hungry:{isHungry}, HasAppleInv:{hasAppleInInventory}, ApplesInWorld:{hasAppleInWorld}");

        // --- PERBAIKAN: Jika tidak ada apel di dunia, utilitas otomatis 0 ---
        if (!hasAppleInWorld)
        {
            return 0.0f; // Tidak ada apel di dunia, tidak mungkin mengambil apel
        }

        if (isHungry && !hasAppleInInventory && hasAppleInWorld)
        {
            return 1.0f;
        }
        else if (!isHungry && !hasAppleInInventory && hasAppleInWorld)
        {
            return 0.5f;
        }
        return 0.0f;
    }

    private float CalculateUtilityIdle()
    {
        // Langsung akses properti dari DataBehaviour yang sudah di-cache
        bool isHungry = dataBehaviour.hunger >= 50;
        bool hasAppleInInventory = dataBehaviour.appleCount > 0;

        bool enemyNearby = false; // Placeholder jika tidak ada logika deteksi musuh

        // Debugging:
        Debug.Log($"   [Utility Debug] Idle - Hungry:{isHungry}, HasAppleInv:{hasAppleInInventory}, Enemy:{enemyNearby}");

        if (!isHungry && hasAppleInInventory && !enemyNearby)
        {
            return 1.0f;
        }
        else if ((isHungry || !hasAppleInInventory) && !enemyNearby)
        {
            return 0.5f;
        }
        return 0.0f;
    }
}