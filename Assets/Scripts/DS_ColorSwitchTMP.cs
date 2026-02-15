using UnityEngine;
using TMPro;

public class DS_ColorSwitchTMP : MonoBehaviour
{
    public enum Mode { Gradient, Rainbow }

    [Header("Target")]
    public TMP_Text targetText;

    [Header("Settings")]
    public Mode cycleMode = Mode.Gradient;
    [Range(0.1f, 10f)] public float cycleSpeed = 1f;
    public Gradient customGradient;

    private float t;

    void Update()
    {
        if (targetText == null) return;

        t += Time.deltaTime * cycleSpeed;
        Color color = cycleMode == Mode.Rainbow ? RainbowColor(t) : customGradient.Evaluate(Mathf.PingPong(t, 1f));
        targetText.color = color;
    }

    Color RainbowColor(float time)
    {
        float r = Mathf.Sin(time + 0f) * 0.5f + 0.5f;
        float g = Mathf.Sin(time + 2f) * 0.5f + 0.5f;
        float b = Mathf.Sin(time + 4f) * 0.5f + 0.5f;
        return new Color(r, g, b);
    }
}