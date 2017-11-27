Shader "PostEffect/SobelFilter_a02" {

	Properties {
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_BackgroundColor ( "Background Color", Color ) = ( 1.0, 1.0, 1.0, 1.0)
		_ImagePresence ( "Image Presence", Range ( 0.0, 1.0 ) ) = 1.0
		_EdgePresence ( "Edge Presence", Range ( 0.0, 1.0 ) ) = 1.0
		_EdgeColor ( "Edge Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_EdgeWidth ( "Edge Width", Float ) = 0.001
		_DepthLevel ( "Depth Level", Range ( 0, 5 ) ) = 1
	}
	
	SubShader {
		Pass {

			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			//user defined variable
			float4 _BackgroundColor;
			float _ImagePresence;
			float _EdgePresence;
			float _EdgeWidth;
			float4 _EdgeColor;
			float _DepthLevel;


			struct vertexInput {
				float4 vertex : POSITION;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			vertexOutput vert ( vertexInput v) {
				vertexOutput o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.vertex.xy );
				return o;
			}
			
			float sobel ( sampler2D tex, float2 uv ) {
				float2 delta = float2( _EdgeWidth, _EdgeWidth );
				
				float4 hr = float4( 0, 0, 0, 0 );
				float4 vt = float4( 0, 0, 0, 0 );
				
				hr += tex2D( tex, ( uv + float2( -1.0, -1.0 ) * delta ) ) *  1.0;
				hr += tex2D( tex, ( uv + float2(  0.0, -1.0 ) * delta ) ) *  0.0;
				hr += tex2D( tex, ( uv + float2(  1.0, -1.0 ) * delta ) ) * -1.0;
				hr += tex2D( tex, ( uv + float2( -1.0,  0.0 ) * delta ) ) *  2.0;
				hr += tex2D( tex, ( uv + float2(  0.0,  0.0 ) * delta ) ) *  0.0;
				hr += tex2D( tex, ( uv + float2(  1.0,  0.0 ) * delta ) ) * -2.0;
				hr += tex2D( tex, ( uv + float2( -1.0,  1.0 ) * delta ) ) *  1.0;
				hr += tex2D( tex, ( uv + float2(  0.0,  1.0 ) * delta ) ) *  0.0;
				hr += tex2D( tex, ( uv + float2(  1.0,  1.0 ) * delta ) ) * -1.0;
				
				vt += tex2D( tex, ( uv + float2( -1.0, -1.0 ) * delta ) ) *  1.0;
				vt += tex2D( tex, ( uv + float2(  0.0, -1.0 ) * delta ) ) *  2.0;
				vt += tex2D( tex, ( uv + float2(  1.0, -1.0 ) * delta ) ) *  1.0;
				vt += tex2D( tex, ( uv + float2( -1.0,  0.0 ) * delta ) ) *  0.0;
				vt += tex2D( tex, ( uv + float2(  0.0,  0.0 ) * delta ) ) *  0.0;
				vt += tex2D( tex, ( uv + float2(  1.0,  0.0 ) * delta ) ) *  0.0;
				vt += tex2D( tex, ( uv + float2( -1.0,  1.0 ) * delta ) ) * -1.0;
				vt += tex2D( tex, ( uv + float2(  0.0,  1.0 ) * delta ) ) * -2.0;
				vt += tex2D( tex, ( uv + float2(  1.0,  1.0 ) * delta ) ) * -1.0;
				
				return sqrt(hr * hr + vt * vt);
			}
			
			float4 frag ( vertexOutput i ) : COLOR {
				
				//Calculate the Edges and apply color and opacity
				float sobelFact = sobel( _MainTex, i.uv );
				float4 edgeColored = _EdgeColor * sobelFact * _EdgePresence;
				float4 edgeSub = sobelFact * _EdgePresence;
				
				//Get The Depth of the object and apply it to the edge oppacity
				float depth = UNITY_SAMPLE_DEPTH( tex2D( _CameraDepthTexture, i.uv ) );
				depth = pow( Linear01Depth( depth ), _DepthLevel );
				edgeColored *= 1-depth;
				edgeSub *= 1- depth;

				//Find the inputImageTexture and apply opacity
				float4 mainTexColor = tex2D( _MainTex, i.uv );
				mainTexColor *= _ImagePresence;
				//mainTexColor = mainTexColor - ( 1 - edgeColored );

				float4 backgroundColor = _BackgroundColor * ( 1 - _ImagePresence );

				//mainTexColor -= sobelFact * _EdgePresence;

				//return depth;
				//return edgeColored;
				//return mainTexColor;
				return edgeColored + (mainTexColor - edgeSub) + backgroundColor;
				//return 1 - (edgeColored + mainTexColor + backgroundColor);
			}
			
			ENDCG
		}
	}
	//fallback commented out during development
	FallBack "Diffuse"
}
