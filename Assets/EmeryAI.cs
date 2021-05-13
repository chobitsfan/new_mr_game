using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmeryAI : MonoBehaviour
{
    VirtualAction act;
    float shootingCD = 1f;
    // Start is called before the first frame update
    void Start()
    {
        act = gameObject.GetComponent<VirtualAction>();
    }

    // Update is called once per frame
    void Update()
    {
        shootingCD -= Time.deltaTime;
        if (shootingCD < 0)
        {
            act.Shot();
            shootingCD = 1f;
        }
    }
}
