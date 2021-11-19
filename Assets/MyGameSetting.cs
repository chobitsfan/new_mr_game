public static class MyGameSetting
{
    public const float GameDurationSec = 90;
    public static float FenceAltHigh = 2f;
    public static float FenceAltLow = 0.7f;
    public static float FenceXMax = 2f;
    public static float FenceXMin = -2f;
    public static float FenceZMax = 2f;
    public static float FenceZMin = -2f;
    public static string MocapIp = "127.0.0.1";
    public static int PlayerDroneId = 1;
    public static int EmeryDroneId = 2;
    public static string FpvUrl = "";
    public static UnityEngine.Color PlayerBeamColor;
    public static UnityEngine.Color EmeryBeamColor;

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
                    case "EmeryDroneId":
                        EmeryDroneId = int.Parse(data[1]);
                        break;
                    case "FpvUrl":
                        FpvUrl = data[1];
                        break;
                    case "PlayerBeamColor":
                        {
                            string[] color_txt = data[1].Split('.');
                            int r = int.Parse(color_txt[0]);
                            int g = int.Parse(color_txt[1]);
                            int b = int.Parse(color_txt[2]);
                            PlayerBeamColor = new UnityEngine.Color(r / 255.0f, g / 255.0f, b / 255.0f);
                        }
                        break;
                    case "EmeryBeamColor":
                        {
                            string[] color_txt = data[1].Split('.');
                            int r = int.Parse(color_txt[0]);
                            int g = int.Parse(color_txt[1]);
                            int b = int.Parse(color_txt[2]);
                            EmeryBeamColor = new UnityEngine.Color(r / 255.0f, g / 255.0f, b / 255.0f);
                        }
                        break;
                }
            }
        }
    }

}