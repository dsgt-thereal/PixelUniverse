using UnityEngine;
using TMPro;
using System;

public class LocalTimeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro timeText;

    void Start()
    {
        InvokeRepeating(nameof(UpdateTime), 0f, 1f);
    }

    void UpdateTime()
    {
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("h:mm tt"); // e.g., 8:30 PM
        timeText.text = $"🕒 {formattedTime}";
    }
}