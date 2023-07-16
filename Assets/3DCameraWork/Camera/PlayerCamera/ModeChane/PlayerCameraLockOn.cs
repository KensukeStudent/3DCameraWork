namespace CameraWork
{
    /// <summary>
    /// プレイヤーカメラロックオンモード時の切り替え定義クラス
    /// </summary>
    public class PlayerCameraLockOn : PlayerCameraModeBase
    {
        public PlayerCameraLockOn(PlayerCameraModel _model, PlayerCameraParameter _param) : base(_model, _param)
        {

        }

        public override SwitchCameraModeInfo SwitchCameraMode(CameraMode nextMode)
        {
            return nextMode switch
            {
                CameraMode.Normal => GetNormalInfo(),
                _ => throw DefineCameraModeException()
            };
        }

        private SwitchCameraModeInfo GetNormalInfo()
        {
            var normalParam = model.GetParameter(CameraMode.Normal);
            var nextParam = defaultParam;
            nextParam.ViewLockOn = normalParam.ViewLockOn;
            nextParam.ViewDistance = normalParam.ViewDistance;
            nextParam.AngleLerpPower = normalParam.AngleLerpPower;
            return CreateCameraModeInfo(CameraMode.Normal, nextParam);
        }
    }
}
