# unity-boids-simulation
* Boidsアルゴリズムによる群衆シミュレーションサンプルになります。
  * シミュレーション計算はComputeShader、オブジェクト描画はGPUインスタンシングを使用しています。
  * 参考にさせていただいたリポジトリ
    * <a href="https://github.com/IndieVisualLab/UnityGraphicsProgramming/tree/master/Assets/BoidsSimulationOnGPU">UnityGraphicsProgramming - BoidsSimulationOnGPU</a>

<img width=600 src="/ReadMeContents/01_boids3d.png"></img>

## バージョン
* Unity
  * 2022.3.16f1

## フォルダ構成

| フォルダ名          | 概要                 |
|----------------|--------------------|
| 3D Assets      | シミュレーション用の3D素材     |
| ComputeShaders | ComputeShader      |
| Presets        | シミュレーション用パラメータサンプル |
| Shaders        | 描画シェーダー            |
| Materials      | 描画マテリアル            |
| Runtime        | ランタイム用コード          |

## 使い方

### シーンについて

| フォルダ名                                                                              | 概要              | イメージ                                                       |
|------------------------------------------------------------------------------------|-----------------|------------------------------------------------------------|
| <a href="/Assets/Boids/Scenes/Boids2D.unity">Assets/Boids/Scenes/Boids2D.unity</a> | Boids2Dシミュレーション | <img width=400 src="/ReadMeContents/03_boids2d.png"></img> |
| <a href="/Assets/Boids/Scenes/Boids3D.unity">Assets/Boids/Scenes/Boids3D.unity</a> | Boids3Dシミュレーション | <img width=400 src="/ReadMeContents/01_boids3d.png"></img> |
### 主要クラス
| フォルダ名                                                                                          | 概要 |
|------------------------------------------------------------------------------------------------| - |
| Runtime/BoidsSimulationBase.cs<br>Runtime/Boids2DSimulation.cs<br>Runtime/Boids3DSimulation.cs | シミュレーション実行クラス |
| Runtime/BoidsRenderBase.cs<br>Runtime/Boids2DRender.cs<br>Runtime/Boids3DRender.cs             | オブジェクト描画クラス |
| ComputeShader/Boids2D.compute<br>ComputeShader/Boids3D.compute                                 | シミュレーション用ComputeShader |
| Shaders/Boids2DRender.shader<br>Shaders/Boids3DRender.shader                                   | オブジェクト描画シェーダー |

### コンポーネントのパラメータ
※2D、3Dシミュレーション共通

<img width=320 src="/ReadMeContents/02_boids3d_parameters.png"></img>

| パラメータ | 概要 |
| - | - |
| Max Boids Num | Boids生成最大数 |
| Simulation Area Center | シミュレーション範囲 中心位置 |
| Simulation Area Size | シミュレーション範囲 サイズ |
| Max Speed | 速度の最大値 |
| Max Steer Force | 操舵力の最大値 |
| Separation Distance | 適用する他の個体との半径: 分離 |
| Alignment Distance | 適用する他の個体との半径: 整列 |
| Cohesion Distance | 適用する他の個体との半径: 結合 |
| Separation Coefficient | 適用時の重み係数: 分離 |
| Alignment Coefficient | 適用時の重み係数: 整列 |
| Cohesion Coefficient | 適用時の重み係数: 結合 |
| Avoid Wall Weight | 壁を避ける強さの重み |
