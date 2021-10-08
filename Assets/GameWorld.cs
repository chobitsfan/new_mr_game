using UnityEngine;
using UnityEngine.InputSystem;


public class GameWorld : MonoBehaviour
{
    //public DroneAction PlayerDrone;
    //public DroneAction EmeryDrone;
    //public FPV_CAM FpvCam;
    public UnityEngine.UI.Text HudText;
    public VirtualAction MyVirtualAction;
    public VirtualAction EmeryVirtualAction;

    public bool GameOver => _gameOver;

    float hudTextCd = 0;
    bool _gameOver = false;

    public void ShowHudInfo(string text)
    {
        HudText.text = text;
        HudText.enabled = true;
        hudTextCd = 1f;
    }

    void ResetGame()
    {
        _gameOver = false;
        MyVirtualAction.ResetHP();
        EmeryVirtualAction.ResetHP();
    }

    public void GameStart()
    {

    }

    private void Update()
    {
        if (hudTextCd > 0)
        {
            hudTextCd -= Time.deltaTime;
            if (hudTextCd <= 0)
            {
                HudText.enabled = false;
            }
        }
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetGame();
        }
        if (!_gameOver)
        {
            if (MyVirtualAction.HP <= 0 && EmeryVirtualAction.HP <= 0)
            {
                _gameOver = true;
                HudText.text = "DRAW";
                HudText.enabled = true;
            }
            else if (MyVirtualAction.HP <= 0)
            {
                _gameOver = true;
                HudText.text = "LOSE";
                HudText.enabled = true;
            }
            else if (EmeryVirtualAction.HP <= 0)
            {
                _gameOver = true;
                HudText.text = "WIN";
                HudText.enabled = true;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
