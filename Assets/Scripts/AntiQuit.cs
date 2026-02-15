using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiQuit : MonoBehaviour
{
    void Update()
    {
        Application.CancelQuit();
    }
}
