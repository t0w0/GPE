Shader "PosteEffect/SobelFilter_03" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EdgeWidth ("EdgeWidth", Float) = 0.01
		//_DepthLevel ("Depth Level", Range(1, 3)) = 1
		//_EdgePresence ("Edge Presence", Range(0, 1)) = 0.5
		//_ImagePresence ("Image Presence", Range(0, 1)) = 0.5
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
	}
	
	SubShader {
		Pass {

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma target 2.0
			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			
			sampler2D _MainTex;
	        uniform sampler2D _CameraDepthTexture;
	        uniform fixed _DepthLevel;
	        uniform half4 _MainTex_TexelSize;
	        uniform float _EdgePresence;
	        uniform float _ImagePresence;
	        uniform fixed4 _EdgeColor;

			float _EdgeWidth;

	         struct vsIn {
				float4 position : POSITION;
			};

			struct vsOut {
				float4 screenPosition : SV_POSITION;
				half2 uv : TEXCOORD0;
			};


		    vsOut vertexShader (vsIn v) 
		    {
	            vsOut o;
	            o.screenPosition = UnityObjectToClipPos(v.position);
	            o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.position.xy);
	            // why do we need this? cause sometimes the image I get is flipped. see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
	            #if UNITY_UV_STARTS_AT_TOP
	            	if (_MainTex_TexelSize.y < 0)
	                    o.uv.y = 1 - o.uv.y;
	            #endif

	            return o;
	        }
			
	        float4 sobel (sampler2D tex, half2 uv)
	        {
				float2 delta = float2(_EdgeWidth, _EdgeWidth);
				
				float4 hr = float4(0, 0, 0, 0);
				float4 vt = float4(0, 0, 0, 0);
				float s;
				float4 c;
				float4 ec = _EdgeColor;
				
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
				
				//On calcule le float sobel qui varie de 0 à 1;
				s = sqrt(hr * hr + vt * vt);
				//on l'inverse pour afficher le contour noir (1-0)
				//On récupère la couleur du rendu avant d'appliquer le filtre sobel
				c = tex2D(tex, uv);
				//On multiplie la couleur du contour par le facteur sobel
				ec *= s;

				//On retourne la couleur originale multiplié par 
				return c*(1-ec);
			}

			float4 fragmentShader (vsOut psIn) : SV_Target 
			{
				float4 s = sobel(_MainTex, psIn.uv);
	            return s;
			}

			ENDCG
		}
	}
}