﻿Shader "Arcade/Shadow"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_From ("From", Float) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent"  "RenderType"="Opaque" "CanUseSpriteAtlas"="true"  }

        Cull Front
        Lighting Off
		ZWrite Off
        Blend One OneMinusSrcAlpha
		LOD 100

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

			float _From;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
			    if(i.uv.y < _From) return 0;
				float4 c = _Color;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
