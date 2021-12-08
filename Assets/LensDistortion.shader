Shader "Effect/LensDistortion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		
		k2R("k1R",Float) = 0.61		//with apect ratio, reversed
		k3R("k2R",Float) = 0.29

		k2G("k1G",Float) = 0.65
		k3G("k2G",Float) = 0.3

		k2B("k1B",Float) = 0.69
		k3B("k2B",Float) = 0.31
		

	
		/*k2R("k1R",Float) = 0.9		//with apect ratio, reversed
		k3R("k2R",Float) = 0.1

		k2G("k1G",Float) = 0.9
		k3G("k2G",Float) = 0.1

		k2B("k1B",Float) = 0.9
		k3B("k2B",Float) = 0.1*/

		
		/*k2R("k1R",Float) = 0.90		//with apect ratio
		k3R("k2R",Float) = 0.31

		k2G("k1G",Float) = 0.8
		k3G("k2G",Float) = 0.3

		k2B("k1B",Float) = 0.70
		k3B("k2B",Float) = 0.29*/
		

		/*
		k2R("k1R",Float) = 0.70	//without apect ratio, reversed
		k3R("k2R",Float) = 0.29

		k2G("k1G",Float) = 0.8
		k3G("k2G",Float) = 0.3

		k2B("k1B",Float) = 0.90
		k3B("k2B",Float) = 0.31 */

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

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				float k1R, k2R;
				float k1G, k2G;
				float k1B, k2B;

				// Distortion equations taken from http://www.vision.caltech.edu/bouguetj/calib_doc/htmls/parameters.html
				float4 frag(v2f i) : COLOR
				{
					float2 aspectRatio = float2(1.7778f, 1.0f);
					//float2 aspectRatio = float2(1.0f, 0.562f);

//#if 1				//normalized to +-0.5

					float2 uv = (float2(i.uv.x, i.uv.y) - float2(0.5f, 0.5f)) * aspectRatio;
					float r2 = uv.x * uv.x + uv.y * uv.y; //distance from the center

					//uv = uv * (1.0f + k1R * r2 + k2R * r2 * r2) + float2(0.5f, 0.5f);
					//compal settings
					uv = uv * 1.6; //scaler for compal
					float2 uvR = (uv * (1.0f + k1R * r2 + k2R * r2 * r2)) / aspectRatio + float2(0.5f, 0.5f);
					float2 uvG = (uv * (1.0f + k1G * r2 + k2G * r2 * r2)) / aspectRatio + float2(0.5f, 0.5f);
					float2 uvB = (uv * (1.0f + k1B * r2 + k2B * r2 * r2)) / aspectRatio + float2(0.5f, 0.5f);

					//phantontree settings
					/*uv = uv * 1.2; //scaler for photon
					float2 uvR = uv / aspectRatio + float2(0.5f, 0.5f);
					float2 uvG = uv / aspectRatio + float2(0.5f, 0.5f);
					float2 uvB = uv / aspectRatio + float2(0.5f, 0.5f);*/

					//#else				//normalize to +-1
/*					float2 uv = (float2(i.uv.x, i.uv.y) * 2 - float2(1.0f, 1.0f)) ;
					float r2 = uv.x * uv.x + uv.y * uv.y; //distance from the center

					float2 uvR = (uv * (1.0f + k1R * r2 + k2R * r2 * r2) *0.5 )  + float2(0.5f, 0.5f);
					float2 uvG = (uv * (1.0f + k1G * r2 + k2G * r2 * r2) *0.5 )  + float2(0.5f, 0.5f);
					float2 uvB = (uv * (1.0f + k1B * r2 + k2B * r2 * r2) *0.5 )  + float2(0.5f, 0.5f);
*/
//#endif

					float4 colG = tex2D(_MainTex, uvG);
/*#if 1		//one single distortion for all color
					fixed4 output = colG;
					if (uvG.x < 0 || uvG.x>1 || uvG.y < 0 || uvG.y>1)
						return float4(0, 0, 0, 0);
					else
						return output;
#else	*/	//3 different distortions for RGB
					float4 colR = tex2D(_MainTex, uvR);
					float4 colB = tex2D(_MainTex, uvB);

					float4 output = float4(colR.r, colG.g, colB.b, 1.0f);
					if (uvR.x < 0 || uvR.x>1 || uvR.y < 0 || uvR.y>1 ||
						uvG.x < 0 || uvG.x>1 || uvG.y < 0 || uvG.y>1 ||
						uvB.x < 0 || uvB.x>1 || uvB.y < 0 || uvB.y>1) 
						return float4(0, 0, 0, 0);
					else 
						return output;
//#endif

				}
				ENDCG
			}
		}
}
