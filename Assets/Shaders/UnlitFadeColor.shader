Shader "Unlit/FadeColor" {

	Properties{
		 _Color("Color & Transparency", Color) = (0, 0, 0, 0.5)
	}
		SubShader{
			Lighting Off
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			Tags {"Queue" = "AlphaTest"}
			Color[_Color]
			Pass {
			}
	}
		FallBack "Unlit/Transparent"
}
