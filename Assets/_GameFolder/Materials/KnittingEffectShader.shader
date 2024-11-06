Shader "Custom/KnittingRowByRowEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0
        _RowCount ("Row Count", Float) = 10
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

            sampler2D _MainTex;
            float _Progress;
            float _RowCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv);

                float rowHeight = 1.0 / _RowCount;
                int currentRow = floor(i.uv.y / rowHeight);

                // Satır başına ilerleme
                float rowProgress = (_Progress * _RowCount) - currentRow;

                // Eğer satır tamamen dolmuşsa
                if (rowProgress >= 1.0)
                {
                    return mainColor; // Tamamı görünür
                }
                else if (rowProgress > 0.0)
                {
                    // Satır kısmi olarak doluyorsa
                    if (i.uv.x <= rowProgress)
                    {
                        return mainColor; // Kısmi görünür
                    }
                }

                // Henüz dolmamış kısımlar şeffaf kalır
                return fixed4(0, 0, 0, 0); 
            }
            ENDCG
        }
    }
}
