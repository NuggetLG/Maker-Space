Shader "Custom/SimpleWater_Cube"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0,0.5,1,0.5)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamThreshold ("Foam Threshold", Float) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture;
            float4 _WaterColor;
            float4 _FoamColor;
            float _FoamThreshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = o.screenPos.xy / o.screenPos.w;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Solo renderizar caras que miran hacia arriba
                if (i.worldNormal.y < 0.5)
                    discard;

                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
                float myDepth = i.screenPos.z;

                float diff = abs(sceneDepth - myDepth);

                float foam = smoothstep(0.0, _FoamThreshold, diff);

                float4 col = _WaterColor;
                col.rgb = lerp(col.rgb, _FoamColor.rgb, foam);

                return col;
            }
            ENDCG
        }
    }
}
