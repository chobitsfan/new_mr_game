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
    public GameObject Player;
    public GameObject Emery;
    public Animator UnityChanAnimator;

    VirtualAction playerVirtualAction;
    VirtualAction emeryVirtualAction;
    DroneAction playerDroneAction;
    DroneAction emeryDroneAction;

    public bool IsGameOver => _gameOver;
    public bool Avoid => _avoid;
    public Vector3 AvoidDirection => _avoid_direction;

    float hudTextCd = 0;
    bool _gameOver = false;
    float remainTime = MyGameSetting.GameDurationSec;
    bool _gameStarted = false;
    float delayedSync = 5f;
    AnimatorStateInfo previousState;
    AnimatorStateInfo currentState;
    bool _avoid = false;
    Vector3 _avoid_direction = Vector3.zero;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;

    private void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        playerVirtualAction = Player.GetComponent<VirtualAction>();
        playerDroneAction = Player.GetComponent<DroneAction>();
        emeryVirtualAction = Emery.GetComponent<VirtualAction>();
        emeryDroneAction = Emery.GetComponent<DroneAction>();
        currentState = UnityChanAnimator.GetCurrentAnimatorStateInfo(0);
        previousState = currentState;

        buf = new byte[512];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Any, 18500));
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
        playerDroneAction.Land();
    }

    public void GirlSync()
    {
        UnityChanAnimator.Play("Base Layer.003_NOT01_Final", -1, 0);
        UnityChanAnimator.Play("HandExpression.HandExpression", -1, 0);
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
                byte[] msg_buf = new byte[recvBytes];
                System.Array.Copy(buf, msg_buf, recvBytes);
                MAVLink.MAVLinkMessage msg = mavlinkParse.ReadPacket(msg_buf);
                if (msg != null)
                {
                    if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL)
                    {
                        var ctrl = (MAVLink.mavlink_manual_control_t)msg.data;
                        if (ctrl.buttons == 2)
                        {
                            GameStart();
                        }
                    }
                }
            }
        }
        if (playerDroneAction.Tracked && emeryDroneAction.Tracked && (playerDroneAction.CurPos - emeryDroneAction.CurPos).sqrMagnitude < 1.5f)
        {
            _avoid = true;
            _avoid_direction = playerDroneAction.CurPos - emeryDroneAction.CurPos;
        }
        else
        {
            _avoid = false;
        }
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

        if (_gameStarted)
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
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
