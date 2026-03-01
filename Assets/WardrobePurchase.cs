using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class WardrobePurchase : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("COSMETICS")]
    public GameObject Purchasable;
    public GameObject WardrobePart;

    [Header("BUY")]
    public string CosmeticName;
    public int coinsPrice;
    public AnimationManager playfablogin;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HandTag")
        {
            if (playfablogin.coins >= coinsPrice)
            {
                if (PlayerPrefs.GetInt(CosmeticName) != 1)
                {
                    PlayerPrefs.SetInt(CosmeticName, 1);
                    BuyItem();
                }
                if (PlayerPrefs.GetInt(CosmeticName) == 1)
                {
                    Purchasable.SetActive(true);
                    WardrobePart.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }

    }



    public void BuyItem()
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "TK",
            Amount = coinsPrice
        };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCoinsSuccess, OnError);
    }

    void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Bought item! " + CosmeticName);
        AnimationManager.instance.GetVirtualCurrencies();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(CosmeticName) == 1)
        {
            Purchasable.SetActive(true);
            WardrobePart.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}