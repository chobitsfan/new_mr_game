using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;
    public GameObject smoke;
    public void Shot()
    {
        GameObject.Instantiate(beamShot, transform.position + transform.forward * 0.1f, Quaternion.LookRotation(-transform.right));
    }

    IEnumerator SmokeLater()
    {
        yield return new WaitForSeconds(1f);
        GameObject.Instantiate(smoke, transform.position, Quaternion.LookRotation(transform.up));
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SmokeLater());
        gameObject.GetComponent<DroneAction>().Land();
    }
}
