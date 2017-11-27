Shader "PosteEffect/SobelFilter_a01" {

	Properties {
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_EdgeWidth ( "Edge Width", Float ) = 0.001
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {

			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;

			//user defined variable
			float _EdgeWidth;

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
				float s = sobel( _MainTex, i.uv );
				return float4( s, s, s, 1.0 );
			}
			
			ENDCG
		}
	}
	//fallback commented out during development
	//FallBack "Diffuse"
}
