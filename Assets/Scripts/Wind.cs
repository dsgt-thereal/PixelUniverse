using UnityEngine;

public class Wind : MonoBehaviour
{
    [Header("SCRIPT BY LUCKMONK. DO NOT CLAIM AS OWN")]
 
    [Header("PLAYER")]
    public Rigidbody gorillaPlayer;
    
    [Header("FORCES")]
    public int xForce;
    public int yForce;
    public int zForce;

    bool Active;

    void OnTriggerEnter()
    {
        Active = true;
    }

    void OnTriggerExit()
    {
        Active = false;
    }

    void Update()
    {
        if (Active)
        {
            gorillaPlayer.AddForce(new Vector3(xForce, yForce, zForce));
        }
    }
}