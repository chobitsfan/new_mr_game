using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShotBehavior : MonoBehaviour
{
    public GameObject explosion;
    float expireTs = 3f;
    float colliderCd = 0.1f;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 2, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += 2 * Time.deltaTime * transform.forward;
        expireTs -= Time.deltaTime;
        if (expireTs <= 0)
        {
            Destroy(gameObject);
        }
        if (colliderCd > 0)
        {
            colliderCd -= Time.deltaTime;
            if (colliderCd <= 0)
            {
                GetComponent<Collider>().enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject boom = GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(boom, 2f);
    }
}
