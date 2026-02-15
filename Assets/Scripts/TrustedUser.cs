using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrustedUser : MonoBehaviour
{
    [Header("Make sure this script is never disabled, otherwise the timer won't go down!")]
    [SerializeField] private GameObject trustedDisable;
    [SerializeField] private TextMeshPro trustedText;
    private int time = 10000; //or however long you want it to be

    void Start()
    {
        int savedTime = PlayerPrefs.GetInt("TrustedTime");
        if (savedTime < 0)
        {
            time = 0;
        }
        else if (savedTime > 0)
        {
            time = savedTime;
        }
        InvokeRepeating("DoTimeMinus", 1f, 1f);
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("TrustedTime", time);
    }

    void DoTimeMinus()
    {
        if (time <= 0)
        {
            trustedText.text = "Trusted";
            trustedDisable.SetActive(false);
            time = -1;
        }
        else
        {
            time--;
            trustedText.text = "" + time.ToString();
        }
    }
    
}
