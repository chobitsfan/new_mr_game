using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySmoke : MonoBehaviour
{
    float expireTs = 1.5f;

    // Update is called once per frame
    void Update()
    {
        expireTs -= Time.deltaTime;
        if (expireTs <= 0)
        {
            Destroy(gameObject);
        }
    }
}
