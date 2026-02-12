Shader "Custom/DSGT_Galaxy"
{
    Properties
    {
        _Cube("Base Cubemap", CUBE) = "" {}
        _CubeOverlay("Overlay Cubemap", CUBE) = "" {}

        _BaseTint("Base Tint Color", Color) = (1,1,1,1)
        _BaseBrightness("Base Brightness", Range(0, 2)) = 1
        _BaseSpinSpeed("Base Spin Speed", Float) = 20
        _BaseTiling("Base Tiling", Vector) = (1,1,1,0)

        _OverlayTint("Overlay Tint Color", Color) = (1,1,1,1)
        _OverlayBrightness("Overlay Brightness", Range(0, 2)) = 1
        _OverlaySpinSpeed("Overlay Spin Speed", Float) = 10
        _OverlayTiling("Overlay Tiling", Vector) = (1,1,1,0)

        _OverlayBlend("Overlay Blend", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Cube;
            samplerCUBE _CubeOverlay;

            float4 _BaseTint;
            float _BaseBrightness;
            float _BaseSpinSpeed;
            float4 _BaseTiling;

            float4 _OverlayTint;
            float _OverlayBrightness;
            float _OverlaySpinSpeed;
            float4 _OverlayTiling;

            float _OverlayBlend;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 baseDir : TEXCOORD0;
                float3 overlayDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;

                // Base rotation
                float angleBase = radians(_Time.y * _BaseSpinSpeed);
                float3x3 rotBase = float3x3(
                    cos(angleBase), 0, -sin(angleBase),
                    0, 1, 0,
                    sin(angleBase), 0, cos(angleBase)
                );

                // Overlay rotation
                float angleOverlay = radians(_Time.y * _OverlaySpinSpeed);
                float3x3 rotOverlay = float3x3(
                    cos(angleOverlay), 0, -sin(angleOverlay),
                    0, 1, 0,
                    sin(angleOverlay), 0, cos(angleOverlay)
                );

                o.baseDir = mul(rotBase, viewDir * _BaseTiling.xyz);
                o.overlayDir = mul(rotOverlay, viewDir * _OverlayTiling.xyz);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dirBase = normalize(i.baseDir);
                float3 dirOverlay = normalize(i.overlayDir);

                fixed4 baseColor = texCUBE(_Cube, dirBase);
                fixed4 overlayColor = texCUBE(_CubeOverlay, dirOverlay);

                baseColor.rgb *= _BaseTint.rgb * _BaseBrightness;
                baseColor.a *= _BaseTint.a;

                overlayColor.rgb *= _OverlayTint.rgb * _OverlayBrightness;
                overlayColor.a *= _OverlayTint.a;

                fixed4 finalColor = lerp(baseColor, overlayColor, _OverlayBlend);
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack Off
}