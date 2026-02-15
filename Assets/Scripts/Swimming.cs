using GorillaLocomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Swimming : MonoBehaviour
{
    private InputDevice LeftControllerDevice;
    private InputDevice RightControllerDevice;
    private Vector3 LeftControllerVelocity;
    private Vector3 RightControllerVelocity;
    private Rigidbody playerRigidBody;
    private Vector3 swimVelocity;
    public LayerMask whatIsWater;
    public float radius = 0.25f;
    public float swimMultiplier = 1;
    public float minControllerForce = 1;
    public float maxControllerForce = 10; 
    public float vibrationDuration = 0.5f; 
    //public bool canSwim => Physics.OverlapSphere(transform.position, radius, whatIsWater).Length > 0;
    public bool canSwim = false;

    // Start is called before the first frame update
    private void Start()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        playerRigidBody = Player.Instance.GetComponent<Rigidbody>();
    }

    private void Awake() => gameObject.layer = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other == Player.Instance.bodyCollider)
        {
            Debug.Log("Entered gravity zone");
            Physics.gravity = new Vector3(0f, -0.31f, 0f);
            canSwim = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == Player.Instance.bodyCollider)
        {
            Debug.Log("Exited gravity zone");
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
            canSwim = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        LeftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerVelocity);
        RightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
        swimVelocity = new Vector3((((LeftControllerVelocity.x + RightControllerVelocity.x) / 2) * swimMultiplier), ((LeftControllerVelocity.y + RightControllerVelocity.y) / 2 * swimMultiplier), ((LeftControllerVelocity.z + RightControllerVelocity.z) / 2) * swimMultiplier);
        swimVelocity = Quaternion.Euler(0, Player.Instance.transform.eulerAngles.y, 0) * swimVelocity; // Rotate the swim velocity vector to be relative to the player's forward direction
        if (canSwim && (LeftControllerVelocity.magnitude > minControllerForce || RightControllerVelocity.magnitude > minControllerForce))
        {
            playerRigidBody.AddForce(-swimVelocity);

            if (LeftControllerVelocity.magnitude > minControllerForce)
            {
                float amplitude = Mathf.InverseLerp(minControllerForce, maxControllerForce, LeftControllerVelocity.magnitude);
                VibrateController(LeftControllerDevice, amplitude, vibrationDuration);
            }

            if (RightControllerVelocity.magnitude > minControllerForce)
            {
                float amplitude = Mathf.InverseLerp(minControllerForce, maxControllerForce, RightControllerVelocity.magnitude);
                VibrateController(RightControllerDevice, amplitude, vibrationDuration);
            }
        }
    }

    private void VibrateController(InputDevice controller, float amplitude, float duration)
    {
        HapticCapabilities capabilities;
        if (controller.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                controller.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }
}
