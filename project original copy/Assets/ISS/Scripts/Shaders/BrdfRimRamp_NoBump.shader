Shader "ISS/BrdfRimRamp_NoBump" {
	Properties 
	{
		_SpecColor("SpecularColor", Color) = (1,1,1,1)
		_Shininess("SpecPower", Range(-1, 2)) = 0.5
		_RimPower("Rim Power", Range(0.1, 20.0)) = 3.0
		_AtmColor("Atmosphere Color", Color) = (1,1,1,1)
		_AtmPower("Atmosphere Power", Range(0, 8.0)) = 1.0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp2D("BRDF Ramp", 2D) = "gray" {}
		_RimRamp2D("RimRamp", 2D) = "gray" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Ramp
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Ramp2D;
		sampler2D _RimRamp2D;
		float _Shininess;
		fixed _RimPower;
		float _AtmPower;
		float4 _AtmColor;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		} ;

		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 halfVector = normalize(lightDir + viewDir);
		
			float NdotL = dot(s.Normal, normalize(lightDir));
			float NdotE = dot(s.Normal, normalize(viewDir));
			
			fixed NdotH = max(0, dot(s.Normal, halfVector));
			fixed spec = pow(NdotH, s.Specular * 128) * s.Gloss;
			
			float diff = (NdotL * 0.3) + 0.5;
			float2 brdfUV = float2(NdotE * 0.8, diff);
			float3 BRDF = tex2D(_Ramp2D, brdfUV.xy).rgb;
			
			float2 rimUV = float2(1 - NdotE, 1 - NdotE);
			float3 rimRamp  = tex2D(_RimRamp2D, rimUV.xy).rgb;
			
			fixed rimLight = 1.0 - NdotE;
			rimLight = pow(rimLight, _RimPower);	
			
			float4 c;
			
			c.rgb = (s.Albedo * _LightColor0.rgb * BRDF + _SpecColor.rgb * spec) * (atten * 2) - rimLight * rimRamp;
			c.a = s.Alpha;
			return c;
		}	
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			
			o.Specular = _Shininess;
			o.Gloss = c.a;
			
			half atm = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _AtmColor.rgb * pow(atm, _AtmPower);
		}
		ENDCG
	}  
	FallBack "Diffuse"
}
