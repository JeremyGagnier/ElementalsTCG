Shader "Unlit/Transparent Colored"
{
	Properties
	{
		_Color ("Color",Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex] {
			  constantColor [_Color]
			  combine constant lerp(texture) previous
			}
			
			SetTexture [_MainTex]
			{
				Combine Texture * previous
			}
		}
	}
}