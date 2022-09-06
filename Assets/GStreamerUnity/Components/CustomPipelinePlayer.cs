using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GstCustomTexture))]
public class CustomPipelinePlayer : BaseVideoPlayer {

	protected override string _GetPipeline()
	{
		return "rtspsrc location=" + transform.parent.GetComponent<DroneAction>().fpvUrl + " latency=0 ! queue ! rtph264depay ! avdec_h264 ! video/x-raw,format=I420 ! videoconvert ! appsink name=videoSink";
	}
}
