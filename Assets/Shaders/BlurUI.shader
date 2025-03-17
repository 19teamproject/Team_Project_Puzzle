Shader "Custom/BlurUI"
{
    Properties
    {
        _BlurRadius ("Blur Radius", Range(0.0, 15.0)) = 2.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        GrabPass { "_GrabTexture" } // GrabPass로 현재까지 렌더된 결과를 _GrabTexture에 담는다.
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _GrabTexture;           // GrabPass가 담긴 텍스처
            float4 _GrabTexture_TexelSize;    // GrabPass 텍스처의 TexelSize (픽셀 크기 정보)
            float _BlurRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 간단한 박스 블러(Box Blur) 샘플링
                // _BlurRadius에 따라 주변 픽셀을 더 많이 참조하게끔 구현 가능
                float2 uv = i.uv;

                // Blur 횟수(샘플링 횟수)를 임의로 9번 정도라고 가정
                float2 offset = _BlurRadius * _GrabTexture_TexelSize.xy;
                fixed4 col = fixed4(0,0,0,0);
                
                // 샘플링 예시(3x3 박스)
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        col += tex2D(_GrabTexture, uv + float2(x, y) * offset);
                    }
                }
                col /= 9.0;

                // A는 원본 GrabPass 픽셀의 알파를 이용하거나, UI 투명도와 연동해도 됨
                return col;
            }
            ENDCG
        }
    }
}
