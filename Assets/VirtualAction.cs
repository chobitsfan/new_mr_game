using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;

    public void Shot()
    {
        GameObject.Instantiate(beamShot, transform.position + transform.forward * 0.1f, Quaternion.LookRotation(-transform.right));
    }
}
