Shader "Custom/DSGT_Hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Hologram Tint", Color) = (0.0, 1.0, 1.0, 1.0)
        _MinFade ("Min Fade Distance", Float) = 1.0
        _MaxFade ("Max Fade Distance", Float) = 5.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _MinFade;
            float _MaxFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 camPos = _WorldSpaceCameraPos;
                float dist = distance(camPos, i.worldPos);

                // Fade calculation
                float fade = saturate((dist - _MinFade) / (_MaxFade - _MinFade));

                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * _TintColor;
                finalColor.a *= fade;

                return finalColor;
            }
            ENDCG
        }
    }
}