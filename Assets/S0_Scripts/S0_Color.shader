Shader "Unlit/S0_Color"
{
    Properties
    {
        _Alpha ("Alpha", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint id : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
            };

            struct Particle
            {
                float3 pPos;
                float4 color;
                float speed;
            };

            StructuredBuffer <Particle> _particles;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                float3 p = _particles[v.id].pPos;
                o.vertex = UnityObjectToClipPos(p);
                float4 color = _particles[v.id].color;
                o.color = color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(i.color.rgb, _Alpha);
            }
            ENDCG
        }
    }
}
