using System.Collections.Generic;

namespace CameraWork
{
    /// <summary>
    /// プレイヤーカメラモード変更担当クラス
    /// </summary>
    public class CameraModeChangeHelper
    {
        /// <summary>
        /// カメラモデルデータ
        /// </summary>
        private readonly PlayerCameraModel model = null;

        private PlayerCameraParameter param = null;

        private readonly Dictionary<CameraMode, PlayerCameraModeBase> switchCameraDic;

        public CameraModeChangeHelper(PlayerCameraModel _model, PlayerCameraParameter _param)
        {
            model = _model;
            param = _param;

            switchCameraDic = new()
            {
                {CameraMode.Normal, new PlayerCameraNormal(model, param)},
                {CameraMode.LockOn, new PlayerCameraLockOn(model, param)}
            };
        }

        /// <summary>
        /// カメラモード切替
        /// </summary>
        /// <param name="preMode">以前のカメラモード</param>
        /// <param name="nextMode">次のカメラモード</param>
        public SwitchCameraModeInfo SwitchCameraMode(CameraMode preMode, CameraMode nextMode)
        {
            if (preMode == nextMode)
            {
                return new SwitchCameraModeInfo() { nextMode = preMode, param = param };
            }

            // preModeからnextModeへ切り替え時のパラメータ更新を定義する
            var switchCamera = switchCameraDic[preMode];
            return switchCamera.SwitchCameraMode(nextMode);
        }
    }
}
