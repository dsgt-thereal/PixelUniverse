Shader "Custom/DSGT_ScrollingTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.2, 0.6, 1, 1) // Shiny blue default
        _ScrollSpeed ("Scroll Speed (X,Y)", Vector) = (0.1, 0.0, 0, 0)
        _Unlit ("Unlit Mode", Range(0,1)) = 0
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0,1)) = 0.8
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float4 _TintColor;
        float4 _ScrollSpeed;
        float _Unlit;
        float _Metallic;
        float _Smoothness;
        float4 _EmissionColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 scrolledUV = IN.uv_MainTex + _ScrollSpeed.xy * _Time.y;
            fixed4 tex = tex2D(_MainTex, scrolledUV) * _TintColor;

            o.Albedo = tex.rgb;
            o.Alpha = tex.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;

            if (_Unlit > 0.5)
            {
                o.Emission = tex.rgb + _EmissionColor.rgb;
                o.Albedo = 0;
                o.Normal = float3(0, 0, 1);
                o.Smoothness = 0;
                o.Metallic = 0;
            }
            else
            {
                o.Emission = _EmissionColor.rgb;
            }
        }
        ENDCG
    }

    FallBack "Diffuse"
}