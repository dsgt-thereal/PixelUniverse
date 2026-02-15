using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class MotdPasteBin : MonoBehaviour
{
    [Header("Its kinda forward its a simple setup")]
    [Header("Give credz to cl1pz cause this script was kinda confusing to make")]
    public string[] pastebinLinks;
    public TextMeshPro textBox;
    string oldText = "";

    void Start()
    {
        StartCoroutine(LoopMessages());
    }

    IEnumerator LoopMessages()
    {
        while (true)
        {
            for (int i = 0; i < pastebinLinks.Length; i++)
            {
                UnityWebRequest req = UnityWebRequest.Get(pastebinLinks[i]);
                yield return req.SendWebRequest();

                if (!req.isNetworkError && !req.isHttpError)
                {
                    string newText = req.downloadHandler.text;
                    if (newText != oldText)
                    {
                        yield return StartCoroutine(TypeText(newText));
                        oldText = newText;
                    }
                    else
                    {
                        textBox.text = newText;
                    }
                }

                yield return new WaitForSeconds(2f);
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        textBox.text = "";
        foreach (char c in text)
        {
            textBox.text += c;
            yield return new WaitForSeconds(0.04f);
        }
    }
}