using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManualControl : MonoBehaviour
{
    enum Stage
    {
        None,
        WaitingForGuided,
        WaitingForArmed,
        WaitingForTakeoffDone,
        WaitingForPosHold,
        PosHold
    }
    public GameObject drone;
    short pitch, roll, throttle, yaw;
    float controlCd = 0;
    float checkCd = 0;
    DroneAction droneAction;
    VirtualAction virtualAction;
    Stage stage = Stage.None;

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
        //droneAction.Stabilize();
        droneAction.Poshold();
    }

    public void OnTakeoff()
    {
        droneAction.Guided();
        stage = Stage.WaitingForGuided;
        Debug.Log("switch to guided");
    }

    public void OnShot()
    {
        virtualAction.Shot();
    }

    public void OnLand()
    {
        droneAction.Land();
    }

    private void Update()
    {
        if ((Gamepad.current != null) && (stage == Stage.PosHold))
        {
            controlCd -= Time.deltaTime;
            if (controlCd <= 0)
            {
                controlCd = 0.05f;
                /*if (droneAction.IsPosHold())
                {
                    float alt = drone.transform.position.y;
                    if (alt > 1.2f && throttle > 500)
                    {
                        throttle = 500;
                    }
                    else if (alt < 0.7f && throttle < 500)
                    {
                        throttle = 500;
                    }
                }*/
                droneAction.ManualControl(pitch, roll, throttle, yaw);
            }
        }
        checkCd -= Time.deltaTime;
        if (checkCd <= 0)
        {
            checkCd = 1f;
            switch (stage)
            {
                case Stage.WaitingForGuided:
                    if (droneAction.IsGuided())
                    {
                        Debug.Log("arm");
                        droneAction.Arm();
                        stage = Stage.WaitingForArmed;
                    }
                    break;
                case Stage.WaitingForArmed:
                    if (droneAction.IsArmed())
                    {
                        Debug.Log("takeoff");
                        droneAction.TakeOff();
                        stage = Stage.WaitingForTakeoffDone;
                    }
                    break;
                case Stage.WaitingForTakeoffDone:
                    if (drone.transform.position.y > 0.9f)
                    {
                        Debug.Log("switch to poshold");
                        droneAction.Poshold();
                        stage = Stage.WaitingForPosHold;
                    }
                    break;
                case Stage.WaitingForPosHold:
                    if (droneAction.IsPosHold())
                    {
                        stage = Stage.PosHold;
                    }
                    else
                    {
                        droneAction.Poshold();
                    }
                    break;
            }
        }
    }
}
