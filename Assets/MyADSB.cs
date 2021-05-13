using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyADSB : MonoBehaviour
{
    public GameObject Drone;
    private void OnTriggerEnter(Collider other)
    {
        Drone.GetComponent<DroneAction>().SendDistSensor();
    }
    private void OnTriggerStay(Collider other)
    {
        Drone.GetComponent<DroneAction>().SendDistSensor();
    }
}
