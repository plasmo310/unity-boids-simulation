// GPUインスタンシングによる描画(3D)
// ※SurfaceShaderを使用しているためインスタンスID取得設定はデフォルトである程度定義されている
//  https://ny-program.hatenablog.com/entry/2023/06/24/144715
// ※通常のシェーダでの記述サンプル
//  https://docs.unity3d.com/ScriptReference/Graphics.RenderMeshIndirect.html
Shader "Custom/Boids3DRender"
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
        float3 _BoidScale;

        // Boidデータの構造体
        struct BoidData
        {
            float3 velocity; // 速度
            float3 position; // 位置
        };

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<BoidData> _BoidDataBuffer;
        #endif

		// 角度を回転行列に変換(3D)
        // (クォータニオンによる計算の方がいいかも？)
		float4x4 radianToRotationMatrix(float3 angles)
		{
			const float cy = cos(angles.y); float sy = sin(angles.y);
			const float cx = cos(angles.x); float sx = sin(angles.x);
			const float cz = cos(angles.z); float sz = sin(angles.z);
			// Ry-Rx-Rz (Yaw Pitch Roll)
			return float4x4(
				cy * cz + sy * sx * sz, -cy * sz + sy * sx * cz, sy * cx, 0,
				cx * sz, cx * cz, -sx, 0,
				-sy * cz + cy * sx * sz, sy * sz + cy * sx * cz, cy * cx, 0,
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
        	float3 boidPosition = boidData.position.xyz;
        	float3 boidScale = _BoidScale;

        	// オブジェクト座標 -> ワールド座標に変換する行列を定義
        	float4x4 object2world = (float4x4) 0;

			// scale
        	object2world._11_22_33_44 = float4(boidScale.xyz, 1.0);

        	// rotation
        	float rotX = -asin(boidData.velocity.y / (length(boidData.velocity.xyz) + 1e-8));
        	float rotY = atan2(boidData.velocity.x, boidData.velocity.z);
	        float4x4 rotMatrix = radianToRotationMatrix(float3(rotX, rotY, 0));
        	object2world = mul(rotMatrix, object2world);

			// position
        	object2world._14_24_34 += boidPosition.xyz;

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
