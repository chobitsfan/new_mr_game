using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualAction : MonoBehaviour
{
    public GameObject beamShot;
    public GameObject smoke;
    public bool isPlayer;
    //int hp = 3;
    float hitVibCd = 0f;
    Color beamColor = Color.green;

    private void Start()
    {
        int r = 200, g = 10, b = 10;
        string f_name;
        if (isPlayer) f_name = "player_beam_color.txt"; else f_name = "emery_beam_color.txt";
        try
        {
            string[] color_txt = System.IO.File.ReadAllText(f_name).Split(',');
            r = int.Parse(color_txt[0]);
            g = int.Parse(color_txt[1]);
            b = int.Parse(color_txt[2]);
        }
        catch (System.Exception)
        {

        }
        beamColor = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public void Shot()
    {
        var beam = GameObject.Instantiate(beamShot, transform.position + transform.forward * 0.1f, Quaternion.LookRotation(-transform.right));
        beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().LineColor = beamColor;
    }

    IEnumerator SmokeLater()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Instantiate(smoke, transform.position, Quaternion.LookRotation(transform.up), transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            /*StartCoroutine(SmokeLater());
            hp--;
            if (hp < 0)
            {
                gameObject.GetComponent<DroneAction>().Land();
            }*/
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
