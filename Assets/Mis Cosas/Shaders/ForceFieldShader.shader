Shader "Custom/ForceFieldShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0.5)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Float) = 3.0
        _ScanLineTex ("Scan Line Texture", 2D) = "white" {}
        _ScanLineWidth ("Scan Line Width", Float) = 0.1
        _ScanLineSpeed ("Scan Line Speed", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            Name "FORCE_FIELD"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform float _ScanLineWidth;
            uniform float _ScanLineSpeed;
            uniform sampler2D _ScanLineTex;
            uniform float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = o.pos.xy * 0.5 + 0.5;
                return o;
            }
            
            half4 frag (v2f i) : COLOR
            {
                float3 normal = normalize(i.pos.xyz);
                float rim = 1.0 - saturate(dot(normal, float3(0,0,1)));
                rim = pow(rim, _RimPower);
                
                half4 rimColor = _RimColor * rim;
                
                float scanLineOffset = _Time.y * _ScanLineSpeed;
                float2 scanUV = i.uv * 2.0;
                float scanLine = tex2D(_ScanLineTex, scanUV + float2(scanLineOffset, 0)).r;
                
                float alpha = step(_ScanLineWidth, scanLine);
                
                half4 col = _Color * (1.0 - alpha) + rimColor * alpha;
                col.a = _Color.a;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

