Shader "Custom/TextoCurvo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveIntensity ("Curve Intensity", Range(-1, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _CurveIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float x = v.texcoord.x - 0.5; // Centrar el texto
                float y = sin(x * _CurveIntensity * 3.14159); // Curva basada en el seno
                o.pos.y += y;

                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.texcoord);
            }
            ENDCG
        }
    }
}
