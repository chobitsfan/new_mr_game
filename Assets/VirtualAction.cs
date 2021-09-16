using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;
    public GameObject smoke;
    //int hp = 3;
    public void Shot()
    {
        var beam = GameObject.Instantiate(beamShot, transform.position + transform.forward * 0.1f, Quaternion.LookRotation(-transform.right));
    }

    IEnumerator SmokeLater()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Instantiate(smoke, transform.position, Quaternion.LookRotation(transform.up), transform);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SmokeLater());
        hp--;
        if (hp < 0)
        {
            gameObject.GetComponent<DroneAction>().Land();
        }
    }*/
}
