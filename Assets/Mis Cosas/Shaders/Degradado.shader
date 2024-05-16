Shader "Custom/Degradado"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTop ("Color Top", Color) = (1,1,1,1)
        _ColorBottom ("Color Bottom", Color) = (0,0,0,1)
        _BlendHeight ("Blend Height", Range(0,1)) = 0.5
    }
 
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        Cull Off
 
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
                float blendFactor : TEXCOORD1; // Variable para controlar la altura de la interpolación de colores
            };
 
            sampler2D _MainTex;
            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            float _BlendHeight; // Variable para controlar la altura de la interpolación de colores
 
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Calcular el factor de mezcla basado en la coordenada Y de la textura
                o.blendFactor = smoothstep(_BlendHeight - 0.1, _BlendHeight + 0.1, v.uv.y);
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                // Interpolar entre los colores utilizando el factor de mezcla
                half4 blendedColor = lerp(_ColorBottom, _ColorTop, i.blendFactor);
                return blendedColor * color;
            }
            ENDCG
        }
    }
}
