Shader "Unlit/BorderRadius"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderRadius ("Border Radius", Range(0,0.5) ) = 0.2
        _Color ("My Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        Cull Off
        ZWrite Off
        Fog { Mode Off }


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // struct appdata
            // {
            //     float4 vertex : POSITION;
            //     float2 uv : TEXCOORD0;
            // };

            // struct v2f
            // {
            //     float2 uv : TEXCOORD0;
            //     UNITY_FOG_COORDS(1)
            //     float4 vertex : SV_POSITION;
            // };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            float _BorderRadius;
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
         
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                // o.uv += 0.1 * _Time.x;
                // o.uv = v.texcoord + 0.5;
                return o;
            }

            float circle( float2 _st, float _radius){
                float2 dist = _st;
                return 1.-smoothstep(_radius-(_radius*0.01),
                                    _radius+(_radius*0.01),
                                    dot(dist,dist)*4.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                
                // fixed4 col = lerp( _Color, tex2D(_MainTex , i.uv ), _Blender );
                // fixed4 col = lerp( _Color, tex2D(_MainTex , i.uv ), _Blender );


                float rectWidth = 0.2;
                fixed4 xRect = step( 0.5 - (0.5 - _BorderRadius), i.uv.x) - step( 0.5 + ( 0.5 - _BorderRadius), i.uv.x);
                fixed4 yRect = step( 0.5 - (0.5 - _BorderRadius), i.uv.y) - step( 0.5 + ( 0.5 - _BorderRadius), i.uv.y);
                // fixed4 yRect = step( 0.5 - rectWidth, i.uv.y) - step( 0.5 + rectWidth, i.uv.y);


                float2 grid = 1;
                
                float2 scaledUV = (i.uv) * grid;
                float2 gridUV = frac(scaledUV);
                
                fixed4 c = tex2D(_MainTex,i.uv);
                float x = gridUV.x;
                float y = gridUV.y;
                float power = 2;
                float dis1 = sqrt( pow((x - _BorderRadius ), power) + pow((y - _BorderRadius), power) );
                float dis2 = sqrt( pow((x - 1 + _BorderRadius), power) + pow((y - 1 + _BorderRadius), power) );
                float dis3 = sqrt( pow((x - _BorderRadius  ), power) + pow((y - 1 + _BorderRadius), power) );
                float dis4 = sqrt( pow((x - 1 + _BorderRadius), power) + pow((y - _BorderRadius), power) );

                fixed4 circles;
                if ( (dis1 < _BorderRadius && dis1 < _BorderRadius - 0.01 ) || dis2 < _BorderRadius || dis3 < _BorderRadius  || dis4 < _BorderRadius ) {
                    c.a = 1;
                } else {
                    c.a = 0;
                }

                c += xRect;
                c += yRect;

                // c.rgb *= 0;
                // c.rgb = _Color;
                clamp( c, 0, 1);

                // return circles + xRect + yRect ;
                // return circles;
                return fixed4(c.rgb, c.a );
            }
            ENDCG
        }
    }
}
