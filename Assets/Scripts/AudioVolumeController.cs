using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class LayerTriggeredAudioVolume : MonoBehaviour
{
    [Header("Target Audio Source")]
    public AudioSource targetAudioSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float targetVolume = 1f;

    [Header("Trigger Layers")]
    public List<string> triggerLayerNames = new List<string> { "LeftHand", "RightHand" };

    private HashSet<int> triggerLayerIndices;

    private void Awake()
    {
        triggerLayerIndices = new HashSet<int>();
        foreach (string layerName in triggerLayerNames)
        {
            int layerIndex = LayerMask.NameToLayer(layerName);
            if (layerIndex != -1)
            {
                triggerLayerIndices.Add(layerIndex);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' not found. Check your project settings.");
            }
        }

        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning("Collider is not set as trigger. Setting it now.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerLayerIndices.Contains(other.gameObject.layer))
        {
            ApplyVolume();
        }
    }

    public void ApplyVolume()
    {
        if (targetAudioSource != null)
        {
            targetAudioSource.volume = targetVolume;
            Debug.Log($"Volume set to {targetVolume} on {targetAudioSource.name}");
        }
        else
        {
            Debug.LogWarning("No AudioSource assigned.");
        }
    }

    public void SetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }
}