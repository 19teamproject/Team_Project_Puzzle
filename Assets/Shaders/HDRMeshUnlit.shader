Shader "Custom/HDRMeshUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("HDR Color", Color) = (1,1,1,1) // HDR 컬러
        _HDRIntensity ("HDR Intensity", Range(1, 50)) = 5.0 // 강한 Bloom을 위한 HDR 강도
        _Alpha ("Alpha", Range(0, 1)) = 1.0 // 투명도 조절
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Blend SrcAlpha One // Additive Blending (HDR을 강하게 표현)
            ZWrite Off
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Vertex Color 지원
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _HDRIntensity;
            float _Alpha;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color * _HDRIntensity; // HDR 컬러 적용
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                texColor.rgb *= i.color.rgb;
                texColor.a = _Alpha * i.color.a; // 투명도 조절
                return texColor;
            }
            ENDCG
        }
    }
}
