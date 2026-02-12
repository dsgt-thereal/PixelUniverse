Shader "Custom/TVStatic"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Intensity ("Noise Intensity", Range(0, 1)) = 0.5
        _Speed ("Flicker Speed", Range(1, 50)) = 10
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

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float timeNoise = _Time.y * _Speed;
                float noise = random(i.uv * timeNoise);
                float brightness = lerp(0.0, 1.0, noise) * _Intensity;
                return _Color * brightness;
            }
            ENDCG
        }
    }
}
