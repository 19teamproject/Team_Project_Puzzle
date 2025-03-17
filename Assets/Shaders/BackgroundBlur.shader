Shader "UI/BackgroundBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0.0, 5.0)) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        GrabPass { "_BackgroundTexture" } // 현재 화면을 복사하여 저장
        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _BackgroundTexture;
            float4 _BackgroundTexture_TexelSize;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 offset = _BackgroundTexture_TexelSize.xy * _BlurSize;

                float4 color = tex2D(_BackgroundTexture, uv) * 0.2;
                color += tex2D(_BackgroundTexture, uv + offset * float2(1, 0)) * 0.15;
                color += tex2D(_BackgroundTexture, uv - offset * float2(1, 0)) * 0.15;
                color += tex2D(_BackgroundTexture, uv + offset * float2(0, 1)) * 0.15;
                color += tex2D(_BackgroundTexture, uv - offset * float2(0, 1)) * 0.15;
                color += tex2D(_BackgroundTexture, uv + offset * float2(1, 1)) * 0.1;
                color += tex2D(_BackgroundTexture, uv - offset * float2(1, 1)) * 0.1;
                color += tex2D(_BackgroundTexture, uv + offset * float2(-1, 1)) * 0.1;
                color += tex2D(_BackgroundTexture, uv + offset * float2(1, -1)) * 0.1;

                return color;
            }
            ENDHLSL
        }
    }
}