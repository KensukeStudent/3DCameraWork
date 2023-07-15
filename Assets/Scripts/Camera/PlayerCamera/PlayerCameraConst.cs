using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// プレイヤーカメラ定数値
    /// </summary>
    public partial class PlayerCameraController
    {
        /// <summary>
        /// 中心のロックオン座標
        /// </summary>
        private const float LOCKON_ANGLE_CENTER = 180.0f;

        /// <summary>
        /// キャラクターが描画されるカメラビューポート最大範囲内
        /// </summary>
        private readonly Vector2 MAX_LOCKON_ANGLE = new Vector2(120.0f, 0.40f);

        /// <summary>
        /// プレイヤーと敵との距離がEXPAND_LOCKON_DISTANCE未満のときにviewport上の判定領域を変更する
        /// </summary>
        private const float EXPAND_LOCKON_DISTANCE = 11;

        /// <summary>
        /// ロックオン時に挙動変更を行うプレイヤーと敵との限界距離
        /// </summary>
        private const float LINIT_LOCKON_DISTANCE = 2;

        /// <summary>
        /// ユーザーが入力可能な最大角度
        /// </summary>
        private const float MAX_ANGLE = 45;

        /// <summary>
        /// ユーザーが入力可能な最大距離
        /// </summary>
        private const float MAX_DISTANCE = 6.0f;

        /// <summary>
        /// ユーザーが入力可能な最小距離
        /// </summary>
        private const float MIN_DISTANCE = 1.2f;
    }
}