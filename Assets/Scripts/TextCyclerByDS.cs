using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextCyclerByDS : MonoBehaviour
{
    [System.Serializable]
    public class TextEntry
    {
        [TextArea]
        public string text;
        public float displayDuration = 2f;
    }

    [Header("Text Settings")]
    public List<TextEntry> textEntries = new List<TextEntry>();
    public bool loop = true;

    [Header("Fade Settings")]
    public bool useFade = true;
    public float fadeDuration = 0.5f;

    private TMP_Text tmpText;
    private Coroutine cycleCoroutine;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        tmpText.alpha = useFade ? 0f : 1f;
    }

    void OnEnable()
    {
        if (textEntries.Count > 0)
            cycleCoroutine = StartCoroutine(CycleText());
    }

    void OnDisable()
    {
        if (cycleCoroutine != null)
            StopCoroutine(cycleCoroutine);
    }

    IEnumerator CycleText()
    {
        int index = 0;

        while (true)
        {
            TextEntry entry = textEntries[index];

            if (useFade)
            {
                // Fade out current text
                yield return StartCoroutine(FadeText(tmpText.alpha, 0f));
            }

            // Switch text at the climax of fade-out
            tmpText.text = entry.text;

            if (useFade)
            {
                // Fade in new text
                yield return StartCoroutine(FadeText(0f, 1f));
            }

            // Wait while fully visible
            yield return new WaitForSeconds(entry.displayDuration);

            index++;

            if (index >= textEntries.Count)
            {
                if (loop)
                    index = 0;
                else
                    break;
            }
        }
    }

    IEnumerator FadeText(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            tmpText.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        tmpText.alpha = to;
    }
}

