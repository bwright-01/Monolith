// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Outline"
{
    Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		// Add values to determine if outlining is enabled and outline color.
		[PerRendererData] _Outline("Outline", Float) = 0
		[PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma shader_feature ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
			};

			fixed4 _Color;
			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSize;

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_TexelSize;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D(_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

            float2 uvPerWorldUnit(float2 uv, float2 space){
                float2 uvPerPixelX = abs(ddx(uv));
                float2 uvPerPixelY = abs(ddy(uv));
                float unitsPerPixelX = length(ddx(space));
                float unitsPerPixelY = length(ddy(space));
                float2 uvPerUnitX = uvPerPixelX / unitsPerPixelX;
                float2 uvPerUnitY = uvPerPixelY / unitsPerPixelY;
                return (uvPerUnitX + uvPerUnitY);
            }

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                fixed2 sampleDistance = uvPerWorldUnit(IN.texcoord, IN.worldPos.xy) * _OutlineSize;

				// If outline is enabled and there is a pixel, try to draw an outline.
				if (_Outline > 0 && c.a != 0) {
					float totalAlpha = 1.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y) * sampleDistance);
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i *  _MainTex_TexelSize.y) * sampleDistance);
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0) * sampleDistance);
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0) * sampleDistance);

						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
					}

					if (totalAlpha == 0) {
						c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
					}
				}

				c.rgb *= c.a;

				return c;
			}

            // fixed4 frag(v2f IN) : SV_TARGET{
            //     //get regular color
            //     fixed4 COL = SampleSpriteTexture(IN.texcoord);
            //     COL *= _Color;
            //     COL *= IN.color;

            //     float2 sampleDistance = uvPerWorldUnit(IN.texcoord, IN.worldPos.xy) * _OutlineSize;
            //     bool hitTransparent = false;

            //     //sample directions
            //     #define DIV_SQRT_2 0.70710678118
            //     float2 directions[8] = {
            //         float2(1, 0),
            //         float2(0, 1),
            //         float2(-1, 0),
            //         float2(0, -1),
            //         float2(DIV_SQRT_2, DIV_SQRT_2),
            //         float2(-DIV_SQRT_2, DIV_SQRT_2),
            //         float2(-DIV_SQRT_2, -DIV_SQRT_2),
            //         float2(DIV_SQRT_2, -DIV_SQRT_2)
            //     };

            //     //generate border - more accurately, determine if the current pixel is within range of any non-transparent pixel
            //     float maxAlpha = 0;
            //     float currentAlpha = 0;
            //     for (uint index = 0; index < 8; index++){
            //         float2 sampleUV = IN.texcoord + directions[index] * sampleDistance;
            //         currentAlpha = SampleSpriteTexture(sampleUV).a;
            //         if (currentAlpha <= 0) hitTransparent = true;
            //         if (currentAlpha > 0) currentAlpha = IN.color.a;
            //         maxAlpha = max(maxAlpha, currentAlpha);
            //     }

            //     if (!hitTransparent) {
            //         // COL.a = IN.color.a;
            //         return COL;
            //     }

            //     //apply border
            //     COL.rgb = _OutlineColor.rgb;
            //     COL.a = max(COL.a, maxAlpha);

            //     return COL;
            // }
			ENDCG
		}
	}
}
