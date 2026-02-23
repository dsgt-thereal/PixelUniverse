using UnityEngine;
using TMPro;

public class ClickerCounter : MonoBehaviour
{
    public TextMeshPro textMesh;
    public ParticleSystem[] particleEffects;
    public AudioSource audioSource;
    private int touchCount = 0;

    private const string TouchCountKey = "TouchCount";

    private void Start()
    {
        touchCount = PlayerPrefs.GetInt(TouchCountKey, 0);
        UpdateCounterText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            touchCount++;
            UpdateCounterText();

            if (touchCount % 100 == 0)
            {
                PlayEffects();
            }

            PlayerPrefs.SetInt(TouchCountKey, touchCount);
            PlayerPrefs.Save();
        }
    }

    private void UpdateCounterText()
    {
        if (textMesh != null)
        {
            textMesh.text = touchCount.ToString();
        }
    }

    private void PlayEffects()
    {
        foreach (ParticleSystem effect in particleEffects)
        {
            if (effect != null)
            {
                effect.Play();
            }
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
