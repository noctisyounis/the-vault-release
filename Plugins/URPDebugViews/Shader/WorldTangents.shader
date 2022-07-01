Shader "Debug View/World Tangents"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half3 worldTangent : TEXCOORD0;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldTangent = UnityObjectToWorldDir(v.tangent);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.worldTangent, 1.0);
            }
            ENDCG
        }
    }
}
