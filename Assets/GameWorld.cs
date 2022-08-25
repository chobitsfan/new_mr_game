using System.Net;
using System.Net.Sockets;
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
    public UnityEngine.UI.Text BatText;
    public UnityEngine.UI.Text StatusText;
    public Animator UnityChanAnimator;

    public bool IsGameOver => _gameOver;
    //public bool Avoid => _avoid;
    //public Vector3 ImpactDirection => _impact_direction;

    float hudTextCd = 0;
    bool _gameOver = false;
    float remainTime = MyGameSetting.GameDurationSec;
    bool _gameStarted = false;
    float delayedSync = 5f;
    AnimatorStateInfo previousState;
    AnimatorStateInfo currentState;
    //bool _avoid = false;
    byte[] buf = new byte[128];
    Socket sock;
    //Vector3 _impact_direction = Vector3.zero;

    float[] gameMsg = new float[32];
    IPEndPoint gameProxy;
    GameObject[] drones;

    private void Start()
    {
        gameProxy = new IPEndPoint(IPAddress.Parse(MyGameSetting.MocapIp), 27500);
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        currentState = UnityChanAnimator.GetCurrentAnimatorStateInfo(0);
        previousState = currentState;

        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Any, 18500));

        drones = GameObject.FindGameObjectsWithTag("Drone");
        System.Array.Sort(drones, delegate (GameObject g1, GameObject g2) { return g1.GetComponent<DroneAction>().MavId.CompareTo(g2.GetComponent<DroneAction>().MavId); });
    }

    public void ShowStatus(string text)
    {
        StatusText.text = text;
    }

    public void ShowHudInfo(string text, bool stayLonger = false)
    {
        HudText.text = text;
        HudText.enabled = true;
        if (stayLonger) hudTextCd = 10f; else hudTextCd = 1.5f;
    }

    void ResetGame()
    {
        _gameOver = false;
        //playerVirtualAction.ResetHP();
        //emeryVirtualAction.ResetHP();
        remainTime = MyGameSetting.GameDurationSec;
        _gameStarted = false;
    }

    public void GameStart()
    {
        ResetGame();
        _gameStarted = true;
        //UpdateHpDisplay(playerVirtualAction.HP);
    }

    public void UpdateHpDisplay(int hp)
    {
        HpText.text = "HP:" + hp;
    }

    public void UpdateBatDisplay(ushort voltage_mv)
    {
        BatText.text = (voltage_mv * 0.001f).ToString("n2") + "V";
    }

    void GameOver()
    {
        _gameOver = true;
        _gameStarted = false;
        //playerDroneAction.Land();
    }

    public void GirlSync()
    {
        UnityChanAnimator.Play("Base Layer.003_NOT01_Final", -1, 0);
        UnityChanAnimator.Play("HandExpression.HandExpression", -1, 0);
    }

    public void PlayerOpenFire()
    {
        byte[] buf = new byte[8];
        float[] msg = { 3.0f, MyGameSetting.PlayerDroneId };
        System.Buffer.BlockCopy(msg, 0, buf, 0, 8);
        sock.SendTo(buf, gameProxy);
    }

    private void Update()
    {
        while (sock.Available > 0)
        {
            int recvBytes = 0;
            try
            {
                recvBytes = sock.Receive(buf);
            }
            catch (SocketException)
            {
                //Debug.LogWarning("socket err " + e.ErrorCode);
                break;
            }
            if (recvBytes > 0)
            {
                System.Buffer.BlockCopy(buf, 0, gameMsg, 0, recvBytes);
                if (gameMsg[0] == 1)
                {
                    GameStart();
                }
                else if (gameMsg[0] == 2)
                {
                    GirlSync();
                }
                else if (gameMsg[0] == 3)
                {
                    drones[(int)gameMsg[1]].GetComponent<VirtualAction>().Shot();
                }
            }
        }
        /*if (playerDroneAction.Tracked && emeryDroneAction.Tracked && (playerDroneAction.CurPos - emeryDroneAction.CurPos).sqrMagnitude < (MyGameSetting.AvoidDist * MyGameSetting.AvoidDist))
        {
            _avoid = true;
            _impact_direction = emeryDroneAction.CurPos - playerDroneAction.CurPos;
        }
        else
        {
            _avoid = false;
        }*/
        if (delayedSync > 0)
        {
            delayedSync -= Time.deltaTime;
            if (delayedSync <= 0)
            {
                sock.SendTo(System.BitConverter.GetBytes(2.0f), gameProxy); // notify all client to sync girl anime
                //GirlSync();
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
                TimeText.text = "00:00:00";
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
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameStart();
            playerDroneAction.SendGameStart();
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            UnityChanAnimator.SetBool("Next", true);
        }*/

        if (UnityChanAnimator.GetBool("Next"))
        {
            currentState = UnityChanAnimator.GetCurrentAnimatorStateInfo(0);
            if (previousState.fullPathHash != currentState.fullPathHash)
            {
                UnityChanAnimator.SetBool("Next", false);
                previousState = currentState;
            }
        }
        else if (UnityChanAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            UnityChanAnimator.SetBool("Next", true);
        }

        /*if (_gameStarted)
        {
            if (_gameOver)
            {
                if (playerVirtualAction.HP > emeryVirtualAction.HP)
                {
                    GameOver();
                    ShowHudInfo("WIN", true);
                } else if (playerVirtualAction.HP == emeryVirtualAction.HP)
                {
                    GameOver();
                    ShowHudInfo("DRAW", true);
                } else
                {
                    GameOver();
                    ShowHudInfo("LOSE", true);
                }
            }
            else
            {
                if (playerVirtualAction.HP <= 0 && emeryVirtualAction.HP <= 0)
                {
                    GameOver();
                    ShowHudInfo("DRAW", true);
                }
                else if (playerVirtualAction.HP <= 0)
                {
                    GameOver();
                    ShowHudInfo("LOSE", true);
                }
                else if (emeryVirtualAction.HP <= 0)
                {
                    GameOver();
                    ShowHudInfo("WIN", true);
                }
            }
        }*/
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
