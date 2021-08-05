using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public DroneAction PlayerDrone;
    public DroneAction EmeryDrone;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayerIDChanged(string player_id)
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
    }
}
