using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManualControl : MonoBehaviour
{
    public GameObject drone;
    short pitch, roll, throttle, yaw;
    float controlCd = 0;
    DroneAction droneAction;
    VirtualAction virtualAction;

    private void Start()
    {
        droneAction = drone.GetComponent<DroneAction>();
        virtualAction = drone.GetComponent<VirtualAction>();
    }
    public void OnArm()
    {       
        droneAction.Arm();
    }

    public void OnDisarm()
    {
        droneAction.Disarm(true);
    }

    public void OnThrottleYaw(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        //Debug.Log("OnThrottleYaw"+ v);
        throttle = (short)((v.y + 1f) * 500f);
        yaw = (short)(v.x * 1000f);
    }

    public void OnPitchRoll(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        //Debug.Log("OnPitchRoll"+v);
        pitch = (short)(v.y * 1000f);
        roll = (short)(v.x * 1000f);
    }

    public void OnStabilize()
    {
        droneAction.Stabilize();
    }

    public void OnPoshold()
    {
        droneAction.Poshold();
    }

    public void OnShot()
    {
        virtualAction.Shot();
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            controlCd -= Time.deltaTime;
            if (controlCd <= 0)
            {
                controlCd = 0.1f;
                droneAction.ManualControl(pitch, roll, throttle, yaw);
            }
        }
    }
}
