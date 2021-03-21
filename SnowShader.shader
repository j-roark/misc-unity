// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

   Shader "Snow Shader" {
        Properties {
            _Tess ("Tessellation", Range(1,32)) = 4
            _SnowTexture ("Snow (RGB)", 2D) = "white" {}
            _SnowColor ("SnowColor", color) = (1,1,1,0)
            _GroundTexture ("Ground (RGB)", 2D) = "white" {}
            _GroundColor ("GroundColor", color) = (1,1,1,0)
            _DispTex ("Disp Texture", 2D) = "black" {}
            _NormalMap ("Normalmap", 2D) = "bump" {}
            _Displacement ("Displacement", Range(0, 1.0)) = 0.3
        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
            #pragma target 4.6
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float _Tess;

            float4 tessDistance (appdata v0, appdata v1, appdata v2) {
                float minDist = 10.0;
                float maxDist = 25.0;
                return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
            }

            sampler2D _DispTex;
            float _Displacement;

            void disp (inout appdata v)
            {
                float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement;
                v.vertex.xyz -= v.normal * d;
                v.vertex.xyz += v.normal * _Displacement;
            }

            struct Input {
                float2 uv_GroundTexture;
                float2 uv_SnowTexture;
                float2 uv_DispTex;
            };

            sampler2D _GroundTexture;
            sampler2D _SnowTexture;
            sampler2D _NormalMap;
            fixed4 _GroundColor;
            fixed4 _SnowColor;

            //half _Glossiness;
            //half _Metallic;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf (Input IN, inout SurfaceOutput o) {
                half amount = tex2Dlod(_DispTex, float4(IN.uv_DispTex,0,0)).r;
                fixed4 c = lerp(
                    tex2D (_SnowTexture, IN.uv_SnowTexture) * _SnowColor,
                    tex2D (_GroundTexture, IN.uv_GroundTexture) * _GroundColor,
                    amount
                );
                o.Albedo = c.rgb;
                o.Specular = 0.9;
                o.Gloss = 1.0;
                //o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_SnowTexture));
                o.Alpha = c.a;
            }
            ENDCG
        }
        FallBack "Diffuse"
    }
