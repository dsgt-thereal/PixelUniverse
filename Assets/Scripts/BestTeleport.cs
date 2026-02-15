using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestTeleport : MonoBehaviour
{
    [Header("Credits: TheCoder")]
    public GameObject gorillaPlayer;

    public GameObject teleportLocation;

    public string rHand = "HandTag";
    public string lHand = "HandTag";
    public string body = "Body";

    private List<Collider> worldColliders = new List<Collider>();
    private Rigidbody gorillaRB;

    void Start()
    {
        gorillaRB = gorillaPlayer.GetComponent<Rigidbody>();

        Collider[] all = FindObjectsOfType<Collider>();
        foreach (Collider col in all)
        {
            if (!col.transform.IsChildOf(gorillaPlayer.transform))
                worldColliders.Add(col);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(rHand) || other.CompareTag(lHand) || other.CompareTag(body))
        {
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        gorillaRB.isKinematic = true;

        for (int i = 0; i < worldColliders.Count; i++)
            worldColliders[i].enabled = false;

        gorillaPlayer.transform.position = teleportLocation.transform.position;

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < worldColliders.Count; i++)
            worldColliders[i].enabled = true;

        gorillaRB.isKinematic = false;
    }
}