Shader "PostEffect/SobelFilter_01" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DeltaX ("Delta X", Float) = 0.01
		_DeltaY ("Delta Y", Float) = 0.01
		_DepthLevel ("Depth Level", Range(1, 3)) = 1
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

		float _DeltaX;
		float _DeltaY;

		struct input {
             float4 pos : POSITION;
             half2 uv : TEXCOORD0;
         };
 
	     struct output
	     {
	         float4 pos : SV_POSITION;
	         half2 uv : TEXCOORD0;
	     };

	     output vert(input i)
         {
             output o;
             o.pos = UnityObjectToClipPos(i.pos);
             o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, i.uv);
             // why do we need this? cause sometimes the image I get is flipped. see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
             #if UNITY_UV_STARTS_AT_TOP
             if (_MainTex_TexelSize.y < 0)
                     o.uv.y = 1 - o.uv.y;
             #endif

             return o;
         }

		float sobel (sampler2D tex, float2 uv) {
			float2 delta = float2(_DeltaX, _DeltaY);
			
			float4 hr = float4(0, 0, 0, 0);
			float4 vt = float4(0, 0, 0, 0);
			
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
			
			return sqrt(hr * hr + vt * vt);
		}
		
		float4 frag (output IN) : COLOR {
			//float s = sobel(_CameraDepthTexture, IN.uv);
			float s = sobel(_MainTex, IN.uv);
			//float s = tex2D(_MainTex, IN.uv);
			//s = 1 - s;

			//float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, IN.uv));
            //depth = pow(Linear01Depth(depth), _DepthLevel);
            //depth = 1-depth;

            //return depth;
			return 1-float4(s, s, s, 1);
			//return 1- float4(s*depth, s*depth, s*depth, 1);
		}
		
		ENDCG
		
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}