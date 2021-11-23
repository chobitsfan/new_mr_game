﻿Shader "CamNV12ToRGB"
{
    Properties
    {
        _YTex("Y", 2D) = "white" {}
        _UVTex("UV", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.uv.y = 1 - o.uv.y;
                return o;
            }

            sampler2D _YTex;
            sampler2D _UVTex;

            float4 frag(v2f i) : SV_Target
            {
                //float2 uv = float2(i.uv.x, 1 - i.uv.y);
                float4 ycol = tex2D(_YTex, i.uv);
                fixed4 uv4 = tex2D(_UVTex, i.uv);
                float y = 1.1643 * (ycol.r - 0.0625);
                float u = uv4.r - 0.5; 
				float v = uv4.g - 0.5;
                
				float r = y + 1.403 * v;
                float g = y - 0.344  * u - 0.714 * v;
                float b = y + 1.770  * u;				
                
				//float r = ycol.a + 1.4022 * vcol.a - 0.7011;
                //float g = ycol.a - 0.3456 * ucol.a - 0.7145 * vcol.a + 0.53005;
                //float b = ycol.a + 1.771 * ucol.a - 0.8855;

                return float4(r, g, b, 1);
            }
            ENDCG
        }
    }
}