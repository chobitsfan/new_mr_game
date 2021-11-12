using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDrone : MonoBehaviour
{
    /*DroneAction droneAction;
    enum Stage
    {
        None,
        WaitingForGuided,
        WaitingForArmed,
        WaitingForTakeoffDone,
        WaitingForPosHold
    }
    Stage stage;

    // Start is called before the first frame update
    void Start()
    {
        droneAction = GetComponent<DroneAction>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case Stage.None:
                break;
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
                if (transform.position.y > 0.9f)
                {
                    Debug.Log("switch to poshold");
                    droneAction.Poshold();
                    stage = Stage.WaitingForPosHold;
                }
                break;
            default:
                break;
        }
    }

    public void TakeOff()
    {
        droneAction.Guided();
        stage = Stage.WaitingForGuided;
        Debug.Log("switch to guided");
    }*/
}
