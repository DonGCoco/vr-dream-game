Shader "Custom/ClippingLiquid"
{
    Properties
    {
        _MainColor ("Color", Color) = (1,0,0,1)
        _ClipPlaneY ("Clip Plane Y", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _MainColor;
            float _ClipPlaneY;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 核心逻辑：如果像素的世界 Y 坐标低于 _ClipPlaneY，直接舍弃该像素
                clip(i.worldPos.y - _ClipPlaneY);
                return _MainColor;
            }
            ENDCG
        }
    }
}