using UnityEngine;
using System.Collections.Generic;

public class DS_Dice : MonoBehaviour
{
    [System.Serializable]
    public class RNGEntry
    {
        [Header("🎯 Drop Settings")]
        public string title = "Drop Title";

        [Tooltip("Chance out of 100 that this drop will succeed (0 = never, 100 = always)")]
        [Range(0f, 100f)]
        public float rarityChance = 50f;

        [Tooltip("GameObjects to enable if this drop succeeds")]
        public List<GameObject> objectsToEnable = new List<GameObject>();
    }

    [Header("🎲 RNG Entries")]
    public List<RNGEntry> rngEntries = new List<RNGEntry>();

    [Header("🟦 Trigger Cube")]
    public GameObject triggerCube;

    [Header("🖐️ XR Hand Tags")]
    public string leftHandTag = "LeftHandController";
    public string rightHandTag = "RightHandController";

    [Header("🧪 Test Mode")]
    public bool testMode = false;

    private void Start()
    {
        if (triggerCube != null)
        {
            var collider = triggerCube.GetComponent<Collider>();
            if (collider == null)
            {
                triggerCube.AddComponent<BoxCollider>();
                Debug.LogWarning("TriggerCube had no collider — added BoxCollider automatically.");
            }

            if (!collider.isTrigger)
                collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(leftHandTag) || other.CompareTag(rightHandTag))
        {
            RunRNG();
        }
    }

    [ContextMenu("Run Test RNG")]
    void RunTestRNG()
    {
        testMode = true;
        RunRNG();
        testMode = false;
    }

    void RunRNG()
    {
        // Disable all GameObjects first
        foreach (var entry in rngEntries)
        {
            foreach (var obj in entry.objectsToEnable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        RNGEntry winningEntry = null;
        float highestRarity = -1f;

        foreach (var entry in rngEntries)
        {
            float roll = Random.value * 100f;
            bool success = roll < entry.rarityChance;

            Debug.Log($"🎯 {entry.title} | Roll: {roll:F2} | Success: {success}");

            if (success && entry.rarityChance > highestRarity)
            {
                winningEntry = entry;
                highestRarity = entry.rarityChance;
            }
        }

        // Fallback to "Common" if nothing succeeded
        if (winningEntry == null)
        {
            winningEntry = rngEntries.Find(e => e.title.ToLower().Contains("common"));
            Debug.Log("❌ No drops succeeded — defaulting to Common.");
        }

        if (winningEntry != null)
        {
            Debug.Log($"🏆 Final Drop: {winningEntry.title}");
            foreach (var obj in winningEntry.objectsToEnable)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}