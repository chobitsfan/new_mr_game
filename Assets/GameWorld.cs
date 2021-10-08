using UnityEngine;
using UnityEngine.InputSystem;


public class GameWorld : MonoBehaviour
{
    //public DroneAction PlayerDrone;
    //public DroneAction EmeryDrone;
    //public FPV_CAM FpvCam;
    public UnityEngine.UI.Text HudText;
    public UnityEngine.UI.Text TimeText;
    public VirtualAction MyVirtualAction;
    public VirtualAction EmeryVirtualAction;

    public bool IsGameOver => _gameOver;

    float hudTextCd = 0;
    bool _gameOver = false;
    float remainTime = 3 * 60;
    bool _gameStarted = false;

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
        remainTime = 3 * 60;
        _gameStarted = false;
    }

    public void GameStart()
    {
        ResetGame();
        _gameStarted = true;
    }

    void GameOver()
    {

    }

    private void Update()
    {
        if (!_gameOver && _gameStarted)
        {
            if (remainTime > 0)
            {
                remainTime -= Time.deltaTime;
                int mm = (int)(remainTime / 60);
                int ss = (int)remainTime % 60;
                int ms = (int)((remainTime - (int)remainTime) * 100);
                TimeText.text = mm.ToString("D2") + ":" + ss.ToString("D2") + "." + ms.ToString("D2");
            }
            else
            {
                _gameOver = true;
                TimeText.text = "Game Over";
            }
        }
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
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameStart();
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
