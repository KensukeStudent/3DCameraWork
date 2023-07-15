using System;
using System.Linq.Expressions;
using UnityEngine;

namespace VoxelBrave
{
    [CreateAssetMenu(fileName = "名前を変更してね(PlayerCameraParameter)", menuName = "Camera/PlayerCameraParameter")]
    public class PlayerCameraParameterScriptable : ScriptableObject
    {
        [SerializeField]
        private CameraMode cameraMode = CameraMode.None;

        [SerializeField]
        public PlayerCameraParameter parameter;

        /// <summary>
        /// パラメータ変数設定時に必ず定義を行う
        /// </summary>
        /// <param name="_cameraMode">このScriptableObjectに設定されるCameraMode</param>
        /// <param name="e">このクラスの変数名</param>
        public void CheckValidation<PlayerCameraParameter>(CameraMode _cameraMode, Expression<Func<PlayerCameraParameter>> e)
        {
            if (cameraMode != _cameraMode)
            {
                var name = ((MemberExpression)e.Body).Member.Name;
                LogUtil.Warning($@"入力されたカメラモードと設定されたカメラパラメータが異なります
引数：{name}のインスぺスターに設定されるパラメータは{_cameraMode}である必要があります");
            }
        }
    }

    /// <summary>
    /// プレイヤーカメラで使用するパラメータ
    /// </summary>
    [Serializable]
    public class PlayerCameraParameter : CameraParameter
    {
        /// <summary>
        /// ラープパワー
        /// </summary>
        [SerializeField, Range(1, 100), Header("1 / 60 * ラープ量になる数値")]
        public float LerpPower;

        /// <summary>
        /// 角度用ラープパワー
        /// </summary>
        [SerializeField, Range(1, 100), Header("1 / 60 * 角度ラープ量になる数値")]
        public float AngleLerpPower;
    }
}
