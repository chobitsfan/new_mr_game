using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class DroneAction : MonoBehaviour
{
    public int DroneID;
    public UnityEngine.UI.Text StatusText;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;
    IPEndPoint myproxy;
    uint apm_mode = 0;
    bool armed = false;

    // Start is called before the first frame update
    void Start()
    {
        buf = new byte[512];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Loopback, 17500 + DroneID));
        myproxy = new IPEndPoint(IPAddress.Loopback, 17500);
    }

    // Update is called once per frame
    void Update()
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
                    if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                    {
                        var status_txt = (MAVLink.mavlink_statustext_t)msg.data;
                        //Debug.Log(System.Text.Encoding.ASCII.GetString(status_txt.text));
                        StatusText.text = System.Text.Encoding.ASCII.GetString(status_txt.text);
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                        apm_mode = heartbeat.custom_mode;
                        armed = (heartbeat.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) != 0;
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.ATT_POS_MOCAP)
                    {
                        var att_pos = (MAVLink.mavlink_att_pos_mocap_t)msg.data;
                        gameObject.transform.localPosition = new Vector3(-att_pos.x, att_pos.y, att_pos.z);
                        gameObject.transform.localRotation = new Quaternion(-att_pos.q[1], att_pos.q[2], att_pos.q[3], -att_pos.q[0]);
                    }
                }
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
            target_system = (byte)DroneID,
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
            target_system = (byte)DroneID,
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
            target_system = (byte)DroneID,
            custom_mode = (uint)MAVLink.COPTER_MODE.AUTO
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, myproxy);
    }

    public void ManualControl(short pitch, short roll, short throttle, short yaw)
    {
        MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
        {
            target = (byte)DroneID,
            x = pitch,
            y = roll,
            z = throttle,
            r = yaw
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
        sock.SendTo(data, myproxy);
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
