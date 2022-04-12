using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MoCapObj : MonoBehaviour
{
    public int StreamId;
    [System.NonSerialized]
    public Vector3 CurPos;
    [System.NonSerialized]
    public Quaternion CurRot;

    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;

    // Start is called before the first frame update
    void Start()
    {
        buf = new byte[512];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Any, 17500 + StreamId));
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
                    switch (msg.msgid)
                    {
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
                                transform.SetPositionAndRotation(moCapData.pos, Quaternion.Lerp(transform.rotation, moCapData.rot, 0.5f));
                                break;
                            }
                    }
                }
            }
        }
    }
}


