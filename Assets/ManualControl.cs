using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManualControl : MonoBehaviour
{
    /*enum Stage
    {
        None,
        WaitingForAltHold,
        WaitingForArmed,
        WaitingForTakeoff,
    }*/
    public GameObject drone;
    public GameWorld gameWorld;
    short pitch, roll, throttle, yaw;
    float controlCd = 0;
    //float checkCd = 1f;
    DroneAction droneAction;
    VirtualAction virtualAction;
    //Stage stage = Stage.None;
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

        pitch = (short)(v.y * 600f);
        roll = (short)(v.x * 600f);
    }

    public void OnStabilize()
    {
        droneAction.Stabilize();
    }

    public void OnTakeoff()
    {
        droneAction.AltHold();
        droneAction.Arm();
        //stage = Stage.WaitingForAltHold;
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

        if (Gamepad.current != null)
        {
            bool emerg = false;
            short pitchOut = pitch;
            short rollOut = roll;
            short throttleOut = throttle;

            controlCd -= Time.deltaTime;

            //Vector3 droneHeading = -drone.transform.right;
            //Vector3 droneHeading = droneAction.CurRot * -Vector3.right;
            //droneHeading.y = 0;
            //Vector3 droneRight = drone.transform.forward;
            //Vector3 droneRight = droneAction.CurRot * Vector3.forward;
            //droneRight.y = 0;
            //Vector3 usr_ctrl = droneHeading * pitch + droneRight * roll;
#if false
            if (pitch != 0 && roll != 0)
            {
                //Vector3 usr_ctrl = drone.transform.TransformDirection(-pitch, 0, roll);
                /*Vector3 droneHeading = droneAction.CurRot * Vector3.left;
                droneHeading.y = 0;
                Vector3 droneRight = droneAction.CurRot * Vector3.forward;
                droneRight.y = 0;
                Vector3 usr_ctrl = droneHeading * pitch + droneRight * roll + Vector3.up * (throttle - 500);*/
                Vector3 usr_ctrl = droneAction.CurRot * new Vector3(-pitch, 0, roll);
                usr_ctrl.y = throttle - 500;
                usr_ctrl.Normalize();
                RaycastHit hit;
                if (Physics.Raycast(droneAction.CurPos, usr_ctrl, out hit, 1f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
                    //|| Physics.Raycast(droneAction.CurPos, usr_ctrl, out hit, 1f, LayerMask.GetMask("DroneAvoid"), QueryTriggerInteraction.Collide))
                {
                    pitch = 0;
                    roll = 0;
                    stopNow = true;
                    gameWorld.ShowHudInfo("stop:" + hit.collider.gameObject.name);
                    //Debug.LogError(hit.collider.gameObject.name + "," + usr_ctrl + "," + (hit.collider.transform.position - droneAction.CurPos).magnitude);
                }
            }
#endif
            /*if (stage == Stage.WaitingForAltHold || stage == Stage.WaitingForArmed)
            {
                throttle = 0;
            }
            else if (stage == Stage.WaitingForTakeoff)
            {
                throttle = 500;
            }*/
            if (gameWorld.Avoid)
            {
                Vector3 avoid_direction = gameWorld.AvoidDirection;
                avoid_direction.y = 0;
                avoid_direction = Quaternion.Inverse(droneAction.CurRot) * avoid_direction;
                //avoid_direction.y = 0;
                //avoid_direction.Normalize();
                float scale;
                if (Mathf.Abs(avoid_direction.x) > Mathf.Abs(avoid_direction.z))
                {
                    scale = 1.0f / Mathf.Abs(avoid_direction.x);
                }
                else
                {
                    scale = 1.0f / Mathf.Abs(avoid_direction.z);
                }
                pitchOut = (short)(avoid_direction.x * scale * -1000.0f);
                rollOut = (short)(avoid_direction.z * scale * 1000.0f);
                emerg = true;
            }

            Vector3 cur_hor_pos = new Vector3(droneAction.CurPos.x, 0, droneAction.CurPos.z);
            bool fence_avoid = false;
            Vector3 fence_avoid_dir = Vector3.zero;
            /*if (cur_hor_pos.x > 1.8f || cur_hor_pos.x < -1.8f || cur_hor_pos.z > 0.8f || cur_hor_pos.z < -1.8f)
            {
                Vector3 to_center = Quaternion.Inverse(droneAction.CurRot) * -cur_hor_pos;
                to_center.y = 0;
                to_center.Normalize();
                //Debug.LogError("test:" + -(vv.x) + "," + vv.z);
                pitchOut = (short)(to_center.x * -1000.0f);
                rollOut = (short)(to_center.z * 1000.0f);
                emerg = true;
                Debug.LogError("fence " + cur_hor_pos + "," + pitchOut + "," + rollOut);
                gameWorld.ShowHudInfo("Fence");
            }*/
            if (cur_hor_pos.x > MyGameSetting.FenceXMax)
            {
                fence_avoid = true;
                fence_avoid_dir.x = -1;
            }
            else if (cur_hor_pos.x < MyGameSetting.FenceXMin)
            {
                fence_avoid = true;
                fence_avoid_dir.x = 1;
            }
            if (cur_hor_pos.z > MyGameSetting.FenceZMax)
            {
                fence_avoid = true;
                fence_avoid_dir.z = -1;
            }
            else if (cur_hor_pos.z < MyGameSetting.FenceZMin)
            {
                fence_avoid = true;
                fence_avoid_dir.z = 1;
            }
            if (fence_avoid)
            {
                fence_avoid_dir = Quaternion.Inverse(droneAction.CurRot) * fence_avoid_dir;
                float scale;
                if (Mathf.Abs(fence_avoid_dir.x) > Mathf.Abs(fence_avoid_dir.z))
                {
                    scale = 1.0f / Mathf.Abs(fence_avoid_dir.x);
                }
                else
                {
                    scale = 1.0f / Mathf.Abs(fence_avoid_dir.z);
                }
                pitchOut = (short)(fence_avoid_dir.x * scale * -1000.0f);
                rollOut = (short)(fence_avoid_dir.z * scale * 1000.0f);
                emerg = true;
                Debug.LogError("fence " + cur_hor_pos + "," + pitchOut + "," + rollOut);
                gameWorld.ShowHudInfo("Fence");
            }

            Debug.LogError("FenceAltHigh "+ MyGameSetting.FenceAltHigh);
            float cur_alt = droneAction.CurPos.y;
            if (cur_alt > MyGameSetting.FenceAltHigh)
            {
                //Debug.LogError("alt " + cur_alt);
                throttleOut = 200;
                emerg = true;
            }
            else if (cur_alt < MyGameSetting.FenceAltLow && throttle < 500)
            {
                throttleOut = 500;
            }
            
            if (controlCd <= 0 || System.Math.Abs(pitchOut - pitchSend) >= 100 || System.Math.Abs(rollOut - rollSend) >= 100 || emerg)
            {
                controlCd = 0.05f;
                droneAction.ManualControl(pitchOut, rollOut, throttleOut, yaw);
                pitchSend = pitchOut;
                rollSend = rollOut;

                //Vector3 vv = Quaternion.Inverse(droneAction.CurRot) * -droneAction.CurPos;
                //Debug.LogError("test:" + -(vv.x) + "," + vv.z);
            }
        }

        /*checkCd -= Time.deltaTime;
        if (checkCd <= 0)
        {
            checkCd = 1f;
            switch (stage)
            {
                case Stage.WaitingForAltHold:
                    Debug.LogError("WaitingForAltHold");
                    if (droneAction.IsAltHold())
                    {
                        droneAction.Arm();
                        stage = Stage.WaitingForArmed;
                    }
                    else
                    {
                        droneAction.AltHold();
                    }
                    break;
                case Stage.WaitingForArmed:
                    Debug.LogError("WaitingForArmed");
                    if (droneAction.IsArmed())
                    {
                        stage = Stage.WaitingForTakeoff;
                    }
                    else
                    {
                        droneAction.Arm();
                    }
                    break;
                case Stage.WaitingForTakeoff:
                    Debug.LogError("WaitingForTakeoff");
                    if (droneAction.CurPos.y > 0.5f)
                    {
                        Debug.LogError("Takeoff done");
                        stage = Stage.None;
                    }
                    else
                    {
                        droneAction.TakeOff();
                    }
                    break;
            }
        }*/

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
