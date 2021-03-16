using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shot()
    {
        GameObject.Instantiate(beamShot, transform.position - transform.up * 0.2f, Quaternion.LookRotation(-transform.right));
    }
}
