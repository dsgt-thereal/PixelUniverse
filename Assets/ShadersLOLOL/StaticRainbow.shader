Shader "Custom/TVStaticRainbow"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Intensity ("Noise Intensity", Range(0, 1)) = 0.5
        _Speed ("Flicker Speed", Range(1, 50)) = 10
        _Rainbow ("Rainbow Mode", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Intensity;
            float _Speed;
            float _Rainbow;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * _Speed;
                float noise = random(i.uv * t);
                float brightness = noise * _Intensity;

                float4 color = _Color;

                if (_Rainbow > 0.5)
                {
                    float hue = frac(_Time.y * 0.2 + noise * 0.2);
                    float3 rgb = HSVtoRGB(float3(hue, 1.0, 1.0));
                    color = float4(rgb, 1.0);
                }

                return color * brightness;
            }
            ENDCG
        }
    }
}
