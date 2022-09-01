using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GstCustomTexture))]
public class CustomPipelinePlayer : BaseVideoPlayer {

	// Use this for initialization
	protected override string _GetPipeline()
	{
		string P = "rtspsrc location=" + MyGameSetting.FpvUrl + " latency=0 ! queue ! rtph264depay ! avdec_h264 ! video/x-raw,format=I420 ! videoconvert ! appsink name=videoSink";

		return P;
	}
}
