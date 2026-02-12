Shader "Custom/DSGT_Holowall"
{
    Properties
    {
        _Cube("Cubemap", CUBE) = "" {}
        _Tint("Tint", Color) = (1,1,1,1)
        _Brightness("Brightness", Range(0, 2)) = 1
        _SpinSpeed("Spin Speed", Float) = 20
        _Tiling("Tiling", Vector) = (1,1,1,0) // X, Y, Z scale for direction vector
    }

    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Cube;
            float4 _Tint;
            float _Brightness;
            float _SpinSpeed;
            float4 _Tiling;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 dir = worldPos - _WorldSpaceCameraPos;

                // Apply time-based Y-axis rotation
                float angle = _Time.y * _SpinSpeed;
                float rad = radians(angle);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float3x3 rotY = float3x3(
                    cosA, 0, -sinA,
                    0,    1,  0,
                    sinA, 0, cosA
                );
                dir = mul(rotY, dir);

                // Apply tiling to direction vector
                dir *= _Tiling.xyz;

                o.dir = dir;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 sampleDir = normalize(i.dir);
                fixed4 col = texCUBE(_Cube, sampleDir);
                col.rgb *= _Tint.rgb * _Brightness;
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
