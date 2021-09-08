﻿Shader "CamYUVToRGB"
{
    Properties
    {
        _YTex("Y", 2D) = "white" {}
        _UTex("U", 2D) = "white" {}
        _VTex("V", 2D) = "white" {}
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
                return o;
            }

            sampler2D _YTex;
            sampler2D _UTex;
            sampler2D _VTex;

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = float2(i.uv.x, 1 - i.uv.y);
                float4 ycol = tex2D(_YTex, uv);
                float4 ucol = tex2D(_UTex, uv);
                float4 vcol = tex2D(_VTex, uv);
                float r = ycol.a + 1.4022 * vcol.a - 0.7011;
                float g = ycol.a - 0.3456 * ucol.a - 0.7145 * vcol.a + 0.53005;
                float b = ycol.a + 1.771 * ucol.a - 0.8855;

                return float4(r, g, b, 1);
            }
            ENDCG
        }
    }
}
