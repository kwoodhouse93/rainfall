Shader "Hidden/Pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel size (must be odd)", Int) = 5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform texture2D _MainTex;
            uniform SamplerState sampler_MainTex;
            uniform int _PixelSize;

            fixed4 frag (v2f_img i) : SV_Target
            {
                // Cast to uint for more performant modulus operator later.
                uint pixelSize = _PixelSize;

                // Get dimensions of texture for calculating relation between UV and pixel coordinates.
                uint2 texSize;
                _MainTex.GetDimensions(texSize[0],texSize[1]);

                // Divide image into pixels
                int x = (int)(i.uv[0]*texSize[0]) % pixelSize;
                int y = (int)(i.uv[1]*texSize[1]) % pixelSize;

                // Bit of maths to get a vector pointing to the nearest 'big pixel' center for each screen pixel
                x = floor(pixelSize / 2.0) - x;
                y = floor(pixelSize / 2.0) - y;

                // Convert to screen pixel coords.
                x = (int)(i.uv[0]*texSize[0]) + x;
                y = (int)(i.uv[1]*texSize[1]) + y;

                // return fixed4(x/(float)texSize[0], y/(float)texSize[1], 0, 0);

                // Back down to UV coords.
                float2 uv = float2(x,y) / texSize;
 
                // And finally sample using these new coordinates.
                fixed4 col = _MainTex.Sample(sampler_MainTex,uv);
                return col;
            }
            ENDCG
        }
    }
}
