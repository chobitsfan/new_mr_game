using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour {

    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;

    ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;

    int shaderProperty;

	void Start ()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps = GetComponentInChildren <ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;

        //ps.Play();

    }
	
	void Update ()
    {
        /*if (timer < spawnEffectTime + pause)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ps.Play();
            timer = 0;
        }*/
        if (timer < spawnEffectTime)
        {
            timer += Time.deltaTime;
            _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ps.Play();
        timer = 0;
    }
}
