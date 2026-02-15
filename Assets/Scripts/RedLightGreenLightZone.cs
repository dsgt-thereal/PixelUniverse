using UnityEngine;
using System.Collections;

public class RedLightGreenLightController : MonoBehaviour
{
    public enum Stage { Green, Yellow, Red }

    [Header("Gorilla Player Settings")]
    public Rigidbody gorillaPlayer;
    public float movementThreshold = 0.05f; // Lower sensitivity
    public float movementCheckInterval = 0.2f;

    [Header("Stage Timing")]
    public bool randomizeGreenTime = false;
    public Vector2 greenTimeRange = new Vector2(3f, 6f);
    public float fixedGreenTime = 5f;

    public bool randomizeYellowTime = false;
    public Vector2 yellowTimeRange = new Vector2(2f, 4f);
    public float fixedYellowTime = 3f;

    public bool randomizeRedTime = true;
    public Vector2 redTimeRange = new Vector2(2f, 5f);
    public float fixedRedTime = 4f;

    [Header("Stage Objects")]
    public GameObject[] greenStageObjects;
    public GameObject[] yellowStageObjects;
    public GameObject[] redStageObjects;
    public GameObject[] punishmentObjects;

    [Header("Zone Settings")]
    public Collider activationZone;

    [Header("Death Settings")]
    public GameObject[] objectsToEnableOnDeath;
    public GameObject[] objectsToDisableOnDeath;
    public bool hasDied = false;

    private Stage currentStage = Stage.Green;
    private Vector3 lastPosition;
    private bool isPlayerInside = false;
    private Coroutine stageCycleCoroutine;
    private Coroutine movementMonitorCoroutine;

    void OnEnable()
    {
        Debug.Log("RedLightGreenLightController enabled");
        ResetDeath(); // Ensures everything restarts properly
    }

    void Update()
    {
        if (activationZone.bounds.Contains(gorillaPlayer.position))
        {
            if (!isPlayerInside)
            {
                isPlayerInside = true;
                lastPosition = gorillaPlayer.position;

                if (movementMonitorCoroutine == null && !hasDied)
                {
                    Debug.Log("Player entered zone — starting movement monitor");
                    movementMonitorCoroutine = StartCoroutine(MovementMonitor());
                }
            }
        }
        else
        {
            if (isPlayerInside)
            {
                isPlayerInside = false;

                if (movementMonitorCoroutine != null)
                {
                    Debug.Log("Player exited zone — stopping movement monitor");
                    StopCoroutine(movementMonitorCoroutine);
                    movementMonitorCoroutine = null;
                }
            }
        }
    }

    IEnumerator StageCycle()
    {
        Debug.Log("StageCycle started");

        while (!hasDied)
        {
            SetStage(Stage.Green);
            yield return new WaitForSeconds(GetStageDuration(Stage.Green));

            SetStage(Stage.Yellow);
            yield return new WaitForSeconds(GetStageDuration(Stage.Yellow));

            SetStage(Stage.Red);
            yield return new WaitForSeconds(GetStageDuration(Stage.Red));
        }

        Debug.Log("StageCycle ended due to death");
    }

    float GetStageDuration(Stage stage)
    {
        switch (stage)
        {
            case Stage.Green:
                return randomizeGreenTime ? Random.Range(greenTimeRange.x, greenTimeRange.y) : fixedGreenTime;
            case Stage.Yellow:
                return randomizeYellowTime ? Random.Range(yellowTimeRange.x, yellowTimeRange.y) : fixedYellowTime;
            case Stage.Red:
                return randomizeRedTime ? Random.Range(redTimeRange.x, redTimeRange.y) : fixedRedTime;
            default:
                return 0f;
        }
    }

    void SetStage(Stage stage)
    {
        currentStage = stage;
        Debug.Log("Stage set to: " + stage);

        ToggleObjects(greenStageObjects, stage == Stage.Green);
        ToggleObjects(yellowStageObjects, stage == Stage.Yellow);
        ToggleObjects(redStageObjects, stage == Stage.Red);
    }

    IEnumerator MovementMonitor()
    {
        Debug.Log("MovementMonitor started");

        while (!hasDied)
        {
            if (currentStage == Stage.Red)
            {
                float movedDistance = Vector3.Distance(gorillaPlayer.position, lastPosition);
                if (movedDistance > movementThreshold)
                {
                    Debug.Log("Player moved during Red Light — triggering death.");
                    TriggerPunishment();
                    TriggerDeath();
                }
            }

            lastPosition = gorillaPlayer.position;
            yield return new WaitForSeconds(movementCheckInterval);
        }

        Debug.Log("MovementMonitor ended due to death");
    }

    void TriggerPunishment()
    {
        Debug.Log("Triggering punishment objects");
        foreach (GameObject obj in punishmentObjects)
        {
            obj.SetActive(true);
        }
    }

    void ToggleObjects(GameObject[] objects, bool enable)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(enable);
        }
    }

    public void TriggerDeath()
    {
        if (hasDied) return;
        hasDied = true;
        Debug.Log("TriggerDeath() called");

        foreach (GameObject obj in objectsToEnableOnDeath)
            obj.SetActive(true);

        foreach (GameObject obj in objectsToDisableOnDeath)
            obj.SetActive(false);

        if (stageCycleCoroutine != null) StopCoroutine(stageCycleCoroutine);
        if (movementMonitorCoroutine != null) StopCoroutine(movementMonitorCoroutine);
    }

    public void ResetDeath()
    {
        Debug.Log("ResetDeath() called");

        hasDied = false;

        foreach (GameObject obj in objectsToEnableOnDeath)
            obj.SetActive(false);

        foreach (GameObject obj in objectsToDisableOnDeath)
            obj.SetActive(true);

        SetStage(Stage.Green); // Reset visuals

        if (stageCycleCoroutine != null) StopCoroutine(stageCycleCoroutine);
        stageCycleCoroutine = StartCoroutine(StageCycle());

        if (movementMonitorCoroutine != null) StopCoroutine(movementMonitorCoroutine);
        movementMonitorCoroutine = null;

        if (activationZone.bounds.Contains(gorillaPlayer.position))
        {
            isPlayerInside = true;
            lastPosition = gorillaPlayer.position;
            movementMonitorCoroutine = StartCoroutine(MovementMonitor());
        }
        else
        {
            isPlayerInside = false;
        }
    }
}