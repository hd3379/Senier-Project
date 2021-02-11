Shader "Custom/Geometry/Pyramid2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Factor ("Factor", Range(0., 2.)) = 0.2
    }

    SubShader
        {
            Tags{ "RenderType" = "Opaque"}
            Cull Off

            Pass
            {
                CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma geometry geom

#include "UnityCG.cginc"

                struct v2g
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            fixed4 col : COLOR;


        