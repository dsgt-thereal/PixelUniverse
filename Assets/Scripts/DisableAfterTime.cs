using UnityEngine;
using System.Collections;

public class DisableAfterTime : MonoBehaviour
{
    public float timeToDisable = 4.5f;
    public GameObject objectToDisable;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timeToDisable);
        objectToDisable.SetActive(false);
    }
}
