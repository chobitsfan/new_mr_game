public static class MyGameSetting
{
    public static int GameDurationSec = 30;
    public static float FenceAltHigh = 2f;
    public static float FenceAltLow = 0.7f;
    public static float FenceXMax = 2f;
    public static float FenceXMin = -2f;
    public static float FenceZMax = 2f;
    public static float FenceZMin = -2f;
    public static string MocapIp = "127.0.0.1";
    public static int PlayerDroneId = 1;
    public static int PlayerDroneId2 = 0;
    public static string FpvUrl = "";
    public static string FpvUrl2 = "";
    public static bool UseMocap = true;
    public static int WinScore = 10;
    public static float LostTrackTime = 0.03f;
    public static int ThrMax = 700;
    public static int YawMax = 700;
    public static int PitchRollMax = 700;
    public static float AvoidDist = 2f;
    static MyGameSetting()
    {
        string[] lines;
        try
        {
            lines = System.IO.File.ReadAllLines("my_game_setting.txt");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("read game setting " + e);
            return;
        }
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            if (data.Length == 2)
            {
                switch (data[0])
                {
                    case "AvoidDist":
                        AvoidDist = float.Parse(data[1]);
                        break;
                    case "ThrMax":
                        ThrMax = int.Parse(data[1]);
                        break;
                    case "YawMax":
                        YawMax = int.Parse(data[1]);
                        break;
                    case "PitchRollMax":
                        PitchRollMax = int.Parse(data[1]);
                        break;
                    case "LostTrackTime":
                        LostTrackTime = float.Parse(data[1]);
                        break;
                    case "FenceAltHigh":
                        FenceAltHigh = float.Parse(data[1]);
                        break;
                    case "FenceAltLow":
                        FenceAltLow = float.Parse(data[1]);
                        break;
                    case "FenceXMax":
                        FenceXMax = float.Parse(data[1]);
                        break;
                    case "FenceXMin":
                        FenceXMin = float.Parse(data[1]);
                        break;
                    case "FenceZMax":
                        FenceZMax = float.Parse(data[1]);
                        break;
                    case "FenceZMin":
                        FenceZMin = float.Parse(data[1]);
                        break;
                    case "MocapIp":
                        MocapIp = data[1];
                        break;
                    case "PlayerDroneId":
                        PlayerDroneId = int.Parse(data[1]);
                        break;
                    case "PlayerDroneId2":
                        PlayerDroneId2 = int.Parse(data[1]);
                        break;
                    case "FpvUrl":
                        FpvUrl = data[1];
                        break;
                    case "FpvUrl2":
                        FpvUrl2 = data[1];
                        break;
                    case "UseMocap":
                        if (data[1] == "No")
                        {
                            UseMocap = false;
                        }
                        break;
                    case "GameDurationSec":
                        GameDurationSec = int.Parse(data[1]);
                        break;
                    case "WinScore":
                        WinScore = int.Parse(data[1]);
                        break;
                }
            }
        }
    }

}