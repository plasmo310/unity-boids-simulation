using UnityEngine;

namespace Boids
{
    /// <summary>
    /// Boidsオブジェクト描画クラス(2D)
    /// </summary>
    [RequireComponent(typeof(Boids2DSimulation))]
    public class Boids2DRender : BoidsRenderBase
    {
        /// <summary>
        /// Boidsシミュレーションスクリプト
        /// </summary>
        [SerializeField]
        private Boids2DSimulation _boids2DSimulation;

        /// <summary>
        /// メッシュ描画
        /// https://docs.unity3d.com/ScriptReference/Graphics.RenderMeshIndirect.html
        /// </summary>
        protected override void OnRenderMesh()
        {
            if (_boidsRenderMesh == null
                || _boidsRenderMaterial == null
                || !SystemInfo.supportsInstancing)
            {
                return;
            }

            // シミュレーションデータの取得
            uint maxBoidsCount = (uint) _boids2DSimulation.MaxBoidsNum;
            GraphicsBuffer boidsDataBuffer = _boids2DSimulation.BoidsDataBuffer;
            Vector2 simulationAreaCenter = _boids2DSimulation.SimulationAreaCenter;
            Vector2 simulationAreaSize = _boids2DSimulation.SimulationAreaSize;

            // 引数バッファにインデックス数、生成数を設定
            var numIndices = _boidsRenderMesh.GetIndexCount(0);
            _gpuInstanceArgs[0].indexCountPerInstance = numIndices;
            _gpuInstanceArgs[0].instanceCount = maxBoidsCount;
            _gpuInstanceArgsBuffer.SetData(_gpuInstanceArgs);

            // マテリアルに描画用のデータを設定
            var renderParams = new RenderParams(_boidsRenderMaterial);
            renderParams.matProps = new MaterialPropertyBlock();
            renderParams.matProps.SetBuffer("_BoidDataBuffer", boidsDataBuffer);
            renderParams.matProps.SetVector("_BoidScale", _boidScale);
            renderParams.worldBounds = new Bounds(simulationAreaCenter, simulationAreaSize);

            // メッシュ描画
            Graphics.RenderMeshIndirect(
                renderParams,
                _boidsRenderMesh,
                _gpuInstanceArgsBuffer);
        }
    }
}
