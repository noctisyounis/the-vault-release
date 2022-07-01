Shader "Debug View/Shaded Wireframe"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0)
		_Smoothing ("Smoothing", Range(0, 5)) = 1
		_Thickness ("Thickness", Range(0, 5)) = 1
		_Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Offset -0.1, 0
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geometryWireframe

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            struct InterpolatorsGeometry {
	            v2f data;
	            float2 index : TEXCOORD9;
            };
            
            [maxvertexcount(3)]
            void geometryWireframe (triangle v2f i[3], inout TriangleStream<InterpolatorsGeometry> stream) 
	        {
	            InterpolatorsGeometry g0, g1, g2;
	            g0.data = i[0];
	            g1.data = i[1];
	            g2.data = i[2];

                g0.index = float2(1, 0);
                g1.index = float2(0, 1);
                g2.index = float2(0, 0);
                
                stream.Append(g0);
                stream.Append(g1);
                stream.Append(g2);
            }

            float3 _Color;
            float _Smoothing;
            float _Thickness;
            float _Alpha;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (InterpolatorsGeometry i) : SV_Target
            {
                float3 edges;
                edges.xy = i.index;
                edges.z = 1 - edges.x - edges.y;
                float3 deltas = fwidth(edges);
                float3 smoothing = deltas * _Smoothing;
                float3 thickness = deltas * _Thickness;
                edges = smoothstep(thickness, thickness + smoothing, edges);
                float minEdge = min(edges.x, min(edges.y, edges.z));
                float alpha = lerp(0, _Alpha, 1.0 - minEdge);
                return fixed4(_Color.xyz, alpha);
            }
            ENDCG
        }
    }
}
