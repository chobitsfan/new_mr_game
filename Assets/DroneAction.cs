using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class MoCapData
{
    public Vector3 pos;
    public Quaternion rot;
    public ulong ts;
}

public class DroneAction : MonoBehaviour
{
    public int MavId;
    public GameWorld gameWorld;
    [System.NonSerialized]
    public Vector3 CurPos;
    [System.NonSerialized]
    public Quaternion CurRot;
    public GameObject FpvCameraPrefab;
    public GameObject DroneModelPrefab;

    public bool Tracked => _tracked;

    bool IsPlayer = false;
    byte[] buf = new byte[512];
    MAVLink.MavlinkParse mavlinkParse = new MAVLink.MavlinkParse();
    Socket sock;
    IPEndPoint myproxy;
    uint apm_mode = 0;
    bool armed = false;
    float hb_cd = 2f;
    IPEndPoint game_proxy;
    VirtualAction virtualAction;
    Queue<MoCapData> moCapDataQueue = new Queue<MoCapData>();
    const ulong RTSP_BUF_DELAY_US = 100000;
    float lastMocapDataTs = 0;
    private bool _tracked = false;
    ulong mocapTimeOffsetUs = 0;
    bool skipNxtHb = false;
    bool sys_status_rcved = false;

    private void Awake()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Any, 17800 + MavId));
        myproxy = new IPEndPoint(IPAddress.Parse(MyGameSetting.MocapIp), 17500);
        game_proxy = new IPEndPoint(IPAddress.Parse(MyGameSetting.MocapIp), 27500);
    }

    // Start is called before the first frame update
    void Start()
    {
        virtualAction = gameObject.GetComponent<VirtualAction>();
        if (MavId == MyGameSetting.PlayerDroneId)
        {
            IsPlayer = true;
            Instantiate(FpvCameraPrefab, gameObject.transform);
        }
        else
        {
            Instantiate(DroneModelPrefab, gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MyGameSetting.UseMocap)
        {
            lastMocapDataTs += Time.deltaTime;
        }
        MoCapData delayedMoCapData = null;
        ulong now_ts = (ulong)(Time.time * 1000000);
        //Debug.LogError("moCapDataQueue count " + moCapDataQueue.Count);
        while (moCapDataQueue.Count > 0)
        {
            MoCapData moCapData = moCapDataQueue.Peek();
            if ((now_ts + mocapTimeOffsetUs - moCapData.ts) >= RTSP_BUF_DELAY_US)
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
            sock.SendTo(data, myproxy); // pass windows firewall
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
                                if (IsPlayer) gameWorld.ShowStatus(System.Text.Encoding.ASCII.GetString(status_txt.text));
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT:
                            {
                                if (msg.sysid == MavId)
                                {
                                    if (skipNxtHb)
                                    {
                                        skipNxtHb = false;
                                    }
                                    else
                                    {
                                        var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                                        apm_mode = heartbeat.custom_mode;
                                        armed = (heartbeat.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) != 0;
                                    }
                                    if (IsPlayer && !sys_status_rcved)
                                    {
                                        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
                                        {
                                            command = (ushort)MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                                            param1 = (float)MAVLink.MAVLINK_MSG_ID.SYS_STATUS,
                                            param2 = 1000000
                                        };
                                        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
                                        sock.SendTo(data, myproxy);
                                    }
                                }
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.ATT_POS_MOCAP:
                            {
                                var att_pos = (MAVLink.mavlink_att_pos_mocap_t)msg.data;
                                //convert from optitrack motive coordinate system to unity coordinate system
                                MoCapData moCapData = new MoCapData
                                {
                                    ts = att_pos.time_usec,
                                    pos = new Vector3(-att_pos.x, att_pos.y, att_pos.z),
                                    rot = new Quaternion(-att_pos.q[1], att_pos.q[2], att_pos.q[3], -att_pos.q[0])
                                };
                                moCapDataQueue.Enqueue(moCapData);
                                CurPos = moCapData.pos;
                                CurRot = moCapData.rot;
                                lastMocapDataTs = 0;
                                _tracked = true;
                                if (mocapTimeOffsetUs == 0)
                                {
                                    mocapTimeOffsetUs = att_pos.time_usec - (ulong)(Time.time * 1000000);
                                    //Debug.LogError("time offset " + mocapTimeOffsetUs);
                                }
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL:
                            {
                                var ctrl = (MAVLink.mavlink_manual_control_t)msg.data;
                                if (ctrl.buttons == 1)
                                {
                                    virtualAction.Shot();
                                }
                                break;
                            }
                        case (uint)MAVLink.MAVLINK_MSG_ID.SYS_STATUS:
                            {
                                sys_status_rcved = true;
                                var sys_status = (MAVLink.mavlink_sys_status_t)msg.data;
                                gameWorld.UpdateBatDisplay(sys_status.voltage_battery);
                                break;
                            }

                    }
                }
            }
        }
        if (lastMocapDataTs > MyGameSetting.LostTrackTime)
        {
            _tracked = false;
            if (IsPlayer)
            {
                //Debug.LogError("lost track, force landing");
                gameWorld.ShowHudInfo("lost track");
                Land();
            }
        }
    }

    /*public bool IsGuided()
    {
        return apm_mode == (uint)MAVLink.COPTER_MODE.GUIDED;
    }*/

    public bool IsAltHold()
    {
        return apm_mode == (uint)MAVLink.COPTER_MODE.ALT_HOLD;
    }

    /*public bool IsPosHold()
    {
        return apm_mode == (uint)MAVLink.COPTER_MODE.POSHOLD;
    }*/

    public bool IsArmed()
    {
        return armed;
    }

    public void TakeOff()
    {

        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            command = (ushort)MAVLink.MAV_CMD.TAKEOFF,
            param3 = 1f,
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
        skipNxtHb = true;
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
            target_system = (byte)MavId,
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
            target_system = (byte)MavId,
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
            target_system = (byte)MavId,
            custom_mode = (uint)MAVLink.COPTER_MODE.AUTO
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void ManualControl(short pitch, short roll, short throttle, short yaw)
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            target = (byte)MavId,
            x = pitch,
            y = roll,
            z = throttle,
            r = yaw
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, myproxy);
    }

    /*public void FireLaser()
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            buttons = 1
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, game_proxy);
    }

    public void SendGirlSync()
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            buttons = 3
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, game_proxy);
    }*/

    /*public void Guided()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = (uint)MAVLink.COPTER_MODE.GUIDED
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }*/

    public void AltHold()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = (uint)MAVLink.COPTER_MODE.ALT_HOLD
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
