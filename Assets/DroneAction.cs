using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class DroneAction : MonoBehaviour
{
    public int DroneID;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;
    IPEndPoint myproxy;
    // Start is called before the first frame update
    void Start()
    {
        buf = new byte[1024];
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
        int recvBytes = 0;
        try
        {
            recvBytes = sock.Receive(buf);
        }
        catch (SocketException) { }
        if (recvBytes > 0)
        {
            MAVLink.MAVLinkMessage msg = mavlinkParse.ReadPacket(buf);
            if (msg != null)
            {
                if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                {
                    var status_txt = (MAVLink.mavlink_statustext_t)msg.data;
                    Debug.Log(System.Text.Encoding.ASCII.GetString(status_txt.text));
                }
                else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                {
                    var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                }
            }
        }
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
}
