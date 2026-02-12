Shader "Custom/DSGT_RGB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Color Cycle Speed", Float) = 1.0
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

            sampler2D _MainTex;
            float _Speed;
            float4 _MainTex_ST;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * _Speed;
                float3 rgb = float3(
                    0.5 + 0.5 * sin(t),
                    0.5 + 0.5 * sin(t + 2.094), // + 2π/3
                    0.5 + 0.5 * sin(t + 4.188)  // + 4π/3
                );

                fixed4 texColor = tex2D(_MainTex, i.uv);
                return fixed4(texColor.rgb * rgb, texColor.a);
            }
            ENDCG
        }
    }
}
