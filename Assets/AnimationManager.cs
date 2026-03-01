using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.Globalization;
using System;
using TMPro;
using Photon.VR;

public class AnimationManager : MonoBehaviour
{
    [Header("COSMETICS")]
    public static AnimationManager instance;
    public string MyPlayFabID;
    public string CatalogName;
    public List<GameObject> specialitems;
    public List<GameObject> disableitems;

    [Header("CURRENCY")]
    public string CurrencyName;
    public TextMeshPro currencyText;
    public int coins;

    [Header("BAN STUFF")]
    public GameObject[] StuffToDisable;
    public GameObject[] StuffToEnable;
    public MeshRenderer[] StuffToMaterialChange;
    public Material MaterialToChangeToo;
    public TextMeshPro[] BanTimes;
    public TextMeshPro[] BanReasons;

    [Header("TITLE DATA")]
    public TextMeshPro MOTDText;

    [Header("PLAYER DATA")]
    public TextMeshPro UserName;
    public string StartingUsername;
    public string Name;
    [SerializeField]
    public bool UpdateName;

    [Header("DON'T DESTROY ON LOAD")]
    public GameObject[] DDOLObjects;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        login();
    }

    public void login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("logging in");
        GetAccountInfoRequest InfoRequest = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(InfoRequest, AccountInfoSuccess, OnError);
        StartCoroutine(DDOLStuff());
        GetVirtualCurrencies();
        GetMOTD();
        CheckDailyReward(); // 👈 Daily reward check
    }

    public void AccountInfoSuccess(GetAccountInfoResult result)
    {
        MyPlayFabID = result.AccountInfo.PlayFabId;

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        (result) =>
        {
            foreach (var item in result.Inventory)
            {
                if (item.CatalogVersion == CatalogName)
                {
                    for (int i = 0; i < specialitems.Count; i++)
                    {
                        if (specialitems[i].name == item.ItemId)
                        {
                            specialitems[i].SetActive(true);
                        }
                    }
                    for (int i = 0; i < disableitems.Count; i++)
                    {
                        if (disableitems[i].name == item.ItemId)
                        {
                            disableitems[i].SetActive(false);
                        }
                    }
                }
            }
        },
        (error) =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    async void Update()
    {
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }

    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        if (result.VirtualCurrency != null && result.VirtualCurrency.ContainsKey("TK"))
        {
            coins = result.VirtualCurrency["TK"];
            currencyText.text = "YOU CURRENTLY HAVE:" + coins.ToString();
        }
        else
        {
            coins = 0;
            currencyText.text = "0" + CurrencyName;
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public void CheckDailyReward()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("LastDailyClaim"))
            {
                DateTime lastClaim = DateTime.Parse(result.Data["LastDailyClaim"].Value, null, DateTimeStyles.RoundtripKind);
                TimeSpan timeSince = DateTime.UtcNow - lastClaim;

                if (timeSince.TotalHours >= 24)
                {
                    GrantDailyCurrency();
                }
                else
                {
                    Debug.Log("Daily reward not yet available.");
                }
            }
            else
            {
                GrantDailyCurrency(); // First-time claim
            }
        }, OnError);
    }

    void GrantDailyCurrency()
    {
        var grantRequest = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "TK",
            Amount = 100 // 💰 Customize your reward amount
        };
        PlayFabClientAPI.AddUserVirtualCurrency(grantRequest, result =>
        {
            Debug.Log("Daily currency granted.");
            SaveClaimTime();
            GetVirtualCurrencies(); // Refresh UI
        }, OnError);
    }

    void SaveClaimTime()
    {
        var updateRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "LastDailyClaim", DateTime.UtcNow.ToString("o") }
            }
        };
        PlayFabClientAPI.UpdateUserData(updateRequest, result =>
        {
            Debug.Log("Claim time saved.");
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            PhotonVRManager.Manager.Disconnect();
            foreach (GameObject obj in StuffToDisable)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in StuffToEnable)
            {
                obj.SetActive(true);
            }
            foreach (MeshRenderer rend in StuffToMaterialChange)
            {
                rend.material = MaterialToChangeToo;
            }
            foreach (var item in error.ErrorDetails)
            {
                foreach (TextMeshPro BanTime in BanTimes)
                {
                    if (item.Value[0] == "Indefinite")
                    {
                        BanTime.text = "Permanent Ban";
                    }
                    else
                    {
                        string playFabTime = item.Value[0];
                        DateTime unityTime;
                        try
                        {
                            unityTime = DateTime.ParseExact(playFabTime, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                            TimeSpan timeLeft = unityTime.Subtract(DateTime.UtcNow);
                            int hoursLeft = (int)timeLeft.TotalHours;
                            BanTime.text = string.Format("Hours Left: {0}", hoursLeft);
                        }
                        catch (FormatException ex)
                        {
                            Debug.LogErrorFormat("Failed to parse PlayFab time '{0}': {1}", playFabTime, ex.Message);
                        }
                    }
                }
                foreach (TextMeshPro BanReason in BanReasons)
                {
                    BanReason.text = string.Format("Reason: {0}", item.Key);
                }
            }
        }
        else
        {
            login();
        }
    }

    public void GetMOTD()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), MOTDGot, OnError);
    }

    public void MOTDGot(GetTitleDataResult result)
    {
        if (result.Data == null || result.Data.ContainsKey("MOTD") == false)
        {
            Debug.Log("No MOTD");
            return;
        }
        MOTDText.text = result.Data["MOTD"];
    }

    IEnumerator DDOLStuff()
    {
        Scene scene = SceneManager.GetActiveScene();
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject Obj in DDOLObjects)
        {
            DontDestroyOnLoad(Obj);
        }
    }
}