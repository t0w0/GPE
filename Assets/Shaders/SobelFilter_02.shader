Shader "PostEffect/SobelFilter_02" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DeltaX ("Delta X", Float) = 0.01
		_DeltaY ("Delta Y", Float) = 0.01
		_DepthLevel ("Depth Level", Range(1, 3)) = 1
		_EdgePresence ("Edge Presence", Range(0, 1)) = 0.5
		_ImagePresence ("Image Presence", Range(0, 1)) = 0.5
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGINCLUDE

		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
        uniform sampler2D _CameraDepthTexture;
        uniform fixed _DepthLevel;
        uniform half4 _MainTex_TexelSize;
        uniform float _EdgePresence;
        uniform float _ImagePresence;
        uniform float4 _EdgeColor;

		float _DeltaX;
		float _DeltaY;

		struct v2f {
             float4 pos : SV_POSITION;
             half2 uv : TEXCOORD0;
         };

	     v2f vert(appdata_full v) {
             v2f o;
             o.pos = UnityObjectToClipPos(v.vertex);
             o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.vertex.xy);
             // why do we need this? cause sometimes the image I get is flipped. see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
             #if UNITY_UV_STARTS_AT_TOP
             	if (_MainTex_TexelSize.y < 0)
                    o.uv.y = 1 - o.uv.y;
             #endif

            return o;
         }

		float4 sobel (sampler2D tex, float2 uv) {
			float2 delta = float2(_DeltaX, _DeltaY);
			
			float4 hr = float4(0, 0, 0, 0);
			float4 vt = float4(0, 0, 0, 0);
			float r;
			float d;
			float p;
			float c;
			
			hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
			hr += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  0.0;
			hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
			hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
			hr += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
			hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
			hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
			hr += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) *  0.0;
			hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
			
			vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
			vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
			vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
			vt += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  0.0;
			vt += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
			vt += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) *  0.0;
			vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
			vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
			vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
			
			r = sqrt(hr * hr + vt * vt);
			d = _EdgePresence;
			p = _ImagePresence;
			c = _EdgeColor;
			if (p > 0)
				return ( 1 - float4 ( r*d, r*d, r*d, 1) *c) *1-p/tex2D(_MainTex, uv);
			return ( 1 - float4 ( r*d, r*d, r*d, 1 ) * c );
			//return sqrt(hr * hr + vt * vt);
		}
		
		float4 frag (v2f i) : COLOR {
			//float4 s = sobel(_CameraDepthTexture, i.uv);
			float4 s = sobel(_MainTex, i.uv);
			//float s = tex2D(_CameraDepthTexture, i.uv);
			//float s = tex2D(_MainTex, i.uv);
			//float n = tex2D(_MainTex, i.uv);
			//s *= n/10;

			//float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
            //depth = pow(Linear01Depth(depth), _DepthLevel);
            //s = depth;

            //return depth;
            return s;
            //return 1-float4(s, s, s, 1);
		}
		
		ENDCG
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}