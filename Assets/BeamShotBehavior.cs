using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShotBehavior : MonoBehaviour
{
    public GameObject explosion;
    float expireTs = 3f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 2;
        expireTs -= Time.deltaTime;
        if (expireTs <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject boom = GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(boom, 1);
    }
}
