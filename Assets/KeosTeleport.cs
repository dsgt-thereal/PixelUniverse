using UnityEngine;

public class KeosTeleport : MonoBehaviour
{
    [Header("This was made by Keo.CS")]
    public GameObject gorillaPlayer;
    public Transform teleportPoint;
    public string handTag = "HandTag";
    public float disableDuration = 0.5f;
    public float gravityDisableTime = 1f;

    Rigidbody gorillaRigidbody;
    Collider[] allColliders;

    void Start()
    {
        gorillaRigidbody = gorillaPlayer.GetComponent<Rigidbody>();
        allColliders = FindObjectsOfType<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(handTag))
        {
            foreach (var collider in allColliders)
            {
                collider.enabled = false;
            }

            TeleportGorilla();
        }
    }

    void TeleportGorilla()
    {
        gorillaRigidbody.useGravity = false;
        Invoke(nameof(EnableGravity), gravityDisableTime);

        gorillaPlayer.transform.position = teleportPoint.position;

        Invoke(nameof(EnableColliders), disableDuration);
    }

    void EnableGravity()
    {
        gorillaRigidbody.useGravity = true;
    }

    void EnableColliders()
    {
        foreach (var collider in allColliders)
        {
            collider.enabled = true;
        }
    }
}