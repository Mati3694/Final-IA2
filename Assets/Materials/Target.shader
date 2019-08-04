Shader "Custom/Target"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_SecColor("Secondary Color", Color) = (1,1,1,1)


		[IntRange]_CenterSize("Center Size", Range(0,10)) = 1.0
		_CenterColor("Center Color", Color) = (1,1,1,1)

		_RingDistance("Ring Distance", Float) = 1.0

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
			float3 worldPos;
			float3 centerPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _SecColor;
		fixed4 _CenterColor;

		float _RingDistance;
		float _CenterSize;

		void vert(inout appdata_full v, out Input o)
		{
			o.centerPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			fixed4 c = 1;
			float distanceToCenter = distance(IN.worldPos, IN.centerPos);

			c.rgb = lerp(_Color, _SecColor , step(0.5,frac(distanceToCenter / _RingDistance)));
			c.rgb = lerp(_CenterColor,c.rgb , step(0.5 * _CenterSize, distanceToCenter / _RingDistance));
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
