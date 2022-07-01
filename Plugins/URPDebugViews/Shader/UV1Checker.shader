Shader "Debug View/UV1 Checker"
{
    Properties
    {
        _Checker ("Checker", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            sampler2D _Checker;
            float4 _Checker_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Checker);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_Checker, i.uv);
            }
            ENDCG
        }
    }
}
