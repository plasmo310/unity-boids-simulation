using System.Runtime.InteropServices;
using UnityEngine;

namespace Boids
{
    /// <summary>
    /// Boidsシミュレーション(3D)
    /// </summary>
    public class Boids3DSimulation : BoidsSimulationBase
    {
        /// <summary>
        /// Boid基本データ構造体
        /// </summary>
        struct BoidData
        {
            public Vector3 Velocity;
            public Vector3 Position;
        }

        /// <summary>
        /// シミュレーション範囲 中心位置
        /// </summary>
        [SerializeField] protected Vector3 _simulationAreaCenter = Vector3.zero;
        public Vector3 SimulationAreaCenter => _simulationAreaCenter;

        /// <summary>
        /// シミュレーション範囲 サイズ
        /// </summary>
        [SerializeField]
        protected Vector3 _simulationAreaSize = new Vector3(32f, 32f, 32f);
        public Vector3 SimulationAreaSize => _simulationAreaSize;

        /// <summary>
        /// Boidsの基本データ格納バッファ
        /// </summary>
        private GraphicsBuffer _boidsDataBuffer;
        public GraphicsBuffer BoidsDataBuffer => _boidsDataBuffer;

        /// <summary>
        /// Boidsの操舵力格納バッファ
        /// </summary>
        private GraphicsBuffer _boidsForceBuffer;

        /// <summary>
        /// バッファ初期化
        /// </summary>
        protected override void InitBuffer()
        {
            _boidsDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, MaxBoidsNum, Marshal.SizeOf(typeof(BoidData)));
            _boidsForceBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, MaxBoidsNum, Marshal.SizeOf(typeof(Vector3)));

            // データを初期化して設定
            var dataCount = MaxBoidsNum;
            var boidsDataArray = new BoidData[dataCount];
            var boidsForceArray = new Vector3[dataCount];
            for (var i = 0; i < dataCount; i++)
            {
                // 範囲内からランダムに設定
                boidsDataArray[i].Position = UnityEngine.Random.insideUnitSphere * 1.0f;
                boidsDataArray[i].Velocity = UnityEngine.Random.insideUnitSphere * 0.5f;
                boidsForceArray[i] = Vector3.zero;
            }
            _boidsDataBuffer.SetData(boidsDataArray);
            _boidsForceBuffer.SetData(boidsForceArray);
        }

        /// <summary>
        /// シミュレーション実行
        /// </summary>
        protected override void OnSimulation()
        {
            ComputeShader cs = _boidsCs;
            var csId = -1;

            // スレッドグループ数
            var threadGroupSize = Mathf.CeilToInt(MaxBoidsNum / SimulationBlockSize);

            // 操舵力の計算
            csId = cs.FindKernel("ForceCS");
            cs.SetInt("_MaxBoidNum", MaxBoidsNum);
            cs.SetFloat("_MaxSpeed", _maxSpeed);
            cs.SetFloat("_MaxSteerForce", _maxSteerForce);
            cs.SetFloat("_SeparationDistance", _separationDistance);
            cs.SetFloat("_AlignmentDistance", _alignmentDistance);
            cs.SetFloat("_CohesionDistance", _cohesionDistance);
            cs.SetFloat("_SeparationCoefficient", _separationCoefficient);
            cs.SetFloat("_AlignmentCoefficient", _alignmentCoefficient);
            cs.SetFloat("_CohesionCoefficient", _cohesionCoefficient);
            cs.SetVector("_SimulationAreaCenter", SimulationAreaCenter);
            cs.SetVector("_SimulationAreaSize", SimulationAreaSize);
            cs.SetFloat("_AvoidWallWeight", _avoidWallWeight);
            cs.SetBuffer(csId, "_BoidDataBufferRead", _boidsDataBuffer);
            cs.SetBuffer(csId, "_BoidForceBufferWrite", _boidsForceBuffer);
            cs.Dispatch(csId, threadGroupSize, 1, 1);

            // 速度と位置を計算
            csId = cs.FindKernel("IntegrateCS");
            cs.SetFloat("_DeltaTime", Time.deltaTime);
            cs.SetBuffer(csId, "_BoidDataBufferWrite", _boidsDataBuffer);
            cs.SetBuffer(csId, "_BoidForceBufferRead", _boidsForceBuffer);
            cs.Dispatch(csId, threadGroupSize, 1, 1);
        }

        /// <summary>
        /// バッファ解放
        /// </summary>
        protected override void ReleaseBuffer()
        {
            if (_boidsDataBuffer != null)
            {
                _boidsDataBuffer.Release();
                _boidsDataBuffer = null;
            }

            if (_boidsForceBuffer != null)
            {
                _boidsForceBuffer.Release();
                _boidsForceBuffer = null;
            }
        }

        /// <summary>
        /// シミュレーション範囲のデバッグ表示
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_simulationAreaCenter, _simulationAreaSize);
        }
    }
}
