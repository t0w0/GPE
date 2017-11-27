Shader "Normals/ShadowsFromNormals_01" {

	Properties {
		_CameraDepthTexture ("Albedo (RGB)", 2D) = "white" {}
		_TopColor ("Top Color", Color) = (0, 0, 0, 1)
		_Color ("Color", Color) = (0, 0, 0, 1)
		_ShadowsIntensity ("Shadow Intensity", Float) = 0.02
		_TopFaceAngle ( "Top Face Angle", Vector ) = ( 0, 0, 0, 0)

		[HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 300
		Pass {
			
			Blend [_SrcBlend] [_DstBlend]
            ZWrite On ZTest LEqual

			CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #include "UnityCG.cginc"

	        uniform float4 _Color;
	        uniform float4 _TopColor;
	        uniform float _ShadowsIntensity;
	        uniform float4 _TopFaceAngle;

	        struct v2f {
	            float4 pos : SV_POSITION;
	            fixed4 color : COLOR;
	        };
	        
	        v2f vert ( appdata_full v) {
	           	v2f o;
                o.pos = UnityObjectToClipPos( v.vertex );
                float m = dot( v.normal, _TopFaceAngle );
              	if ( m > 0) {
              		o.color = _Color * ( 1 - m ) + _TopColor * ( m );
              	}
              	//else if ( abs( v.normal.x ) < 0.5 && abs( v.normal.x ) > -0.5 ) {
              	//	o.color = _Color * (1 - _ShadowsIntensity);
              	//}
              	else {
              		o.color = _Color;
              		//o.color = _Color * ( 1 - v.normal.x * _ShadowsIntensity );
              	}
                return o;
	        }
	        
	        fixed4 frag (v2f i) : COLOR {
	        	//i.depth = UNITY_TRANSFER_DEPTH(i.pos);
	        	//i.color.b = UNITY_OUTPUT_DEPTH(i.pos);
	        	//return 1-UNITY_TRANSFER_DEPTH(i.pos);
	        	return fixed4 (i.color.xyz, 1.0);
	        }
			ENDCG
		}
	} 
	FallBack "Diffuse"
}