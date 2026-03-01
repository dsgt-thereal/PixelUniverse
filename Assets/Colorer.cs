using UnityEngine;
using Photon.VR;

public class Colorer : MonoBehaviour
{
    public Color YourColor;

    const string SaveKey = "PlayerColor_HEX";

    void Start()
    {
        // Load saved HEX color
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string hex = PlayerPrefs.GetString(SaveKey);

            Color loadedColor;
            if (ColorUtility.TryParseHtmlString(hex, out loadedColor))
            {
                YourColor = loadedColor;

                // Apply to PhotonVR
                PhotonVRManager.SetColour(loadedColor);
            }
        }
    }

    void OnTriggerEnter()
    {
        // Apply the new color
        PhotonVRManager.SetColour(YourColor);

        // Convert to HEX and save
        string hex = "#" + ColorUtility.ToHtmlStringRGB(YourColor);
        PlayerPrefs.SetString(SaveKey, hex);
        PlayerPrefs.Save();
    }
}