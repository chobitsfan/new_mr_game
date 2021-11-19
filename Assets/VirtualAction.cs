using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;
    public GameObject smoke;
    public bool isPlayer = false;
    public GameWorld gameWorld;

    public int HP => _hp;

    private int _hp = 10;
    float hitVibCd = 0f;
    Color beamColor = Color.green;
    AudioSource beamShotSound;

    private void Start()
    {
        if (isPlayer)
        {
            beamColor = MyGameSetting.PlayerBeamColor;
        }
        else
        {
            beamColor = MyGameSetting.EmeryBeamColor;
        }
        beamShotSound = GetComponent<AudioSource>();
    }

    public void Shot()
    {
        var beam = GameObject.Instantiate(beamShot, transform.position + transform.forward * 0.1f, Quaternion.LookRotation(-transform.right));
        Physics.IgnoreCollision(beam.GetComponent<Collider>(), GetComponent<Collider>());
        beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().LineColor = beamColor;
        beamShotSound.Play();
    }

    IEnumerator SmokeLater()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Instantiate(smoke, transform.position, Quaternion.LookRotation(transform.up), transform);
    }

    public void ResetHP()
    {
        _hp = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Beam")
        {
            _hp--;
        }
        if (isPlayer)
        {
            /*StartCoroutine(SmokeLater());
            hp--;
            if (hp < 0)
            {
                gameObject.GetComponent<DroneAction>().Land();
            }*/
            gameWorld.UpdateHpDisplay(_hp);
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(0f, 0.7f);
                hitVibCd = 1f;
            }
        }
    }

    private void Update()
    {
        if (isPlayer)
        {
            if (hitVibCd > 0)
            {
                hitVibCd -= Time.deltaTime;
                if (hitVibCd <= 0)
                {
                    Gamepad.current.ResetHaptics();
                }
            }
        }
    }
}
