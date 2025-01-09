Shader "Custom/FollowTheLine"
{
    Properties
    {
        _MainTex ("Paint Texture", 2D) = "white" {}
        _GuideTex ("Guide Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GuideTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Get the paintable texture color
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                
                // Get the guide texture color
                fixed4 guideColor = tex2D(_GuideTex, i.uv);
                
                // Blend the textures (e.g., overlay the guide at 50% opacity)
                return lerp(mainColor, guideColor, 0.5);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
