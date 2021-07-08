 Shader "Unlit/flowRecrec"
{
    Properties{
        _MainTex("MainTex",2D) = "white"{}
        _Alpha("Alpha",Float) = 1
        _isCapture("isCapture",Int) = 1
    }
    SubShader
    {
Tags {"Queue"="Transparent" "RenderType"="Transparent"}
 Blend SrcAlpha OneMinusSrcAlpha 


        GrabPass
        {
            "_GrabTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 grabPos : TEXCOORD0;
                float3 normal :NORMAL;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal :TEXCOORD1;

            };
            int _isCapture;
            sampler2D _MainTex;
            float _Intensity,_Alpha;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            sampler2D _GrabTexture;
            fixed4 oldcol;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = 0;
             //노멀값을 -1~1 에서 0~1로 바꿔줌
                c.rgb = i.worldNormal*0.5+0.5;
             //왜곡을 주기위해 _Intensity 만큼 더해줌
                fixed4 col;
                if (_isCapture == 1) {
                    float4 distortion = tex2D(_MainTex,i.grabPos)+_Intensity;
                    col = tex2Dproj(_GrabTexture, i.grabPos + c.r);
                    oldcol = col;
                }
                else {
                    col = oldcol;
                }
                return float4(col.rgb,_Alpha);
            }
            ENDCG
        }
    }
}