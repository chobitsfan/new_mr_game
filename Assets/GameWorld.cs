using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public DroneAction PlayerDrone;
    public DroneAction EmeryDrone;
    public FPV_CAM FpvCam;

    /*private void Start()
    {
        try
        {
            PlayerDrone.DroneID = int.Parse(System.IO.File.ReadAllText("player_drone_id.txt").Trim());
        }
        catch (System.Exception)
        {
            PlayerDrone.DroneID = 2;
        }
        try
        {
            EmeryDrone.DroneID = int.Parse(System.IO.File.ReadAllText("emery_drone_id.txt").Trim());
        }
        catch (System.Exception)
        {
            EmeryDrone.DroneID = 3;
        }
        try
        {
            FpvCam.ConnectCamera(System.IO.File.ReadAllText("fpv_rtsp_url.txt").Trim());
        }
        catch (System.Exception)
        {

        }
    }*/

    public void QuitGame()
    {
        Application.Quit();
    }

    /*public void PlayerIDChanged(string player_id)
    {
        int playerID;
        try
        {
            playerID = int.Parse(player_id);
        }
        catch (System.FormatException)
        {
            return;
        }
        PlayerDrone.DroneIDChanged(playerID);
        if (playerID == 2) EmeryDrone.DroneIDChanged(3); else EmeryDrone.DroneIDChanged(2);
    }*/
}
