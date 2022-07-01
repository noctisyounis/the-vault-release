Shader "Unlit/ShadowCascades"
{
    Properties
    {
        _ColorCascade0 ("Color cascade 0", Color) = (0.54, 0.54, 0.63)
        _ColorCascade1 ("Color cascade 1", Color) = (0.54, 0.63, 0.54)
        _ColorCascade2 ("Color cascade 2", Color) = (0.63, 0.63, 0.54)
        _ColorCascade3 ("Color cascade 3", Color) = (0.63, 0.54, 0.54)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        HLSLINCLUDE
        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float4 objectPos : TEXCOORD0;
         };

         float4 _ColorCascade0;
         float4 _ColorCascade1;
         float4 _ColorCascade2;
         float4 _ColorCascade3;
         
         float4 _DebugViewShadowDistances;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.objectPos = v.vertex;
            return o;
        }

        half4 frag (v2f i) : SV_Target
        {            
            float distance = length(UnityObjectToViewPos(i.objectPos).xyz);
            
            if (distance < _DebugViewShadowDistances.x)
                return _ColorCascade0;
            if (distance < _DebugViewShadowDistances.y)
                return _ColorCascade1;
            if (distance < _DebugViewShadowDistances.z)
                return _ColorCascade2;
            if (distance < _DebugViewShadowDistances.w)
                return _ColorCascade3;
            // black means out of shadow distance
            return float4(0, 0, 0, 1);
        }
            
        ENDHLSL
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDHLSL
        }
    }
}
