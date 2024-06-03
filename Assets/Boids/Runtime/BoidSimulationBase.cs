using UnityEngine;

namespace Boids
{
    /// <summary>
    /// Boidsシミュレーション(共通)
    /// </summary>
    public abstract class BoidsSimulationBase : MonoBehaviour
    {
        /// <summary>
        /// スレッドグループのスレッドサイズ
        /// </summary>
        protected const int SimulationBlockSize = 256;

        /// <summary>
        /// Boidsシミュレーション用ComputeShader
        /// </summary>
        [SerializeField] protected ComputeShader _boidsCs;

        /// <summary>
        /// Boids生成数
        /// ※2の冪乗かつ256で割り切れる数で設定
        /// </summary>
        protected enum BoidsNumType
        {
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192,
        }

        /// <summary>
        /// Boids最大数
        /// </summary>
        [SerializeField] protected BoidsNumType _maxBoidsNum = BoidsNumType._256;
        public int MaxBoidsNum => (int) _maxBoidsNum;

        /// <summary>
        /// 速度の最大値
        /// </summary>
        [SerializeField] protected float _maxSpeed = 5.0f;

        /// <summary>
        /// 操舵力の最大値
        /// </summary>
        [SerializeField] protected float _maxSteerForce = 0.5f;

        /// <summary>
        /// 適用する他の個体との半径
        /// </summary>
        [SerializeField] protected float _separationDistance = 1.75f; // 分離
        [SerializeField] protected float _alignmentDistance = 3.2f;  // 整列
        [SerializeField] protected float _cohesionDistance = 3.2f;   // 結合

        /// <summary>
        /// 適用時の重み係数
        /// </summary>
        [SerializeField] protected float _separationCoefficient = 16f; // 分離
        [SerializeField] protected float _alignmentCoefficient = 10f;  // 整列
        [SerializeField] protected float _cohesionCoefficient = 6f;   // 結合

        /// <summary>
        /// 壁を避ける強さの重み
        /// </summary>
        [SerializeField] protected float _avoidWallWeight = 10.0f;

#region MonoBehaviour Functions

        private void Start()
        {
            InitBuffer();
        }

        private void Update()
        {
            OnSimulation();
        }

        private void OnDestroy()
        {
            ReleaseBuffer();
        }

#endregion

        protected abstract void InitBuffer();

        protected abstract void OnSimulation();

        protected abstract void ReleaseBuffer();
    }
}
