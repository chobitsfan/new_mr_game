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
    public GameWorld gameWorld;
    short pitch, roll, throttle, yaw;
    float controlCd = 0;
    float checkCd = 0;
    DroneAction droneAction;
    VirtualAction virtualAction;
    Stage stage = Stage.None;
    short pitchSend, rollSend;
    bool armed = false;
    float armVibCd = 0f;
    float beamShotCd = 0f;

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
        if (beamShotCd <= 0)
        {
            droneAction.FireLaser();
            virtualAction.Shot();
            beamShotCd = 0.5f;
        }
    }

    public void OnLand()
    {
        droneAction.Land();
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            droneAction.Disarm(true);
        }
        if (beamShotCd > 0)
        {
            beamShotCd -= Time.deltaTime;
        }
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            droneAction.Land();
        }
        bool stopNow = false;
        if ((Gamepad.current != null) && (stage == Stage.PosHold))
        {
            controlCd -= Time.deltaTime;

            //Vector3 droneHeading = -drone.transform.right;
            //Vector3 droneHeading = droneAction.CurRot * -Vector3.right;
            //droneHeading.y = 0;
            //Vector3 droneRight = drone.transform.forward;
            //Vector3 droneRight = droneAction.CurRot * Vector3.forward;
            //droneRight.y = 0;
            //Vector3 usr_ctrl = droneHeading * pitch + droneRight * roll;
            if (pitch != 0 && roll != 0)
            {
                Vector3 usr_ctrl = drone.transform.TransformDirection(-pitch, 0, roll);
                usr_ctrl.Normalize();
                RaycastHit hit;
                //if (Physics.Raycast(drone.transform.position, usr_ctrl, 0.6f))
                if (Physics.Raycast(droneAction.CurPos, usr_ctrl, out hit, 0.6f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
                {
                    pitch = 0;
                    roll = 0;
                    stopNow = true;
                    gameWorld.ShowHudInfo("obs:" + hit.collider.gameObject.name);
                }
            }

            if (controlCd <= 0 || System.Math.Abs(pitch - pitchSend) >= 100 || System.Math.Abs(roll - rollSend) >= 100 || stopNow)
            {
                controlCd = 0.05f;
                droneAction.ManualControl(pitch, roll, throttle, yaw);
                pitchSend = pitch;
                rollSend = roll;
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

        if (!armed && droneAction.IsArmed())
        {
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(0f, 0.5f);
                armVibCd = 2f;
            }
        }
        armed = droneAction.IsArmed();
        if (armVibCd > 0)
        {
            armVibCd -= Time.deltaTime;
            if (armVibCd <= 0)
            {
                Gamepad.current.ResetHaptics();
            }
        }
    }
}
