using UnityEngine;

namespace Boids
{
    /// <summary>
    /// Boidsオブジェクト描画クラス(共通)
    /// </summary>
    public abstract class BoidsRenderBase : MonoBehaviour
    {
        /// <summary>
        /// Boids描画用メッシュ
        /// </summary>
        [SerializeField]
        protected Mesh _boidsRenderMesh;

        /// <summary>
        /// Boids描画用マテリアル
        /// </summary>
        [SerializeField]
        protected Material _boidsRenderMaterial;

        /// <summary>
        /// 描画するBoidの大きさ
        /// </summary>
        [SerializeField]
        protected Vector3 _boidScale = new Vector3(0.1f, 0.2f, 0.5f);

        /// <summary>
        /// GPUインスタンシングのための引数データ
        /// </summary>
        protected GraphicsBuffer.IndirectDrawIndexedArgs[] _gpuInstanceArgs;

        /// <summary>
        /// GPUインスタンシングのための引数バッファ
        /// </summary>
        protected GraphicsBuffer _gpuInstanceArgsBuffer;

#region MonoBehaviour Functions

        private void Start()
        {
            InitBuffer();
        }

        private void Update()
        {
            OnRenderMesh();
        }

        private void OnDestroy()
        {
            ReleaseBuffer();
        }

#endregion

        /// <summary>
        /// バッファ初期化
        /// </summary>
        private void InitBuffer()
        {
            // GraphicsBuffer.Target: https://docs.unity3d.com/ScriptReference/GraphicsBuffer.Target.html
            var commandCount = 1;
            _gpuInstanceArgs = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
            _gpuInstanceArgsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        }

        /// <summary>
        /// メッシュ描画
        /// </summary>
        protected abstract void OnRenderMesh();

        /// <summary>
        /// バッファ解放
        /// </summary>
        private void ReleaseBuffer()
        {
            if (_gpuInstanceArgsBuffer != null)
            {
                _gpuInstanceArgsBuffer.Release();
                _gpuInstanceArgsBuffer = null;
            }
        }
    }
}
