// GPUインスタンシングによる描画(2D)
Shader "Custom/Boids2DRender"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
        	"RenderType" = "Opaque"
        }
        LOD 200

        // 裏面描画
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard vertex:vert SimpleLambert
        #pragma instancing_options procedural:setup
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;

        // Boidオブジェクトの大きさ
        float2 _BoidScale;

        // Boidデータの構造体
        struct BoidData
        {
            float2 velocity; // 速度
            float2 position; // 位置
        };

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<BoidData> _BoidDataBuffer;
        #endif

		// 角度を回転行列に変換(2D)
		float4x4 radianToRotationMatrix(float radians)
		{
			const float ca = cos(radians);
			const float sa = sin(radians);
			return float4x4(
				ca, -sa, 0, 0,
				sa,  ca, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1
			);
		}

        void setup()
		{
		}

        void vert(inout appdata_full v)
        {
        	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

			// Boidデータを取得
        	BoidData boidData = _BoidDataBuffer[unity_InstanceID];
        	float2 boidPosition = boidData.position.xy;
        	float2 boidScale = _BoidScale;

        	// オブジェクト座標 -> ワールド座標に変換する行列を定義
        	float4x4 object2world = (float4x4) 0;

			// scale
        	object2world._11_22_33_44 = float4(boidScale.xy, 1.0, 1.0);

        	// rotation
        	float rot = atan2(-boidData.velocity.x, boidData.velocity.y);
	        float4x4 rotMatrix = radianToRotationMatrix(rot);
        	object2world = mul(rotMatrix, object2world);

			// position
			object2world._14_24 += boidPosition.xy;

        	// 行列から頂点、法線を設定
        	v.vertex = mul(object2world, v.vertex);
        	v.normal = normalize(mul(object2world, v.normal));

        	#endif
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			clip(c.a - 0.1); // 透明部分は非表示
            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
