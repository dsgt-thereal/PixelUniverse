using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
public class ChangeCosmetic : MonoBehaviour
{
    public enum CosmeticType
    {
        Head,
        Face,
        Body,
        LeftHand,
        RightHand,
        EntityHead,
        EntityBody,
        EntityLeft,
        EntityRight,
        Tag,
        Ears,
        Audio,
        PVP,
        Hair
    }

    public CosmeticType type;
    public string name;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic(type.ToString(), name);
        }
    }
}