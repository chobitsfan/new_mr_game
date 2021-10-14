using UnityEngine;
using UnityEngine.InputSystem;


public class GameWorld : MonoBehaviour
{
    //public DroneAction PlayerDrone;
    //public DroneAction EmeryDrone;
    //public FPV_CAM FpvCam;
    public UnityEngine.UI.Text HudText;
    public UnityEngine.UI.Text TimeText;
    public UnityEngine.UI.Text HpText;
    public GameObject Player;
    public GameObject Emery;
    public Animator UnityChanAnimator;

    VirtualAction playerVirtualAction;
    VirtualAction emeryVirtualAction;
    DroneAction playerDroneAction;
    DroneAction emeryDroneAction;

    public bool IsGameOver => _gameOver;

    float hudTextCd = 0;
    bool _gameOver = false;
    float remainTime = MyGameSetting.GameDurationSec;
    bool _gameStarted = false;
    float delayedSync = 5f;

    private void Start()
    {
        playerVirtualAction = Player.GetComponent<VirtualAction>();
        playerDroneAction = Player.GetComponent<DroneAction>();
        emeryVirtualAction = Emery.GetComponent<VirtualAction>();
        emeryDroneAction = Emery.GetComponent<DroneAction>();
    }

    public void ShowHudInfo(string text)
    {
        HudText.text = text;
        HudText.enabled = true;
        hudTextCd = 1f;
    }

    void ResetGame()
    {
        _gameOver = false;
        playerVirtualAction.ResetHP();
        emeryVirtualAction.ResetHP();
        remainTime = MyGameSetting.GameDurationSec;
        _gameStarted = false;
    }

    public void GameStart()
    {
        ResetGame();
        _gameStarted = true;
        UpdateHpDisplay(playerVirtualAction.HP);
    }

    public void UpdateHpDisplay(int hp)
    {
        HpText.text = "HP:" + playerVirtualAction.HP;
    }

    void GameOver()
    {

    }

    public void GirlSync()
    {
        UnityChanAnimator.Play("Base Layer.003_NOT01_Final", -1, 0);
        UnityChanAnimator.Play("HandExpression.HandExpression", -1, 0);
    }

    private void Update()
    {
        if (delayedSync > 0)
        {
            delayedSync -= Time.deltaTime;
            if (delayedSync <= 0)
            {
                playerDroneAction.SendGirlSync();
                GirlSync();
            }
        }
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
                ShowHudInfo("Game Over");
                playerDroneAction.Land();
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
        /*else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetGame();
        }*/
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameStart();
            playerDroneAction.SendGameStart();
        }
        if (!_gameOver)
        {
            if (playerVirtualAction.HP <= 0 && emeryVirtualAction.HP <= 0)
            {
                _gameOver = true;
                HudText.text = "DRAW";
                HudText.enabled = true;
            }
            else if (playerVirtualAction.HP <= 0)
            {
                _gameOver = true;
                HudText.text = "LOSE";
                HudText.enabled = true;
            }
            else if (emeryVirtualAction.HP <= 0)
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
