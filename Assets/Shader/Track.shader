﻿Shader "Arcade/Track"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Speed ("Speed", Float) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent"  "RenderType"="Opaque" "CanUseSpriteAtlas"="true"  }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float _Speed;
			sampler2D _MainTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
			    float2 p = i.uv;
				p.y = (p.y + _Time.y * abs(_Speed) * 3) % 1;
				return tex2D(_MainTex,p);
			}
			ENDCG
		}
	}
}
