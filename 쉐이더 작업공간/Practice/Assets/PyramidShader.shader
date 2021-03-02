Shader "Custom/Geometry/Pyramids2"
{
    Properties{
        _MainTex("MainTex",2D) = "white"{}
        _Alpha("Alpha",Float) = 1
        _Factor("Factor", Range(0., 2.)) = 0.2
    }
        SubShader
        {
           Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
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
                #pragma geometry geom

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 grabPos : TEXCOORD0;
                    float3 normal :NORMAL;
                };

                struct v2g
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct g2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 col : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                // vert 생성
                v2g vert(appdata_base v)
                {
                    v2g o;
                    o.vertex = v.vertex;
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.normal = v.normal;
                    return o;
                }

             
                float _Factor;

                [maxvertexcount(12)]
                void geom(triangle v2g IN[3], inout TriangleStream<g2f> tristream)
                {
                    g2f o;

                    //외적으로 앞면 노멀값
                    float3 normalFace = normalize(cross(IN[1].vertex - IN[0].vertex, IN[2].vertex - IN[0].vertex));

                    float edge1 = distance(IN[1].vertex, IN[0].vertex);
                    float edge2 = distance(IN[2].vertex, IN[0].vertex);
                    float edge3 = distance(IN[2].vertex, IN[1].vertex);

                    //중심 위치와 중심uv좌표구하기 step함수는 앞에게 더크면 1 아니면 0을 줌
                    float3 centerPos = (IN[0].vertex + IN[1].vertex) / 2;
                    float2 centerTex = (IN[0].uv + IN[1].uv) / 2;
                   
                    if ((step(edge1, edge2) * step(edge3, edge2)) == 1.0)
                    {
                        centerPos = (IN[2].vertex + IN[0].vertex) / 2;
                        centerTex = (IN[2].uv + IN[0].uv) / 2;
                    }
                    else if ((step(edge2, edge3) * step(edge1, edge3)) == 1.0)
                    {
                        centerPos = (IN[1].vertex + IN[2].vertex) / 2;
                        centerTex = (IN[1].uv + IN[2].uv) / 2;
                    }

                    centerPos += float4(normalFace, 0) * _Factor;

                    //3개의 vertex 중 2개와 센터 vertex 와 함께 새 삼각형 스트림 생성
                    for (int i = 0; i < 3; i++)
                    {
                        o.pos = UnityObjectToClipPos(IN[i].vertex);
                        o.uv = IN[i].uv;
                        o.col = fixed4(0., 0., 0., 1.);
                        tristream.Append(o);

                        int inext = (i + 1) % 3;
                        o.pos = UnityObjectToClipPos(IN[inext].vertex);
                        o.uv = IN[inext].uv;
                        o.col = fixed4(0., 0., 0., 1.);
                        tristream.Append(o);

                        o.pos = UnityObjectToClipPos(float4(centerPos, 1));
                        o.uv = centerTex;
                        o.col = fixed4(1.0, 1.0, 1.0, 1.);
                        tristream.Append(o);

                        tristream.RestartStrip();
                    }

                    o.pos = UnityObjectToClipPos(IN[0].vertex);
                    o.uv = IN[0].uv;
                    o.col = fixed4(0., 0., 0., 1.);
                    tristream.Append(o);

                    o.pos = UnityObjectToClipPos(IN[1].vertex);
                    o.uv = IN[1].uv;
                    o.col = fixed4(0., 0., 0., 1.);
                    tristream.Append(o);

                    o.pos = UnityObjectToClipPos(IN[2].vertex);
                    o.uv = IN[2].uv;
                    o.col = fixed4(0., 0., 0., 1.);
                    tristream.Append(o);

                    tristream.RestartStrip();
                }

                fixed4 frag(g2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.col;
                    return col;
                }
                ENDCG
            }
           
        }
}