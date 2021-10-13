using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class MoCapData
{
    public Vector3 pos;
    public Quaternion rot;
    public double ts;
}

public class DroneAction : MonoBehaviour
{
    public UnityEngine.UI.Text StatusText;
    public bool IsPlayer;
    public GameWorld gameWorld;
    [System.NonSerialized]
    public Vector3 CurPos;
    [System.NonSerialized]
    public Quaternion CurRot;

    public bool Tracked => _tracked;

    int _droneId = -1;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;
    IPEndPoint myproxy;
    uint apm_mode = 0;
    bool armed = false;
    float hb_cd = 2f;
    static string mocap_ip = "";
    IPEndPoint game_proxy;
    VirtualAction virtualAction;
    Queue<MoCapData> moCapDataQueue = new Queue<MoCapData>();
    const double RTSP_BUF_DELAY_S = 0.1;
    float lastMocapDataTs = 0;
    private bool _tracked = false;

    // Start is called before the first frame update
    void Start()
    {
        buf = new byte[512];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        if (mocap_ip.Equals(""))
        {
            try
            {
                mocap_ip = System.IO.File.ReadAllText("mocap_ip.txt").Trim();
            }
            catch (System.Exception)
            {
                mocap_ip = "127.0.0.1";
            }
        }
        myproxy = new IPEndPoint(IPAddress.Parse(mocap_ip), 17500);
        game_proxy = new IPEndPoint(IPAddress.Parse(mocap_ip), 27500);
        virtualAction = gameObject.GetComponent<VirtualAction>();

        if (IsPlayer)
        {
            try
            {
                _droneId = int.Parse(System.IO.File.ReadAllText("player_drone_id.txt").Trim());
            }
            catch (System.Exception)
            {
                _droneId = 2;
            }
        }
        else
        {
            try
            {
                _droneId = int.Parse(System.IO.File.ReadAllText("emery_drone_id.txt").Trim());
            }
            catch (System.Exception)
            {
                _droneId = 3;
            }
        }
        sock.Bind(new IPEndPoint(IPAddress.Any, 17500 + _droneId));
    }

    /*public int DroneID
    {
        get => _droneId;
        set
        {
            _droneId = value;
            sock.Bind(new IPEndPoint(IPAddress.Any, 17500 + _droneId));
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        lastMocapDataTs += Time.deltaTime;
        MoCapData delayedMoCapData = null;
        while (moCapDataQueue.Count > 0)
        {
            MoCapData moCapData = moCapDataQueue.Peek();
            if (Time.time - moCapData.ts >= RTSP_BUF_DELAY_S)
            {
                delayedMoCapData = moCapDataQueue.Dequeue();
            }
            else
            {
                break;
            }
        }
        if (delayedMoCapData != null)
        {
            transform.SetPositionAndRotation(delayedMoCapData.pos, Quaternion.Lerp(transform.rotation, delayedMoCapData.rot, 0.5f));
        }
        if (sock.IsBound)
        {
            hb_cd -= Time.deltaTime;
            if (hb_cd <= 0)
            {
                hb_cd = 2f;
                MAVLink.mavlink_heartbeat_t cmd = new MAVLink.mavlink_heartbeat_t
                {
                    autopilot = 8,
                    type = 6,
                    mavlink_version = 3
                };
                byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, cmd);
                sock.SendTo(data, game_proxy);
            }
        }
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
                    switch (msg.msgid)
                    {
                        case (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT:
                            {
                                var status_txt = (MAVLink.mavlink_statustext_t)msg.data;
                                //Debug.Log(System.Text.Encoding.ASCII.GetString(status_txt.text));
                                if (IsPlayer) StatusText.text = System.Text.Encoding.ASCII.GetString(status_txt.text);
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT:
                            {
                                var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                                apm_mode = heartbeat.custom_mode;
                                armed = (heartbeat.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) != 0;
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.ATT_POS_MOCAP:
                            {
                                var att_pos = (MAVLink.mavlink_att_pos_mocap_t)msg.data;
                                MoCapData moCapData = new MoCapData
                                {
                                    ts = Time.time,
                                    pos = new Vector3(-att_pos.x, att_pos.y, att_pos.z),
                                    rot = new Quaternion(-att_pos.q[1], att_pos.q[2], att_pos.q[3], -att_pos.q[0])
                                };
                                moCapDataQueue.Enqueue(moCapData);
                                CurPos = moCapData.pos;
                                CurRot = moCapData.rot;
                                lastMocapDataTs = 0;
                                _tracked = true;
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL:
                            {
                                var ctrl = (MAVLink.mavlink_manual_control_t)msg.data;
                                if (ctrl.buttons == 1)
                                {
                                    virtualAction.Shot();
                                }
                                else if (ctrl.buttons == 2)
                                {
                                    gameWorld.GameStart();
                                }
                                break;
                            }
                    }
                }
            }
            if (lastMocapDataTs > 0.3f)
            {
                _tracked = false;
                if (IsPlayer) gameWorld.ShowHudInfo("lost track");
            }
        }
    }

    public bool IsGuided()
    {
        return apm_mode == (uint)MAVLink.COPTER_MODE.GUIDED;
    }

    public bool IsPosHold()
    {
        return apm_mode == (uint)MAVLink.COPTER_MODE.POSHOLD;
    }

    public bool IsArmed()
    {
        return armed;
    }

    public void TakeOff()
    {

        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            command = (ushort)MAVLink.MAV_CMD.TAKEOFF,
            param7 = 1f
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, myproxy);
    }    

    public void Arm()
    {
        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
            param1 = 1
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, myproxy);
    }

    public void Disarm(bool forced)
    {
        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
            param1 = 0,
            param2 = forced ? 21196 : 0
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, myproxy);
    }

    public void Poshold()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = (uint)MAVLink.COPTER_MODE.POSHOLD
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void Stabilize()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = (byte)_droneId,
            custom_mode = (uint)MAVLink.COPTER_MODE.STABILIZE
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void Land()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = (byte)_droneId,
            custom_mode = (uint)MAVLink.COPTER_MODE.LAND
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void Auto()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = (byte)_droneId,
            custom_mode = (uint)MAVLink.COPTER_MODE.AUTO
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void ManualControl(short pitch, short roll, short throttle, short yaw)
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            target = (byte)_droneId,
            x = pitch,
            y = roll,
            z = throttle,
            r = yaw
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, myproxy);
    }

    public void FireLaser()
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            buttons = 1
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, game_proxy);
    }

    public void GameStart()
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            buttons = 2
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, game_proxy);
    }

    public void Guided()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = (uint)MAVLink.COPTER_MODE.GUIDED
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void SendDistSensor()
    {
        MAVLink.mavlink_distance_sensor_t cmd = new MAVLink.mavlink_distance_sensor_t
        {
            orientation = 10
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.DISTANCE_SENSOR, cmd);
        sock.SendTo(data, myproxy);
    }
}
