﻿// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Instanced/DrawMeshInstanced" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#pragma target 5.0

		#include "../Cginc/Transform.cginc"
		#include "../Cginc/Color.cginc"

		struct TransformStruct {
			float3 translate;
			float3 rotation;
			float3 scale;
			float sumTime;
			float speed;
		};
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<TransformStruct> _TransformBuff;
		#endif

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void setup()
		{
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			TransformStruct t = _TransformBuff[unity_InstanceID];
			unity_ObjectToWorld = mul(translate_m(t.translate), mul(rotate_m(t.rotation), scale_m(t.scale)));
			#endif
		}
		void surf (Input IN, inout SurfaceOutputStandard o) {
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			TransformStruct t = _TransformBuff[unity_InstanceID];
			o.Albedo = rgb((length(t.translate) % 360) / 360, 1.0, 1.0) + 0.7;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
			#endif
		}
		ENDCG
	}
	FallBack "Diffuse"
}
