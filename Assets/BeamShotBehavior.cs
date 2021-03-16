using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShotBehavior : MonoBehaviour
{
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject boom = GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(boom, 1);
    }
}
